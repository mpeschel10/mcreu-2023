using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pillars : MonoBehaviour
{
    [SerializeField] GameObject sourceCell;
    [SerializeField] Material winMaterial;
    [SerializeField] FireworksOnOff fireworks;
    int count = 50;
    PillarCover[] pillarCovers;
    float[] heights;
    void Start()
    {
        float pillarWidth = sourceCell.transform.lossyScale.x;
        Vector3 start = sourceCell.transform.position;
        Debug.Log(start);
        start += Vector3.left * pillarWidth * 0.5f * count;
        Debug.Log(start);

        pillarCovers = new PillarCover[count + 2];
        for (int i = 1; i < pillarCovers.Length - 1; i++)
        {
            Vector3 offset = i * Vector3.right * pillarWidth;
            GameObject cell = Object.Instantiate(sourceCell, start + offset, sourceCell.transform.rotation, transform);
            
            PillarCover pillarCover = cell.GetComponentInChildren<PillarCover>();
            pillarCovers[i] = pillarCover;
            pillarCover.index = i;
            pillarCover.parent = this;
        }

        heights = new float[count + 2];
        for (int i = 0; i < heights.Length; i++)
            heights[i] = float.NaN;
        heights[0] = 0f;
        heights[heights.Length - 1] = 0f;
        
        sourceCell.SetActive(false);
    }

    float MAX_HEIGHT = 1f, MIN_HEIGHT = 0.001f;
    public void Collapse(int index)
    {
        index -= 1;
        float height = index / (float) (count - 1); // [0, 1];
        height = height * (MAX_HEIGHT - MIN_HEIGHT) + MIN_HEIGHT; // [MIN_HEIGHT, MAX_HEIGHT]
        index += 1;

        heights[index] = height;
    }

    public void Click(int index)
    {
        Collapse(index);
        PillarCover pillarCover = pillarCovers[index];
        Vector3 oldSize = pillarCover.transform.localScale;
        pillarCover.pillarOffset.transform.localScale = new Vector3(oldSize.x, heights[index], oldSize.z);
        pillarCover.Reveal();
        CheckWin();
    }

    bool won = false;
    public void CheckWin()
    {
        for (int i = 1; i < heights.Length - 1; i++)
        {
            if (heights[i - 1] <= heights[i] && heights[i] >= heights[i + 1])
            {
                pillarCovers[i].pillar.GetComponent<MeshRenderer>().material = winMaterial;
                fireworks.enabled = true;
                Invoke(nameof(turnOffFireworks), 7);
            }
        }
    }

    public void turnOffFireworks() { fireworks.enabled = false; }
}
