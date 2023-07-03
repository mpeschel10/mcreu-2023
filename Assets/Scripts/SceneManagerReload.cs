using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerReload : MonoBehaviour
{
    public void Main()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
