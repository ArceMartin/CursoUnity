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
  public CinemachineVirtualCamera cm;

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
  public  bool puedeDash;    // Nos limita si se puede hacer dash
  public  bool puedoMover = true;
  public  bool vibrando = false;

  private void Awake() 
  {
    cm = GameObject.FindGameObjectWithTag("VirtualCamera")
      .GetComponent<CinemachineVirtualCamera>();
  }

  void Start(){}  // Una vez antes de la actualización del primer fotograma
  
  void Update() // Una vez por fotograma
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

    // Logica para cambiar direccion del sprite
    if (moveValue * transform.localScale.x < 0) // input y direccion opuestos
      transform.localScale = new Vector2(-1 * transform.localScale.x, transform.localScale.y);
  }

  // Metodo OnDash se ejecuta cuando el Input Action Asset detecta el evento Dash
  // por medio de los inputs que especificamos (left-shift)
  private void OnDash()  
  {
    // Verifica que el player esta en movimiento
    if (rbody.velocity != Vector2.zero && !haciendoDash) 
    {
      // Efecto de ripple centrado en el player
      Vector3 posicionJugador = Camera.main.WorldToViewportPoint(transform.position);
      Camera.main.GetComponent<RippleEffect>().Emit(posicionJugador);

      // Efecto de agitar camara
      StartCoroutine(AgitarCamara());

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
  private IEnumerator AgitarCamara() 
  {
    vibrando = true;
    CinemachineBasicMultiChannelPerlin cmNoise;
    cmNoise = cm.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

    // Cambiamos la amplitud a 10 por 0.3 segundos
    cmNoise.m_AmplitudeGain  = 10;
    yield return new WaitForSeconds(0.3f);
    cmNoise.m_AmplitudeGain  = 0;
    vibrando = false;
  }

  /*Corutina para aislar el movimiento dash (0.5 segundos)*/
  private IEnumerator EjecutaDash()  
  {
    rbody.gravityScale = 0;
    haciendoDash = true;
    anim.SetBool("dash", true); 

    yield return new WaitForSeconds(0.5f);
    rbody.gravityScale = 1;
    anim.SetBool("dash", false);
    rbody.velocity = velocityPreDash;
  }

  /*Esta corutina limita el dash con un tiempo de espera de 5 segundos*/
  private IEnumerator LimitaDash() 
  {
    yield return new WaitForSeconds(5f);
    haciendoDash = false;
  }

  // Usamos propiedad de Polimorfismo 
  // Segunda corutina identica pero que admite un argumento
/*  private IEnumerator AgitarCamara(float tiempo) 
  {
    vibrando = true;
    CinemachineBasicMultiChannelPerlin cmNoise;
    cmNoise = cm.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

    // Cambiamos la amplitud a 10 por 0.3 segundos
    cmNoise.m_AmplitudeGain  = 10;
    yield return new WaitForSeconds(tiempo);
    cmNoise.m_AmplitudeGain  = 0;
    vibrando = false;
  }*/
}
