using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LessBadXRPokeInteractor : MonoBehaviour
{
    [SerializeField] Collider pokeCollider;
    [SerializeField] AnimateHands state;
    public interface Pokeable { void Poke(); }
    void OnTriggerEnter(Collider other)
    {
        if (!state.grip || state.trigger) return;
        Pokeable pokeable = other.GetComponent<Pokeable>();
        if (pokeable == null) return;
        pokeable.Poke();
    }
}
