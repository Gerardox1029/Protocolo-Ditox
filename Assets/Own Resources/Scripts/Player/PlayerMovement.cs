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

        // 1. Detectar movimiento horizontal
        movimientoX = Input.GetAxisRaw("Horizontal");

        // 2. Controlar la orientación de la imagen
        if (movimientoX < 0) spriteRenderer.flipX = true;  
        else if (movimientoX > 0) spriteRenderer.flipX = false; 

        // 3. Detectar salto
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

        // 4. Gestionar sonido de correr
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

    // Solo cambiamos a enSuelo = true si el objeto tocado es el suelo o plataforma
    private void OnCollisionEnter2D(Collision2D colision)
    {
        if (colision.gameObject.CompareTag("Suelo") || colision.gameObject.CompareTag("Untagged"))
        {
            enSuelo = true;
        }
    }

    // Solo cambiamos a enSuelo = false si nos separamos del suelo (ignorando bombas o proyectiles)
    private void OnCollisionExit2D(Collision2D colision)
    {
        if (colision.gameObject.CompareTag("Suelo") || colision.gameObject.CompareTag("Untagged"))
        {
            enSuelo = false;

            if (audioSource.clip == sonidoCorrer && audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }
}