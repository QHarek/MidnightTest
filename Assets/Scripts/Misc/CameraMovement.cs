using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f;

    private float minZ = -70f;
    private float maxZ = -15f;

    void Update()
    {
        float currentZ = transform.position.z;

        if (Input.GetKey(KeyCode.W))
        {
            currentZ += moveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            currentZ -= moveSpeed * Time.deltaTime;
        }

        currentZ = Mathf.Clamp(currentZ, minZ, maxZ);

        transform.position = new Vector3(transform.position.x, transform.position.y, currentZ);
    }
}