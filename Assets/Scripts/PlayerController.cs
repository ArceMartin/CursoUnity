// PlayerController.cs

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
  private Rigidbody2D rb;
  public float velocidad = 10;

  private void awake() 
  {
    rb = GetComponent<Rigidbody2D>();
  }

  void Start(){}  // Se llama antes de la actualización del primer fotograma
  void Update(){} // Se llama una vez por fotograma

  private void Movimiento() 
  {
    float x = Input.GetAxis("Horizontal"); // Recupera el eje input de Unity
    float y = Input.GetAxis("Vertical");   // Recupera el eje input de Unity
  }

  private void Caminar() 
  {
    
  }

}
