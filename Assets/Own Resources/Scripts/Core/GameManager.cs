using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    [Header("Configuración de Escenas")]
    public string nombreSiguienteEscena = "Scene2";
    public string nombreMenuPrincipal = "MainMenu";

    [Header("Configuración de Tiempo")]
    public float tiempoSobrevivir = 30f;

    [Header("Referencias de Gameplay")]
    public AirStrikeManager airStrikeManager;
    public GameObject jugadorObject;

    [Header("Tiempos de Espera")]
    public float tiempoPausaVictoria = 3.5f;
    public float tiempoPausaGameOver = 3.0f;

    private bool juegoTerminado = false;
    private bool juegoIniciado = false;
    private bool esperandoEnter = false;

    void Start()
    {
        if (airStrikeManager != null) airStrikeManager.enabled = false;

        // Iniciar pantalla de carga en UI
        if (UIManager.Instance != null)
        {
            UIManager.Instance.IniciarPantallaCarga(AlTerminarCarga);
        }
        else
        {
            AlTerminarCarga();
        }
    }

    void AlTerminarCarga()
    {
        juegoIniciado = true;
        if (airStrikeManager != null) airStrikeManager.enabled = true;
    }

    void Update()
    {
        if (!juegoIniciado) return;

        if (!juegoTerminado)
        {
            if (tiempoSobrevivir > 0)
            {
                tiempoSobrevivir -= Time.deltaTime;

                if (tiempoSobrevivir <= 0)
                {
                    tiempoSobrevivir = 0;
                    TerminarMision();
                }

                if (UIManager.Instance != null)
                {
                    UIManager.Instance.ActualizarContadorTiempo(tiempoSobrevivir);
                }
            }
        }

        if (esperandoEnter && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
        {
            esperandoEnter = false;
            
            if (UIManager.Instance != null) UIManager.Instance.DetenerParpadeoEnter();

            if (UIManager.Instance != null)
            {
                UIManager.Instance.RealizarFadeOutNegro(() => SceneManager.LoadScene(nombreSiguienteEscena));
            }
            else
            {
                SceneManager.LoadScene(nombreSiguienteEscena);
            }
        }
    }

    // ==========================================
    // ESTADO: GAME OVER
    // ==========================================
    public void GameOver()
    {
        if (juegoTerminado) return;
        juegoTerminado = true;

        if (airStrikeManager != null)
        {
            airStrikeManager.StopAllCoroutines();
            airStrikeManager.enabled = false;
        }

        if (jugadorObject != null) jugadorObject.SetActive(false);

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ReproducirMuertePlayer();
            AudioManager.Instance.DesvanecerMusica(0.5f);
        }

        if (UIManager.Instance != null)
        {
            UIManager.Instance.MostrarGameOver(() => {
                StartCoroutine(EsperarYRegresarMenu());
            });
        }
    }

    private System.Collections.IEnumerator EsperarYRegresarMenu()
    {
        yield return new WaitForSeconds(tiempoPausaGameOver);
        SceneManager.LoadScene(nombreMenuPrincipal);
    }

    // ==========================================
    // ESTADO: VICTORIA
    // ==========================================
    void TerminarMision()
    {
        juegoTerminado = true;

        if (airStrikeManager != null)
        {
            airStrikeManager.StopAllCoroutines();
            airStrikeManager.enabled = false;
        }

        StartCoroutine(SecuenciaVictoria());
    }

    private System.Collections.IEnumerator SecuenciaVictoria()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.DesvanecerMusica(1.5f);

        yield return new WaitForSeconds(tiempoPausaVictoria);

        if (AudioManager.Instance != null) AudioManager.Instance.ReproducirVictoria();

        if (UIManager.Instance != null)
        {
            UIManager.Instance.MostrarVictoria(() => {
                esperandoEnter = true;
            });
        }
    }
}