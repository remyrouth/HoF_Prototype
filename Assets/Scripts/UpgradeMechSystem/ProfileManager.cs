using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfileManager : MonoBehaviour
{
    public GameObject profilePrefab;
    public Transform profileContainer;

    public string[] pilotNames = {};
    public Sprite[] pilotSprites;

    public string[] descriptions;

    void Start(){
        //profileContainer = Transform.Find("Content");
        if (pilotNames.Length != pilotSprites.Length || pilotNames.Length != descriptions.Length){
            Debug.LogError("Mismatch between pilot names and sprites");
        }


        for (int i = 0; i < pilotNames.Length; i++){
            GameObject profile = Instantiate(profilePrefab, profileContainer);
            profile.transform.localScale = Vector3.one;
            Profile script =  profile.GetComponent<Profile>();
            script.SetupProfile(pilotSprites[i], pilotNames[i], i.ToString(), descriptions[i]);
            Canvas.ForceUpdateCanvases();
        }
    }
}
