using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Adversary : MonoBehaviour
{
    public class RegionCell
    {
        public int r, c, region;
    }
    (int, int)[] cardinalOffsets = {
        (-1, -1), ( 0, -1), (1, -1),
        (-1,  0),           (1,  0),
        (-1,  1), ( 0,  1), (1,  1),
    };
    int[][] regionMap = null;
    List<RegionCell> regions;
    List<bool> activeRegions;
    List<RegionCell> GetBorderingRegions(int r, int c)
    {
        List<RegionCell> cells = new List<RegionCell>();
        foreach ((int, int) offsets in cardinalOffsets)
        {
            (int rOff, int cOff) = offsets;
            int adjacent = regionMap[r + rOff][c + cOff];
            if (adjacent < 2) continue;
            while (adjacent >= cells.Count)
                cells.Add(null);
            // This will replace regions we see twice, but that's ok
            cells[adjacent] = new RegionCell { r = r + rOff, c = c + cOff, region = adjacent };
        }
        List<RegionCell> cellList = new List<RegionCell>();
        foreach (RegionCell cell in cells)
        {
            if (cell == null) continue;
            cellList.Add(cell);
        }
        return cellList;
    }
    List<RegionCell> GetBorderingRegionsExcept(int r, int c, int regionToExclude)
    {
        return GetBorderingRegions(r, c).FindAll(cell => cell.region != regionToExclude);
    }

    public void AssignRegions(float[][] heights)
    {
        activeRegions = new List<bool>();
        regions = new List<RegionCell>();
        activeRegions.Add(false); // 0 is uninitialized.
        activeRegions.Add(false); // 1 is cells already revealed.
        regions.Add(null);
        regions.Add(null);
        regionMap = new int[heights.Length][];
        for (int r = 0; r < regionMap.Length; r++)
            regionMap[r] = new int[heights[r].Length];
        for (int r = 0; r < regionMap.Length; r++)
        {
            int[] row = regionMap[r];
            for (int c = 0; c < row.Length; c++)
            {
                // try {
                if (r == 0 || c == 0 || r == regionMap.Length -1 || c == row.Length - 1 || !float.IsNaN(heights[r][c]))
                {
                    row[c] = 1;
                    continue;
                }
                List<RegionCell> borderingRegions = GetBorderingRegions(r, c);
                if (borderingRegions.Count == 0)
                {
                    // Probably a new region. Can be merged later if not.
                    row[c] = activeRegions.Count; // Start numbering at 2.
                    activeRegions.Add(true);
                    regions.Add(new RegionCell { r = r, c = c, region = row[c] });
                }
                else if (borderingRegions.Count == 1)
                {
                    // Flood fill to match existing region
                    row[c] = borderingRegions[0].region;
                }
                else
                {
                    RegionCell primary = borderingRegions[0];
                    row[c] = primary.region;
                    // Regions previously thought separate should be merged.
                    Debug.Log("Supposed to do flood fill from cell " + c + ", " + r);
                    for (int i = 1; i < borderingRegions.Count; i++)
                    {
                        RegionCell cell = borderingRegions[i];
                        FloodFill(cell.r, cell.c, cell.region, primary.region);
                        activeRegions[cell.region] = false;
                        regions[cell.region] = null;
                    }
                }
                // Debug.Log("Assigned " + row[c]);
                // } catch (System.Exception e) {
                //     Debug.Log("Error on cell " + r + ", " + c);
                //     throw e;
                // }
            }
        }
    }

    bool[][] visited;
    void CleanVisited(int rows, int columns)
    {
        visited = new bool[rows][];
        for (int i = 0; i < rows; i++)
        {
            visited[i] = new bool[columns];
        }
    }
    public List<RegionCell> GetRegionBorder(int r, int c)
    {
        CleanVisited(regionMap.Length, regionMap[0].Length);
        List<RegionCell> border = new List<RegionCell>();
        int targetRegion = regionMap[r][c];
        // if (targetRegion == 1) return border;
        
        Queue<(int, int)> targets = new Queue<(int, int)>();
        targets.Enqueue((r, c));
        while (targets.Count > 0)
        {
            (r, c) = targets.Dequeue();
            if (visited[r][c]) continue;
            visited[r][c] = true;
            int region = regionMap[r][c];
            if (region == 1)
            {
                border.Add(new RegionCell { r = r, c = c, region = regionMap[r][c] });
                continue;
            }
            foreach (RegionCell neighbor in GetNeighbors(r, c))
            {
                targets.Enqueue((neighbor.r, neighbor.c));
            }
        }
        return border;
    }

    RegionCell GetBorderMaximum(List<RegionCell> border, float[][] heights)
    {
        float bestHeight = float.NegativeInfinity;
        RegionCell best = null;
        foreach (RegionCell c in border)
        {
            float height = heights[c.r][c.c];
            if (height > bestHeight)
            {
                bestHeight = height;
                best = c;
            }
        }
        return best;
    }

    RegionCell[] GetNeighbors(int r, int c)
    {
        RegionCell[] neighbors = new RegionCell[cardinalOffsets.Length];
        for (int i = 0; i < cardinalOffsets.Length; i++)
        {
            (int rOff, int cOff) = cardinalOffsets[i];
            neighbors[i] = new RegionCell { r = r + rOff, c = c + cOff, region = regionMap[r + rOff][c + cOff] };
        }
        return neighbors;
    }
    void FloodFill(int r, int c, int regionToErase, int overwritingRegion)
    {
        Queue<(int, int)> targets = new Queue<(int, int)>();
        targets.Enqueue((r, c));
        while (targets.Count > 0)
        {
            (r, c) = targets.Dequeue();
            if (regionMap[r][c] != regionToErase)
                continue;
            regionMap[r][c] = overwritingRegion;
            foreach ((int rOff, int cOff) in cardinalOffsets)
            {
                if (regionMap[r + rOff][c + cOff] != regionToErase) continue;
                targets.Enqueue((r + rOff, c + cOff));
            }
        }
    }

    List<GameObject> regionMarkers = new List<GameObject>();
    [SerializeField] GameObject regionMarker;
    Color[] regionColors = {
        Color.red, Color.red, Color.red, Color.green, Color.blue, Color.cyan, Color.magenta, Color.yellow, Color.black,
        Color.red, Color.red, Color.red, Color.green, Color.blue, Color.cyan, Color.magenta, Color.yellow, Color.black,
        Color.red, Color.red, Color.red, Color.green, Color.blue, Color.cyan, Color.magenta, Color.yellow, Color.black,
        Color.red, Color.red, Color.red, Color.green, Color.blue, Color.cyan, Color.magenta, Color.yellow, Color.black,
        Color.red, Color.red, Color.red, Color.green, Color.blue, Color.cyan, Color.magenta, Color.yellow, Color.black,
        Color.red, Color.red, Color.red, Color.green, Color.blue, Color.cyan, Color.magenta, Color.yellow, Color.black,
    };
    public void HideMarkers()
    {
        foreach (GameObject marker in regionMarkers)
        {
            Destroy(marker);
        }
        regionMarkers = new List<GameObject>();
    }

    public void MarkRegions(PillarCover2[][] pillarCovers)
    {
        HideMarkers();
        for (int r = 0; r < regionMap.Length; r++)
        {
            int[] row = regionMap[r];
            for (int c = 0; c < row.Length; c++)
            {
                int region = row[c];
                if (region == 0) throw new System.Exception("Should not find uninitialized region cell. What is going on.");
                if (region == 1) continue;
                GameObject cover = pillarCovers[r][c].gameObject;
                GameObject marker = Object.Instantiate(regionMarker, transform);
                marker.transform.position = cover.transform.position + Vector3.up;
                marker.GetComponent<MeshRenderer>().material.color = regionColors[region];
                regionMarkers.Add(marker);
                marker.SetActive(true);
            }
        }
    }

    bool collapseInitialized;
    float rOffset, cOffset, rFrequency, cFrequency, rcOffset, rcFrequency;
    public void Start()
    {
        collapseInitialized = false;
    }
    void InitializeSinewaves()
    {
        rOffset     = Random.value * 2 * Mathf.PI;
        cOffset     = Random.value * 2 * Mathf.PI;
        rcOffset    = Random.value * 2 * Mathf.PI;
        rFrequency  = Random.value * .5f + 0.5f;
        cFrequency  = Random.value * .5f + 0.5f;
        rcFrequency = Random.value * .5f + 0.5f;
        collapseInitialized = true;
    }
    public float CollapseSinewaves(float[][] heights, int r, int c)
    {
        if (!collapseInitialized) InitializeSinewaves();
        float cf = (c - 1) / (float) (heights[0].Length - 3); // [0, 1]
        float rf = (r - 1) / (float) (heights.Length - 3); // [0, 1]
        
        float rc = (rf + cf) * 0.5f * 2 * Mathf.PI * rcFrequency;
        cf = cf * 2 * Mathf.PI * cFrequency;
        rf = rf * 2 * Mathf.PI * rFrequency;
        
        float height = Mathf.Sin(cf + cOffset) + Mathf.Sin(rf + rOffset) + Mathf.Sin(rc + rcOffset); // [-3, 3]
        height = (height + 3f) / 6f; // [0, 1]

        heights[r][c] = height;
        return height;
    }

    void InitializeRandomWalk(float[][] heights)
    {
        float MAX_HEIGHT = 1f;
        int gridHeight = heights.Length - 2;
        int gridWidth = heights[0].Length - 2;
        
        int rStart = 1, cStart = 1;
        int rEnd = gridHeight - 1, cEnd = gridWidth - 1;
        int cellCount = gridHeight * gridWidth;
        
        int rPeak = Random.Range(rStart, rEnd);
        int cPeak = Random.Range(cStart, cEnd);
        RandomWalk(heights, rPeak, cPeak, MAX_HEIGHT, cellCount, cellCount);
    }

    int RandomWalk(float[][] heights, int rFocus, int cFocus, float maxHeight, int cellsRemaining, int cellsTotal)
    {
        if (!float.IsNaN(heights[rFocus][cFocus])) return cellsRemaining;
        float ratio = (float) cellsRemaining / (float) cellsTotal;
        heights[rFocus][cFocus] = maxHeight * ratio;
        cellsRemaining--;
        
        (int, int)[] offsets =  MakeCardinalOffsets();
        Shuffle(offsets);
        foreach ((int rOff, int cOff) in offsets)
        {
            int r = rFocus + rOff, c = cFocus + cOff;
            cellsRemaining = RandomWalk(heights, r, c, maxHeight, cellsRemaining, cellsTotal);
        }
        return cellsRemaining;
    }

    (int, int)[] MakeCardinalOffsets()
    {
        (int, int)[] offsets = new (int, int)[cardinalOffsets.Length];
        for (int i = 0; i < offsets.Length; i++)
        {
            offsets[i] = cardinalOffsets[i];
        }
        return offsets;
    }

    void Shuffle<T>(T[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            int swapIndex = Random.Range(i, array.Length);
            T temp = array[i];
            array[i] = array[swapIndex];
            array[swapIndex] = temp;
        }
    }

    public float CollapseRandomWalk(float[][] heights, int r, int c)
    {
        if (!collapseInitialized) InitializeRandomWalk(heights);
        return heights[r][c];
    }

}
