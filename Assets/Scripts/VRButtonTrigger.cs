using UnityEngine;
using UnityEngine.Events;

public class VRButtonTrigger : MonoBehaviour
{
    [SerializeField] private UnityEvent onPressed;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Hands"))
        {
            onPressed.Invoke();
        }
    }
}
