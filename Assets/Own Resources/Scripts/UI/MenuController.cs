using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [Header("UI Elementos")]
    public RectTransform flechaSelector; // Arrastra la flecha/selector
    public RectTransform[] botones;      // Arrastra start_button y exit_button
    public float offsetFlechaX = -120f;  // Ajusta la distancia horizontal a la izquierda

    [Header("Efectos de Sonido")]
    public AudioClip sonidoNavegacion;   // Arrastra aquí el sonido para Arriba/Abajo
    public AudioClip sonidoSeleccionar;  // (Opcional) Arrastra el sonido para Enter
    private AudioSource audioSource;

    private int opcionSeleccionada = 0;

    void Start()
    {
        // Asegura que exista un AudioSource en este objeto
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.spatialBlend = 0f; // Audio 2D para UI

        ActualizarPosicionFlecha();
    }

    void Update()
    {
        // Mover abajo
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            opcionSeleccionada++;
            if (opcionSeleccionada >= botones.Length)
            {
                opcionSeleccionada = 0;
            }
            ReproducirSonido(sonidoNavegacion);
            ActualizarPosicionFlecha();
        }

        // Mover arriba
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            opcionSeleccionada--;
            if (opcionSeleccionada < 0)
            {
                opcionSeleccionada = botones.Length - 1;
            }
            ReproducirSonido(sonidoNavegacion);
            ActualizarPosicionFlecha();
        }

        // Seleccionar
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Space))
        {
            ReproducirSonido(sonidoSeleccionar);
            EjecutarOpcion();
        }
    }

    void ReproducirSonido(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    void ActualizarPosicionFlecha()
    {
        if (botones.Length == 0 || flechaSelector == null) return;

        // Aseguramos que la flecha comparta el mismo padre/Canvas que los botones
        flechaSelector.SetParent(botones[opcionSeleccionada].parent, false);

        // Alineamos usando la posición anclada (UI)
        Vector2 posBoton = botones[opcionSeleccionada].anchoredPosition;
        flechaSelector.anchoredPosition = new Vector2(posBoton.x + offsetFlechaX, posBoton.y);
    }

    void EjecutarOpcion()
    {
        switch (opcionSeleccionada)
        {
            case 0:
                SceneManager.LoadScene("Scene1");
                break;

            case 1:
                Debug.Log("Saliendo del juego...");
                Application.Quit();
                break;
        }
    }
}