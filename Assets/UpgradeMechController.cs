using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeMechController : MonoBehaviour
{
    private MechSaveFileInteractor fileInteractor;
    [SerializeField] UpgradableMechUnit currentMechSelected;


    private List<MechStats> upgradeableMechList = new List<MechStats>();

    private void Start() {
        fileInteractor = FindObjectOfType<MechSaveFileInteractor>();
        if (fileInteractor == null) {
            fileInteractor = gameObject.AddComponent<MechSaveFileInteractor>();
        }

        GetMechList();
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

    public void ChangeViewableMechs(bool shouldIncreaseIndex) {
        // we have only a certain number of portrait frames
        // viewable at a time. lets say 30 of the total X (could be 130 for example) at a time
        // so we need a button to increase what we show
    }
}
