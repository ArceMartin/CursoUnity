using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
  public  Rigidbody2D rbody;
  public  Animator anim;
  private Vector2 velocityPreDash;

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
  public  bool haciendoDash; // Nos indica si se encuentra haciendo dash
  public  bool puedeDash;    // Nos limita si se puede hacer dash
  public  bool puedoMover = true;

  void Start(){}  // Una vez antes de la actualización del primer fotograma
  
  void Update() // Una vez por fotograma
  {
    if (puedoMover && !haciendoDash) 
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
    if (rbody.velocity != Vector2.zero) 
    {
      // Efecto de ripple centrado en el player
      Vector3 posicionJugador = Camera.main.WorldToViewportPoint(transform.position);
      Camera.main.GetComponent<RippleEffect>().Emit(posicionJugador);

      // Animación
      anim.SetBool("dash", true);

      // guarda vector velocidad actual
      velocityPreDash = rbody.velocity; 
      // aplica velocidad aumentada de dash
      rbody.velocity = rbody.velocity.normalized * velocidadDash; 

      StartCoroutine(PrepararDash());
    }
  }

  private IEnumerator PrepararDash()  // Esta corutina prepara el dash del personaje
  {
    StartCoroutine(DashSuelo());
    rbody.gravityScale = 0;
    haciendoDash = true;

    yield return new WaitForSeconds(0.35f);
    rbody.gravityScale = 1;
    haciendoDash = false;
  }

  private IEnumerator DashSuelo() 
  {
    yield return new WaitForSeconds(0.15f);
    if(enSuelo)
    {
      puedeDash = false;
    }
  }

  // Metodo para finalizar la animacion de dash 
  public void FinalizarDash() 
  {
    // Finaliza animacion
    anim.SetBool("dash", false);
    rbody.velocity = velocityPreDash;
  }
}
