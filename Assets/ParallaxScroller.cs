using UnityEngine;
using System.Collections.Generic;

public class ParallaxScroller : MonoBehaviour
{
    [System.Serializable]
    public class ParallaxObject
    {
        public GameObject originalGameObject;
        public float speed;
        public float safetyOffest = 4f;
        // I noticed that there is a random space sometimes present in wrapping 
        // of objects. I have no idea why that is, but we can fix that with
        // a safety offset. Easy fix. 
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

        public bool ShouldWrap(GameObject obj, Vector3 minBounds, Vector3 maxBounds)
        {
            Vector3 position = obj.transform.position;
            
            bool pastMinX = position.x < minBounds.x;
            bool pastMaxX = position.x > maxBounds.x;
            bool pastMinY = position.y < minBounds.y;
            bool pastMaxY = position.y > maxBounds.y;
            bool pastMinZ = position.z < minBounds.z;
            bool pastMaxZ = position.z > maxBounds.z;

            // Return true if the object has passed any of the bounds walls
            return pastMinX || pastMaxX || pastMinY || pastMaxY || pastMinZ || pastMaxZ;
        }

        public void WrapObjects(Vector3 minBounds, Vector3 maxBounds) {
            foreach(GameObject parallaxObject in parallaxObjectList) {
                if(ShouldWrap(parallaxObject, minBounds, maxBounds)) {
                    Debug.Log("Object needs wrapping");
                    WrapPosition(parallaxObject, minBounds, maxBounds);
                }
            }
        }

        public void WrapPosition(GameObject obj, Vector3 minBounds, Vector3 maxBounds)
        {
            Vector3 position = obj.transform.position;
            Vector3 size = obj.GetComponent<Renderer>().bounds.size;

            if (position.x < minBounds.x)
                position.x = maxBounds.x - size.x- safetyOffest;
            else if (position.x > maxBounds.x)
                position.x = minBounds.x - safetyOffest;

            if (position.y < minBounds.y)
                position.y = maxBounds.y - size.y- safetyOffest;
            else if (position.y > maxBounds.y)
                position.y = minBounds.y- safetyOffest;

            if (position.z < minBounds.z)
                position.z = maxBounds.z - size.z- safetyOffest;
            else if (position.z > maxBounds.z)
                position.z = minBounds.z- safetyOffest;

            obj.transform.position = position;
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
        Vector3 minBounds = transform.position + offset - new Vector3(boundsWidth, boundsHeight, boundsDepth);
        Vector3 maxBounds = transform.position + offset + new Vector3(boundsWidth, boundsHeight, boundsDepth);
        
        foreach (ParallaxObject parallaxObject in parallaxObjects)
        {
            parallaxObject.Move(flowDirection);
            parallaxObject.WrapObjects(minBounds, maxBounds);
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