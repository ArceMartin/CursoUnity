using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
  public Rigidbody2D rbody;
  public float jumpForce = 10;

  void Start(){}  // Se llama a Start antes de la actualización del primer fotograma
  void Update(){} // Update se llama una vez por fotograma

  // Metodo OnJump se ejecuta cuando el Input Action Asset detecta el evento Jump
  // por medio de los inputs que especificamos (up, w, space)
  private void OnJump() 
  {
    Debug.Log("Jump!");
    rbody.velocity = new Vector2(rbody.velocity.x, jumpForce);
  }
}
