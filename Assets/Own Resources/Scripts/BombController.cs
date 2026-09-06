using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BombController : MonoBehaviour
{
    [Header("Configuración de Ajuste Inicial")]
    public float tiempoEsperaActivacionColision = 0.15f; 

    [Header("Configuración de Rotación")]
    public float velocidadGiro = 200f;
    public float anguloFinalZ = -90f;

    [Header("Efectos de Explosión y Sonido")]
    public GameObject prefabExplosion; // <--- Arrastra aquí tu Prefab_Explosion
    public AudioClip sonidoCaida;       
    [Range(0f, 2f)] public float volumenCaida = 1f;

    public AudioClip sonidoExplosion;   
    [Range(0f, 3f)] public float volumenExplosion = 2f;
    
    [Header("Efectos de Impacto")]
    public float intensidadTemblor = 0.15f;
    public float duracionTemblor = 0.2f;
    public float radioExplosion = 6f; 

    private Rigidbody2D rb;
    private Collider2D col;
    private AudioSource audioCaidaSource;
    private bool yaColisiono = false;

    public void ConfigurarInclinacionInicial(float anguloInicial)
    {
        transform.rotation = Quaternion.Euler(0, 0, anguloInicial);
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        if (col != null)
        {
            col.enabled = false;
            StartCoroutine(ActivarColisionRutina());
        }

        if (sonidoCaida != null)
        {
            audioCaidaSource = gameObject.AddComponent<AudioSource>();
            audioCaidaSource.clip = sonidoCaida;
            audioCaidaSource.volume = volumenCaida;
            audioCaidaSource.spatialBlend = 0f; 
            audioCaidaSource.loop = true;      
            audioCaidaSource.Play();
        }
    }

    IEnumerator ActivarColisionRutina()
    {
        yield return new WaitForSeconds(tiempoEsperaActivacionColision);
        if (col != null)
        {
            col.enabled = true;
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

        // 1. Instanciar el efecto de partículas de explosión
        if (prefabExplosion != null)
        {
            GameObject explosion = Instantiate(prefabExplosion, transform.position, Quaternion.identity);
            Destroy(explosion, 2f);
        }

        // 2. Sonido de explosión 2D plano
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

        // 3. Temblor de cámara
        CameraShake camaraScript = Camera.main.GetComponent<CameraShake>();
        if (camaraScript != null)
        {
            camaraScript.IniciarTemblor(duracionTemblor, intensidadTemblor);
        }

        // 4. Daño / Parpadeo al jugador si está cerca
        GameObject jugador = GameObject.FindGameObjectWithTag("Player");
        if (jugador != null)
        {
            float distancia = Vector2.Distance(transform.position, jugador.transform.position);
            if (distancia <= radioExplosion)
            {
                PlayerMovement playerScript = jugador.GetComponent<PlayerMovement>();
                if (playerScript != null)
                {
                    playerScript.ActivarParpadeo();
                }
            }
        }

        // 5. Destruir la bomba
        Destroy(gameObject);
    }
}