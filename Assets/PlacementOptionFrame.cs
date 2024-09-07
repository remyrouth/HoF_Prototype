using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlacementOptionFrame : MonoBehaviour
{
    [SerializeField] private Image displayFrame;
    [SerializeField] private GameObject removeTilePieceButton;
    [SerializeField] private Image highlightBackground;
    [SerializeField] private MechStats mech;
    [SerializeField] private CharacterStats pilot;
    private GameObject currentTilePiece = null;
    public void InitializeFromPlacementController(CharacterStats newPilot, MechStats newMech) {
        highlightBackground.enabled = false;
        mech = newMech;
        pilot = newPilot;
        displayFrame.sprite = mech.GetMechSprite();
        // displayFrame.gameObject.SetActive(true);
    }

    public void PlaceTilePiece(GameObject tileObject) {
        if (currentTilePiece != null) {
            return;
        }

        // creating physical object
        currentTilePiece = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        currentTilePiece.tag = "Player";

        // setting up player script
        PlayerController controller = currentTilePiece.AddComponent<PlayerController>();
        controller.SetPilotAndMechFromRosterScript(pilot, mech);

        // moving physical object
        currentTilePiece.transform.position = tileObject.transform.position;

    }

    public void RemoveTilePiece() {
        Destroy(currentTilePiece);
        currentTilePiece = null;
    }

    // called by button component on same object
    public void SelectPlacementOption() {
        UnitPlacementController unitPlacementController = FindObjectOfType<UnitPlacementController>();
        if (unitPlacementController != null) {
            unitPlacementController.SelectNewPlacementOption(this);
            highlightBackground.enabled = true;
        }
    }

    // called by unit placement controller
    public void Deselect() {
        highlightBackground.enabled = false;
    }

    // will return if this script has an object tile
    // placed yet. We want to know if we can start the game
    // returns true if used.
    public bool UseCheck() {
        return currentTilePiece != null;
    }


}
