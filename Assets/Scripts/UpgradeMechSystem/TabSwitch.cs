using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabSwitch : MonoBehaviour
{

    public GameObject tab;

    public GameObject tabPrefab;


    public ScrollRect profileSelection;
    public Transform tabContainer;

    public void pressedMech(){
        clearOutExistingTabs();
        Destroy(tab);
        tab = Instantiate(tabPrefab, tabContainer);
    }

    public void pressedPilot(){
        clearOutExistingTabs();
        Destroy(tab);
        tab = Instantiate(tabPrefab, tabContainer);
    }

    private void clearOutExistingTabs(){
        Profile[] profiles = profileSelection.content.GetComponentsInChildren<Profile>();
        Debug.LogError("profile size" + profiles.Length);
        foreach(Profile pr in profiles){
            Destroy(pr.gameObject);
            Debug.LogError("destroyed profile"+profiles.Length);
        }
    }
}
