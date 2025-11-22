using UnityEngine;

public class Player : MonoBehaviour
{
    public float velocidade = 3f;
    public float forcaDePulo = 7.25f;

    private Rigidbody2D rb2D;
    private float eixoX;

    void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float eixoX = Input.GetAxis("Horizontal");
        rb2D.linearVelocity = new Vector2(eixoX * velocidade, rb2D.linearVelocityY);

        if (eixoX != 0)
        {
            FlipSprite(eixoX);
        }

        Pular();
    }

    void Pular()
    {
        if (Input.GetButtonDown("Jump"))
        {
            rb2D.AddForce(Vector2.up * forcaDePulo, ForceMode2D.Impulse);
        }
    }

    void FlipSprite(float direction)
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (direction < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (direction > 0)
        {
            spriteRenderer.flipX = false;
        }
    }
}
