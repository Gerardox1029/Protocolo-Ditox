using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MenuInicioManager : MonoBehaviour
{
    [Header("Elementos de tu Hierarchy")]
    public GameObject objetoAviones;          
    public GameObject objetoAirStrikeManager; 
    
    [Header("Elementos de UI")]
    public GameObject panelPressEnter;        
    public Image imagenFadeNegro;             

    [Header("Configuración de Tiempos y Efectos")]
    [Tooltip("Tiempo que tarda en completarse un ciclo de parpadeo (más alto = más lento).")]
    public float velocidadParpadeo = 1.2f; 
    [Tooltip("Tiempo en segundos que dura el desvanecimiento para revelar el juego al pulsar Enter.")]
    public float duracionDesvanecimientoInicio = 3.0f;

    [Header("Sonido de Enter y Música")]
    public AudioClip sonidoEnter;             // <--- Arrastra aquí el sonido al pulsar Enter
    [Range(0f, 3f)] public float volumenEnter = 1f;
    public AudioClip musicaMenu;
    public AudioClip musicaJuego; 
    private AudioSource audioSource;

    private bool juegoIniciado = false;

    void Start()
    {
        // 1. Desactivar aviones y manager al arrancar
        if (objetoAviones != null) objetoAviones.SetActive(false);
        if (objetoAirStrikeManager != null) objetoAirStrikeManager.SetActive(false);

        // 2. Configurar la música del menú
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();

        if (musicaMenu != null)
        {
            audioSource.clip = musicaMenu;
            audioSource.spatialBlend = 0f;
            audioSource.loop = true;
            audioSource.Play();
        }

        // 3. Empezar la escena TOTALMENTE OSCURA
        if (imagenFadeNegro != null)
        {
            Color c = imagenFadeNegro.color;
            c.a = 1f; // Opacidad al máximo (Negro total)
            imagenFadeNegro.color = c;
            imagenFadeNegro.gameObject.SetActive(true);
        }

        // Iniciar la animación de parpadeo del texto
        StartCoroutine(ParpadearTextoRutina());

        // Opcional: Si quieres que al iniciar el juego también se aclare solo desde el inicio, 
        // puedes descomentar la siguiente línea. Si prefieres que el menú se vea primero y 
        // solo se aclare al pulsar Enter, déjalo como está.
    }

    void Update()
    {
        // Al presionar Enter o Intro
        if (!juegoIniciado && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
        {
            StartCoroutine(IniciarJuegoRutina());
        }
    }

    // Corrutina para el parpadeo del "Press Enter" (ahora con velocidad ajustable)
    IEnumerator ParpadearTextoRutina()
    {
        CanvasGroup canvasGroup = panelPressEnter.GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = panelPressEnter.AddComponent<CanvasGroup>();

        float mitadCiclo = velocidadParpadeo / 2f;

        while (!juegoIniciado)
        {
            // Bajar opacidad a 0.2
            float t = 0f;
            while (t < mitadCiclo && !juegoIniciado)
            {
                t += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(1f, 0.2f, t / mitadCiclo);
                yield return null;
            }

            // Subir opacidad a 1.0
            t = 0f;
            while (t < mitadCiclo && !juegoIniciado)
            {
                t += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(0.2f, 1f, t / mitadCiclo);
                yield return null;
            }
        }
    }

    // Corrutina al presionar Enter: Sonido, activar juego y desvanecer a claro
    IEnumerator IniciarJuegoRutina()
    {
        juegoIniciado = true;

        // 1. Reproducir sonido de Enter de forma segura (sin que se corte)
        if (sonidoEnter != null)
        {
            GameObject tempAudio = new GameObject("TempAudio_Enter");
            AudioSource aSource = tempAudio.AddComponent<AudioSource>();
            aSource.clip = sonidoEnter;
            aSource.volume = volumenEnter;
            aSource.spatialBlend = 0f;
            aSource.Play();
            Destroy(tempAudio, sonidoEnter.length);
        }

        // Ocultar el aviso de press enter de inmediato
        if (panelPressEnter != null) panelPressEnter.SetActive(false);

        // 2. ¡AQUÍ APARECE TODO EL JUEGO! Activamos los aviones y gestores de inmediato debajo de la oscuridad
        if (objetoAviones != null) objetoAviones.SetActive(true);
        if (objetoAirStrikeManager != null) objetoAirStrikeManager.SetActive(true);

        // Cambiar música de fondo si tienes una para el juego
        if (musicaJuego != null)
        {
            audioSource.Stop();
            audioSource.clip = musicaJuego;
            audioSource.loop = true;
            audioSource.Play();
        }

        // 3. Desvanecimiento progresivo: de totalmente oscuro (1) a transparente (0) en los segundos configurados
        float tiempo = 0f;
        while (tiempo < duracionDesvanecimientoInicio)
        {
            tiempo += Time.deltaTime;
            float alfa = Mathf.Lerp(1f, 0f, tiempo / duracionDesvanecimientoInicio);
            
            if (imagenFadeNegro != null)
            {
                Color c = imagenFadeNegro.color;
                c.a = alfa;
                imagenFadeNegro.color = c;
            }
            yield return null;
        }

        // Desactivar el objeto de la imagen negra por completo para que no interfiera con clics o eventos
        if (imagenFadeNegro != null)
        {
            imagenFadeNegro.gameObject.SetActive(false);
        }
    }
}