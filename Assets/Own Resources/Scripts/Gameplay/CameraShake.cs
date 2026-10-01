using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private Vector3 posicionOriginal;

    void Awake()
    {
        posicionOriginal = transform.localPosition;
    }

    public void IniciarTemblor(float duracion, float magnitud)
    {
        StartCoroutine(RutinaTemblor(duracion, magnitud));
    }

    IEnumerator RutinaTemblor(float duracion, float magnitud)
    {
        float tiempoTranscurrido = 0f;

        while (tiempoTranscurrido < duracion)
        {
            // Genera desplazamientos aleatorios en X e Y
            float offsetX = Random.Range(-3f, 3f) * magnitud;
            float offsetY = Random.Range(-3f, 3f) * magnitud;

            // Aplica el temblor manteniendo la posición Z original de la cámara
            transform.localPosition = new Vector3(posicionOriginal.x + offsetX, posicionOriginal.y + offsetY, posicionOriginal.z);

            tiempoTranscurrido += Time.deltaTime;
            yield return null; // Espera al siguiente fotograma
        }

        // Devuelve la cámara a su posición exacta al terminar
        transform.localPosition = posicionOriginal;
    }
}