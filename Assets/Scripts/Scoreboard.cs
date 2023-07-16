using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Scoreboard : MonoBehaviour, Fixable
{
    [SerializeField] TMP_Text[] displays;
    int _cost = 0;
    public int cost
    {
        get => _cost;
        set { _cost = value; Fix(); }
    }
    string _best = "0";
    public string best
    {
        get => _best;
        set { _best = value; Fix(); }
    }
    string text;

    public void Fix()
    {
        text = "Cost: " + cost + "\nBest: " + best;
        foreach (TMP_Text display in displays)
        {
            display.text = text;
        }
    }
}
