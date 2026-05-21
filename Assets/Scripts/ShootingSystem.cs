using UnityEngine;

/// <summary>
/// Sistema básico de disparos con Raycast para un Rail Shooter.
/// Detecta si se apunta a un objetivo de la capa ("Layer") de los Enemigos 
/// y dispara de forma automática.
/// </summary>
public class ShootingSystem : MonoBehaviour
{
    [Header("Configuración del Raycast")]

    [Tooltip("El origen desde donde sale el disparo (puede ser la punta del arma). Si está vacío, usará la posición de este objeto.")]
    [SerializeField] private Transform puntoDeDisparo;

    [Tooltip("El objeto que dicta Hacia Donde ves o la dirección Frontal del disparo (ej: la Main Camera o un punto vacío). Si se deja nulo, tomará la Main Camera.")]
    [SerializeField] private Transform referenciaDireccion;

    [Tooltip("El alcance máximo del arma.")]
    [SerializeField] private float alcance = 100f;

    [Tooltip("La capa (o capas) que el raycast considerará como 'Enemigo'.")]
    [SerializeField] private LayerMask capaEnemigo;

    [Header("Configuración de Disparo")]

    [Tooltip("El intervalo en segundos entre cada disparo automático.")]
    [SerializeField] private float tiempoEntreDisparos = 0.5f;

    // Temporalizador del próximo disparo
    private float tiempoRestanteParaDisparar = 0f;

    void Start()
    {
        // Usar la posición del componente base en caso de no definir un cañón
        if (puntoDeDisparo == null)
        {
            puntoDeDisparo = this.transform;
        }

        // Si no se asigno hacia donde ver, usará la camara principal del juego
        if (referenciaDireccion == null)
        {
            if (Camera.main != null)
                referenciaDireccion = Camera.main.transform;
            else
                referenciaDireccion = this.transform; // Fallback
        }
    }

    void Update()
    {
        // Disminuir tiempo en el cooldown de disparos
        if (tiempoRestanteParaDisparar > 0)
        {
            tiempoRestanteParaDisparar -= Time.deltaTime;
        }

        DetectarEnemigoYDisparar();
    }

    /// <summary>
    /// Intenta detectar enemigos frente al cañón y ejecuta la lógica de disparo.
    /// </summary>
    private void DetectarEnemigoYDisparar()
    {
        // Configurar la caja / vector del Raycast. Usamos la POSICIÓN del punto del arma, 
        // pero usamos la DIRECCIÓN (forward) de la cámara o de la referencia dada
        Ray rayo = new Ray(puntoDeDisparo.position, referenciaDireccion.forward);
        RaycastHit hit;

        // Validar si el Raycast choca con algo de la capa requerida
        if (Physics.Raycast(rayo, out hit, alcance, capaEnemigo))
        {
            // Validar si se puede disparar o estamos en 'cooldown'
            if (tiempoRestanteParaDisparar <= 0f)
            {
                EmpezarDisparo(hit);
                
                // Reiniciar el contador
                tiempoRestanteParaDisparar = tiempoEntreDisparos;
            }
        }
    }

    /// <summary>
    /// Maneja lo que ocurre cuando el arma reacciona al detectar al enemigo.
    /// </summary>
    /// <param name="hitInfo">Información opcional sobre el impacto, contiene posición del enemigo o objeto impactado.</param>
    private void EmpezarDisparo(RaycastHit hitInfo)
    {
        // De momento, simplemente imprime una señal de impacto en nuestra consola como el usuario indicó.
        Debug.Log($"Impacto al enemigo localizado en: {hitInfo.transform.name}");
        
        // Espacio para modularizar si más adelante se requiere instanciar una bala, 
        // reproducir efectos sonoros o restar vida al enemigo, por ejemplo:
        //
        // IEnemy enemigo = hitInfo.transform.GetComponent<IEnemy>();
        // if (enemigo != null) 
        //     enemigo.RecibirDano(10);
    }
    
    /// <summary>
    /// Metodo auxiliar de Unity para dibujar una linea roja en la pantalla de escenas (Solo visible para Editor)
    /// </summary>
    private void OnDrawGizmos()
    {
        if (puntoDeDisparo != null)
        {
            Gizmos.color = Color.red;
            
            // Revisa qué dirección usar para dibujar el rayo de prueba
            Vector3 direccion = referenciaDireccion != null ? referenciaDireccion.forward : (Camera.main != null ? Camera.main.transform.forward : puntoDeDisparo.forward);
            Gizmos.DrawRay(puntoDeDisparo.position, direccion * alcance);
        }
    }
}
