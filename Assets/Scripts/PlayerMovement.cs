using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
  public Rigidbody2D rbody;

  [Header("Estadisticas")]
  public float jumpForce = 10;
  public float walkSpeed = 10;
  private float currentSpeed = 0;

  [Header("Colisiones")]
  public Vector2 abajo = new Vector2(0, -0.79f); // para conocer la posicion de los pies
  public float radioColision = 0.1f; // separacion entre el pie y el piso para la colision
  public  LayerMask layerPiso; // La capa con la que existe la colision

  [Header("Booleanos / Banderas")] 
  public bool enSuelo;

  void Start(){}  // Se llama a Start antes de la actualización del primer fotograma
  void Update() // Update se llama una vez por fotograma
  {
    // Permite al personaje permanecer con su velovidad al caer al suelo
    rbody.velocity = new Vector2(currentSpeed, rbody.velocity.y);
    TocarSuelo();
  }

  // Metodo OnJump se ejecuta cuando el Input Action Asset detecta el evento Jump
  // por medio de los inputs que especificamos (up, w, space)
  private void OnJump() 
  {
    /* TAREA: Implementar una restriccion para no poder saltar en el aire */
    /*if (rbody.velocity.y == 0) 
    {
      rbody.velocity = new Vector2(rbody.velocity.x, jumpForce);
    }*/
    if (enSuelo) 
    {
      Debug.Log("Jump!");
      rbody.velocity = new Vector2(rbody.velocity.x, jumpForce); // Aplica fuerza del salto
    }
  }

  // Metodo OnMove se ejecuta cuando el Input Action Asset detecta el evento Move
  // por medio de los inputs que especificamos (left, right, a, d)
  private void OnMove(InputValue inputValue) 
  {
    // Recuperamos el input izquierda o derecha -1 / 0 / 1
    float moveValue = inputValue.Get<float>();
    // Actualizamos la velocidad con el valor 
    // rbody.velocity = new Vector2(moveValue*walkSpeed, rbody.velocity.y);
    currentSpeed = moveValue * walkSpeed;


    // Logica para cambiar direccion del personaje
    if (moveValue * transform.localScale.x < 0) // input y direccion opuestos
    {
      transform.localScale = new Vector2(-1 * transform.localScale.x, transform.localScale.y);
    }

  }

  private void TocarSuelo() 
  {
    // determina coordenadas de los pies
    Vector2 pies = (Vector2)transform.position + abajo; 
    // checa si existe colision
    enSuelo = Physics2D.OverlapCircle(pies, radioColision, layerPiso); 
  }
}
