using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    
    [SerializeField] public bool isPlayerTurn = true;
    [SerializeField] public bool isPaused = false;

    private GameObject movingPiece;
    private GameObject targetedTile;
    private SelectionManager sm;
    private CharacterCanvasController ccc;
    private bool completeWaitChecks = false;
    public GameObject EndTurnCanvas;

    private void Start() {
        sm = FindObjectOfType<SelectionManager>();
        ccc = FindObjectOfType<CharacterCanvasController>();
    }

    private void Update() {
        Debug.Log("update continuing");
        Debug.Log("completeWaitChecks status: " + completeWaitChecks + " isPaused Status: " + isPaused);
        if (completeWaitChecks && !isPaused) {
            reachedDestinationCheck();
        }
    }

    public void PauseTurnSystem() {
        isPaused = true;
    }

    public void ResumeTurnSystem() {
        isPaused = false;

        // Ensure we don't start another move too early
        if (!completeWaitChecks && !isPaused) {
            StartIndividualEnemyAction();
        }
    }

    // called by selectionmanager, so that we cannot tell a new piece to move
    // while another piece was just told to move
    public void BeginWait(GameObject entity, GameObject tile) {
        movingPiece = entity;
        targetedTile = tile;
        sm.enabled = false;
        completeWaitChecks = true;
    }

    private void reachedDestinationCheck() {
        if (movingPiece != null) {
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
        } else {
            if (isPlayerTurn) {
                
                movingPiece = null;
                targetedTile = null;
                sm.enabled = true;
            } else {

                movingPiece = null;
                targetedTile = null;
                StartIndividualEnemyAction();
            }
        }

    }

    private void StartIndividualEnemyAction() {
        Debug.Log("StartIndividualEnemyAction method called");
        if (isPaused) {
            // Don't start the next new action if paused
            Debug.Log("StartIndividualEnemyAction method canceled from pause state");
            return;
        }
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
        Debug.Log("enemyControlled Count: " + enemyControlled.Count);
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

    // called by end turn button on canvas
    public void EndPlayerTurn() {
        Debug.Log("EndPlayerTurn button method called");
        isPlayerTurn = false;
        EndTurnCanvas.SetActive(false);
        ccc.MenuCleanup();
        isPaused = false; // Ensure the game is not paused when ending the player's turn
        StartIndividualEnemyAction();
    }
}
