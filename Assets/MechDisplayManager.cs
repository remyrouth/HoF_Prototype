using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechDisplayManager : MonoBehaviour
{
    [SerializeField] private Transform mechStartPosition;
    private GameObject currentMechObject;

    public void DisplayMech(MechStats mechStats)
    {
        CleanUpCurrentMech();
        if (mechStats != null)
        {
            GameObject mechPrefab = mechStats.GetMechGFXPrefab();
            currentMechObject = Instantiate(mechPrefab, mechStartPosition.position, mechStartPosition.rotation);
        }
    }

    private void CleanUpCurrentMech()
    {
        if (currentMechObject != null)
        {
            Destroy(currentMechObject);
            currentMechObject = null;
        }
    }
}