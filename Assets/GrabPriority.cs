using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabPriority : MonoBehaviour
{
    [SerializeField] int grabPriority = 10;
    public int Main() { return grabPriority; }
}
