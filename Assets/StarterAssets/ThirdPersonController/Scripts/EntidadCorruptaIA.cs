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
    private float tiempoGolpe, tiempoRayo;

    [Header("Efectos Visuales")]
    public GameObject efectoGolpe;
    public GameObject efectoRayo;

    [Header("Movimiento")]
    public float distanciaAtaque = 3f; // distancia para atacar cuerpo a cuerpo
    public float velocidadMovimiento = 3f;
    public float velocidadGiro = 5f;
    private bool enCombate = false;

    [Header("Interfaz de Vida")]
    public Slider barraVidaEntidad;

    [Header("Detector de Combate")]
    public SphereCollider detectorCombate;

    private void Start()
    {
        vidaActual = vidaMaxima;
        ActualizarBarraDeVida();
    }

    private void Update()
    {
        if (vidaActual <= 0)
        {
            Morir();
            return;
        }

        if (enCombate && objetivo != null)
        {
            MoverYAtacar();
        }
        else
        {
            animador.SetBool("EstaCaminando", false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            objetivo = other.transform;
            enCombate = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            enCombate = false;
            animador.SetBool("EstaCaminando", false);
            objetivo = null;
        }
    }

    void MoverYAtacar()
    {
        Vector3 direccion = objetivo.position - transform.position;
        direccion.y = 0;

        float distancia = direccion.magnitude;

        if (distancia > distanciaAtaque)
        {
            // Caminar hacia el jugador
            transform.position += direccion.normalized * velocidadMovimiento * Time.deltaTime;
            animador.SetBool("EstaCaminando", true);
        }
        else
        {
            animador.SetBool("EstaCaminando", false);

            // Atacar solo si el cooldown paso y estamos en rango
            if (Time.time >= tiempoGolpe)
            {
                animador.SetTrigger("GolpePesado");
                tiempoGolpe = Time.time + cooldownGolpe;
            }

            if (Time.time >= tiempoRayo)
            {
                animador.SetTrigger("RayoOscuro");
                tiempoRayo = Time.time + cooldownRayo;
            }
        }

        // Girar hacia el jugador suavemente
        if (direccion != Vector3.zero)
        {
            Quaternion rotacionObjetivo = Quaternion.LookRotation(direccion);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo, velocidadGiro * Time.deltaTime);
        }
    }

    // Métodos que llamarán los Animation Events para activar los efectos visuales en el momento exacto de la animación

    public void EfectoGolpe()
    {
        if (efectoGolpe != null)
        {
            Vector3 posicionEfecto = transform.position + transform.forward * 1.5f + Vector3.up * 1f; 
            Instantiate(efectoGolpe, posicionEfecto, transform.rotation);
        }
    }

    public void EfectoRayo()
    {
        if (efectoRayo != null)
        {
            Vector3 posicionEfecto = transform.position + transform.forward * 2f + Vector3.up * 1.5f; 
            Instantiate(efectoRayo, posicionEfecto, transform.rotation);
        }
    }

    public void RecibirDanio(float danio)
    {
        vidaActual -= danio;
        ActualizarBarraDeVida();

        if (vidaActual <= 0)
        {
            Morir();
        }
    }

    void Morir()
    {
        animador.SetTrigger("Morir");
        Destroy(gameObject, 2f);
        enCombate = false;
    }

    void ActualizarBarraDeVida()
    {
        if (barraVidaEntidad != null)
        {
            barraVidaEntidad.value = vidaActual / vidaMaxima;
        }
    }
}
