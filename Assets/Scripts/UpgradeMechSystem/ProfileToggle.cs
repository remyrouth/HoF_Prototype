using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class ProfileToggle : MonoBehaviour
{
    private Profile profile;  
    public ScrollRect profileSelection; 
    public GameObject popupPanelPrefab; 

    private GameObject popupObject;

    public Transform popupPanelContainer;

    /*
        Opens up the selected profile in a more detailed popup window. 

        :param prof: profile to be opened.
    */
    public void ToggleProfileAndPopup(Profile prof)
    {
            profileSelection.gameObject.SetActive(false);

            profile = prof;
            popupObject = Instantiate(popupPanelPrefab, popupPanelContainer);
            popupObject.SetActive(true);
            CharPanel panel = popupObject.GetComponent<CharPanel>();
            if(panel != null){
            panel.SetupPopup(profile.sprite, profile.pilotName, profile.description);
            }
            else{
                Debug.LogError("panel is null");
            }
    }

    /*
        Closes the popped-up profile, returning back to the original profile selection.
    */
    public void ReturnFromPopup(){
        profileSelection.gameObject.SetActive(true);
        Destroy(popupObject);
    }


    /*
        Opens up the next available profile in the profile selection in a looping fashion. 

        :param left: if true, iterate leftwards, else iterate rightwards.
    */
    public void IteratePopup(bool left){
        Profile[] profiles = profileSelection.content.GetComponentsInChildren<Profile>();
        int currID = int.Parse(profile.id);
        int nextID = 0;
        if(left){
            if(currID == 0){
            nextID = profiles.Length - 1;
                }
            else{
                nextID = currID - 1;
            }
        }
        else{
            if(currID == profiles.Length - 1){
            nextID = 0;
            }
            else{
                nextID = currID + 1;
            }
        }
        
        Profile nextProfile = profiles[nextID];
        Destroy(popupObject);
        ToggleProfileAndPopup(nextProfile);
    }

}
