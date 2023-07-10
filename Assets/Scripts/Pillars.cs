using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pillars : MonoBehaviour
{
    [SerializeField] GameObject sourceCell, scaleCell;
    [SerializeField] Material winMaterial;
    [SerializeField] FireworksOnOff fireworks;
    [SerializeField] Scoreboard scoreboard;
    int count = 40;
    public PillarCover[] pillarCovers;
    public float[] heights;
    GameObject[] pillarObjects;
    [SerializeField] GameObject maximumMarker;
    int cost = 0;
    float maximumSoFar = 0;
    public bool won = false;
    int nextIndex = -1;
    void Awake()
    {
        MakePillars();
        MakeHeights();
        ResetVariables();
        
        sourceCell.SetActive(false);
        FixNext();
    }

    void ResetVariables()
    {
        cost = 0;
        scoreboard.cost = cost;
        scoreboard.best = GetBest(count);
        maximumSoFar = 0;
        won = false;
        nextStepHint = false;
    }

    public void Reset()
    {
        foreach (GameObject g in pillarObjects)
            Destroy(g);
        sourceCell.SetActive(true);
        Awake();
    }

    float pillarWidth;
    void MakePillars()
    {
        float minimumWidth = scaleCell.transform.lossyScale.x;
        float targetSpan = 1f; // Width of table.
        pillarWidth = Mathf.Clamp(targetSpan / (float) count, minimumWidth, float.PositiveInfinity);
        Vector3 oldScale = scaleCell.transform.localScale;
        scaleCell.transform.localScale = new Vector3(pillarWidth, oldScale.y, oldScale.z);
        
        Vector3 start = sourceCell.transform.position;
        start += Vector3.left * pillarWidth * 0.5f * (count + 4);

        pillarCovers = new PillarCover[count + 4];
        pillarObjects = new GameObject[count + 4];
        for (int i = 0; i < pillarCovers.Length - 0; i++)
        {
            Vector3 offset = i * Vector3.right * pillarWidth;
            GameObject cell = Object.Instantiate(sourceCell, start + offset, sourceCell.transform.rotation, transform);
            pillarObjects[i] = cell;
            
            PillarCover pillarCover = cell.GetComponentInChildren<PillarCover>();
            pillarCovers[i] = pillarCover;
            pillarCover.index = i;
            pillarCover.parent = this;
            pillarCover.UpdateDisplayIndex();
        }
        pillarCovers[0].transform.parent.parent.parent.gameObject.SetActive(false);
        pillarCovers[1].transform.parent.parent.parent.gameObject.SetActive(false);
        pillarCovers[pillarCovers.Length - 2].transform.parent.parent.parent.gameObject.SetActive(false);
        pillarCovers[pillarCovers.Length - 1].transform.parent.parent.parent.gameObject.SetActive(false);
    }

    void MakeHeights()
    {
        heights = new float[count + 4];
        for (int i = 0; i < heights.Length; i++)
            heights[i] = float.NaN;
        heights[0] = 0f;
        heights[1] = MIN_HEIGHT;
        heights[heights.Length - 2] = MIN_HEIGHT;
        heights[heights.Length - 1] = 0f;
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

    public static float Interpolate(int index, int leftIndex, int rightIndex, float leftHeight, float rightHeight)
    {
        float span = rightIndex - leftIndex;
        // Debug.Log("Span: " + span);
        float deltaH = (rightHeight - leftHeight) / span;
        // Debug.Log("delta height: " + deltaH);
        return deltaH * (index - leftIndex) + leftHeight;
    }
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

            if (spanIfAbove >= spanIfBetween)
            {
                bool closerToRight = rightEdgeBoundary - index < index - leftEdgeBoundary;
                if (closerToRight)
                {
                    leftRegionBoundary += 1;
                    left = MAX_HEIGHT;
                } else {
                    rightRegionBoundary -= 1;
                    right = MAX_HEIGHT;
                }
            }
            height = Interpolate(index, leftRegionBoundary, rightRegionBoundary, left, right);
        }
        heights[index] = height;
    }

    private bool _hideHint = false;
    public bool hideHint
    {
        get { return _hideHint; }
        set
        {
            _hideHint = value;
            if (_hideHint)
                HideBadPillars();
            else
                for (int i = 2; i < pillarCovers.Length - 2; i++)
                    pillarCovers[i].gameObject.SetActive(!pillarCovers[i].IsRevealed());
        }
    }

    (int, int, int) GetPeakZone()
    {
        float maximum = 0;
        int maximumIndex = 0;
        for (int i = 2; i < heights.Length - 2; i++)
        {
            if (heights[i] > maximum)
            {
                maximum = heights[i];
                maximumIndex = i;
            }
        }
        if (maximumIndex == 0) return (-1, -1, -1);
        int leftBorder = SeekCollapsed(maximumIndex - 1, -1);
        int rightBorder = SeekCollapsed(maximumIndex + 1, 1);
        return (leftBorder, maximumIndex, rightBorder);
    }

    void HideBadPillars()
    {
        (int left, int max, int right) = GetPeakZone();
        if (max == -1) return;
        for (int i = 2; i < pillarCovers.Length - 2; i++)
            if (!pillarCovers[i].IsRevealed())
                pillarCovers[i].gameObject.SetActive(left < i && i < right);
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
            Transform cell = pillarCover.transform.parent.parent.parent;
            maximumMarker.transform.localPosition = new Vector3(cell.localPosition.x, oldPosition.y, oldPosition.z);
            maximumSoFar = height;
            // Debug.Log(oldPosition);
            // Debug.Log(cell.localPosition.x);
        }
        if (_hideHint)
            HideBadPillars();
        pillarCover.Reveal();
    }

    public bool pairsHint { get; set; }
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

        scoreboard.cost = cost;
        CheckWin();
        FixNext();
    }

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

    bool _nextStepHint = false;
    public bool nextStepHint
    {
        get { return _nextStepHint; }
        set {
            _nextStepHint = value;
            FixNext();
        }
    }
    void FixNext()
    {
        HideNext();
        if (_nextStepHint) ShowNext();
    }


    void HideNext()
    {
        if (nextIndex != -1)
            SetNext(nextIndex, false);
    }

    void SetNext(int index, bool value)
    {
        pillarCovers[index].GetComponent<CanClickHoverableMaterial>().SetNext(value);
        if (value)
        {
            if (nextIndex != -1)
                throw new System.Exception("Should not SetNext while a next already exists. Call HideNext first. nextIndex: " + nextIndex);
            nextIndex = index;
        } else {
            nextIndex = -1;
        }
    }

    void ShowNext()
    {
        if (won) return;
        if (pillarCovers.Length == 4) return;
        if (pillarCovers.Length == 5)
        {
            SetNext(2, true);
            return;
        }
        
        (int left, int max, int right) = GetPeakZone();
        if (max == -1)
        {
            SetNext(pillarCovers.Length / 2, true);
            return;
        }

        int left_span = max - left;
        int right_span = right - max;
        if (left_span != 1 && right_span != 1)
        {
            // The peak is alone and does not effectively divide the array.
            // Next step is to reveal something next to the peak and bisect.
            int direction_of_larger_span = right_span > left_span ? 1 : -1;
            SetNext(max + direction_of_larger_span, true);
        } else {
            if (left_span > right_span)
            {
                left = left + 1;
                right = max - 1;
            } else {
                left = max + 1;
                right = right - 1;
            }
            if (left == right)
                SetNext(left, true);
            else
                SetNext((left + right) / 2 + 1, true);
        }
    }

    public void turnOffFireworks() { fireworks.enabled = false; }

    public int PositionToIndex(Vector3 worldSpacePosition)
    {
        Vector3 localPosition = transform.InverseTransformPoint(worldSpacePosition);
        Vector3 localLeftEdge = transform.InverseTransformPoint(pillarCovers[2].transform.position);
        float xOffset = localPosition.x - localLeftEdge.x;
        return (int) Mathf.Round(xOffset / pillarWidth);
    }
}
