using UnityEngine;

public class Jugador : MonoBehaviour
{
    public float vidaMaxima = 100f;
    private float vidaActual;

    private void Start()
    {
        vidaActual = vidaMaxima;
    }

    public void RecibirDanio(float danio)
    {
        vidaActual -= danio;
        if (vidaActual <= 0)
        {
            Morir();
        }
    }

    void Morir()
    {
        Debug.Log("El jugador ha muerto.");
        // Aquí puedes agregar lógica para manejar la muerte del jugador (respawn, game over, etc.)
    }
}
