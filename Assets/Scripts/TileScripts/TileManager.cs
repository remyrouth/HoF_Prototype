using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class BoardManager : MonoBehaviour
{
    public int rows = 8; // Number of rows
    public int columns = 8; // Number of columns
    public GameObject tilePrefab; // Tile prefab
    public Vector3 startPosition = new Vector3(0, 0, 0); // Starting position of the board
    public float tileSpacing = 1.1f; // Spacing between tiles

    void Awake()
    {
        if (!Application.isPlaying)
            return;

        ClearBoard();
        CreateBoard();
    }

    void Update()
    {
        if (!Application.isPlaying)
        {
            if (needsUpdate)
            {
                ClearBoard();
                CreateBoard();
                needsUpdate = false;
            }
        }
    }

    public void CreateBoard()
    {
        for (int x = 0; x < columns; x++)
        {
            for (int z = 0; z < rows; z++)
            {
                Vector3 tilePosition = new Vector3(startPosition.x + (x * tileSpacing), startPosition.y, startPosition.z + (z * tileSpacing));
                GameObject tile = Instantiate(tilePrefab, tilePosition, Quaternion.identity);
                tile.name = $"Tile_{x}_{z}";
                tile.transform.parent = transform; // Set the parent of the tile to this object
            }
        }
    }

    public void ClearBoard()
    {
        // Destroy all child objects
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }

    public void UpdateBoard()
    {
        ClearBoard();
        CreateBoard();
    }

    private bool needsUpdate = false;

    private void OnValidate()
    {
        needsUpdate = true;
    }
}

[CustomEditor(typeof(BoardManager))]
public class BoardManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        BoardManager boardManager = (BoardManager)target;
        if (GUILayout.Button("Update Board"))
        {
            boardManager.UpdateBoard();
        }
    }
}
