using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillarCover2 : MonoBehaviour, Fixable, MouseSelector.Clickable, MouseSelector.SmearClickable, MyPoker.Pokeable
{
    [SerializeField] Pillars2D pillars2d;
    [SerializeField] public GameObject coverParent, pillarParent, cellParent, pillar;
    public int r, c;
    bool _isRevealed = false, _isEliminated = false;
    public bool isRevealed
    {
        get => _isRevealed;
        set { _isRevealed = value; Fix(); }
    }
    public bool isEliminated
    {
        get => _isEliminated;
        set { _isEliminated = value; Fix(); }
    }
    
    public void Fix()
    {
         coverParent.SetActive(!isRevealed);
        pillarParent.SetActive( isRevealed);
          cellParent.SetActive(!isEliminated);
        pillar.GetComponent<MeshRenderer>().material.color = color;
    }

    Color _color;
    public Color color
    {
        get => _color;
        set { _color = value; Fix(); }
    }
    
    public void Click() { pillars2d.Click(r, c); }
    public void Poke() { Click(); }
}
