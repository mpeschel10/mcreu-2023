using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireworksOnOff : MonoBehaviour, Fixable
{
    [SerializeField] ParticleSystem[] launchers;

    public void Awake()
    {
        Fix();
    }
    
    public void OnEnable()
    {
        Fix();
    }

    public void OnDisable()
    {
        Fix();
    }

    public void Fix()
    {
        foreach (ParticleSystem launcher in launchers)
        {
            ParticleSystem.EmissionModule module = launcher.emission;
            module.enabled = enabled;
        }
    }
}
