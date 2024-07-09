using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.SceneManagement;

public class VNController : MonoBehaviour
{
    private List<Sprite> sprites = new List<Sprite>();
    private Image currImage;
    private bool notAtEnd = true;
    public int idx = -1;

    void Start()
    {
        //initializes the sprites list 
        CollectAllSprites();

        //ensures that the currImage is never null
        if (currImage == null) {
            currImage = GetComponentInChildren<Image>();
        }

        //ensures that the cycling through sprites list is delayed at each call
        if (notAtEnd)
        {
            InvokeRepeating("CycleThroughSprites", 1.0f, 2.5f);
        } 
    }

    //finds all visual novel scene sprites in folder and stores them in list
    private void CollectAllSprites() 
    {   
        //prevents duplicates
        sprites.Clear();

        //finds the folder containing visual novel sprites
        string folderPath = "Assets/UI/Visual_Novel_Sprites";
        string[] spritePaths = AssetDatabase.FindAssets("t:Sprite", new string[] { folderPath });

        //puts each sprite found into the list of sprites
        foreach (string spritePath in spritePaths) 
        {
            string path = AssetDatabase.GUIDToAssetPath(spritePath);
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (sprite != null) {
                sprites.Add(sprite);
            }
        }
    }

    //assigns a Sprite to an Image
    private void CycleThroughSprites()
    {
        //ensures that the sprite isn't null
        Sprite a_sprite = CycleThroughSpritesHelp();
        if (a_sprite != null) {
            currImage.sprite = a_sprite;
        }

        //loads next scene if we've reached the end of the sprites list 
        if (idx >= sprites.Count - 1) {
            CancelInvoke("CycleThroughSprites");
            Debug.Log("Loading next scene...");
            SceneManager.LoadScene("SampleScene");
        }
    }

    //iterates through the list of sprites 
    private Sprite CycleThroughSpritesHelp() 
    {
        idx++; //increment index count
        if (sprites.Count != 0 && idx <= sprites.Count - 1) {
            Debug.Log("Returning next sprite");
            return sprites[idx];
        }
        
        Debug.Log("At end of sprites list");
        notAtEnd = false;
        return null;
    }
}
