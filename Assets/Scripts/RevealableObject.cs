using UnityEngine;
using System.Collections;

public class RevealableObject : MonoBehaviour
{
    [SerializeField] private float revealTime = 1f;

    private Renderer rend;

    private void Start()
    {
        rend = GetComponent<Renderer>();
        rend.enabled = false; // Esconde o objeto inicialmente
    }

    public void Reveal()
    {
        StopAllCoroutines();
        StartCoroutine(RevealCoroutine());
    }

    // Coroutine para revelar o objeto por um tempo determinado
    public IEnumerator RevealCoroutine()
    {
        rend.enabled = true; // Mostra o objeto
        yield return new WaitForSeconds(revealTime);
        rend.enabled = false; // Esconde o objeto novamente
    }
}
