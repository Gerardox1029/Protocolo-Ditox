using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    [Header("Audio Sources")]
    public AudioSource audioSourceMusica;       
    public AudioSource audioSourceVictoria;     
    public AudioSource audioSourceMuertePlayer; 

    public void ReproducirVictoria()
    {
        if (audioSourceVictoria != null) audioSourceVictoria.Play();
    }

    public void ReproducirMuertePlayer()
    {
        if (audioSourceMuertePlayer != null) audioSourceMuertePlayer.Play();
    }

    public void DesvanecerMusica(float duracion)
    {
        if (audioSourceMusica != null)
        {
            StartCoroutine(RutinaDesvanecerAudio(audioSourceMusica, duracion));
        }
    }

    private IEnumerator RutinaDesvanecerAudio(AudioSource aSource, float duracion)
    {
        float volumenInicial = aSource.volume;
        float crono = 0f;

        while (crono < duracion)
        {
            crono += Time.deltaTime;
            aSource.volume = Mathf.Lerp(volumenInicial, 0f, crono / duracion);
            yield return null;
        }

        aSource.Stop();
    }
}