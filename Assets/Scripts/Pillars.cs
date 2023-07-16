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
    [System.NonSerialized] public bool won = false;
    int nextIndex = -1;
    [SerializeField] GameObject pillarControllerObject;
    PillarControllerState pillarController;
    void Awake()
    {
        pillarController = pillarControllerObject.GetComponent<PillarControllerState>();
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
        scoreboard.best = GetBest(count).ToString();
        won = false;
        nextStepHint = false;
    }

    public void Reset()
    {
        foreach (GameObject g in pillarObjects)
            Destroy(g);
        sourceCell.SetActive(true);
        Awake();
        pillarController.Fix();
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
        start += sourceCell.transform.right * -1 * pillarWidth * 0.5f * (count + 4);

        pillarCovers = new PillarCover[count + 4];
        pillarObjects = new GameObject[count + 4];
        for (int i = 0; i < pillarCovers.Length - 0; i++)
        {
            Vector3 offset = i * sourceCell.transform.right * pillarWidth;
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

    int SeekInflection(int index, int direction)
    {
        float target = heights[index];
        index += direction;
        while (!(heights[index] <= target))
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
        float height;
        if (cost == 1)
        {
            height = 0.5f;
        } else {
            int leftBoundaryIfAbove = SeekCollapsed(index, -1);
            int rightBoundaryIfAbove = SeekCollapsed(index, 1);
            
            float left = heights[leftBoundaryIfAbove];
            float right = heights[rightBoundaryIfAbove];
            int leftBoundaryIfBetween = SeekInflection(leftBoundaryIfAbove, -1);
            int rightBoundaryIfBetween = SeekInflection(rightBoundaryIfAbove, 1);

            int spanIfAbove = rightBoundaryIfAbove - leftBoundaryIfAbove;
            int spanIfBetween = right > left ? rightBoundaryIfBetween - index : index - leftBoundaryIfBetween;
            //int spanIfBelow; // This is always <= spanIfBetween, but might add flavor if I have time to implement it.

            // Debug.Log(spanIfAbove + " >= " + spanIfBetween);
            if (spanIfAbove >= spanIfBetween)
            {
                // Debug.Log("Span if above > spanifbetween");
                bool closerToRight = rightBoundaryIfAbove - index < index - leftBoundaryIfAbove;
                // Debug.Log((rightBoundaryIfAbove - index) + " < " + (index - leftBoundaryIfAbove));
                if (closerToRight)
                {
                    // Debug.Log("Closer to right");
                    leftBoundaryIfAbove += 1;
                    left = MAX_HEIGHT;
                } else {
                    // Debug.Log("Closer or equal to left");
                    rightBoundaryIfAbove -= 1;
                    right = MAX_HEIGHT;
                }
            } else {
                // Debug.Log("Span if above < spanifbetween");
            }
            // Debug.Log("Interpolate args: " + index + ", " + leftBoundaryIfAbove + ", " + rightBoundaryIfAbove + ", " + left + ", " + right);
            height = Interpolate(index, leftBoundaryIfAbove, rightBoundaryIfAbove, left, right);
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
        FixMaximum();
        if (_hideHint)
            HideBadPillars();
        pillarCover.Reveal();
    }

    bool _pairsHint = false;
    public bool pairsHint
    {
        get => _pairsHint;
        set
        {
            // Debug.Log("Pairs hint is being set to " + value);
            _pairsHint = value;
        }
    }
    private bool _maximumHint = false;
    public bool maximumHint {
        get { return _maximumHint; }
        set {
            _maximumHint = value;
            FixMaximum();
        }
    }

    public void FixMaximum()
    {
        float bestHeight = 0;
        for (int i = 1; i < heights.Length - 1; i++)
        {
            float height = heights[i];
            if (height > bestHeight)
            {
                Transform pillar = pillarCovers[i].pillar.transform;
                Transform cell = pillar.parent.parent.parent;
                float distanceInFrontOfCell = pillarWidth;
                float markerHeight = maximumMarker.transform.GetChild(0).lossyScale.y;
                // Debug.Log("Setting amximmu marker with respect to pillar " + pillar.gameObject + " and cell " + cell.gameObject);
                float distanceAboveCell = pillar.transform.lossyScale.y / 2f + markerHeight;
                distanceAboveCell = Mathf.Clamp(distanceAboveCell, 0, MAX_HEIGHT / 2f * scaleCell.transform.localScale.y);
                // Debug.Log("pillar y: " + pillar.transform.position.y + " height offset: " + pillar.transform.lossyScale.y / 2f + " My offset: " + markerHeight);
                Vector3 position = pillar.transform.position;
                position += cell.transform.up * distanceAboveCell + cell.transform.forward * distanceInFrontOfCell * -1;
                maximumMarker.transform.position = position;
                bestHeight = height;
                // Debug.Log(oldPosition);
                // Debug.Log(cell.localPosition.x);
            }
        }
        // Debug.Log("Fixing maximum; SetActive(" + (maximumHint && cost > 0) + ")");
        maximumMarker.SetActive(maximumHint && cost > 0);
    }

    public void Click(int index)
    {
        // Debug.Log("Click index " + index);
        cost++;
        Reveal(index);

        if (pairsHint)
        {
            int pair = -1;
            if (index > 0 && float.IsNaN(heights[index - 1]))
                pair = index - 1;
            else if (index < heights.Length - 1 && float.IsNaN(heights[index + 1]))
                pair = index + 1;
            if (pair != -1)
            {
                cost++;
                Reveal(pair);
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
                // Debug.Log("Won");
                won = true;
                pillarController.OnWin();
                pillarCovers[i].pillar.GetComponent<MeshRenderer>().material = winMaterial;
                fireworks.enabled = true;
                Invoke(nameof(turnOffFireworks), 7);
                break;
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
