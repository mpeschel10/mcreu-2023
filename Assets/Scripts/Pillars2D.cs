using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pillars2D : MonoBehaviour
{
    int gridWidth = 16, gridHeight = 16;
    PillarCover2[][] pillarCovers;
    GameObject[][] pillarObjects;
    float[][] heights;
    [SerializeField] GameObject sourceCell;
    [SerializeField] FireworksOnOff fireworks;
    [SerializeField] Material winMaterial;
    [SerializeField] Scoreboard scoreboard;
    Adversary adversary;
    void Awake()
    {
        adversary = GetComponent<Adversary>();
    }
    void Start()
    {
        won = false;
        scoreboard.cost = 0;
        sourceCell.SetActive(true);
        if (pillarObjects != null)
        {
            foreach (GameObject[] row in pillarObjects)
                foreach (GameObject o in row)
                    Destroy(o);
            pillarObjects = null;
            pillarCovers = null;
            heights = null;
        }

        pillarCovers = new PillarCover2[gridHeight + 2][];
        pillarObjects = new GameObject[gridHeight + 2][];
        heights = new float[gridHeight + 2][];

        float pillarWidth = 1 / 32f;

        Vector3 backLeft = sourceCell.transform.position;
        backLeft += sourceCell.transform.right * -1 * (gridWidth + 1) * 0.5f * pillarWidth;
        backLeft += sourceCell.transform.forward * -1 * (gridHeight + 1) * 0.5f * pillarWidth;
        Quaternion q = sourceCell.transform.rotation;

        for (int r = 0; r < heights.Length; r++)
        {
            PillarCover2[] coverRow = new PillarCover2[gridWidth + 2];
            GameObject[] objectRow = new GameObject[gridWidth + 2];
            float[] heightsRow = new float[gridWidth + 2];
            
            pillarCovers[r] = coverRow;
            pillarObjects[r] = objectRow;
            heights[r] = heightsRow;
            for (int c = 0; c < coverRow.Length; c++)
            {
                if (c == 0 || r == 0 || c == heightsRow.Length - 1 || r == heights.Length - 1)
                {
                    heightsRow[c] = 0;
                    continue;
                }
                Vector3 v = backLeft + (r * Vector3.right + c * Vector3.forward) * pillarWidth;
                GameObject g = Object.Instantiate(sourceCell, v, q, transform);
                
                PillarCover2 p = g.GetComponentInChildren<PillarCover2>();
                p.r = r; p.c = c;

                objectRow[c] = g;
                coverRow[c] = p;
                heightsRow[c] = float.NaN;
            }
        }
        sourceCell.SetActive(false);

        adversary.Reset(heights, pillarCovers);
    }

    public static Color HeightToColor(float height)
    {
        // assume height [0, 1]
        // height *= height;
        float hue = height * 0.8f; // Looks better if highest point is purple instead of cycling back to red
        float saturation = 0.7f + height * 0.3f;
        float value = 0.25f + height * 0.25f;
        return Color.HSVToRGB(hue, saturation, value);
    }

    const float MAX_HEIGHT = 0.3f * 0.75f;

    public Color GetColor(int r, int c) { return HeightToColor(heights[r][c]); }
    void Collapse(int r, int c)
    {
        float height = GetComponent<Adversary>().CollapseRandomWalk(heights, r, c);
        // float height = GetComponent<Adversary>().CollapseSinewaves(heights, r, c);
        // Debug.Log("setting height to " + height);
        
        Transform t = pillarCovers[r][c].pillarParent.transform;
        Vector3 o = t.localScale;
        t.localScale = new Vector3(o.x, height * height * MAX_HEIGHT, o.z);
        pillarCovers[r][c].color = GetColor(r, c);
    }

    bool _hintHide = false;
    bool hintHide
    {
        get => _hintHide;
        set
        {
            _hintHide = value;
            HideEliminated();
        }
    }
    void HideEliminated()
    {
        adversary.AssignRegions(heights);
        adversary.AssignIsCellEliminated();
        for (int r = 1; r < pillarObjects.Length - 1; r++)
        {
            bool[] eliminatedRow = adversary.isCellEliminated[r];
            GameObject[] pillarRow = pillarObjects[r];
            for (int c = 1; c < pillarRow.Length - 1; c++)
            {
                pillarRow[c].SetActive(!hintHide || !eliminatedRow[c]);
            }
        }
    }

    public void Click(int r, int c)
    {
        // Debug.Log("Click " + r + " " + c);
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (!Input.GetMouseButtonDown(0))
                return;
            GetComponent<Adversary>().AssignRegions(heights);
            string strArray = "";
            foreach (Adversary.RegionCell cell in GetComponent<Adversary>().GetRegionBorder(r, c))
            {
                float height = heights[cell.r][cell.c];
                strArray += height.ToString() + ", ";
            }
            // Debug.Log(strArray);
        }
        else
        {
            Collapse(r, c);
            pillarCovers[r][c].isRevealed = true;
            scoreboard.cost += 1;
            CheckWin();
        }
        HideEliminated();
    }

    (int, int)[] cardinalOffsets = {
        (-1, -1), ( 0, -1), (1, -1),
        (-1,  0),           (1,  0),
        (-1,  1), ( 0,  1), (1,  1),
    };
    Vector2[] nearbyOffsets = {
        new Vector2(-1, -1), new Vector2( 0, -1), new Vector2( 1, -1), 
        new Vector2(-1,  0),                      new Vector2( 1,  0), 
        new Vector2(-1,  1), new Vector2( 0,  1), new Vector2( 1,  1), 
    };

    bool IsRevealed(int r, int c)
    {
        PillarCover2 p = pillarCovers[r][c];
        return p != null ? p.isRevealed : true;
    }
    
    bool IsPeak(int r, int c)
    {
        if (!IsRevealed(r, c)) return false;
        
        float myValue = heights[r][c];
        foreach ((int rOff, int cOff) in cardinalOffsets)
        {
            int rComp = r + rOff, cComp = c + cOff;
            float comparisonValue = heights[rComp][cComp];
            if ( (!IsRevealed(rComp, cComp)) || (comparisonValue > myValue) )
                return false;
        }
        return true;
    }

    bool won;
    public void CheckWin()
    {
        if (won) return;
        Vector2 position = new Vector2(1, 1);
        for (Vector2 rowStart = new Vector2(1, 1); rowStart.y < gridHeight + 2 - 1; rowStart += Vector2.up)
        {
            for (Vector2 cell = rowStart; cell.x < gridWidth + 2 - 1; cell += Vector2.right)
            {
                int r = (int) cell.x, c = (int) cell.y;
                if (IsPeak(r, c))
                {
                    Debug.Log("Won!");
                    PillarCover2 cover = pillarCovers[(int) cell.x][(int) cell.y];
                    cover.pillar.gameObject.GetComponent<MeshRenderer>().material = winMaterial;
                    fireworks.enabled = true;
                    Invoke(nameof(StopFireworks), 7);
                    won = true;
                    return;
                }
            }
        }
    }
    
    public void StopFireworks() { fireworks.enabled = false; }
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            Debug.Log("Click all");
            Start();
            for (int r = 1; r < gridHeight + 2 - 1; r++)
            {
                for (int c = 1; c < gridWidth + 2 - 1; c++)
                {
                    Click(r, c);
                }
            }
        }
        if (Input.GetKeyDown("a"))
        {
            GetComponent<Adversary>().AssignRegions(heights);
        }
        if (Input.GetKeyDown("m"))
        {
            Debug.Log("Assigning regions.");
            GetComponent<Adversary>().AssignRegions(heights);
            GetComponent<Adversary>().HideMarkers();
            Debug.Log("Drawing regions.");
            // GetComponent<Adversary>().MarkRegions(pillarCovers);
            GetComponent<Adversary>().MarkEliminated();
        }
        if (Input.GetKeyDown("r"))
        {
            Debug.Log("Assigning regions.");
            GetComponent<Adversary>().AssignRegions(heights);
            GetComponent<Adversary>().HideMarkers();
            Debug.Log("Drawing regions.");
            GetComponent<Adversary>().MarkRegions();
            // GetComponent<Adversary>().MarkEliminated(pillarCovers);
        }
        if (Input.GetKeyDown("e"))
        {
            HideEliminated();
        }
        if (Input.GetKeyDown("h"))
        {
            GetComponent<Adversary>().HideMarkers();
        }
    }

}
