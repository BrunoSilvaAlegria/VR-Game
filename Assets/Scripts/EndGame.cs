using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGame : MonoBehaviour
{
    [SerializeField] private AudioClip endGameScream;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private GameObject deathScreen;

    public void Die()
    {
        deathScreen.SetActive(true);

        if (audioSource != null && endGameScream != null)
        {
            audioSource.PlayOneShot(endGameScream);
        }

        Time.timeScale = 0f;

        End(5f);
    }
    public void End(float time)
    {
        StartCoroutine(EndRoutine(time));
    }

    private IEnumerator EndRoutine(float time)
    {
        // Wait 5 real seconds (ignores timeScale)
        yield return new WaitForSecondsRealtime(time);

        Time.timeScale = 1f; // restore time before loading
        SceneManager.LoadScene(0);
    }
}
