using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuShowIfNotHidden : MonoBehaviour, Fixable
{
    public void Fix()
    {
        // Debug.Log("Fixing menushowifnothiedden " +  gameObject + " menu is " + GameControllerState.menuLocation);
        gameObject.SetActive(GameControllerState.menuLocation != MenuLocation.Hidden);
    }
}
