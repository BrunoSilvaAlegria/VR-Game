using System.Collections;
using TreeEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class SphereController : MonoBehaviour
{
    [SerializeField] float timeToWait;
    [SerializeField] Transform pocket;
    Coroutine waitTimeCoroutine;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        waitTimeCoroutine = StartCoroutine(WaitTime());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionExit(Collision collision)
    {
        waitTimeCoroutine = StartCoroutine(WaitTime());
        print("Saiu collision");
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Pocket") || collision.gameObject.layer == LayerMask.NameToLayer("Hands"))
        {
            if (waitTimeCoroutine != null)
            {
                StopCoroutine(waitTimeCoroutine);
                waitTimeCoroutine = null;
            }
            print("Entrou mãos ou bolso");
        }
    }
    IEnumerator WaitTime()
    {
        yield return new WaitForSeconds(timeToWait);
        print("Spawn ball");
    }

    public void GoToPocket()
    {
        transform.position = pocket.position;
    }
    
}
