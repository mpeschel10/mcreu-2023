using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pillars : MonoBehaviour
{
    [SerializeField] GameObject sourceCell;
    int count = 10;
    void Start()
    {
        float pillarWidth = sourceCell.transform.localScale.x;
        Vector3 start = sourceCell.transform.position + Vector3.left * pillarWidth * 0.5f;
        for (int i = 0; i < count; i++)
        {
            Vector3 offset = i * Vector3.right * pillarWidth;
            GameObject cell = Object.Instantiate(sourceCell, start + offset, sourceCell.transform.rotation, transform);
            // cell.transform.SetParent(this.transform);
        }
        sourceCell.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
