using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Configuraciones de Movimiento")]
    public float velocidad = 5f;
    public float fuerzaSalto = 10f;
    public float gravedad = 2f;

    private Rigidbody2D rb;
    private float movimientoX;
    private bool enSuelo;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = gravedad;
    }

    void Update()
    {
        // 1. Detectar movimiento horizontal (Izquierda / Derecha)
        movimientoX = Input.GetAxisRaw("Horizontal");

        // 2. Detectar salto (Flecha Arriba o 'W') solo si está en el suelo
        if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) && enSuelo)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, fuerzaSalto);
        }
    }

    void FixedUpdate()
    {
        // Aplicar la velocidad en el eje X, manteniendo la velocidad actual en Y (para caer bien)
        rb.linearVelocity = new Vector2(movimientoX * velocidad, rb.linearVelocity.y);
        
        // Mantener la gravedad actualizada en caso de que la cambies desde el Inspector
        rb.gravityScale = gravedad; 
    }

    // Detección muy básica de suelo usando las colisiones que ya tienes
    private void OnCollisionEnter2D(Collision2D colision)
    {
        enSuelo = true;
    }

    private void OnCollisionExit2D(Collision2D colision)
    {
        enSuelo = false;
    }
}