using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideMenu : MonoBehaviour
{
    [SerializeField] GameObject menuParent;
    public void Main()
    {
        GameControllerState.menuLocation = MenuLocation.Hidden;
        menuParent.GetComponent<Fixable>().Fix();
    }
}
