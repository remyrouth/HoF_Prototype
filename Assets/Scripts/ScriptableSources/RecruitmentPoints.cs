using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Recruitment Points SO", menuName = "Scriptable Objects/RecruitmentPointsSO", order = 1)]
public class RecruitmentPoints : MonoBehaviour
{
    [Tooltip("Current amount of recruitment points")] [SerializeField] private int recruitmentPointsAvailable;
    [Tooltip("Amount of Recruitment Points you'll start the game with")] [SerializeField] private int initialValue;
    public bool resetRecruitmentPoints;

    private void OnEnable()
    {
        if (resetRecruitmentPoints)
        {
            recruitmentPointsAvailable = initialValue;
        }
    }

    public int GetRecruitmentPointsAvailable()
    {
        return recruitmentPointsAvailable;
    }
    
    public void SetRecruitmentPointsAvailable(int newValue)
    {
        recruitmentPointsAvailable = newValue;
    }
}
