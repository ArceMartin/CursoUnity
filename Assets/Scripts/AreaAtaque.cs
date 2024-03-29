using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaAtaque : MonoBehaviour
{
  public Vector2 knockbackForce; // manipulamos el vector de empuje desde el editor
  public Transform playerTransform; // Para obtener la escala en x del player
  public int hits = 3;

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

      // Recuperamos los sprites de los corazones
      SpriteRenderer[] corazones = collision.GetComponentsInChildren<SpriteRenderer>();
      // Logica para desaparecer los corazones y el enemigo con cada golpe
      if (hits > 0) 
      {
        // Los primeros 3 golpes eliminan cada corazon
        corazones[hits].GetComponent<Renderer>().enabled = false;
        hits--;
        Debug.Log($"hits = {hits}");
        // El 3er golpe desaparece al enemigo
        if (hits==0) collision.GetComponent<Renderer>().enabled = false;
      }
    }
  }
}
