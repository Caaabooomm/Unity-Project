using UnityEngine;
using TMPro;

public class HUDManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI txtVida;
    [SerializeField] private TextMeshProUGUI txtDocumentos;
    [SerializeField] private TextMeshProUGUI txtInimigos;

    void Update()
    {
        if (GameManager.instancia == null) return;

        txtVida.text = $"Vida: {GameManager.instancia.GetVidaAtual()}/{GameManager.instancia.GetVidaMax()}";
        txtDocumentos.text = $"Documentos: {GameManager.instancia.GetDocumento()}";
        txtInimigos.text = $"Inimigos: {GameManager.instancia.GetInimigosVivos()}";
    }
}
