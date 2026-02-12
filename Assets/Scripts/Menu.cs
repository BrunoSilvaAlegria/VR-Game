using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    // Nome da cena do jogo (coloca o mesmo nome que está no Build Settings)
    public string gameSceneName = "Ambient";

    public void StartGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game"); // útil para testar no editor
        Application.Quit();
    }
}
