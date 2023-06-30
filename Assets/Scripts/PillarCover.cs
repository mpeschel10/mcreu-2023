using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillarCover : MonoBehaviour, FreeMovement.Clickable, LessBadXRPokeInteractor.Pokeable
{
    [SerializeField] GameObject pillar;
    public void Click()
    {
        pillar.SetActive(true);
        gameObject.SetActive(false);
    }

    public void Poke() { Click(); }
}
