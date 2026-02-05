using UnityEngine;


public class SoundPulse : MonoBehaviour
{
    [SerializeField] private float maxRadius = 8f;
    [SerializeField] private float speed = 6f;
    [SerializeField] private Transform visual;

    [SerializeField] private Renderer ringRenderer;

    private Material mat;

    private SphereCollider col;

    private void Start()
    {
        mat = ringRenderer.material;
        // Inicializa o colisor com um raio pequeno
        col = GetComponent<SphereCollider>();
        col.radius = 0.1f;
        col.isTrigger = true;

        visual.localScale = Vector3.zero;
    }

    private void Update()
    {
        float t = col.radius / maxRadius;

        Color c = mat.color;

        c.a = Mathf.Lerp(0.6f, 0f, t);

        mat.color = c;

        // Aumenta o raio do colisor para criar o efeito de expansão
        col.radius += speed * Time.deltaTime;

        float diameter = col.radius * 2f;

        visual.localScale = new Vector3(diameter, diameter, 1f);

        // Destroi o objeto quando o raio atingir o valor máximo
        if (col.radius >= maxRadius)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Verifica se o objeto colidido possui o componente RevealableObject
        RevealableObject reveal = other.GetComponent<RevealableObject>();
        if (reveal != null)
        {
            // Chama o método Reveal para revelar o objeto
            reveal.Reveal();
        }
    }
}
