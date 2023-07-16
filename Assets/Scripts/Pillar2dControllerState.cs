using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pillar2dControllerState : MonoBehaviour, NextButton.HasOnNext
{
    public const int TUTORIAL = 0, ELIMINATE = 1, RUNTIME_HINT = 2, RULER = 3, MAXIMUM = 4, COLUMNS = 5, BEST = 6, CONGRATULATIONS = 7;
    public const int OFFSET_FROM_SCENARIO_TO_HINT_INDEX_WHICH_IS_NEGATIVE_FIRST_THING_AFTER_TUTORIAL = -ELIMINATE;
    [SerializeField] GameObject activeGroup, inactiveGroup;
    [SerializeField] GameObject menuInstructionXR, menuInstructionPC, controlsInstructionXR, controlsInstructionPC, goalInstruction;
    [SerializeField] GameObject eliminateInstruction, runtimeHintInstruction, rulerInstruction, maxInstruction, columnsInstruction, bestInstruction, congratulationsInstruction;
    [SerializeField] GameObject ruler, nextButtonPC, nextButtonXR, scoreboard, menuMainScreen, menuInstructionsScreen;
    [SerializeField] Pillars2D pillars;
    [SerializeField] MenuToggle[] menuToggles;
    
    int scenario = TUTORIAL;

    GameObject[] allInstructions, basicInstructionsXR, basicInstructionsPC, hintInstructions;
    void Start()
    {
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
        HashSet<GameObject> allInstructionsSet = new HashSet<GameObject>();
        allInstructionsSet.UnionWith(basicInstructionsPC);
        allInstructionsSet.UnionWith(basicInstructionsXR);
        allInstructionsSet.UnionWith(hintInstructions);
        allInstructions = new GameObject[allInstructionsSet.Count];
        allInstructionsSet.CopyTo(allInstructions);
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
        // Debug.Log("is xr? " + GameControllerState.isXR);
        foreach (GameObject instruction in basicInstructions)
        {
            Show(instruction);
        }
    }
    
    void ShowHintInstructions()
    {
        for (int i = TUTORIAL + 1; i <= scenario; i++)
        {
            // Debug.Log("Showing hint instruction " + i);
            Show(hintInstructions[i + OFFSET_FROM_SCENARIO_TO_HINT_INDEX_WHICH_IS_NEGATIVE_FIRST_THING_AFTER_TUTORIAL]);
        }
    }
    public void Fix()
    {
        CleanInstructions();
        ShowBasicInstructions();
        ShowHintInstructions();
        
        ruler.SetActive(scenario >= RULER);
        pillars.hintHide = (scenario == ELIMINATE);
        pillars.hintMaximum = (scenario >= MAXIMUM);
        
        nextButtonPC.SetActive(!GameControllerState.isXR && pillars.won && scenario < CONGRATULATIONS);
        nextButtonXR.SetActive( GameControllerState.isXR && pillars.won && scenario < CONGRATULATIONS);
        scoreboard.SetActive( scenario > TUTORIAL);
        
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
        Fix();
    }

    public void CleanActiveInstructions()
    {
        foreach (GameObject instruction in allInstructions)
             instruction.SetActive(false);
    }

    void ShowMenu()
    {
        CleanActiveInstructions();
        if (scenario == TUTORIAL)
        {
            menuInstructionPC.SetActive(true);
            menuInstructionXR.SetActive(true);
        }
        else
        {
            for (int hintIndex = 0; hintIndex < hintInstructions.Length; hintIndex++)
            {
                GameObject instruction = hintInstructions[hintIndex];
                instruction.SetActive(scenario + OFFSET_FROM_SCENARIO_TO_HINT_INDEX_WHICH_IS_NEGATIVE_FIRST_THING_AFTER_TUTORIAL == hintIndex);
            }
        }

        GameControllerState.menuLocation = GameControllerState.isXR ? GameControllerState.lastMenuHand : MenuLocation.FullScreen;
        foreach (MenuToggle toggle in menuToggles)
        {
            if (toggle.gameObject.activeInHierarchy)
            {
                toggle.Fix();
            }
        }

        menuMainScreen.SetActive(false);
        menuInstructionsScreen.SetActive(true);
    }

    public void OnNext()
    {
        if (scenario < CONGRATULATIONS)
        {
            scenario++;
            pillars.Reset();
            Fix();

            ShowMenu();
        }
        else
        {
            Debug.LogWarning(
                "It should not be possible for "+ gameObject +
                " to OnNext while scenario is CONGRATULATIONS. Did you not hide the button?"
            );
        }
    }
}
