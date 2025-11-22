using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform alvo;

    public float minX = -1f;
    public float maxX = 999f;
    public float minY = -1.5f;
    public float maxY = 0.5f;

    private float zFixo;

    void Start()
    {
        zFixo = transform.position.z;
    }

    void LateUpdate()
    {
        if (alvo == null)
            return;

        float x = Mathf.Clamp(alvo.position.x, minX, maxX);
        float y = Mathf.Clamp(alvo.position.y, minY, maxY);

        transform.position = new Vector3(x, y, zFixo);
    }

}
