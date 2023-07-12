using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableRotate : MonoBehaviour, MouseSelector.Clickable
{
    [SerializeField] GameObject table;
    public void Click()
    {
        table.transform.rotation *= Quaternion.Euler(0, 90, 0);
    }
}
