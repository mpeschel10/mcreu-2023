using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuShowIfNotHidden : MonoBehaviour, Fixable
{
    public void Fix()
    {
        gameObject.SetActive(GameControllerState.menuLocation != MenuLocation.Hidden);
    }
}
