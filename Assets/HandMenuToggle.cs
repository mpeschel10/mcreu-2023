using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandMenuToggle : MonoBehaviour, Fixable
{
    [SerializeField] GameObject menu;
    [SerializeField] MenuLocation thisLocation;
    public void Fix()
    {
        if (GameControllerState.menuLocation == thisLocation)
        {
            
        }
    }
}
