using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Plot Weight SO", menuName = "Scriptable Objects/PlotWeightSO", order = 1)]

public class PlotWeightSO : ScriptableObject
{
    public int max;
    public int wynnter;
    public int aurelio;
    public int startWeight = 0;

    public bool resetOnStart = true;

	public void Start() 
	{
		if (resetOnStart) { Debug.LogWarning("resetOnStart set to true. Use only for testing."); }
	}

    public void OnEnable()
    {
        if (! resetOnStart)
        {
            return;
        }

        this.max = startWeight;
        this.aurelio = startWeight;
        this.wynnter = startWeight;
    }

    public void ChangeMaxValueBy(int plus)
    {
        max += plus;
    }
    
    public void ChangeWynnterValueBy(int plus)
    {
        wynnter += plus;
    }
    
    public void ChangeAurelioValueBy(int plus)
    {
        aurelio += plus;
    }
}