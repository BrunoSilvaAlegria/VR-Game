using UnityEngine;
using System.Collections;

public class BallLightSpawner : MonoBehaviour
{
    [Header("Light Settings")]
    [SerializeField] private Light lightPrefab;

    [SerializeField] private float targetRange = 10f;
    [SerializeField] private float startIntensity = 0.2f;
    [SerializeField] private float targetIntensity = 1f;

    [SerializeField] private float growDuration = 1f;

    [Header("Destroy Light After")]
    [SerializeField] private float destroyAfter = 3f;
    [SerializeField] private float shrinkDuration = 1f;

    [Header("Spawn Offset")]
    [SerializeField] private float surfaceOffset = 0.3f;

    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint contact = collision.contacts[0];

        Vector3 spawnPosition =
            contact.point + contact.normal * surfaceOffset;

        Light spawnedLight = Instantiate(
            lightPrefab,
            spawnPosition,
            Quaternion.identity
        );

        spawnedLight.range = 0f;
        spawnedLight.intensity = startIntensity;

        StartCoroutine(LightLifeCycle(spawnedLight));
    }

    private IEnumerator LightLifeCycle(Light light)
    {
        // Grow
        yield return StartCoroutine(GrowLight(light));

        // Stay alive
        yield return new WaitForSeconds(destroyAfter);

        // Shrink (reverse)
        yield return StartCoroutine(ShrinkLight(light));

        Destroy(light.gameObject);
    }

    private IEnumerator GrowLight(Light light)
    {
        float elapsed = 0f;

        while (elapsed < growDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / growDuration;

            light.range = Mathf.Lerp(0f, targetRange, t);
            light.intensity = Mathf.Lerp(startIntensity, targetIntensity, t);

            yield return null;
        }

        light.range = targetRange;
        light.intensity = targetIntensity;
    }

    private IEnumerator ShrinkLight(Light light)
    {
        float elapsed = 0f;

        float startRange = light.range;
        float startIntensity = light.intensity;

        while (elapsed < shrinkDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / shrinkDuration;

            light.range = Mathf.Lerp(startRange, 0f, t);
            light.intensity = Mathf.Lerp(startIntensity, 0f, t);

            yield return null;
        }

        light.range = 0f;
        light.intensity = 0f;
    }
}
