// PlayerController.cs

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
  private Rigidbody2D rb;
  private Vector2 direccion;   // Vector para guardar los inputs en cada momento
  public float velocidad = 10; // velocidad de caminar
  public float fuerzaSalto = 5; // fuerza del salto

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
    direccion = new Vector2(x, y); // Vector con los inputs

    Caminar();
    DuracionSalto();

    // ejecuta este bloque si la barra espaciadora esta presionada
    if (Input.GetKeyDown(KeyCode.Space))
    {
      Saltar();
    }
  }

  private void Caminar() 
  {
    // Al caminar tomamos en cuenta el input horizontal * la variable velocidad
    // La velocidad vertical permanece igual
    rb.velocity = new Vector2(direccion.x * velocidad, rb.velocity.y);
  }

  private void DuracionSalto() 
  {
    // Personaje cayendo
    if (rb.velocity.y < 0) 
    {
      // Esta parte depende de la experiencia física que queremos dar al juego
      rb.velocity += Vector2.up 
        * Physics2D.gravity.y // nos da 'gravedad'
        * (2.5f - 1)          // cambiamos estos valores a gusto
        * Time.deltaTime;     // conforme pasa el tiempo, aumenta velocidad de caida
    }
    // Personaje saltando pero ya dejamos de presionar barra espaciadora
    else if (rb.velocity.y > 0 && Input.GetKey(KeyCode.Space))
    {
      rb.velocity += Vector2.up * Physics2D.gravity.y * (2.5f - 1) * Time.deltaTime; // igual al caso anterior
    }
  }

  private void Saltar() 
  {
    // ponemos en 0 la velocidad vertical inicial (inicio del salto)
    rb.velocity = new Vector2(rb.velocity.x, 0); 

    // ponemos la velocidad vertical proporcional a la fuerza del salto
    rb.velocity += Vector2.up * fuerzaSalto;
  }


}
