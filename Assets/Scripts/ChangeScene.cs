using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public void Main(int offset)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + offset);
    }
}
