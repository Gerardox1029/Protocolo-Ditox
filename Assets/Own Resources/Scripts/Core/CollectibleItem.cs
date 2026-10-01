using System.Collections;
using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    [Header("Configuración del Misil")]
    public GameObject prefabMisil; 

    [Header("Efectos Visuales (Levitación y Giro)")]
    public float velocidadLevitacion = 3f;
    public float alturaLevitacion = 0.25f;
    public float velocidadGiroY = 5f; 

    [Header("Efectos de Sonido")]
    public AudioClip sonidoRecolectar;
    [Range(0f, 3f)] public float volumenRecolectar = 1f;

    private Vector3 posicionInicial;
    private SpriteRenderer spriteRenderer;
    private Collider2D colisionador;

    void Start()
    {
        posicionInicial = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        colisionador = GetComponent<Collider2D>();
    }

    void Update()
    {
        float nuevaY = posicionInicial.y + Mathf.Sin(Time.time * velocidadLevitacion) * alturaLevitacion;
        float escalaXGiro = Mathf.Cos(Time.time * velocidadGiroY); 

        transform.position = new Vector3(transform.position.x, nuevaY, transform.position.z);
        transform.localScale = new Vector3(escalaXGiro, transform.localScale.y, transform.localScale.z);
    }

    private void OnTriggerEnter2D(Collider2D colision)
    {
        if (colision.CompareTag("Player"))
        {
            // Reproducir sonido al recolectar usando GameObject temporal seguro
            if (sonidoRecolectar != null)
            {
                GameObject tempAudio = new GameObject("TempAudio_Recolectar");
                AudioSource aSource = tempAudio.AddComponent<AudioSource>();
                aSource.clip = sonidoRecolectar;
                aSource.volume = volumenRecolectar;
                aSource.spatialBlend = 0f; // Sonido 2D plano
                aSource.Play();
                Destroy(tempAudio, sonidoRecolectar.length);
            }

            // Desplegar el misil
            if (prefabMisil != null)
            {
                Instantiate(prefabMisil, transform.position, Quaternion.identity);
            }

            // Ocultar y reaparecer en 3 a 5 segundos
            StartCoroutine(ReaparecerRutina());
        }
    }

    IEnumerator ReaparecerRutina()
    {
        spriteRenderer.enabled = false;
        colisionador.enabled = false;

        float tiempoEspera = Random.Range(5f, 10f);
        yield return new WaitForSeconds(tiempoEspera);

        spriteRenderer.enabled = true;
        colisionador.enabled = true;
    }
}