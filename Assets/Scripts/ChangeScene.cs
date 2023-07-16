using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public static void ByOffset(int offset)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + offset);
    }
    public void Main(int offset)
    {
        ChangeScene.ByOffset(offset);
    }
}
