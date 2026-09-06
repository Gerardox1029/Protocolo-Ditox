using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BombController : MonoBehaviour
{
    [Header("Configuración de Rotación")]
    public float velocidadGiro = 200f;
    public float anguloFinalZ = -90f;

    [Header("Efectos de Sonido")]
    public AudioClip sonidoCaida;       // Arrastra aquí el sonido de la bomba cayendo
    [Range(0f, 2f)] public float volumenCaida = 1f;

    public AudioClip sonidoExplosion;   // El sonido de impacto que ya tenías
    [Range(0f, 3f)] public float volumenExplosion = 2f;
    
    [Header("Efectos de Impacto")]
    public float intensidadTemblor = 0.15f;
    public float duracionTemblor = 0.2f;

    private Rigidbody2D rb;
    private AudioSource audioCaidaSource;
    private bool yaColisiono = false;

    public void ConfigurarInclinacionInicial(float anguloInicial)
    {
        transform.rotation = Quaternion.Euler(0, 0, anguloInicial);
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Reproducir sonido de caída en bucle al nacer la bomba
        if (sonidoCaida != null)
        {
            audioCaidaSource = gameObject.AddComponent<AudioSource>();
            audioCaidaSource.clip = sonidoCaida;
            audioCaidaSource.volume = volumenCaida;
            audioCaidaSource.spatialBlend = 0f; // Sonido 2D plano para que se escuche bien
            audioCaidaSource.loop = true;      // Se repite mientras cae
            audioCaidaSource.Play();
        }
    }

    void Update()
    {
        Quaternion rotacionDestino = Quaternion.Euler(0, 0, anguloFinalZ);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotacionDestino, velocidadGiro * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (yaColisiono) return;
        yaColisiono = true;

        // 1. Reproducir sonido de explosión 2D plano
        if (sonidoExplosion != null)
        {
            GameObject tempAudio = new GameObject("TempAudio_Explosion");
            AudioSource aSource = tempAudio.AddComponent<AudioSource>();
            aSource.clip = sonidoExplosion;
            aSource.volume = volumenExplosion; 
            aSource.spatialBlend = 0f; 
            aSource.Play();
            Destroy(tempAudio, sonidoExplosion.length);
        }

        // 2. Temblor de cámara
        CameraShake camaraScript = Camera.main.GetComponent<CameraShake>();
        if (camaraScript != null)
        {
            camaraScript.IniciarTemblor(duracionTemblor, intensidadTemblor);
        }

        // 3. Destruir la bomba (esto borra automáticamente el sonido de caída que tenía integrado)
        Destroy(gameObject);
    }
}