using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirStrikeManager : MonoBehaviour
{
    [Header("Referencias")]
    public PlaneController[] todosLosAviones; // Arrastra tus 8 aviones aquí

    [Header("Tiempos de Salida y Cooldown")]
    public float tiempoMinimoIntento = 3f;
    public float tiempoMaximoIntento = 6f;
    public float tiempoEnfriamientoAvion = 15f; // Tiempo que descansa el avión antes de poder volver a salir

    // Diccionario interno para llevar el control del tiempo de descanso de cada avión
    private Dictionary<PlaneController, float> temporizadoresCooldown = new Dictionary<PlaneController, float>();

    void Start()
    {
        // Inicializar el diccionario
        foreach (PlaneController avion in todosLosAviones)
        {
            if (avion != null)
            {
                temporizadoresCooldown[avion] = 0f;
            }
        }

        StartCoroutine(BucleAtaquesAereos());
    }

    void Update()
    {
        // Reducir el tiempo de enfriamiento de los aviones que están descansando
        foreach (PlaneController avion in todosLosAviones)
        {
            if (avion != null && !avion.estaDisponible && !avion.gameObject.activeSelf)
            {
                if (temporizadoresCooldown[avion] > 0f)
                {
                    temporizadoresCooldown[avion] -= Time.deltaTime;
                    if (temporizadoresCooldown[avion] <= 0f)
                    {
                        avion.estaDisponible = true; // ¡Vuelve a estar disponible!
                    }
                }
            }
        }
    }

    IEnumerator BucleAtaquesAereos()
    {
        while (true)
        {
            float tiempoEspera = Random.Range(tiempoMinimoIntento, tiempoMaximoIntento);
            yield return new WaitForSeconds(tiempoEspera);

            List<PlaneController> avionesDisponibles = new List<PlaneController>();
            foreach (PlaneController avion in todosLosAviones)
            {
                if (avion != null && avion.estaDisponible)
                {
                    avionesDisponibles.Add(avion);
                }
            }

            if (avionesDisponibles.Count > 0)
            {
                int indiceAleatorio = Random.Range(0, avionesDisponibles.Count);
                PlaneController avionElegido = avionesDisponibles[indiceAleatorio];

                // Inicia su salida y reinicia su cronómetro de cooldown para el futuro
                avionElegido.IniciarSalida();
                temporizadoresCooldown[avionElegido] = tiempoEnfriamientoAvion;
            }
        }
    }
}