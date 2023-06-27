using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplicationQuit : MonoBehaviour
{
    public void Main() {
        UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }
}
