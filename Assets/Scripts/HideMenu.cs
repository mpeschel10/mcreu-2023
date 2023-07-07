using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HideMenu : MonoBehaviour
{
    [SerializeField] GameObject menuParent;
    public void Main()
    {
        // Debug.Log("Hiding menu by UI button.");
        GameControllerState.menuLocation = MenuLocation.Hidden;
        menuParent.GetComponent<Fixable>().Fix();
        FixXRRayInteractor.FixAll();
        
        // Hack to prevent the button from re-pressing itself the next time the menu is enabled.
        GetComponent<Button>().interactable = false;
        GetComponent<Button>().interactable = true;
    }
}
