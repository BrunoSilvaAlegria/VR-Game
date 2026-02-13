using UnityEngine;

public class EnemyAudio : MonoBehaviour
{
    [SerializeField] private AudioClip[] sounds;
    [SerializeField] private AudioSource audioSource;

    [SerializeField] private float playEveryMin = 2f;
    [SerializeField] private float playEveryMax = 5f;

    private float nextPlayTime;
    private float timer;

    void Start()
    {
        SetNextTime();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= nextPlayTime)
        {
            PlayRandomSound();
            SetNextTime();
            timer = 0f;
        }
    }

    private void PlayRandomSound()
    {
        if (sounds == null || sounds.Length == 0)
            return;

        int randomIndex = Random.Range(0, sounds.Length);
        audioSource.clip = sounds[randomIndex];
        audioSource.Play();
    }

    private void SetNextTime()
    {
        nextPlayTime = Random.Range(playEveryMin, playEveryMax);
    }
}
