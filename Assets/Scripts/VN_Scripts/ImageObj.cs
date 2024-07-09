using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Represents an ImageObj that holds the name of a sprite as well as the sprite it is associated with.
public class ImageObj
{
    public string name;
    public Sprite sprite;

    //constructor for ImageObj class
    public ImageObj(string name, Sprite sprite) {
        this.name = name;
        this.sprite = sprite;
    }
}
