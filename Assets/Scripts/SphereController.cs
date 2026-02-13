using System.Collections;
using TreeEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class SphereController : MonoBehaviour
{
    [SerializeField] InputActionProperty leftTrigger;
    [SerializeField] InputActionProperty rightTrigger;
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
        if (
        leftTrigger.action.ReadValue<float>() >= 0.5f ||
        rightTrigger.action.ReadValue<float>() >= 0.5f
        )
        {
            GoToPocket();
        }
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

    void GoToPocket()
    {
        transform.position = pocket.position;
    }
    
}
