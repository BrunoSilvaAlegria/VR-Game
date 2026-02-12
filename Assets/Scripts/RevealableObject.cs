using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Renderer))]
public class RevealableObject : MonoBehaviour
{
    [Header("Timing")]
    [SerializeField] private float revealTime = 1f;

    [Header("Outline Amount")]
    [SerializeField] private float outlineOn = 0.15f;  // valor normal do teu shader
    [SerializeField] private float outlineOff = 0f;     // 0 = sem outline

    private Renderer rend;
    private MaterialPropertyBlock mpb;
    private Coroutine routine;

    private static readonly int OutlineAmountID = Shader.PropertyToID("_OutlineAmount");

    private void Awake()
    {
        rend = GetComponent<Renderer>();
        mpb = new MaterialPropertyBlock();

        // Começa sem outline (mas o renderer continua ativo)
        SetOutline(outlineOff);
    }

    public void Reveal()
    {
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(RevealRoutine());
    }

    private IEnumerator RevealRoutine()
    {
        SetOutline(outlineOn);
        yield return new WaitForSeconds(revealTime);
        SetOutline(outlineOff);
        routine = null;
    }

    private void SetOutline(float amount)
    {
        rend.GetPropertyBlock(mpb);
        mpb.SetFloat(OutlineAmountID, amount);
        rend.SetPropertyBlock(mpb);
    }
}
