using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{

    public bool isPlayerTurn = true;


    public GameObject movingPiece;
    public GameObject targetedTile;
    private SelectionManager sm;
    private bool completeWaitChecks = false;

    private void Start() {
        sm = FindObjectOfType<SelectionManager>();
    }

    private void Update() {
        // Debug.Log("completeWaitChecks boolean: " + completeWaitChecks);
        if (completeWaitChecks) {
            reachedDestinationCheck();
        }
    }

    public void BeginWait(GameObject entity, GameObject tile) {
        bool someNull = (movingPiece == null || targetedTile == null);
        // Debug.Log("someNull" + someNull);
        movingPiece = entity;
        targetedTile = tile;
        sm.enabled = false;
        completeWaitChecks = true;
    }

    private void reachedDestinationCheck() {
        // if ((movingPiece == null || targetedTile == null) && completeWaitChecks) {
        //     Debug.LogError("Moving piece and/or tile not given, and therefore null");
        //     return;
        // }

        Vector3 tilePos = targetedTile.transform.position;
        Vector3 piecePos = movingPiece.transform.position;
        bool reachedDestination = (piecePos.x == tilePos.x && piecePos.z == tilePos.z);
        if (reachedDestination) {
            completeWaitChecks = false;

            if (isPlayerTurn) {
                
                movingPiece = null;
                targetedTile = null;
                sm.enabled = true;
            } else {

                movingPiece = null;
                targetedTile = null;
            }

        }




    }
}
