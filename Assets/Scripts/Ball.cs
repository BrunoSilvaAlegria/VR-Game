using UnityEngine;

public class Ball : MonoBehaviour
{
    // Referencia ao prefab do efeito sonoro de pulse
    [SerializeField] private GameObject soundPulsePrefab;

    private void OnCollisionEnter(Collision other)
    {
        Instantiate(soundPulsePrefab, transform.position, Quaternion.identity);

        SoundSystem.Emit(transform.position, 1f); // Emite um som com intensidade 1 na posição da colisão
    }
}
