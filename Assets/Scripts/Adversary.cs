using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Adversary : MonoBehaviour
{
    public class RegionCell
    {
        public int r, c, region;
        public RegionCell(int r, int c, int region)
        {
            this.r = r; this.c = c; this.region = region;
        }
        public override string ToString()
        {
            return "RegionCell(" + r + ", " + c + ", " + region + ")";
        }
    }
    (int, int)[] cardinalOffsets = {
        (-1, -1), ( 0, -1), (1, -1),
        (-1,  0),           (1,  0),
        (-1,  1), ( 0,  1), (1,  1),
    };
    int[][] regionMap = null;
    RegionCell[] borderMaximums;
    List<RegionCell> regions;
    List<bool> activeRegions;
    public RegionCell[] GetNeighbors(RegionCell regionCell) { return GetNeighbors(regionCell.r, regionCell.c); }
    RegionCell[] GetNeighbors(int r, int c)
    {
        RegionCell[] neighbors = new RegionCell[cardinalOffsets.Length];
        for (int i = 0; i < cardinalOffsets.Length; i++)
        {
            (int rOff, int cOff) = cardinalOffsets[i];
            neighbors[i] = new RegionCell(r + rOff, c + cOff, regionMap[r + rOff][c + cOff] );
        }
        return neighbors;
    }
    List<RegionCell> GetIndexedNeighbors(RegionCell cell)
    {
        int r = cell.r; int c = cell.c;
        List<RegionCell> cells = new List<RegionCell>();
        foreach (RegionCell neighbor in GetNeighbors(cell.r, cell.c))
        {
            if (neighbor.region < 2) continue;
            while (neighbor.region >= cells.Count)
                cells.Add(null);
            // This will replace regions we see twice, but that's ok
            cells[neighbor.region] = neighbor;
        }
        return cells;
    }

    List<RegionCell> GetBorderingRegions(int r, int c)
    {
        List<RegionCell> cells = GetIndexedNeighbors(new RegionCell(r, c, -1));
        List<RegionCell> cellList = new List<RegionCell>();
        foreach (RegionCell cell in cells)
        {
            if (cell == null) continue;
            cellList.Add(cell);
        }
        return cellList;
    }

    public void AssignBorderMaximums(float[][] heights)
    {
        borderMaximums = new RegionCell[regions.Count];
        for (int i = 0; i < regions.Count; i++)
        {
            if (!activeRegions[i]) continue;
            RegionCell regionStart = regions[i];
            borderMaximums[i] = GetBorderMaximum(GetRegionBorder(regionStart.r, regionStart.c), heights);
        }
    }
    
    public class RegionNode
    {
        public List<RegionNode> outLinks = new List<RegionNode>();
        public List<RegionNode> inLinks = new List<RegionNode>();
        public int regionIndex;
        public RegionCell cell;
        public Adversary parent;
        public RegionNode(Adversary parent, int regionIndex)
        {
            this.parent = parent;
            this.regionIndex = regionIndex;
            cell = parent.regions[regionIndex];
        }

        public override string ToString()
        {
            return "RegionNode(" + parent + ", " + regionIndex + ")";
        }
    }

    public bool HasHigherNeighbor(float[][] heights, RegionCell supposedPeak)
    {
        float myHeight = heights[supposedPeak.r][supposedPeak.c];
        foreach (RegionCell neighbor in GetNeighbors(supposedPeak))
        {
            if (heights[neighbor.r][neighbor.c] >= myHeight)
                return true;
        }
        return false;
    }
    public RegionNode[] regionNodes;
    bool[] isEliminated;
    public void AssignEliminated(float[][] heights)
    {
        regionNodes = new RegionNode[regions.Count];
        isEliminated = new bool[regions.Count];
        for (int i = 0; i < isEliminated.Length; i++)
        {
            isEliminated[i] = !activeRegions[i];
            if (isEliminated[i]) continue;
            regionNodes[i] = new RegionNode(this, i);
        }
        
        for (int i = 0; i < isEliminated.Length; i++)
        {
            if (isEliminated[i]) continue;
            RegionNode node = regionNodes[i];
            RegionCell maximum = borderMaximums[node.regionIndex];
            if (heights[maximum.r][maximum.c] == Pillars2D.MIN_HEIGHT)
                // When game is uninitialized, pretend that there must be a peak even though not really.
                continue;
            // Debug.Log("Considering region " + i);
            // Debug.Log("Region has maximum located at " + maximum);
            // Debug.Log("Maximum is bordered by: ");
            List<RegionCell> neighborCells = GetIndexedNeighbors(maximum);
            foreach (RegionCell cell in neighborCells)
            {
                if (cell == null) continue;
                if (cell.region == node.regionIndex) continue;
                RegionNode neighborNode = regionNodes[cell.region];
                node.outLinks.Add(neighborNode);
                neighborNode.inLinks.Add(node);
            }
        }

        for (int i = 0; i < isEliminated.Length; i++)
        {
            if (isEliminated[i]) continue;
            RegionNode node = regionNodes[i];
            foreach (RegionNode neighbor in node.outLinks)
            {
                if (!neighbor.outLinks.Contains(node))
                {
                    isEliminated[i] = true;
                    break;
                }
            }
            RegionCell cell = borderMaximums[node.regionIndex];
            // This is kind of hacky...
            // Basically, we're looking to see if the ridge from node
            //  "leads" anywhere. Ideally, we would actually follow the ridge
            //  to identify all outLinks and inLinks,
            //  but this works well enough since we never use outLinks etc.
            // Debug.Log("Considering higher neighbors of cell " + cell);
            // MarkCell(cell);
            if (HasHigherNeighbor(heights, cell))
            {
                isEliminated[i] = true;
                // Debug.Log("Cell " + cell + " has higher neighbors.");
            }
            else
            {
                // Debug.Log("Cell " + cell + " has no higher neighbors.");
            }
        }

        string activeNodes = "";
        for (int i = 0; i < activeRegions.Count; i++)
        {
            if (!activeRegions[i]) continue;
            activeNodes += i + ", ";
        }
        // Debug.Log("Active regions: " + activeNodes);
        for (int i = 0; i < activeRegions.Count; i++)
        {
            if (!activeRegions[i]) continue;
            RegionNode node = regionNodes[i];
            // Debug.Log("Region " + i + " connects to " + repr(node.outLinks));
        }

        isEliminated[1] = false; // Do not hide revealed cells
    }

    public static string repr(System.Object o)
    {
        return o.ToString();
    }

    public static string repr<T>(IEnumerable<T> e)
    {
        string buffer = "[";
        foreach (T element in e)
        {
            string s = repr(element);
            if (buffer == "[")
                buffer += s;
            else
                buffer += ", " + s;
        }
        return buffer + "]";
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
                    regions.Add(new RegionCell(r, c, row[c]));
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
                    // Debug.Log("Supposed to do flood fill from cell " + c + ", " + r);
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
        AssignBorderMaximums(heights);
        AssignEliminated(heights);
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
                border.Add(new RegionCell(r, c, regionMap[r][c]) );
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

    void MarkCell(RegionCell cell) { MarkCell(cell.r, cell.c, cell.region); }
    void MarkCell(int r, int c, int region)
    {
        GameObject cover = pillarCovers[r][c].gameObject;
        GameObject marker = Object.Instantiate(regionMarker, transform);
        marker.transform.position = cover.transform.position + Vector3.up;
        marker.GetComponent<MeshRenderer>().material.color = regionColors[region];
        regionMarkers.Add(marker);
        marker.SetActive(true);
    }

    bool IsRegionPeak(int r, int c)
    {
        foreach (RegionCell peak in borderMaximums)
        {
            if (peak == null) continue;
            if (peak.r == r && peak.c == c)
            {
                return true;
            }
        }
        return false;
    }
    
    public void MarkRegions()
    {
        for (int r = 1; r < regionMap.Length - 1; r++)
        {
            int[] row = regionMap[r];
            for (int c = 1; c < row.Length - 1; c++)
            {
                int region = row[c];
                if (region == 0) throw new System.Exception("Should not find uninitialized region cell. What is going on.");
                if (region == 1)
                {
                    continue;
                }
                RegionCell cell = new RegionCell(r, c, region);
                MarkCell(cell);
            }
        }
    }

    public bool[][] isCellEliminated;
    public void AssignIsCellEliminated()
    {
        isCellEliminated = new bool[regionMap.Length][];
        for (int r = 1; r < regionMap.Length - 1; r++)
        {
            int[] rowIn = regionMap[r];
            bool[] rowOut = new bool[rowIn.Length];
            for (int c = 1; c < rowIn.Length - 1; c++)
            {
                rowOut[c] = isEliminated[rowIn[c]];
            }
            isCellEliminated[r] = rowOut;
        }
    }
    public void MarkEliminated()
    {
        for (int r = 1; r < regionMap.Length - 1; r++)
        {
            int[] row = regionMap[r];
            for (int c = 1; c < row.Length - 1; c++)
            {
                int region = row[c];
                // Debug.Log("Region is " + region);
                if (!isEliminated[region]) continue;
                MarkCell(r, c, region);
            }
        }
    }

    // public List<RegionCell> GetRegionsWithPeaks()
    // {
    //     List<RegionCell> l = new List<RegionCell>();
    //     foreach (RegionCell c in regions)
    //     {
            
    //     }
    //     return l;
    // }



    bool collapseInitialized;
    float rOffset, cOffset, rFrequency, cFrequency, rcOffset, rcFrequency;
    float[][] walkHeights;
    PillarCover2[][] pillarCovers;
    public void Reset(float[][] heights, PillarCover2[][] pillarCovers)
    {
        collapseInitialized = false;
        this.pillarCovers = pillarCovers;
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

    void OpenRandom() { Random.InitState(seed); }
    void CloseRandom() { seed = Random.Range(int.MinValue, int.MaxValue); }
    void InitializeRandomWalk(float[][] heights)
    {
        float MAX_HEIGHT = 1f;
        int gridHeight = heights.Length - 2;
        int gridWidth = heights[0].Length - 2;
        
        int rStart = 1, cStart = 1;
        int rEnd = gridHeight - 1, cEnd = gridWidth - 1;
        int cellCount = gridHeight * gridWidth;
        
        OpenRandom();
        int rPeak = Random.Range(rStart, rEnd);
        int cPeak = Random.Range(cStart, cEnd);
        CloseRandom();

        walkHeights = new float[heights.Length][];
        for (int i = 0; i < walkHeights.Length; i++)
        {
            float[] rowIn = heights[i];
            float[] rowOut = new float[rowIn.Length];
            walkHeights[i] = rowOut;
            for (int c = 0; c < rowIn.Length; c++)
            {
                rowOut[c] = rowIn[c];
                // Debug.Log("At " + i + ", " + c + " = " + rowOut[c]);
            }
        }
        RandomWalk(walkHeights, rPeak, cPeak, MAX_HEIGHT, cellCount, cellCount);
        collapseInitialized = true;
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

    int seed = 531;
    void Shuffle<T>(T[] array)
    {
        // Debug.Log("Seed is " + seed);
        OpenRandom();
        for (int i = 0; i < array.Length; i++)
        {
            int swapIndex = Random.Range(i, array.Length);
            T temp = array[i];
            array[i] = array[swapIndex];
            array[swapIndex] = temp;
        }
        CloseRandom();
    }

    public float CollapseRandomWalk(float[][] heights, int r, int c)
    {
        if (!collapseInitialized) InitializeRandomWalk(heights);
        heights[r][c] = walkHeights[r][c];
        return walkHeights[r][c];
    }

}
