using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    [Header("UI de Salud / Jugador (TU LÓGICA ANTERIOR)")]
    public Slider sliderVida;
    public TextMeshProUGUI txtVida;

    [Header("UI de Pantalla de Carga")]
    public GameObject panelCargando;             
    public TextMeshProUGUI txtCargando;          
    public float tiempoPantallaCarga = 2.0f;     
    public float tiempoDesvanecidoCarga = 1.0f;  

    [Header("UI General / HUD")]
    public TextMeshProUGUI txtContador;

    [Header("UI de Victoria / Misión Final")]
    public GameObject imgMisionCompletada;      
    public TextMeshProUGUI txtPressEnter;       
    public Image panelFade;                     
    public float tiempoFadeInImagen = 1.5f;     
    public float tiempoFadeInNegro = 2.5f;      
    public float velocidadParpadeoTexto = 2.0f; 

    [Header("UI de Game Over")]
    public GameObject txtGameOver;               
    public Image panelFiltroGris;               
    public float tiempoFadeInGameOver = 1.5f;

    private Coroutine corrutinaParpadeo;

    void Start()
    {
        if (panelFade != null)
        {
            Color c = panelFade.color; c.a = 0f; panelFade.color = c;
            panelFade.gameObject.SetActive(false);
        }

        if (imgMisionCompletada != null) imgMisionCompletada.SetActive(false);
        if (txtGameOver != null) txtGameOver.SetActive(false);

        if (panelFiltroGris != null)
        {
            Color c = panelFiltroGris.color; c.a = 0f; panelFiltroGris.color = c;
            panelFiltroGris.gameObject.SetActive(false);
        }

        if (txtPressEnter != null)
        {
            txtPressEnter.gameObject.SetActive(false);
            Color c = txtPressEnter.color; c.a = 0f; txtPressEnter.color = c;
        }
    }

    // ==========================================
    // MÉTODOS DE VIDA Y SLIDER (TUS MÉTODOS ORIGINALES)
    // ==========================================
    public void ActualizarVidaUI(float vidaActual, float vidaMaxima)
    {
        if (sliderVida != null)
        {
            sliderVida.maxValue = vidaMaxima;
            sliderVida.value = vidaActual;
        }

        if (txtVida != null)
        {
            txtVida.text = Mathf.CeilToInt(vidaActual) + " / " + Mathf.CeilToInt(vidaMaxima);
        }
    }

    public void ActualizarContadorTiempo(float tiempoRestante)
    {
        if (txtContador != null)
        {
            txtContador.text = "Sobrevive: " + tiempoRestante.ToString("F2");
        }
    }

    // ==========================================
    // CARGA
    // ==========================================
    public void IniciarPantallaCarga(Action alFinalizar)
    {
        StartCoroutine(RutinaPantallaCarga(alFinalizar));
    }

    private IEnumerator RutinaPantallaCarga(Action alFinalizar)
    {
        if (panelCargando != null) panelCargando.SetActive(true);
        if (txtCargando != null) txtCargando.gameObject.SetActive(true);

        float crono = 0f, tempPuntos = 0f;
        int puntos = 1;

        while (crono < tiempoPantallaCarga)
        {
            crono += Time.deltaTime;
            tempPuntos += Time.deltaTime;

            if (tempPuntos >= 0.3f)
            {
                tempPuntos = 0f;
                puntos = (puntos % 3) + 1;
                if (txtCargando != null) txtCargando.text = "Cargando" + new string('.', puntos);
            }
            yield return null;
        }

        if (panelCargando != null)
        {
            CanvasGroup cg = panelCargando.GetComponent<CanvasGroup>();
            if (cg == null) cg = panelCargando.AddComponent<CanvasGroup>();

            float cronoFade = 0f;
            while (cronoFade < tiempoDesvanecidoCarga)
            {
                cronoFade += Time.deltaTime;
                cg.alpha = Mathf.Lerp(1f, 0f, cronoFade / tiempoDesvanecidoCarga);
                yield return null;
            }
            
            // Ocultar tanto el panel como el texto explícitamente
            panelCargando.SetActive(false);
            if (txtCargando != null) txtCargando.gameObject.SetActive(false);
            
            cg.alpha = 1f; // Restaurar el alfa a 1 para futuras cargas
        }

        alFinalizar?.Invoke();
    }

    // ==========================================
    // GAME OVER UI
    // ==========================================
    public void MostrarGameOver(Action alCompletarFade)
    {
        StartCoroutine(RutinaGameOverUI(alCompletarFade));
    }

    private IEnumerator RutinaGameOverUI(Action alCompletarFade)
    {
        if (panelFiltroGris != null)
        {
            panelFiltroGris.gameObject.SetActive(true);
            float cronoGris = 0f;
            while (cronoGris < 1.0f)
            {
                cronoGris += Time.deltaTime;
                Color c = panelFiltroGris.color;
                c.a = Mathf.Lerp(0f, 0.6f, cronoGris / 1.0f);
                panelFiltroGris.color = c;
                yield return null;
            }
        }

        if (txtGameOver != null) txtGameOver.SetActive(true);

        if (panelFade != null)
        {
            panelFade.gameObject.SetActive(true);
            float crono = 0f;
            while (crono < tiempoFadeInGameOver)
            {
                crono += Time.deltaTime;
                Color c = panelFade.color;
                c.a = Mathf.Clamp01(crono / tiempoFadeInGameOver);
                panelFade.color = c;
                yield return null;
            }
        }

        alCompletarFade?.Invoke();
    }

    // ==========================================
    // VICTORIA UI
    // ==========================================
    public void MostrarVictoria(Action alListosParaEnter)
    {
        StartCoroutine(RutinaVictoriaUI(alListosParaEnter));
    }

    private IEnumerator RutinaVictoriaUI(Action alListosParaEnter)
    {
        if (imgMisionCompletada != null)
        {
            imgMisionCompletada.SetActive(true);
            CanvasGroup cg = imgMisionCompletada.GetComponent<CanvasGroup>();
            if (cg == null) cg = imgMisionCompletada.AddComponent<CanvasGroup>();

            float crono = 0f;
            while (crono < tiempoFadeInImagen)
            {
                crono += Time.deltaTime;
                cg.alpha = Mathf.Clamp01(crono / tiempoFadeInImagen);
                yield return null;
            }
            cg.alpha = 1f;
        }

        if (txtPressEnter != null)
        {
            txtPressEnter.gameObject.SetActive(true);
            float cronoTxt = 0f;
            while (cronoTxt < 0.8f)
            {
                cronoTxt += Time.deltaTime;
                Color c = txtPressEnter.color;
                c.a = Mathf.Clamp01(cronoTxt / 0.8f);
                txtPressEnter.color = c;
                yield return null;
            }

            corrutinaParpadeo = StartCoroutine(RutinaParpadeoTexto());
        }

        alListosParaEnter?.Invoke();
    }

    public void DetenerParpadeoEnter()
    {
        if (corrutinaParpadeo != null) StopCoroutine(corrutinaParpadeo);
    }

    private IEnumerator RutinaParpadeoTexto()
    {
        while (true)
        {
            float alfa = (Mathf.Sin(Time.time * velocidadParpadeoTexto) + 1.0f) / 2.0f;
            alfa = Mathf.Lerp(0.15f, 1.0f, alfa);

            if (txtPressEnter != null)
            {
                Color c = txtPressEnter.color;
                c.a = alfa;
                txtPressEnter.color = c;
            }
            yield return null;
        }
    }

    public void RealizarFadeOutNegro(Action alTerminar)
    {
        StartCoroutine(RutinaFadeOutNegro(alTerminar));
    }

    private IEnumerator RutinaFadeOutNegro(Action alTerminar)
    {
        if (panelFade != null)
        {
            panelFade.gameObject.SetActive(true);
            float crono = 0f;
            while (crono < tiempoFadeInNegro)
            {
                crono += Time.deltaTime;
                Color c = panelFade.color;
                c.a = Mathf.Clamp01(crono / tiempoFadeInNegro);
                panelFade.color = c;
                yield return null;
            }
            Color cFinal = panelFade.color; cFinal.a = 1f; panelFade.color = cFinal;
        }
        alTerminar?.Invoke();
    }
}