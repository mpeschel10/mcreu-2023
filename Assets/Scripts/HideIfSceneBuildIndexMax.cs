using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HideIfSceneBuildIndexMax : MonoBehaviour
{
    void OnEnable() {
        gameObject.SetActive(
            SceneManager.GetActiveScene().buildIndex <
            SceneManager.sceneCountInBuildSettings - 1
        );
    }
}
