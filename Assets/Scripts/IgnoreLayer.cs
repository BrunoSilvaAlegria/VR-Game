using UnityEngine;

public class IgnoreLayer : MonoBehaviour
{
    [SerializeField] int layer1;
    [SerializeField] int layer2;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Physics.IgnoreLayerCollision(layer1, layer2);
    }
}
