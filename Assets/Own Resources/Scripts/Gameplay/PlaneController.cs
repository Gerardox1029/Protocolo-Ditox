using UnityEngine;

public class PlaneController : MonoBehaviour
{
    [Header("Configuraciones del Avión")]
    public GameObject prefabBomba;
    public float velocidad = 5f;
    
    [Tooltip("1 para ir a la derecha, -1 para ir a la izquierda")]
    public int direccion = 1; 
    
    [Header("Rango de Caída (Eje X)")]
    public float zonaMinima = -8f;
    public float zonaMaxima = 8f;

    [Header("Efectos de Sonido del Avión")]
    public AudioClip[] sonidosAvion; 
    [Range(0f, 2f)] public float volumenAvion = 1f;
    private AudioSource audioAvionSource;

    [HideInInspector] public bool estaDisponible = true;

    private bool enVuelo = false;
    private bool bombaSoltada = false;
    private float puntoDeCaidaX;
    private Vector3 posicionInicial;

    void Start()
    {
        posicionInicial = transform.position;

        audioAvionSource = GetComponent<AudioSource>();
        if (audioAvionSource == null)
        {
            audioAvionSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        if (!enVuelo) return;

        // Mover el avión
        transform.Translate(Vector3.right * direccion * velocidad * Time.deltaTime, Space.Self);

        // Comprobar punto de caída de la bomba
        bool cruzoPunto = direccion == 1 ? transform.position.x >= puntoDeCaidaX : transform.position.x <= puntoDeCaidaX;
        
        if (cruzoPunto && !bombaSoltada)
        {
            SoltarBomba();
            bombaSoltada = true;
        }

        // Si el avión sale de la pantalla, termina su vuelo
        if ((direccion == 1 && transform.position.x > 35f) || (direccion == -1 && transform.position.x < -35f))
        {
            TerminarVuelo();
        }
    }

    public void IniciarSalida()
    {
        // Asegurar que el objeto esté activo
        gameObject.SetActive(true);

        enVuelo = true;
        bombaSoltada = false;
        estaDisponible = false; // Ya no está disponible porque despegó
        puntoDeCaidaX = Random.Range(zonaMinima, zonaMaxima);

        // Reproducir audio
        if (sonidosAvion != null && sonidosAvion.Length > 0)
        {
            int indexAleatorio = Random.Range(0, sonidosAvion.Length);
            if (sonidosAvion[indexAleatorio] != null)
            {
                audioAvionSource.clip = sonidosAvion[indexAleatorio];
                audioAvionSource.volume = volumenAvion;
                audioAvionSource.spatialBlend = 0f; 
                audioAvionSource.loop = true;      
                audioAvionSource.Play();
            }
        }
    }

    void SoltarBomba()
    {
        if (prefabBomba != null)
        {
            GameObject bomba = Instantiate(prefabBomba, transform.position, Quaternion.identity);
            
            Rigidbody2D rbBomba = bomba.GetComponent<Rigidbody2D>();
            if (rbBomba != null)
            {
                rbBomba.linearVelocity = new Vector2(velocidad * 0.5f * direccion, rbBomba.linearVelocity.y);
            }

            BombController controladorBomba = bomba.GetComponent<BombController>();
            if (controladorBomba != null)
            {
                float anguloInicial = (direccion == 1) ? -30f : 30f;
                controladorBomba.ConfigurarInclinacionInicial(anguloInicial);
            }
        }
    }

    // ==========================================
    // MÉTODO AGREGADO PARA INTERACCIÓN CON MISIL
    // ==========================================
    public void Derribar()
    {
        TerminarVuelo();
    }
    // ==========================================

    void TerminarVuelo()
    {
        enVuelo = false;
        transform.position = posicionInicial; // Regresa a su posición original

        if (audioAvionSource != null && audioAvionSource.isPlaying)
        {
            audioAvionSource.Stop();
        }

        // Se desactiva visualmente, pero el Manager sabrá cuándo volver a activarlo
        gameObject.SetActive(false);
    }
}