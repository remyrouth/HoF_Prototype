using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGraphicsController : MonoBehaviour
{
    public List<GameObject> tileBorders;
    public GameObject tileCenter;

    public Color centerOriginal;
    public Color borderOriginal;

    public Color centerSelected;
    public Color borderSelected;

    public Color centerHovering;
    public Color borderHovering;

    public TileState currentState = TileState.Default;

    public enum TileState {
        Default,
        Selected,
        Hovering,
        Walkable
    }

    private void Start() {
        ChangeToDefaultState();
    }

    public void ChangeToDefaultState() {
        currentState = TileState.Default;
        foreach (GameObject border in tileBorders) {
            border.GetComponent<SpriteRenderer>().color = borderOriginal;
        }
        tileCenter.GetComponent<SpriteRenderer>().color = centerOriginal;
    }
    public void ChangeToSelectedState() {
        currentState = TileState.Selected;
        foreach (GameObject border in tileBorders) {
            border.GetComponent<SpriteRenderer>().color = borderSelected;
        }
        tileCenter.GetComponent<SpriteRenderer>().color = centerSelected;
    }

    public void ChangeToHoveringState() {

        if (currentState != TileState.Default) {
            return;
        }

        currentState = TileState.Hovering;
        foreach (GameObject border in tileBorders) {
            border.GetComponent<SpriteRenderer>().color = centerHovering;
        }
        tileCenter.GetComponent<SpriteRenderer>().color = borderHovering;
    }

    public void UnHoverState() {
        if (currentState == TileState.Hovering) {
            ChangeToDefaultState();
            Debug.Log("called unhover from helper script");
        }
    }



    // public void 
}
