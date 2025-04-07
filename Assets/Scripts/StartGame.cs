using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
   public void GameStart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void GameExit()
    {
        Application.Quit();
    }
}
