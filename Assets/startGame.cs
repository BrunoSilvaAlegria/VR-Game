using UnityEngine;

public class StartGame : MonoBehaviour
{
    [SerializeField] private GameObject objectToActivate;
    [SerializeField] private AudioSource audioSource;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering is on the Player layer
        if (other.gameObject.layer == LayerMask.NameToLayer("Ignore Raycast"))
        {
            objectToActivate.SetActive(true);
            audioSource.Play();
        }
    }
}
