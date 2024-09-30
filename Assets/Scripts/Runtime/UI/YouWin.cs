using UnityEngine;
using UnityEngine.SceneManagement;

public class YouWin : MonoBehaviour
{
    public void GoBackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
