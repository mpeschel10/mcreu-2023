using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Scoreboard : MonoBehaviour
{
    [SerializeField] TMP_Text[] displays;
    int cost = 0;
    int best = -1;
    private string _text;
    public string text
    {
        get { return _text; }
        set { _text = value; UpdateDisplay(); }
    }
    public void SetCost(int cost)
    {
        this.cost = cost;
        UpdateText();
    }

    public void SetBest(int best)
    {
        this.best = best;
        UpdateText();
    }

    public void UpdateText()
    {
        text = "Cost: " + cost + "\nBest: " + best;
    }

    public void UpdateDisplay()
    {
        foreach (TMP_Text display in displays)
        {
            display.text = text;
        }
    }
}
