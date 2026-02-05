using UnityEngine;

public class BallSpawner : MonoBehaviour
{
    [SerializeField] private GameObject ballPrefab;

    [SerializeField] private float throwForce = 10f;

    private void Update()
    {
        // Verifica se a tecla de espaço foi pressionada para lançar a bola
        if(Input.GetKeyDown(KeyCode.Space))
        {
            GameObject ball = Instantiate(ballPrefab, transform.position, Quaternion.identity);
            
            Rigidbody rb = ball.GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * throwForce, ForceMode.Impulse);
        }
    }
}
