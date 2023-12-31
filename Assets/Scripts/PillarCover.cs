using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PillarCover : MonoBehaviour, MouseSelector.Clickable, MouseSelector.SmearClickable, MyPoker.Pokeable
{
    public GameObject pillar, pillarOffset;
    [SerializeField] TMP_Text textFront, textBack;
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

    public Color color
    {
        set { pillar.GetComponent<MeshRenderer>().material.color = value; }
    }
    public void UpdateDisplayIndex()
    {
        textFront.text = (index - 2).ToString();
        textBack.text = (index - 2).ToString();
    }

    public void Poke() { Click(); }
}
