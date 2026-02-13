using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit;

public class Menu : MonoBehaviour
{
    [Header("Scene")]
    [SerializeField] private string gameSceneName = "Ambient";

    [Header("Canvases")]
    [SerializeField] private GameObject mainMenuCanvas;
    [SerializeField] private GameObject introCanvas1;
    [SerializeField] private GameObject introCanvas2;

    [Header("VR Fade")]
    [SerializeField] private VRFader vrFader;

    void Start()
    {
        mainMenuCanvas.SetActive(true);
        introCanvas1.SetActive(false);
        introCanvas2.SetActive(false);

        StartCoroutine(FadeIn());
    }

    public void OpenIntroStep1()
    {
        mainMenuCanvas.SetActive(false);
        introCanvas1.SetActive(true);
    }

    public void OpenIntroStep2()
    {
        introCanvas1.SetActive(false);
        introCanvas2.SetActive(true);
    }

    public void FinalStartGame()
    {
        StartCoroutine(FadeOutAndLoad());
    }

    public void QuitGame()
    {
        StartCoroutine(FadeOutAndQuit());
    }

    IEnumerator FadeIn()
    {
        yield return vrFader.FadeIn();
    }

    IEnumerator FadeOutAndLoad()
    {
        yield return vrFader.FadeOut();
        SceneManager.LoadScene(gameSceneName);
    }

    IEnumerator FadeOutAndQuit()
    {
        yield return vrFader.FadeOut();
        Application.Quit();
    }
}
