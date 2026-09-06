using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirStrikeManager : MonoBehaviour
{
    [Header("Referencias")]
    public PlaneController[] todosLosAviones; // Arrastra tus 6 aviones aquí

    [Header("Tiempos de Salida del Gestor")]
    public float tiempoMinimoIntento = 3f;
    public float tiempoMaximoIntento = 6f;

    void Start()
    {
        StartCoroutine(BucleAtaquesAereos());
    }

    IEnumerator BucleAtaquesAereos()
    {
        while (true)
        {
            // Espera el intervalo aleatorio para el próximo lanzamiento
            float tiempoEspera = Random.Range(tiempoMinimoIntento, tiempoMaximoIntento);
            yield return new WaitForSeconds(tiempoEspera);

            // Filtrar solo los aviones que están libres y fuera de cooldown
            List<PlaneController> avionesDisponibles = new List<PlaneController>();
            foreach (PlaneController avion in todosLosAviones)
            {
                if (avion != null && avion.estaDisponible)
                {
                    avionesDisponibles.Add(avion);
                }
            }

            // Si hay al menos un avión libre, elegimos uno al azar de esa lista
            if (avionesDisponibles.Count > 0)
            {
                int indiceAleatorio = Random.Range(0, avionesDisponibles.Count);
                avionesDisponibles[indiceAleatorio].IniciarSalida();
            }
            // Si todos están volando o descansando, el bucle ignorará este turno y reintentará en el siguiente intervalo.
        }
    }
}