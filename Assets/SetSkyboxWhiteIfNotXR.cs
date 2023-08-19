using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetSkyboxWhiteIfNotXR : MonoBehaviour
{
    [SerializeField] Material skyboxWhite, skyboxNormal;

    void Start()
    {
        Fix();
    }

    void Fix()
    {
        RenderSettings.skybox = GameControllerState.isXR ? skyboxNormal : skyboxWhite;
    }
}
