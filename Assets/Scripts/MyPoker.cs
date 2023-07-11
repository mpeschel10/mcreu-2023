using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPoker : MonoBehaviour
{
    [SerializeField] AnimateHands state;
    public interface Pokeable { void Poke(); }
    void OnTriggerEnter(Collider other)
    {
        // Debug.Log("Poker " + gameObject + " had ontrigger enter with " + other.gameObject);
        if (!state.grip || state.trigger) return;
        Pokeable pokeable = other.GetComponent<Pokeable>();
        if (pokeable == null) return;
        pokeable.Poke();
    }
}
