using UnityEngine;

public class BulletFacts : MonoBehaviour
{
    public int damage = 1;
    public float speed = 10f;
    public float stretchSpeed = 5f;
    public float maxScaleX = 1.5f;
    public float lifeTime = 1f;

    private Rigidbody2D rb;
    private Vector3 initialScale;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        initialScale = transform.localScale;
        rb.gravityScale = 0f;
        rb.bodyType = RigidbodyType2D.Dynamic;

        float direction = transform.right.x > 0 ? 1 : -1;
        rb.linearVelocity = new Vector2(speed * direction, 0);

        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        if (transform.localScale.x < maxScaleX)
        {
            transform.localScale += new Vector3(stretchSpeed * Time.deltaTime, 0f, 0f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            // deal damage to enemy but do not destroy bullet
            EnemyBase enemy = collision.GetComponent<EnemyBase>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
        else
        {
            // if hit ground (layer named "Ground"), destroy
            if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                Destroy(gameObject);
            }
        }
    }
}
