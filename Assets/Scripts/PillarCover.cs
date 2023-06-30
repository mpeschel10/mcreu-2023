using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillarCover : MonoBehaviour, FreeMovement.Clickable
{
    [SerializeField] GameObject pillar;
    public void Click()
    {
        pillar.SetActive(true);
        gameObject.SetActive(false);
    }
}
