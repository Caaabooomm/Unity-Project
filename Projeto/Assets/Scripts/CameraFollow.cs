using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    [Header("Limits")]
    public float minX = -1f;
    public float maxX = 999f;
    public float minY = -1.5f;
    public float maxY = 0.5f;

    private float fixedZ;

    void Start()
    {
        fixedZ = transform.position.z;
    }

    void LateUpdate()
    {
        if (target == null) return;

        float newX = Mathf.Clamp(target.position.x, minX, maxX);
        float newY = Mathf.Clamp(target.position.y, minY, maxY);

        transform.position = new Vector3(newX, newY, fixedZ);
    }
}
