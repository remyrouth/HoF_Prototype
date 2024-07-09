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

    public Color centerWalkable;
    public Color borderWalkable;

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
        ChangeColors(centerOriginal, borderOriginal);
    }
    public void ChangeToSelectedState() {
        currentState = TileState.Selected;
        ChangeColors(centerSelected, borderSelected);
    }

    public void ChangeToWalkableState() {
        currentState = TileState.Selected;
        ChangeColors(centerWalkable, borderWalkable);
    }

    public void ChangeToHoveringState() {

        if (currentState != TileState.Default) {
            return;
        }

        currentState = TileState.Hovering;
        ChangeColors(centerHovering, borderHovering);
    }

    public void UnHoverState() {
        if (currentState == TileState.Hovering) {
            ChangeToDefaultState();
            Debug.Log("called unhover from helper script");
        }
    }


    private void ChangeColors(Color centerColor, Color borderColor) {
        foreach (GameObject border in tileBorders) {
            border.GetComponent<SpriteRenderer>().color = centerColor;
        }
        tileCenter.GetComponent<SpriteRenderer>().color = borderColor;
    }



    // public void 
}
