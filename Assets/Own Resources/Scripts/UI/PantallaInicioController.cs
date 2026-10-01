using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PantallaInicioController : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject panelInicio; // El objeto Mision1
    public AirStrikeManager airStrikeManager; // Tu script de aviones
    
    [Header("Configuración de Fade")]
    public float tiempoEspera = 3f;  // Cuánto tiempo se queda estático antes de desvanecerse
    public float duracionFade = 1f;   // Cuánto dura el desvanecimiento

    private CanvasGroup canvasGroup;

    void Start()
    {
        if (panelInicio != null)
        {
            panelInicio.SetActive(true);

            // Obtener o agregar automáticamente el CanvasGroup
            canvasGroup = panelInicio.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = panelInicio.AddComponent<CanvasGroup>();
            }
            canvasGroup.alpha = 1f; // Comienza completamente visible
        }

        if (airStrikeManager != null) airStrikeManager.enabled = false;

        // Iniciar la corrutina de espera y desvanecimiento
        StartCoroutine(SecuenciaInicioJuego());
    }

    IEnumerator SecuenciaInicioJuego()
    {
        // 1. Esperar los 3 segundos estáticos
        yield return new WaitForSeconds(tiempoEspera);

        // 2. Desvanecer poco a poco (Fade Out del cartel)
        float crono = 0f;
        while (crono < duracionFade)
        {
            crono += Time.deltaTime;
            float alfaActual = Mathf.Lerp(1f, 0f, crono / duracionFade);
            
            if (canvasGroup != null)
            {
                canvasGroup.alpha = alfaActual;
            }
            yield return null; // Espera al siguiente frame
        }

        // Asegurar que quede totalmente invisible y desactivarlo
        if (canvasGroup != null) canvasGroup.alpha = 0f;
        if (panelInicio != null) panelInicio.SetActive(false);

        // 3. Activar el juego y los aviones
        if (airStrikeManager != null) airStrikeManager.enabled = true;
    }
}