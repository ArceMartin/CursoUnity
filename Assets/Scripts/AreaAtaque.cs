using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaAtaque : MonoBehaviour
{
  public Vector2 knockbackForce; // manipulamos el vector de empuje desde el editor
  public Transform playerTransform; // Para obtener la escala en x del player

  void Start(){}
  void Update(){}

  private void OnTriggerEnter2D(Collider2D collision) 
  {
    if (collision.CompareTag("Enemigo"))
    {
      Debug.Log("Aplicar daño a enemigo");

      // extraemos el rigid body del enemigo con el que se colisiono
      Rigidbody2D enemyRb =  collision.GetComponent<Rigidbody2D>(); 

      // Obtenemos la dirección a la que esta volteando el personaje
      float direccion = playerTransform.localScale.x;
      // Debug.Log($"direccion = {direccion}");

      // Aplicamos fuerza de empuje al enemigo
      Vector2 directedKnockback = new Vector2(knockbackForce.x*direccion, knockbackForce.y);
      enemyRb.AddForce(directedKnockback, ForceMode2D.Impulse);
      // enemyRb.AddForce(directedKnockback);
    }
  }
}
