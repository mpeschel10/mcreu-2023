using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillarCover : MonoBehaviour, FreeMovement.Clickable, LessBadXRPokeInteractor.Pokeable
{
    public GameObject pillar, pillarOffset;
    // pillarOffset is used by Pillars for setting the height via pillarOffset.transform.localScale
    public int index;
    public Pillars parent;
    public void Click()
    {
        parent.Click(index);
    }

    public void Reveal()
    {
        pillar.SetActive(true);
        gameObject.SetActive(false);
    }

    public bool IsRevealed()
    {
        return pillar.gameObject.activeSelf;
    }

    public void Poke() { Click(); }
}
