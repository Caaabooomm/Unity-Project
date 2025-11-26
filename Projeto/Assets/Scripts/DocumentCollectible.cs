// DocumentCollectible.cs
using UnityEngine;

public class DocumentCollectible : MonoBehaviour
{
    [Tooltip("Quantos documentos este coletável dá ao player.")]
    public int amount = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!CompareTag("Coletável")) return;

        PlayerController player = collision.GetComponent<PlayerController>();
        if (player != null)
        {
            player.documents += amount;
            GameManager.instance.AddDocument();
            Destroy(gameObject);
        }
    }
}
