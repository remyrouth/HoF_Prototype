using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeMechController : MonoBehaviour
{
    private MechSaveFileInteractor fileInteractor;


    [Header("UI Option Array variables")]
    // the object that holds all objects associated with the array so we can easily de-activate said objects
    [SerializeField] private GameObject arrayGroupParent;
    [SerializeField] private int maxMechPerSlide = 2;
    private int currentOptionsWindowIndex = 0;
    [SerializeField] private GameObject mechArrayHolder;
    [SerializeField] private Text WindowDisplay;

    [Header("UI Frame variables")]
    [SerializeField] private GameObject mechOptionPrefab;
    [SerializeField] private float frameSize = 0.5f;

    [Header("UI Mech Display variables")]
    [SerializeField] private Image ChosenMechDisplay;
    // the object that holds all objects associated with the display so we can easily de-activate said objects
    [SerializeField] private GameObject DisplayGroupParent;


    [Header("Mech variables")]
    [SerializeField] private MechStats currentMechSelected = null;


    [SerializeField] private List<UpgradableMechUnit> upgradeableMechList = new List<UpgradableMechUnit>();

    private void Start() {
        fileInteractor = FindObjectOfType<MechSaveFileInteractor>();
        if (fileInteractor == null) {
            fileInteractor = gameObject.AddComponent<MechSaveFileInteractor>();
        }
        UpdateUI();
        GetMechList();
        ChangeViewableMechs(false);
    }

    // called by MechOptionFrame.cs on prefab which is called by a button component
    public void ChooseSpecificMech(MechStats chosenMech) {
        currentMechSelected = chosenMech;
        ChosenMechDisplay.gameObject.SetActive(true);
        ChosenMechDisplay.sprite = chosenMech.GetMechSprite();
        UpdateUI();
    }

    // called by button
    public void CancelChosenMech() {
        currentMechSelected = null;
        UpdateUI();
    }

    private void ShowSpecificMechFrame(MechStats mech) {
        // creating object
        GameObject childObject = Instantiate(mechOptionPrefab);
        childObject.transform.SetParent(mechArrayHolder.transform);
        childObject.transform.localScale = new Vector3(frameSize, frameSize, frameSize);
        // initializing script
        MechOptionFrame mechFrameScript = mechOptionPrefab.GetComponent<MechOptionFrame>();
        mechFrameScript.InitializeFromUpgradeMechController(mech);
    }



    private void GetMechList() {
        upgradeableMechList = fileInteractor.ExtractMechsFromFile();
    }


    public class UpgradableMechUnit {
        public MechStats mechBaseModel;
        public int maxClarityUpgradeCount = 0;
        public int maxHealthUpgradeCount = 0;
    }





    // UI methods

    // called by arrow buttons with button component which change the window slide
    public void ChangeViewableMechs(bool shouldIncreaseIndex) {
        Debug.Log("Pre Use index = " + currentOptionsWindowIndex);
        if (upgradeableMechList.Count == 0) {
            // Handle empty list case
            WindowDisplay.text = "No mechs available";
            return;
        }

        int maxWindowCount = Mathf.CeilToInt((float)upgradeableMechList.Count / maxMechPerSlide) - 1;

        if (shouldIncreaseIndex) {
            currentOptionsWindowIndex++;
            currentOptionsWindowIndex = Mathf.Min(currentOptionsWindowIndex, maxWindowCount);
        } else {
            currentOptionsWindowIndex--;
            currentOptionsWindowIndex = Mathf.Max(currentOptionsWindowIndex, 0);
        }
        Debug.Log("Post Use index = " + currentOptionsWindowIndex);

        int starterIndex = maxMechPerSlide * currentOptionsWindowIndex;
        int endIndex = Mathf.Min(starterIndex + maxMechPerSlide, upgradeableMechList.Count);

        DeleteAllFrames();

        for (int i = starterIndex; i < endIndex; i++) {
            ShowSpecificMechFrame(upgradeableMechList[i].mechBaseModel);
        }

        // Update window display
        WindowDisplay.text = $"Window {currentOptionsWindowIndex + 1} / {maxWindowCount + 1}";
    }

    private void UpdateUI() {
        if (currentMechSelected != null) {
            arrayGroupParent.SetActive(false);
            DisplayGroupParent.SetActive(true);
        } else {
            arrayGroupParent.SetActive(true);
            DisplayGroupParent.SetActive(false);
        }
    }

    public void DeleteAllFrames()
    {
        // Iterate through all child objects
        foreach (Transform child in mechArrayHolder.transform)
        {
            // Destroy each child GameObject
            Destroy(child.gameObject);
        }
    }
}
