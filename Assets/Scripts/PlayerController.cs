// PlayerController.cs

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
  private Rigidbody2D rb;
  public float velocidad = 10;

  private void Awake() 
  {
    rb = GetComponent<Rigidbody2D>();
  }

  void Start(){}  // Se llama antes de la actualización del primer fotograma
  void Update()   // Se llama una vez por fotograma
  {
    Movimiento();
  } 

  private void Movimiento() 
  {
    float x = Input.GetAxis("Horizontal"); // Recupera el eje input de Unity
    float y = Input.GetAxis("Vertical");   // Recupera el eje input de Unity

    Vector2 direccion = new Vector2(x, y); // Creamos un vector con los inputs

    Caminar(direccion);
  }

  private void Caminar(Vector2 direccion) 
  {
    // Al caminar tomamos en cuenta el input horizontal * la variable velocidad
    // La velocidad vertical permanece igual
    rb.velocity = new Vector2(
      direccion.x * velocidad,
      rb.velocity.y
      );
  }

}
