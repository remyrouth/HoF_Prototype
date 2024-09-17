using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MechOptionFrame : MonoBehaviour
{
    [SerializeField] private MechStats mechInFrame;
    [SerializeField] private Image frameImage;

    // called by UpgradeMechController
    public void InitializeFromUpgradeMechController(MechStats givenMech) {
        mechInFrame = givenMech;
        frameImage.gameObject.SetActive(true);
        frameImage.sprite = givenMech.GetMechSprite();
    }

    // called by button
    public void ChooseMech() {
        UpgradeMechController mechUpgradeSystem = FindObjectOfType<UpgradeMechController>();
        mechUpgradeSystem.ChooseSpecificMech(mechInFrame);
    }
}
