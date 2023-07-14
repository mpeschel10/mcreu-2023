using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsoOrthoButton : MonoBehaviour, MouseSelector.Hoverable, MouseSelector.Clickable, Fixable
{
    [SerializeField] GameObject isometricCamera, orthographicCamera;
    public GameObject GetGameObject() { return this.gameObject; }
    [SerializeField] Material toIsometricMaterial, toOrthogonalMaterial, hoverMaterial;

    public void Hover()
    {
        GetComponent<MeshRenderer>().material = hoverMaterial;
    }
    public void Unhover()
    {
        GetComponent<MeshRenderer>().material = GameControllerState.activeCamera == ActiveCamera.Isometric ?
                                                toOrthogonalMaterial : toIsometricMaterial;
    }
    public void Click()
    {
        GameControllerState.activeCamera = GameControllerState.activeCamera == ActiveCamera.Isometric ?
                                           ActiveCamera.Orthographic : ActiveCamera.Isometric;
        Unhover(); // Not a clean solution, but necessary since disabling the camera does not trigger OnUnhover()
        Fix();
    }
    
    public void Fix()
    {
           isometricCamera.SetActive(GameControllerState.activeCamera == ActiveCamera.Isometric);
        orthographicCamera.SetActive(GameControllerState.activeCamera == ActiveCamera.Orthographic);
    }
}
