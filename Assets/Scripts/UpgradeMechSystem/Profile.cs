using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Profile : MonoBehaviour
{
    public Image sprite;
    public string pilotName;
    public string description;
    public string id;

    public GameObject profileToggleObj;


    public void SetupProfile(Sprite sprite, string name, string id, string description){
       
       Image imageComponent = transform.Find("Image").GetComponent<Image>();
       imageComponent.sprite = sprite;
       this.pilotName = name;
       this.id = id;
       this.description = description;
       this.profileToggleObj = GameObject.Find("Toggler");
    }

    public void onProfileButtonClick(){
        Profile profile = GetComponent<Profile>();
        ProfileToggle profileToggle = profileToggleObj.GetComponent<ProfileToggle>();
        profileToggle.ToggleProfileAndPopup(profile);
    }

}
