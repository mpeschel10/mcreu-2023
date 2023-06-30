using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireworksOnOff : MonoBehaviour
{
    [SerializeField] ParticleSystem[] launchers;
    
    bool fireworksEnabled = false;
    public void OnEnable()
    {
        fireworksEnabled = true; Apply();
    }

    public void OnDisable()
    {
        fireworksEnabled = false; Apply();
    }

    public void Apply()
    {
        foreach (ParticleSystem launcher in launchers)
        {
            ParticleSystem.EmissionModule module = launcher.emission;
            module.enabled = fireworksEnabled;
        }
}
}
