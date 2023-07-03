using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuplicateChild : MonoBehaviour
{
    [SerializeField] GameObject target;
    [SerializeField] float scaleFactor = 1f / 2048f;
    void Start()
    {
        GameObject newChild = Object.Instantiate(target, transform);
        newChild.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
    }
}
