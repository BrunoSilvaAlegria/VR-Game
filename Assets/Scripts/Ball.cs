using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] private GameObject soundPulsePrefab;
    [SerializeField] private float collisionCooldown = 5f;

    private Collider ballCollider;
    private AudioSource audioSource;

    // Keeps track of objects currently ignored
    private Dictionary<Collider, Coroutine> ignoredColliders = new Dictionary<Collider, Coroutine>();

    private void Awake()
    {
        ballCollider = GetComponent<Collider>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ignore Raycast"))
            return;

        Collider otherCol = other.collider;

        // If already ignoring this collider, skip
        if (ignoredColliders.ContainsKey(otherCol))
            return;

        Instantiate(soundPulsePrefab, transform.position, Quaternion.identity);

        float impact = other.relativeVelocity.magnitude;
        
        float intensity = Mathf.InverseLerp(0f, 8f, impact);

        SoundSystem.Emit(transform.position, intensity);


        if(audioSource != null)
            audioSource.Play();

        Coroutine routine = StartCoroutine(ReenableCollision(otherCol));
        ignoredColliders.Add(otherCol, routine);
    }

    private IEnumerator ReenableCollision(Collider otherCol)
    {
        yield return new WaitForSeconds(collisionCooldown);

        ignoredColliders.Remove(otherCol);
    }
}
