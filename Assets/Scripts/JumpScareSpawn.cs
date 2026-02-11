using UnityEngine;
using System.Collections;

public class JumpScareSpawn : MonoBehaviour
{
    [SerializeField] private GameObject objectToSpawn;
    [SerializeField] private bool walk;
    [SerializeField] private Transform spawnPosition;
    [SerializeField] private Transform goToPosition;
    [SerializeField] private float timeFrom1to2 = 2f;

    [SerializeField] private float lifeTimeAfterSpawn = 2f;

    private bool hasSpawned;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ignore Raycast"))
        {
            if (hasSpawned) return;
            hasSpawned = true;

            GameObject spawnedObject = Instantiate(
                objectToSpawn,
                spawnPosition.position,
                spawnPosition.rotation
            );

            // Destroy after lifetime
            Destroy(spawnedObject, lifeTimeAfterSpawn);

            if (walk)
            {
                StartCoroutine(
                    MoveFromTo(
                        spawnedObject.transform,
                        spawnPosition.position,
                        goToPosition.position,
                        timeFrom1to2
                    )
                );
            }
        }
    }

    private IEnumerator MoveFromTo(
        Transform obj,
        Vector3 start,
        Vector3 end,
        float duration
    )
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            obj.position = Vector3.Lerp(start, end, t);
            yield return null;
        }

        obj.position = end;
    }
}
