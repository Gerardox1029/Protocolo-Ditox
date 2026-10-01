using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Configuración de Salud")]
    public float vidaMaxima = 100f;
    public float vidaActual;

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        vidaActual = vidaMaxima;
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Actualizar la UI al iniciar
        ActualizarBarraUI();
    }

    public void RecibirDano(float cantidad)
    {
        vidaActual -= cantidad;
        if (vidaActual < 0) vidaActual = 0;

        ActualizarBarraUI();

        if (vidaActual <= 0)
        {
            // Avisar al GameManager que murió
            if (GameManager.Instance != null)
            {
                GameManager.Instance.GameOver();
            }
        }
    }

    public void ActivarParpadeo()
    {
        // Aplicar daño directo al recibir explosión
        RecibirDano(20f); // Cambia 20f por el daño que quieras

        // Evitar lanzar corrutina si el jugador ya está inactivo/muerto
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(RutinaParpadeo());
        }
    }

    private System.Collections.IEnumerator RutinaParpadeo()
    {
        if (spriteRenderer != null)
        {
            for (int i = 0; i < 3; i++)
            {
                spriteRenderer.color = new Color(1f, 0f, 0f, 0.5f); // Rojo transparente
                yield return new WaitForSeconds(0.1f);
                spriteRenderer.color = Color.white;
                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    void ActualizarBarraUI()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ActualizarVidaUI(vidaActual, vidaMaxima);
        }
    }
}