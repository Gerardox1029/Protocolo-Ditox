using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Configuraciones de Movimiento")]
    public float velocidad = 5f;
    public float fuerzaSalto = 10f;
    public float gravedad = 2f;

    [Header("Efectos de Sonido y Volumen")]
    public AudioClip sonidoSalto;
    public AudioClip sonidoCorrer;
    [Range(0f, 3f)] public float volumenAudio = 1f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private AudioSource audioSource;
    private float movimientoX;
    private bool enSuelo;
    private bool estaParpadeando = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        rb.gravityScale = gravedad;

        audioSource.spatialBlend = 0f; 
        audioSource.volume = volumenAudio;
    }

    void Update()
    {
        audioSource.volume = volumenAudio;

        // 1. Detectar movimiento horizontal (Izquierda / Derecha)
        movimientoX = Input.GetAxisRaw("Horizontal");

        // 2. Controlar la orientación de la imagen (FlipX)
        if (movimientoX < 0)
        {
            spriteRenderer.flipX = true;  
        }
        else if (movimientoX > 0)
        {
            spriteRenderer.flipX = false; 
        }

        // 3. Detectar salto (Flecha Arriba o 'W') solo si está en el suelo
        if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) && enSuelo)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, fuerzaSalto);

            if (sonidoSalto != null)
            {
                GameObject tempAudio = new GameObject("TempAudio_Salto");
                AudioSource aSource = tempAudio.AddComponent<AudioSource>();
                aSource.clip = sonidoSalto;
                aSource.volume = volumenAudio;
                aSource.spatialBlend = 0f;
                aSource.Play();
                Destroy(tempAudio, sonidoSalto.length);
            }
        }

        // 4. Gestionar sonido de correr / caminar
        if (enSuelo && movimientoX != 0)
        {
            if (sonidoCorrer != null && !audioSource.isPlaying)
            {
                audioSource.clip = sonidoCorrer;
                audioSource.loop = true;
                audioSource.Play();
            }
        }
        else
        {
            if (audioSource.clip == sonidoCorrer && audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }

        // 5. Enviar datos al Animator
        animator.SetBool("enSuelo", enSuelo);
        animator.SetFloat("velocidadMovimiento", Mathf.Abs(movimientoX));
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(movimientoX * velocidad, rb.linearVelocity.y);
        rb.gravityScale = gravedad; 
    }

    // Método público para activar el parpadeo de daño
    public void ActivarParpadeo()
    {
        if (!estaParpadeando)
        {
            StartCoroutine(RutinaParpadeo());
        }
    }

    private IEnumerator RutinaParpadeo()
    {
        estaParpadeando = true;
        float duracionTotal = 1.5f;   // Tiempo total que dura el parpadeo (ej: 1.5 segundos)
        float intervalo = 0.1f;       // Tiempo de cambio de opacidad
        float tiempoTranscurrido = 0f;

        while (tiempoTranscurrido < duracionTotal)
        {
            // Bajar opacidad a 0.2
            Color colorBajo = spriteRenderer.color;
            colorBajo.a = 0.2f;
            spriteRenderer.color = colorBajo;

            yield return new WaitForSeconds(intervalo);

            // Subir opacidad a 1.0 (normal)
            Color colorNormal = spriteRenderer.color;
            colorNormal.a = 1f;
            spriteRenderer.color = colorNormal;

            yield return new WaitForSeconds(intervalo);

            tiempoTranscurrido += (intervalo * 2f);
        }

        // Asegurarse de que termine con opacidad completa
        Color colorFinal = spriteRenderer.color;
        colorFinal.a = 1f;
        spriteRenderer.color = colorFinal;

        estaParpadeando = false;
    }

    private void OnCollisionEnter2D(Collision2D colision)
    {
        enSuelo = true;
    }

    private void OnCollisionExit2D(Collision2D colision)
    {
        enSuelo = false;

        if (audioSource.clip == sonidoCorrer && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}