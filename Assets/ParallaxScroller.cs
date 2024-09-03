using UnityEngine;
using System.Collections.Generic;

public class ParallaxScroller : MonoBehaviour
{
    [System.Serializable]
    public class ParallaxObject
    {
        public GameObject originalGameObject;
        public float speed;
        private float size = 0f;
        private List<GameObject> parallaxObjectList = new List<GameObject>();
        public void UpdateSize(float newSize) {
            size = newSize;
        }
        public float GetSize() {
            return size;
        }

        public void AddToObjectList(GameObject newGameObject) {
            parallaxObjectList.Add(newGameObject);
        }

        public void Move(Vector3 flowDirection)
        {
            for (int i = 0; i < parallaxObjectList.Count; i++)
            {
                GameObject obj = parallaxObjectList[i];
                obj.transform.position += flowDirection * speed * Time.deltaTime;
            }
        }

        public bool ShouldWrap(GameObject obj, Vector3 boundsCenter)
        {
            Vector3 pos = obj.transform.position;
            Vector3 halfSize = new Vector3(size / 2, size / 2, size / 2);

            bool wrapX = pos.x > boundsCenter.x + halfSize.x || pos.x < boundsCenter.x - halfSize.x;
            bool wrapY = pos.y > boundsCenter.y + halfSize.y || pos.y < boundsCenter.y - halfSize.y;
            bool wrapZ = pos.z > boundsCenter.z + halfSize.z || pos.z < boundsCenter.z - halfSize.z;

            // Return true only if both sides of the object's bounds are out of the ParallaxScroller's bounds.
            return wrapX && wrapY && wrapZ;
        }

        public void WrapObject(Vector3 boundsEdge) {
            foreach(GameObject parallaxObject in parallaxObjectList) {
                if(ShouldWrap(parallaxObject, boundsCenter)) {
                    // place it at the end wall opposite of the flow direction where its just out of bounds
                }
            }
        }

        public void WrapPosition(GameObject obj, Vector3 boundsCenter)
        {
            Vector3 pos = obj.transform.position;
            Vector3 halfBounds = new Vector3(size / 2, size / 2, size / 2);

            if (pos.x > boundsCenter.x + halfBounds.x)
                pos.x = boundsCenter.x - halfBounds.x;
            else if (pos.x < boundsCenter.x - halfBounds.x)
                pos.x = boundsCenter.x + halfBounds.x;

            if (pos.y > boundsCenter.y + halfBounds.y)
                pos.y = boundsCenter.y - halfBounds.y;
            else if (pos.y < boundsCenter.y - halfBounds.y)
                pos.y = boundsCenter.y + halfBounds.y;

            if (pos.z > boundsCenter.z + halfBounds.z)
                pos.z = boundsCenter.z - halfBounds.z;
            else if (pos.z < boundsCenter.z - halfBounds.z)
                pos.z = boundsCenter.z + halfBounds.z;

            obj.transform.position = pos;
        }
    }

    [SerializeField] private List<ParallaxObject> parallaxObjects;
    [SerializeField] private float boundsWidth = 20f;
    [SerializeField] private float boundsHeight = 10f;
    [SerializeField] private float boundsDepth = 100f;
    [SerializeField] private Vector3 offset = Vector3.zero;
    [SerializeField] private Vector3 flowDirection = Vector3.left;

    void Start()
    {
        flowDirection = flowDirection.normalized;
        foreach (ParallaxObject parallaxObject in parallaxObjects)
        {
            Vector3 objectSize = parallaxObject.originalGameObject.GetComponent<Renderer>().bounds.size;
            float objectSizeInDirection = Mathf.Abs(Vector3.Dot(objectSize, flowDirection));
            parallaxObject.UpdateSize(objectSizeInDirection);

            // Calculate the number of objects needed to fill the bounds in the flow direction
            float boundsSizeInDirection = Mathf.Abs(Vector3.Dot(new Vector3(boundsWidth, boundsHeight, boundsDepth), flowDirection));
            int numberOfObjectsNeeded = Mathf.CeilToInt(boundsSizeInDirection / objectSizeInDirection);

            // Instantiate the necessary number of objects and add them to the list
            for (int i = 0; i < numberOfObjectsNeeded; i++)
            {
                Vector3 spawnPosition = transform.position + offset + flowDirection * (i * objectSizeInDirection);
                // Adjust spawn position to inherit the x, y, or z positions from the original if flowDirection doesn't affect them
                if (flowDirection.x == 0)
                {
                    spawnPosition.x = parallaxObject.originalGameObject.transform.position.x;
                }
                if (flowDirection.y == 0)
                {
                    spawnPosition.y = parallaxObject.originalGameObject.transform.position.y;
                }
                if (flowDirection.z == 0)
                {
                    spawnPosition.z = parallaxObject.originalGameObject.transform.position.z;
                }

                GameObject spawnObject = parallaxObject.originalGameObject;
                GameObject newObject = Instantiate(spawnObject, spawnPosition, spawnObject.transform.rotation, transform);
                parallaxObject.AddToObjectList(newObject);

            }

            // Log the result
            Debug.Log($"Number of {parallaxObject.originalGameObject.name} objects needed: {numberOfObjectsNeeded}");
            parallaxObject.originalGameObject.SetActive(false);
        }
    }

    void Update()
    {
        foreach (ParallaxObject parallaxObject in parallaxObjects)
        {
            parallaxObject.Move(flowDirection);
        }
    }

    void OnDrawGizmos()
    {
        // Set the color for the gizmo
        Gizmos.color = Color.yellow;
        // Calculate the center of the bounds, including the offset
        Vector3 center = transform.position + offset + new Vector3(0, 0, -boundsDepth / 2);
        // Draw a wire cube to represent the bounds
        Gizmos.DrawWireCube(center, new Vector3(boundsWidth, boundsHeight, boundsDepth));
        // Draw an arrow to show the flow direction
        Gizmos.color = Color.red;
        Vector3 arrowStart = center;
        Vector3 arrowEnd = arrowStart + flowDirection * boundsWidth / 2;
        Gizmos.DrawLine(arrowStart, arrowEnd);
        Gizmos.DrawLine(arrowEnd, arrowEnd - flowDirection * 0.5f + Vector3.Cross(flowDirection, Vector3.up) * 0.25f);
        Gizmos.DrawLine(arrowEnd, arrowEnd - flowDirection * 0.5f - Vector3.Cross(flowDirection, Vector3.up) * 0.25f);
    }
}