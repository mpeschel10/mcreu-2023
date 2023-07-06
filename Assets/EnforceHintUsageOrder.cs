using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnforceHintUsageOrder : MonoBehaviour
{
    static Toggle[] otherHints;
    void Start()
    {
        if (otherHints == null)
        {
            Transform hintsTransform = transform.parent;
            otherHints = new Toggle[hintsTransform.childCount];
        }
        int myIndex = transform.GetSiblingIndex();
        otherHints[myIndex] = GetComponent<Toggle>();
    }

    public void Fix()
    {
        FixStatic();
    }

    public static void FixStatic()
    {
        bool seenOn = false;
        for (int i = otherHints.Length - 1; i > 0; i--)
        {
            Toggle t = otherHints[i];
            if (seenOn)
            {
                t.interactable = true;
                continue;
            }
            Toggle precedingT = otherHints[i - 1];
             if (!precedingT.isOn && !t.isOn) {
                t.interactable = false;
            } else {
                otherHints[i].interactable = true;
                seenOn = true;
            }
        }
        otherHints[0].interactable = true;
    }
}
