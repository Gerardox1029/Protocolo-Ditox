using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("UI Elementos")]
    public GameObject panelPausa;        // Arrastra el objeto PanelPausa
    public RectTransform flechaSelector;  // Arrastra el objeto Selector
    public RectTransform[] botones;       // Element 0: Reanudar, Element 1: Salir

    [Header("Ajustes Selector")]
    public float offsetFlechaY = 60f;     // Distancia vertical por ENCIMA del botón

    [Header("Audio")]
    public AudioSource musicaFondo;      // Arrastra el AudioSource de la música
    public AudioClip sonidoNavegacion;    // Sonido al mover de izquierda/derecha
    public AudioClip sonidoSeleccion;     // Sonido al presionar Enter
    private AudioSource audioSourceUI;

    private bool juegoPausado = false;
    private int opcionSeleccionada = 0;

    void Start()
    {
        // Crear componente de audio para el menú si no existe
        audioSourceUI = GetComponent<AudioSource>();
        if (audioSourceUI == null)
        {
            audioSourceUI = gameObject.AddComponent<AudioSource>();
        }
        audioSourceUI.spatialBlend = 0f; // Audio 2D

        // 1. OCULTAR PANEL AL INICIAR
        if (panelPausa != null)
        {
            panelPausa.SetActive(false);
        }

        // Asegurar tiempo normal al arrancar
        Time.timeScale = 1f;
        AudioListener.pause = false;
    }

    void Update()
    {
        // Activar/Desactivar pausa con Tecla ESC o P
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            if (juegoPausado)
            {
                Reanudar();
            }
            else
            {
                Pausar();
            }
        }

        // Si el juego está pausado, gestionar el selector con teclas horizontales
        if (juegoPausado)
        {
            // Mover a la DERECHA
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {
                opcionSeleccionada++;
                if (opcionSeleccionada >= botones.Length)
                {
                    opcionSeleccionada = 0; // Regresa al primer botón
                }
                ReproducirSonido(sonidoNavegacion);
                ActualizarPosicionFlecha();
            }

            // Mover a la IZQUIERDA
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                opcionSeleccionada--;
                if (opcionSeleccionada < 0)
                {
                    opcionSeleccionada = botones.Length - 1; // Salta al último botón
                }
                ReproducirSonido(sonidoNavegacion);
                ActualizarPosicionFlecha();
            }

            // Confirmar con ENTER o ESPACIO
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Space))
            {
                ReproducirSonido(sonidoSeleccion);
                EjecutarOpcion();
            }
        }
    }

    public void Pausar()
    {
        juegoPausado = true;
        opcionSeleccionada = 0; // Iniciar en el primer botón (Reanudar)

        if (panelPausa != null)
        {
            panelPausa.SetActive(true);
        }

        ActualizarPosicionFlecha();

        Time.timeScale = 0f;            // Congelar juego
        AudioListener.pause = true;     // Pausar efectos de sonido

        if (musicaFondo != null)
        {
            musicaFondo.ignoreListenerPause = true; // Que la música continúe
        }
    }

    public void Reanudar()
    {
        juegoPausado = false;

        if (panelPausa != null)
        {
            panelPausa.SetActive(false);
        }

        Time.timeScale = 1f;            // Restaurar tiempo
        AudioListener.pause = false;    // Restaurar sonidos
    }

    void ActualizarPosicionFlecha()
    {
        if (botones.Length == 0 || flechaSelector == null) return;

        // Asegura que la flecha comparta el mismo Canvas/padre
        flechaSelector.SetParent(botones[opcionSeleccionada].parent, false);

        // Alinea la flecha por ENCIMA del botón correspondiente
        Vector2 posBoton = botones[opcionSeleccionada].anchoredPosition;
        flechaSelector.anchoredPosition = new Vector2(posBoton.x, posBoton.y + offsetFlechaY);
    }

    void EjecutarOpcion()
    {
        switch (opcionSeleccionada)
        {
            case 0:
                Reanudar();
                break;

            case 1:
                SalirAlMenu();
                break;
        }
    }

    public void SalirAlMenu()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadScene("MainMenu");
    }

    void ReproducirSonido(AudioClip clip)
    {
        if (clip != null && audioSourceUI != null)
        {
            audioSourceUI.PlayOneShot(clip);
        }
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
    }
}