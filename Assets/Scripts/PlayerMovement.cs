// Taller de Videojuegos Unity 2022-4
// Martín Arce
// Matricula: 1103883

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class PlayerMovement : MonoBehaviour
{
  public  Rigidbody2D rbody;
  public  Animator anim;
  private Vector2 velocityPreDash;
  public  CinemachineVirtualCamera cm;
  private Vector2 direccionAtaque = new Vector2(1f,0f);

  [Header("Estadisticas")]
  public  float jumpForce = 6;
  public  float walkSpeed = 6;
  private float currentSpeed = 0;
  public  float velocidadDash = 20;

  [Header("Colisiones")]
  public  Vector2 abajo = new Vector2(0, -0.79f); // Posicion de los pies
  public  float radioColision = 0.1f; // Radio de colision pies/piso
  public  LayerMask layerPiso; // Capa con la que existe la colision

  [Header("Booleanos / Banderas")] 
  public  bool enSuelo;
  public  bool segundoSaltoDisponible;
  public  bool haciendoDash = false; // Nos indica si se encuentra haciendo dash
  private bool puedeDash = true;    // Nos limita si se puede hacer dash
  public  bool puedoMover = true;
  public  bool vibrando = false;
  public  bool atacando = false; // Indica si el personaje esta atacando

  private void Awake() 
  {
    cm = GameObject.FindGameObjectWithTag("VirtualCamera")
      .GetComponent<CinemachineVirtualCamera>();
  }

  /*Una vez antes de la actualización del primer fotograma*/
  void Start(){}  
  
  /*Una vez por fotograma*/
  void Update() 
  {
    // if (puedoMover && !haciendoDash) 
    if (puedoMover) 
    {
      // Actualiza velocidad horizontal
      rbody.velocity = new Vector2(currentSpeed, rbody.velocity.y);

      // Checa si existe colision con el suelo
      Vector2 pies = (Vector2)transform.position + abajo; 
      enSuelo = Physics2D.OverlapCircle(pies, radioColision, layerPiso); 

      // activa 2do salto al caer de una orilla
      if (enSuelo) 
        segundoSaltoDisponible = true;

      // Logica para animaciones
      if (enSuelo) 
      {
        anim.SetBool("saltar", false);
        // asignacion depende de velocidad horizontal (izq/der)
        anim.SetBool("caminar", rbody.velocity.x != 0); 
      }
      else
      {
        anim.SetBool("caminar", false);
        anim.SetBool("saltar", true);
        // asignacion depende de velocidad vertical (saltando/cayendo)
        anim.SetFloat("velocidadVertical", rbody.velocity.y > 0 ? 1 : -1); 
      }
    }
  }

  // Metodo OnJump se ejecuta cuando el Input Action Asset detecta el evento Jump
  // por medio de los inputs que especificamos (up, w, space)
  private void OnJump() 
  {
    if (enSuelo || segundoSaltoDisponible)
    {
      rbody.velocity = new Vector2(rbody.velocity.x, jumpForce); // Aplica fuerza del salto
      segundoSaltoDisponible = enSuelo; // limita y reestablece segundo salto
    }
  }

  // Metodo OnMove se ejecuta cuando el Input Action Asset detecta el evento Move
  // por medio de los inputs que especificamos (left, right, a, d)
  private void OnMove(InputValue inputValue) 
  {
    // Recuperamos el input izquierda o derecha -1 / 0 / 1
    float moveValue = inputValue.Get<float>();
    currentSpeed = moveValue * walkSpeed;
    // Copiamos input horizontal al vector direccionAtaque
    // if (moveValue != 0) 
      direccionAtaque.x = moveValue;

    // Logica para cambiar direccion del sprite
    if (moveValue * transform.localScale.x < 0) // input y direccion opuestos
      transform.localScale = new Vector2(-1 * transform.localScale.x, transform.localScale.y);
  }

  // Metodo OnDash se ejecuta cuando el Input Action Asset detecta el evento Dash
  // por medio de los inputs que especificamos (left-shift)
  private void OnDash()  
  {
    // Verifica que el player esta en movimiento
    if (rbody.velocity != Vector2.zero && puedeDash) 
    {
      // Efecto de ripple centrado en el player
      Vector3 posicionJugador = Camera.main.WorldToViewportPoint(transform.position);
      Camera.main.GetComponent<RippleEffect>().Emit(posicionJugador);

      // Efecto de agitar camara
      StartCoroutine(AgitarCamara(0.3f));

      // Guarda vector velocidad actual
      velocityPreDash = rbody.velocity; 
      Debug.Log($"velocityPreDash = {velocityPreDash.x}, {velocityPreDash.y}");
      // Aplica velocidad aumentada de dash
      rbody.velocity = rbody.velocity.normalized * velocidadDash; 
      Debug.Log($"rbody.velocity  = {rbody.velocity.x}, {rbody.velocity.y}");

      // Corutina para aislar el movimiento dash (0.5 segundos)
      StartCoroutine(EjecutaDash());
      // Corutina para limitar frecuencia del dash (5 segundos)
      StartCoroutine(LimitaDash());
    }
  }

  /*Corutina para modificar los parámetros del ruido de camara al hacer dash*/
  private IEnumerator AgitarCamara(float tiempo) 
  {
    vibrando = true;
    CinemachineBasicMultiChannelPerlin cmNoise;
    cmNoise = cm.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    cmNoise.m_AmplitudeGain  = 10;
    yield return new WaitForSeconds(tiempo);
    cmNoise.m_AmplitudeGain  = 0;
    vibrando = false;
  }

  /*Corutina para aislar el movimiento dash (0.5 segundos)*/
  private IEnumerator EjecutaDash()  
  {
    rbody.gravityScale = 0;
    haciendoDash = true;
    puedeDash = false;
    anim.SetBool("dash", true); 
    yield return new WaitForSeconds(0.5f);
    rbody.gravityScale = 1;
    anim.SetBool("dash", false);
    rbody.velocity = velocityPreDash;
    haciendoDash = false;
  }

  /*Corutina que limita el dash con un tiempo de espera de 5 segundos*/
  private IEnumerator LimitaDash() 
  {
    yield return new WaitForSeconds(5f);
    puedeDash = true;
  }

  // Metodo OnAttack se ejecuta cuando el Input Action Asset detecta el evento Attack
  // por medio del input que especificamos (Z)
  private void OnAttack() 
  {
    if (!atacando && !haciendoDash) 
    {
      atacando = true;
      anim.SetBool("ataque", true);
      anim.SetFloat("ataqueX", direccionAtaque.x);
      anim.SetFloat("ataqueY", direccionAtaque.y);
      Debug.Log($"x = {direccionAtaque.x}");
      Debug.Log($"y = {direccionAtaque.y}");
    }
  }

  private void FinalizarAtaque() 
  {
    anim.SetBool("ataque", false);
    atacando = false;
  }

  // Metodo OnLook se ejecuta cuando el Input Action Asset detecta el evento Look
  // por medio del input que especificamos (arriba, abajo, w, s)
  private void OnLook(InputValue inputValue) 
  {
    // Recuperamos el valor del input (-1, 0 o 1)
    float lookValue = inputValue.Get<float>();
    // Debug.Log("Look!" + lookValue);
    // Asignamos valor a vector direccionAtaque en base al input
    direccionAtaque.y = lookValue;
  }
}
