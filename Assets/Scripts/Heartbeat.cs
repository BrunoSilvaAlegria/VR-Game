using UnityEngine;

public class Heartbeat : MonoBehaviour
{
    [SerializeField] private Transform enemy;
    [SerializeField] private AudioClip heartbeatClip;
    [SerializeField] private float triggerDistance = 5f;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private EndGame endGame;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (enemy == null) return;

        float distance = Vector3.Distance(transform.position, enemy.position);

        if (distance <= triggerDistance)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.clip = heartbeatClip;
                audioSource.Play();
            }
        }
        else
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }

        if(distance < 1f)
        {
            endGame.Die();
        }
    }
}
