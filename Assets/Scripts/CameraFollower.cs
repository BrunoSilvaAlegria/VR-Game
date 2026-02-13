using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    [SerializeField] Transform camera;
    [SerializeField] Transform parent;
    Vector3 rotation;
    Vector3 currentPosition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        /*currentPosition = transform.position;
        initialPosition = transform.position;*/
    }

    // Update is called once per frame
    void Update()
    {
        /*print("before: " + currentPosition);
        currentPosition = initialPosition + camera.position;
        print("after: " + currentPosition);
        transform.position = currentPosition; */
        parent.position = camera.position;
        rotation = camera.localEulerAngles;
        rotation.x = 0f;
        rotation.z = 0f;
        parent.localEulerAngles = rotation;
    }
}
