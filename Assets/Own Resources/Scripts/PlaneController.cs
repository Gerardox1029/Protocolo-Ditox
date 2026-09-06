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

    [Header("Enfriamiento")]
    public float tiempoEnfriamiento = 15f;

    [Header("Efectos de Sonido del Avión")]
    public AudioClip[] sonidosAvion; // Arrastra aquí tus 3 audios diferentes de aviones
    [Range(0f, 2f)] public float volumenAvion = 1f;
    private AudioSource audioAvionSource;

    [HideInInspector] public bool estaDisponible = true;

    private bool enVuelo = false;
    private bool bombaSoltada = false;
    private float puntoDeCaidaX;
    private Vector3 posicionInicial;
    private float temporizadorCooldown = 0f;

    void Start()
    {
        posicionInicial = transform.position;

        // Asegurar que el avión tenga un componente AudioSource integrado
        audioAvionSource = GetComponent<AudioSource>();
        if (audioAvionSource == null)
        {
            audioAvionSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        // Controlar el tiempo de espera (cooldown) cuando está en tierra
        if (!enVuelo && !estaDisponible)
        {
            temporizadorCooldown -= Time.deltaTime;
            if (temporizadorCooldown <= 0f)
            {
                estaDisponible = true;
            }
        }

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

        // Si el avión sale de la pantalla, se reinicia y empieza su cuenta atrás
        if ((direccion == 1 && transform.position.x > 35f) || (direccion == -1 && transform.position.x < -35f))
        {
            enVuelo = false;
            transform.position = posicionInicial;
            estaDisponible = false;
            temporizadorCooldown = tiempoEnfriamiento;

            // Detener el sonido del motor cuando el avión sale de la pantalla
            if (audioAvionSource != null && audioAvionSource.isPlaying)
            {
                audioAvionSource.Stop();
            }
        }
    }

    public void IniciarSalida()
    {
        enVuelo = true;
        bombaSoltada = false;
        estaDisponible = false;
        puntoDeCaidaX = Random.Range(zonaMinima, zonaMaxima);

        // Elegir y reproducir un audio aleatorio de los 3 disponibles
        if (sonidosAvion != null && sonidosAvion.Length > 0)
        {
            int indexAleatorio = Random.Range(0, sonidosAvion.Length);
            if (sonidosAvion[indexAleatorio] != null)
            {
                audioAvionSource.clip = sonidosAvion[indexAleatorio];
                audioAvionSource.volume = volumenAvion;
                audioAvionSource.spatialBlend = 0f; // Sonido 2D plano para que se escuche con fuerza
                audioAvionSource.loop = true;      // Se repite en bucle mientras cruza el cielo
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

            // Asignar inclinación inicial según la dirección del vuelo
            BombController controladorBomba = bomba.GetComponent<BombController>();
            if (controladorBomba != null)
            {
                float anguloInicial = (direccion == 1) ? -30f : 30f;
                controladorBomba.ConfigurarInclinacionInicial(anguloInicial);
            }
        }
    }
}