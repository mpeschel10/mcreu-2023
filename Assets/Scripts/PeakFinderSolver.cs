using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeakFinderSolver : MonoBehaviour
{
    [SerializeField] Pillars pillars;
    [SerializeField] GameObject marker;
    public void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            SolveOneStep();
        }
    }

    public int SeekCollapsed(int startIndex)
    {
        for (int i = startIndex; i < pillars.heights.Length; i++)
        {
            if (!float.IsNaN(pillars.heights[i]))
                return i;
        }
        return pillars.heights.Length;
    }

    Color[] colors = new Color[] {
        Color.black,
        Color.cyan,
        Color.magenta,
        Color.gray,
        Color.white,
        Color.blue,
        Color.magenta,
        Color.red,
        Color.green,
        Color.yellow,
    };

    Color workingColor;
    public void Mark(int index)
    {
        Transform target = pillars.pillarCovers[index]?.transform;
        if (target == null) return;
        GameObject gameObject = Object.Instantiate(marker, target.position + Vector3.up * 0.25f, target.rotation);
        gameObject.GetComponent<MeshRenderer>().material.color = workingColor;
    }

    public void Mark((int, int, int) triad)
    {
        (int x, int y, int z) = triad;
        Mark(x); Mark(y); Mark(z);
    }

    public int SeekLarger(int i)
    {
        for (int j = i + 1; j < pillars.heights.Length; j++)
        {
            if (pillars.heights[j] >= pillars.heights[i])
                return j;
        }
        return pillars.heights.Length;
    }

    public int SeekSmaller(int i)
    {
        for (int j = i + 1; j < pillars.heights.Length; j++)
        {
            if (pillars.heights[j] <= pillars.heights[i])
                return j;
        }
        return pillars.heights.Length;
    }

    public List<(int, int, int)> FindTriads()
    {
        List<(int, int, int)> triads = new List<(int, int, int)>();
        for (int leftMost = SeekCollapsed(0); leftMost < pillars.heights.Length; )
        {
            int peak = SeekLarger(leftMost);
            int valley = SeekSmaller(peak);
            if (valley < pillars.heights.Length)
            {
                triads.Add((leftMost, peak, valley));
            }
            leftMost = SeekCollapsed(leftMost + 1);
        }
        return triads;
    }

    public (int, int, int) FindSmallestTriad()
    {
        (int, int, int) bestTriad = (int.MinValue, 0, int.MaxValue);
        int bestSize = int.MaxValue;
        foreach ((int, int, int) triad in FindTriads())
        {
            int size = 12;
            if (size < bestSize)
            {
                bestSize = size;
                bestTriad = triad;
            }
        }
        return bestTriad;
    }


    bool first = true;
    int colorIndex = 0;
    public void SolveOneStep()
    {
        colorIndex++;
        colorIndex = colorIndex % colors.Length;
        workingColor = colors[colorIndex];
        if (first)
        {
            pillars.Click(pillars.heights.Length / 2);
            first = false;
        } else {
            (int, int, int) triad = FindSmallestTriad();
            Mark(triad);
            // (int leftIndex, int peakIndex, int rightIndex) 
        }
        Debug.Log("Step");
    }
}
