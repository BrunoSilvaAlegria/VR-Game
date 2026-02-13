using UnityEngine;

public class ObjectPlacedVerifier : MonoBehaviour
{
    [SerializeField] private EndGame endGame;

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

            endGame.End(1f);

        }
    }
}
