using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pillar2dControllerState : MonoBehaviour
{
    public const int TUTORIAL = 0, ELIMINATE = 1, RUNTIME_HINT = 2, RULER = 3, MAXIMUM = 4, COLUMNS = 5, BEST = 6, CONGRATULATIONS = 7;
    [SerializeField] GameObject activeGroup, inactiveGroup;
    [SerializeField] GameObject menuInstructionXR, menuInstructionPC, controlsInstructionXR, controlsInstructionPC, goalInstruction;
    [SerializeField] GameObject eliminateInstruction, runtimeHintInstruction, rulerInstruction, maxInstruction, columnsInstruction, bestInstruction, congratulationsInstruction;
    [SerializeField] GameObject ruler;
    [SerializeField] Pillars2D pillars;
    
    int scenario = TUTORIAL;

    GameObject[] allInstructions, basicInstructionsXR, basicInstructionsPC, hintInstructions;
    void Start()
    {
        allInstructions = new GameObject[]
        {
            menuInstructionXR, menuInstructionPC, controlsInstructionXR, controlsInstructionPC, goalInstruction,
            // maximumInstruction, eliminateInstruction, rulerInstruction,
        };
        basicInstructionsPC = new GameObject[]
        {
            menuInstructionPC, controlsInstructionPC, goalInstruction,
        };
        basicInstructionsXR = new GameObject[]
        {
            menuInstructionXR, controlsInstructionXR, goalInstruction,
        };
        hintInstructions = new GameObject[]
        {
            eliminateInstruction, runtimeHintInstruction, rulerInstruction, maxInstruction, columnsInstruction, bestInstruction, congratulationsInstruction,
        };
        Fix();
    }

    void CleanInstructions()
    {
        foreach (GameObject g in allInstructions)
        {
            g.transform.SetParent(inactiveGroup.transform);
        }
    }

    void Show(GameObject instruction) { instruction.transform.SetParent(activeGroup.transform); }

    void ShowBasicInstructions()
    {
        GameObject[] basicInstructions = GameControllerState.isXR ? basicInstructionsXR : basicInstructionsPC;
        Debug.Log("is xr? " + GameControllerState.isXR);
        foreach (GameObject instruction in basicInstructions)
        {
            Show(instruction);
        }
    }
    
    void ShowHintInstructions()
    {
        const int NEGATIVE_FIRST_THING_AFTER_TUTORIAL = -1;
        for (int i = MAXIMUM; i <= scenario; i++)
        {
            Show(hintInstructions[i + NEGATIVE_FIRST_THING_AFTER_TUTORIAL]);
        }
    }
    public void Fix()
    {
        CleanInstructions();
        ShowBasicInstructions();
        // ShowHintInstructions();
        
        ruler.SetActive(scenario >= RULER);
        pillars.hintHide = (scenario == ELIMINATE);
        
        foreach (GameObject instruction in allInstructions)
        {
            foreach (Fixable button in instruction.GetComponentsInChildren<Fixable>(true))
            {
                button.Fix();
            }
        }
    }
    public void OnWin()
    {
        Debug.Log("Pillar2dcontrollerstate win");
        Fix();
    }

    public void OnNext()
    {
        Debug.Log("Pillar2dcontrollerstate next");
        if (scenario < CONGRATULATIONS)
        {
            scenario++;
        }
        else
        {
            Debug.LogWarning(
                "It should not be possible for "+ gameObject +
                " to OnNext while scenario is CONGRATULATIONS. Did you not hide the button?"
            );
        }
        Fix();
    }
}
