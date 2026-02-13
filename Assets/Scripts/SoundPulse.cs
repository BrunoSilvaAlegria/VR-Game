using System.Collections;
using UnityEngine;

public class SoundPulse : MonoBehaviour
{
    [Header("Tamanho do Collider")]
    [SerializeField] private float minRadius = 0.1f;
    [SerializeField] private float maxRadius = 8f;

    [Header("Tempo")]
    [SerializeField] private float expansionDuration = 2f;
    [SerializeField] private float revealDuration = 5f;

    [Header("Visual Pulse")]
    [SerializeField] private Transform visual;
    [SerializeField] private Renderer ringRenderer;

    [Header("Material Swap")]
    [SerializeField] private Material baseMaterial;
    [SerializeField] private Material materialToApply;
    [SerializeField] private Material materialToApplyItem;
    [SerializeField] private Material materialToApplyEnemy;
    

    private SphereCollider col;
    private float timer;

    private void Start()
    {
        col = GetComponent<SphereCollider>();
        col.isTrigger = true;
        col.radius = minRadius;

        visual.localScale = Vector3.zero;
        timer = 0f;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        float t = timer / expansionDuration;
        float currentRadius = Mathf.Lerp(minRadius, maxRadius, t);
        col.radius = currentRadius;

        float diameter = currentRadius * 2f;
        visual.localScale = new Vector3(diameter, diameter, 1f);

        if (timer >= revealDuration + 3f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ball") ||
            other.gameObject.layer == LayerMask.NameToLayer("Girl"))
            return;

        Renderer[] renderers = other.GetComponentsInChildren<Renderer>(true);

        if (renderers.Length == 0) return;

        foreach (Renderer reveal in renderers)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Items"))
            {
                reveal.materials = new Material[] { materialToApplyItem };
            }
            else if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                reveal.materials = new Material[] { materialToApplyEnemy };
            }
            else
            {
                reveal.materials = new Material[] { materialToApply };
            }

            if (other.gameObject.layer != LayerMask.NameToLayer("Items"))
            {
                StartCoroutine(RestoreMaterialAfterTime(reveal));
            }
        }
    }


    private IEnumerator RestoreMaterialAfterTime(Renderer rendererToRestore)
    {
        yield return new WaitForSeconds(revealDuration);

        if (rendererToRestore != null)
        {
            rendererToRestore.materials = new Material[] { baseMaterial };
        }
    }
}
