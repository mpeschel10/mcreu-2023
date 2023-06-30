using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LessBadXRPokeInteractor : MonoBehaviour
{
    Collider pokeCollider;
    AnimateHands state;
    void Awake() {
        pokeCollider = GetComponent<Collider>();
        state = GetComponentInParent<AnimateHands>();
    }
    public interface Pokeable { void Poke(); }
    void OnTriggerEnter(Collider other)
    {
        if (!state.grip || state.trigger) return;
        Pokeable pokeable = other.GetComponent<Pokeable>();
        if (pokeable == null) return;
        pokeable.Poke();
    }
}
