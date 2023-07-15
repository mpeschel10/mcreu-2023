using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum Scenario : int
{
    Tutorial = 0,
    Maximum = 1,
    Eliminate = 2,
    Ruler = 3,
    Pairs = 4,
    Next = 5,
}
public class PillarControllerState : MonoBehaviour, NextButton.HasOnNext
{
    [SerializeField] Pillars pillars;
    [SerializeField] GameObject nextButton, nextButtonVR;
    [SerializeField] Transform activeGroup, inactiveGroup;
    [SerializeField] GameObject menuInstructionVR, menuInstructionPC, controlsInstructionVR, controlsInstructionPC, goalInstruction;
    [SerializeField] GameObject maximumInstruction, eliminateInstruction, rulerInstruction, pairsInstruction, nextInstruction;
    Scenario scenario = Scenario.Tutorial;

    GameObject[] allInstructions;
    GameObject[] basicInstructionsPC, basicInstructionsVR, hintInstructions;
    [SerializeField] MenuToggle[] menuToggles;
    [SerializeField] GameObject ruler, scoreBoard;
    
    void Awake()
    {
        allInstructions = new GameObject[]
        {
            menuInstructionPC, menuInstructionVR, controlsInstructionPC, controlsInstructionVR, goalInstruction,
            maximumInstruction, eliminateInstruction, rulerInstruction, pairsInstruction, nextInstruction,
        };
        basicInstructionsPC = new GameObject[]
        {
            menuInstructionPC, controlsInstructionPC, goalInstruction,
        };
        basicInstructionsVR = new GameObject[]
        {
            menuInstructionVR, controlsInstructionVR, goalInstruction,
        };
        hintInstructions = new GameObject[]
        {
            maximumInstruction, eliminateInstruction, rulerInstruction, pairsInstruction, nextInstruction,
        };
    }

    void Start()
    {
        // Debug.Log("Fixing pillar control state start");
        Fix();
    }

    public void OnWin()
    {
        Debug.Log("pillar controller Won.");
        Fix();
    }

    void CleanActiveness()
    {
        foreach (GameObject g in allInstructions)
        {
            g.SetActive(false);
        }
    }

    public void OnNext()
    {
        if ((int) scenario < (int) Scenario.Next)
        {
            scenario++;
        }
        
        pillars.Reset(); // Sets won to false; call BEFORE Fix
        Fix();
        
        // Open the menu to the curren instruction.
        CleanActiveness();
        if (scenario == Scenario.Tutorial)
        {
            goalInstruction.SetActive(true);
        }
        else if (scenario == Scenario.Maximum)
        {
            maximumInstruction.SetActive(true);
        }
        else if (scenario == Scenario.Eliminate)
        {
            eliminateInstruction.SetActive(true);
        }
        else if (scenario == Scenario.Ruler)
        {
            rulerInstruction.SetActive(true);
        }
        else if (scenario == Scenario.Pairs)
        {
            pairsInstruction.SetActive(true);
        }
        else if (scenario == Scenario.Next)
        {
            nextInstruction.SetActive(true);
        }
        else
        {

        }
        
        GameControllerState.menuLocation = GameControllerState.isXR ? MenuLocation.LeftHand : MenuLocation.FullScreen;
        foreach (MenuToggle toggle in menuToggles)
        {
            if (toggle.gameObject.activeInHierarchy)
            {
                toggle.Fix();
            }
        }
    }

    void CleanMenu()
    {
        foreach (GameObject g in allInstructions)
        {
            g.transform.SetParent(inactiveGroup);
        }
    }

    void Activate(GameObject g)
    {
        g.transform.SetParent(activeGroup);
    }

    void FixBasicInstructions()
    {
        GameObject[] instructions = GameControllerState.isXR ? basicInstructionsVR : basicInstructionsPC;
        foreach (GameObject g in instructions)
        {
            Activate(g);
        }
    }

    void FixHintInstructions()
    {
        for (int enumSpaceIndex = (int) Scenario.Maximum; enumSpaceIndex <= (int) scenario; enumSpaceIndex++)
        {
            int instructionSpaceIndex = enumSpaceIndex - 1;
            Activate(hintInstructions[instructionSpaceIndex]);
        }
    }

    public void Fix()
    {
        // Debug.Log("Fix clean menu. scneario " + scenario);
        CleanMenu();
        // Debug.Log("basic instructions");
        FixBasicInstructions();
        // Debug.Log("hint instructions");
        FixHintInstructions();
        nextButton.SetActive(  pillars.won && !GameControllerState.isXR);
        nextButtonVR.SetActive(pillars.won &&  GameControllerState.isXR);

        // Debug.Log("fixable cleanup");
        foreach (GameObject g in allInstructions)
        {
            // Make the buttons appear/disappear as necessary
            foreach (Fixable f in g.GetComponentsInChildren<Fixable>(true))
            {
                f.Fix();
            }
        }
        pillars.maximumHint = true;// (int) scenario >= (int) Scenario.Maximum;
        pillars.hideHint =     scenario == Scenario.Eliminate;
        ruler.SetActive( (int) scenario >= (int) Scenario.Ruler);
        pillars.pairsHint =    scenario == Scenario.Pairs;
        pillars.nextStepHint = scenario == Scenario.Next;

        scoreBoard.SetActive(scenario != Scenario.Tutorial);

    }
}
