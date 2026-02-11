using UnityEngine;

public class ObjectPlacedVerifier : MonoBehaviour
{
    [SerializeField] private GameObject objectToSpawn;

    private int objectsPlaced;
    private bool hasTriggered;

    public void AddToObjectsPlaced()
    {
        objectsPlaced++;
        VerifyAmount();
    }

    public void TakeFromObjectsPlaced()
    {
        objectsPlaced--;
        VerifyAmount();
    }

    private void VerifyAmount()
    {
        if (hasTriggered) return;

        int requiredAmount = transform.childCount;

        if (objectsPlaced >= requiredAmount && requiredAmount > 0)
        {
            hasTriggered = true;

            // Delete all children
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Destroy(transform.GetChild(i).gameObject);
            }

            // Spawn the object
            Instantiate(
                objectToSpawn,
                transform.position,
                transform.rotation
            );
        }
    }
}
