using UnityEngine;

public class ThunderSound : MonoBehaviour
{
    [SerializeField] private AudioClip thunderAudio;

    [SerializeField] private float playEveryMin = 35f;
    [SerializeField] private float playEveryMax = 45f;

    private float timer = 0;
    private float time;
    private AudioManager audioManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioManager = FindFirstObjectByType<AudioManager>();
        time = Random.Range(playEveryMin, playEveryMax);
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if(timer > time)
        {
            audioManager.PlayAudio(thunderAudio);
            timer = 0;
            time = Random.Range(playEveryMin, playEveryMax);
        }
    }
}
