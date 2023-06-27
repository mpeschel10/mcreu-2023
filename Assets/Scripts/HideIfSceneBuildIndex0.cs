using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HideIfSceneBuildIndex0 : MonoBehaviour
{
    void OnEnable() { gameObject.SetActive(SceneManager.GetActiveScene().buildIndex > 0); }
}
