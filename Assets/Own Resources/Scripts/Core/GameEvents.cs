using System;

public static class GameEvents
{
    // Evento para notificar cambios de vida (vida actual, vida máxima)
    public static Action<float, float> OnVidaCambiada;
    
    // Evento para notificar la muerte del jugador
    public static Action OnJugadorMuere;

    // Evento para reproducir efectos de sonido de forma desacoplada
    public static Action<UnityEngine.AudioClip, float> OnReproducirSonido;
}