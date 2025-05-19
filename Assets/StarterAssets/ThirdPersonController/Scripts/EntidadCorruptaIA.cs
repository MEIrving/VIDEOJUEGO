using UnityEngine;
using UnityEngine.UI;

public class ControladorEntidadCorrupta : MonoBehaviour
{
    [Header("Componentes")]
    public Animator animador;
    public Transform objetivo;
    public float vidaMaxima = 1000f;
    private float vidaActual;

    [Header("Habilidades")]
    public float cooldownGolpe = 3f;
    public float cooldownRayo = 5f;
    public float cooldownCarga = 7f;
    private float tiempoGolpe, tiempoRayo, tiempoCarga;

    [Header("Efectos Visuales")]
    public GameObject efectoGolpe;
    public GameObject efectoRayo;
    public GameObject efectoCargar;

    [Header("Movimiento")]
    public float distanciaAtaque = 10f;
    public float velocidadMovimiento = 3f;
    public float velocidadGiro = 2f;

    [Header("Interfaz de Vida")]
    public Slider barraVidaEntidad;
    public Slider barraVidaJugador;
    public float vidaJugador = 500f;
    private float vidaActualJugador;

    private void Start()
    {
        vidaActual = vidaMaxima;
        vidaActualJugador = vidaJugador;
        ActualizarBarrasDeVida();
    }

    private void Update()
    {
        if (vidaActual <= 0)
        {
            Morir();
            return;
        }

        ControlarMovimiento();
        ControlarHabilidades();
        ControlarRegeneracion();
        ActualizarBarrasDeVida();
    }

    void ControlarMovimiento()
    {
        if (objetivo == null) return;

        Vector3 direccion = objetivo.position - transform.position;
        float distancia = direccion.magnitude;

        if (distancia > distanciaAtaque)
        {
            direccion.y = 0;
            transform.position += direccion.normalized * velocidadMovimiento * Time.deltaTime;
        }

        Quaternion rotacionObjetivo = Quaternion.LookRotation(direccion);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo, velocidadGiro * Time.deltaTime);
    }

    void ControlarHabilidades()
    {
        if (Vector3.Distance(transform.position, objetivo.position) <= distanciaAtaque)
        {
            if (Time.time >= tiempoGolpe)
            {
                GolpePesado();
                tiempoGolpe = Time.time + cooldownGolpe;
            }

            if (Time.time >= tiempoRayo)
            {
                RayoOscuro();
                tiempoRayo = Time.time + cooldownRayo;
            }

            if (Time.time >= tiempoCarga)
            {
                CargarFerozmente();
                tiempoCarga = Time.time + cooldownCarga;
            }
        }
    }

    void GolpePesado()
    {
        animador.SetTrigger("GolpePesado");
        Instantiate(efectoGolpe, transform.position, transform.rotation);
        if (Vector3.Distance(transform.position, objetivo.position) <= 3f)
        {
            objetivo.GetComponent<Jugador>()?.RecibirDanio(50f);
        }
    }

    void RayoOscuro()
    {
        animador.SetTrigger("RayoOscuro");
        Instantiate(efectoRayo, transform.position, transform.rotation);
        objetivo.GetComponent<Jugador>()?.RecibirDanio(20f);
    }

    void CargarFerozmente()
    {
        animador.SetTrigger("Cargar");
        Instantiate(efectoCargar, transform.position, transform.rotation);
    }

    void ControlarRegeneracion()
    {
        if (vidaActual < vidaMaxima)
        {
            vidaActual += 10f * Time.deltaTime;
        }
    }

    void Morir()
    {
        animador.SetTrigger("Morir");
        Destroy(gameObject, 2f);
    }

    public void RecibirDanio(float danio)
    {
        vidaActual -= danio;
        if (vidaActual <= vidaMaxima * 0.5f)
        {
            animador.SetBool("Debilitado", true);
        }
    }

    void ActualizarBarrasDeVida()
    {
        if (barraVidaEntidad) barraVidaEntidad.value = vidaActual / vidaMaxima;
        if (barraVidaJugador) barraVidaJugador.value = vidaActualJugador / vidaJugador;
    }
}
