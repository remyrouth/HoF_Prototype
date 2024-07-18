using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{

    public bool isPlayerTurn = true;


    public GameObject movingPiece;
    public GameObject targetedTile;
    private SelectionManager sm;
    private CharacterCanvasController ccc;
    private bool completeWaitChecks = false;
    public GameObject EndTurnCanvas;

    private void Start() {
        sm = FindObjectOfType<SelectionManager>();
        ccc = FindObjectOfType<CharacterCanvasController>();
    }

    private void Update() {
        // Debug.Log("completeWaitChecks boolean: " + completeWaitChecks);
        if (completeWaitChecks) {
            reachedDestinationCheck();
        }
    }

    public void BeginWait(GameObject entity, GameObject tile) {
        movingPiece = entity;
        targetedTile = tile;
        sm.enabled = false;
        completeWaitChecks = true;
    }

    private void reachedDestinationCheck() {
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

                movingPiece.GetComponent<AIPlayerController>().Attack();
                movingPiece = null;
                targetedTile = null;
                StartIndividualEnemyAction();
            }

        }

    }

    private void StartIndividualEnemyAction() {
        // Collect all enemies. if no enemies available, end enemy turn
        GameObject[] playerObjectPiecesArray = GameObject.FindGameObjectsWithTag("Player");
        List<GameObject> enemyControlled = new List<GameObject>();

        foreach (GameObject piece in playerObjectPiecesArray) {
            AIPlayerController apc = piece.GetComponent<AIPlayerController>();

            if (apc != null) {
                if (!piece.GetComponent<PlayerController>().hasMovedYet) {
                    enemyControlled.Add(piece);
                }
            }
        }

        if (enemyControlled.Count <= 0) {
            StartPlayerTurn();
            return;
        }

        // choose random enemy that has yet to move
        // move piece
        GameObject bestTileToMoveTo = enemyControlled[0].GetComponent<AIPlayerController>().Move();

        // wait for piece
        if (bestTileToMoveTo != null) {
            BeginWait(enemyControlled[0], bestTileToMoveTo);
        }

        // attack with piece (done in reachedDestinationCheck)
    }

    private void StartPlayerTurn() {
        GameObject[] playerObjectPiecesArray = GameObject.FindGameObjectsWithTag("Player");
        // Debug.Log("playerObjectPiecesArray length: " + playerObjectPiecesArray.size);

        foreach (GameObject piece in playerObjectPiecesArray) {
            PlayerController pc = piece.GetComponent<PlayerController>();

            if (pc != null) {
                pc.ResetMoveAndAttackStates();
            }
        }


        isPlayerTurn = true;
        sm.enabled = true;
        EndTurnCanvas.SetActive(true);
    }

    public void EndPlayerTurn() {
        isPlayerTurn = false;
        EndTurnCanvas.SetActive(false);
        ccc.MenuCleanup();
        StartIndividualEnemyAction();
    }
}
