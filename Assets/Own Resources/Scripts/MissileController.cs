using UnityEngine;

public class MissileController : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float velocidadInicial = 2f;
    public float aceleracion = 8f;
    public float velocidadMaxima = 25f;
    private float velocidadActual;

    [Header("Efectos de Explosión y Sonido")]
    public GameObject prefabExplosion; // <--- Arrastra aquí tu Prefab_Explosion
    public AudioClip sonidoDespegue;
    [Range(0f, 3f)] public float volumenDespegue = 1f;

    public AudioClip sonidoExplosion;
    [Range(0f, 3f)] public float volumenExplosion = 2f;

    private bool impactado = false;

    void Start()
    {
        velocidadActual = velocidadInicial;

        if (sonidoDespegue != null)
        {
            GameObject tempAudio = new GameObject("TempAudio_DespegueMisil");
            AudioSource aSource = tempAudio.AddComponent<AudioSource>();
            aSource.clip = sonidoDespegue;
            aSource.volume = volumenDespegue;
            aSource.spatialBlend = 0f;
            aSource.Play();
            Destroy(tempAudio, sonidoDespegue.length);
        }

        Destroy(gameObject, 15f);
    }

    void Update()
    {
        if (impactado) return;

        if (velocidadActual < velocidadMaxima)
        {
            velocidadActual += aceleracion * Time.deltaTime;
        }

        transform.Translate(Vector3.up * velocidadActual * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D colision)
    {
        if (impactado) return;

        if (colision.CompareTag("Avion"))
        {
            impactado = true;

            // 1. Instanciar el efecto de partículas en la posición del misil
            if (prefabExplosion != null)
            {
                GameObject explosion = Instantiate(prefabExplosion, transform.position, Quaternion.identity);
                // Destruir el sistema de partículas después de 2 segundos para no acumular basura en memoria
                Destroy(explosion, 2f); 
            }

            // 2. Sonido de explosión seguro
            if (sonidoExplosion != null)
            {
                GameObject tempAudio = new GameObject("TempAudio_ExplosionMisil");
                AudioSource aSource = tempAudio.AddComponent<AudioSource>();
                aSource.clip = sonidoExplosion;
                aSource.volume = volumenExplosion;
                aSource.spatialBlend = 0f;
                aSource.Play();
                Destroy(tempAudio, sonidoExplosion.length);
            }

            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null) sr.enabled = false;
            
            Collider2D col = GetComponent<Collider2D>();
            if (col != null) col.enabled = false;

            Destroy(colision.gameObject);
            Destroy(gameObject);
        }
    }
}