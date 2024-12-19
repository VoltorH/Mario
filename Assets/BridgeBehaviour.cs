using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeBehaviour : MonoBehaviour
{
    public Transform pivot; // El punto central del balanc�n.
    public float maxSpeed = 3f; // Velocidad m�xima del puente en los extremos.
    public float dampingFactor = 2f; // Factor para reducir el movimiento cerca del centro.
    public LayerMask playerLayer; // Capa para detectar al jugador.

    private Rigidbody rb; // El Rigidbody del puente.
    private float bridgeWidth = 10f; // El ancho del puente, ajusta seg�n el tama�o del puente.
    private bool playerOnBridge = false; // Si el jugador est?sobre el puente.

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody es necesario para este script.");
        }
    }

    void FixedUpdate()
    {
        // Detectar si el jugador est?sobre el puente usando una detecci�n de esfera.
        Collider[] players = Physics.OverlapSphere(transform.position, bridgeWidth, playerLayer);

        if (players.Length > 0)
        {
            playerOnBridge = true;

            // Suponemos que el primer jugador detectado es el relevante.
            Collider player = players[0];
            Vector3 playerPosition = player.transform.position;

            // Calcular la distancia desde el centro del puente (pivot).
            float distanceFromCenter = playerPosition.x - pivot.position.x;

            // Mapeamos esta distancia a un valor de velocidad entre -maxSpeed y maxSpeed
            float speed = Mathf.Clamp(distanceFromCenter / bridgeWidth, -1f, 1f) * -maxSpeed;
            print(speed);
            rb.AddTorque(Vector3.forward * speed); // Aplicar torque alrededor del eje Z.

        }
        else
        {
            // Si el jugador ya no est?sobre el puente, dejar de aplicar torque.
            if (playerOnBridge)
            {
                rb.angularVelocity = Vector3.zero; // Detener la rotaci�n del puente.
                playerOnBridge = false;
            }
        }
    }

    void OnDrawGizmos()
    {
        // Dibujar una esfera para visualizar el �rea de detecci�n en el Editor.
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, bridgeWidth);
    }
    
}
