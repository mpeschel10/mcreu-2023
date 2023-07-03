using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pillars : MonoBehaviour
{
    [SerializeField] GameObject sourceCell;
    [SerializeField] Material winMaterial;
    [SerializeField] FireworksOnOff fireworks;
    [SerializeField] Scoreboard scoreboard;
    int count = 50;
    public PillarCover[] pillarCovers;
    public float[] heights;
    [SerializeField] GameObject maximumMarker;
    void Start()
    {
        float pillarWidth = sourceCell.transform.lossyScale.x;
        Vector3 start = sourceCell.transform.position;
        start += Vector3.left * pillarWidth * 0.5f * count;

        pillarCovers = new PillarCover[count + 4];
        for (int i = 0; i < pillarCovers.Length - 0; i++)
        {
            Vector3 offset = i * Vector3.right * pillarWidth;
            GameObject cell = Object.Instantiate(sourceCell, start + offset, sourceCell.transform.rotation, transform);
            
            PillarCover pillarCover = cell.GetComponentInChildren<PillarCover>();
            pillarCovers[i] = pillarCover;
            pillarCover.index = i;
            pillarCover.parent = this;
        }
        pillarCovers[0].gameObject.SetActive(false);
        pillarCovers[pillarCovers.Length - 1].gameObject.SetActive(false);

        heights = new float[count + 4];
        for (int i = 0; i < heights.Length; i++)
            heights[i] = float.NaN;
        heights[0] = 0f;
        heights[1] = MIN_HEIGHT;
        heights[heights.Length - 2] = MIN_HEIGHT;
        heights[heights.Length - 1] = 0f;
        
        sourceCell.SetActive(false);
        scoreboard.SetCost(0);
        scoreboard.SetBest(GetBest(count));
    }

    float MAX_HEIGHT = 1f, MIN_HEIGHT = 0.001f;
    int SeekCollapsed(int index, int direction)
    {
        while (heights[index].Equals(float.NaN))
        {
            index += direction;
        }
        return index;
    }

    int SeekCollapsedBelow(int index, int direction, float peak)
    {
        while (!(heights[index] < peak))
        {
            index += direction;
        }
        return index;
    }

    int cost = 0;
    public void Collapse(int index)
    {
        float height = -1f;
        if (cost == 0)
        {
            height = 0.5f;
        } else {
            int leftRegionBoundary = SeekCollapsed(index, -1);
            int rightRegionBoundary = SeekCollapsed(index, 1);
            
            float left = heights[leftRegionBoundary];
            float right = heights[rightRegionBoundary];
            int leftEdgeBoundary = SeekCollapsedBelow(leftRegionBoundary, -1, left);
            int rightEdgeBoundary = SeekCollapsedBelow(rightRegionBoundary, 1, right);

            int spanIfAbove = rightRegionBoundary - leftRegionBoundary - 1;
            int spanIfBetween = right > left ? rightEdgeBoundary - index - 1 : index - leftEdgeBoundary - 1;
            //int spanIfBelow; // This is always <= spanIfBetween, but might add flavor if I have time to implement it.

            if (spanIfAbove > spanIfBetween)
            {
                // height must right < height.
                height = System.Math.Max(right, left) + 0.1f;
            } else {
                height = (right + left) / 2;
            }
        }
        heights[index] = height;
    }

    public int GetBest(int count)
    {
        int iterations = 0;
        while (count > 0)
        {
            if (count > 2)
            {
                count -= 2;
                int lesser = count / 2;
                int greater = count - lesser;
                count = greater;
                iterations += 2;
            } else {
                iterations += count;
                count = 0;
            }
        }
        return iterations;
    }

    float maximumSoFar = 0;
    public void Reveal(int index)
    {
        Collapse(index);
        PillarCover pillarCover = pillarCovers[index];
        Vector3 oldSize = pillarCover.transform.localScale;
        float height = heights[index];
        pillarCover.pillarOffset.transform.localScale = new Vector3(oldSize.x, height, oldSize.z);
        if (height > maximumSoFar)
        {
            Vector3 oldPosition = maximumMarker.transform.localPosition;
            Transform cell = pillarCover.transform.parent.parent;
            maximumMarker.transform.localPosition = new Vector3(cell.localPosition.x, oldPosition.y, oldPosition.z);
            maximumSoFar = height;
        }
        pillarCover.Reveal();
    }

    public bool pairsHint = false;
    private bool _maximumHint = false;
    public bool maximumHint {
        get { return _maximumHint; }
        set {
            _maximumHint = value;
            maximumMarker.SetActive(value);
        }
    }

    public void Click(int index)
    {
        Reveal(index);
        cost++;

        if (pairsHint)
        {
            int pair = -1;
            if (index > 0 && float.IsNaN(heights[index - 1]))
                pair = index - 1;
            else if (index < heights.Length - 1 && float.IsNaN(heights[index + 1]))
                pair = index + 1;
            if (pair != -1)
            {
                Reveal(pair);
                cost++;
            }
        }

        scoreboard.SetCost(cost);
        CheckWin();
    }

    public bool won = false;
    public void CheckWin()
    {
        if (won) return;
        for (int i = 1; i < heights.Length - 1; i++)
        {
            if (heights[i - 1] <= heights[i] && heights[i] >= heights[i + 1])
            {
                Debug.Log("Won");
                pillarCovers[i].pillar.GetComponent<MeshRenderer>().material = winMaterial;
                fireworks.enabled = true;
                Invoke(nameof(turnOffFireworks), 7);
                won = true;
            }
        }
    }

    public void turnOffFireworks() { fireworks.enabled = false; }
}
