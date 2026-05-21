using UnityEngine;

/// <summary>
/// Script de controlador de cámara diseñado para un Rail Shooter.
/// Limita el movimiento (rotación) en los ejes X y Y para evitar que
/// el jugador voltee completamente hacia atrás o arriba.
/// </summary>
public class CameraMovement : MonoBehaviour
{
    [Header("Configuración de Movimiento de Cámara")]

    [Tooltip("El ángulo de vista máximo de la cámara horizontalmente (de lado a lado). Por ejemplo: 45 grados.")]
    [SerializeField] private float anguloMaximoHorizontal = 60f;

    [Tooltip("El ángulo de vista máximo de la cámara verticalmente (de arriba a abajo). Por ejemplo: 30 grados.")]
    [SerializeField] private float anguloMaximoVertical = 45f;

    [Tooltip("La sensibilidad al mover el mouse.")]
    [SerializeField] private float sensibilidad = 2f;

    [Tooltip("Si es verdadero, los controles arriba/abajo se invertirán.")]
    [SerializeField] private bool invertirEjeY = false;

    // Variables internas para guardar la rotación acumulada respecto al inicio
    private float rotacionAcumuladaX = 0f;
    private float rotacionAcumuladaY = 0f;

    // Guarda la rotación inicial del objeto de la cámara
    private Quaternion rotacionInicial;

    void Start()
    {
        // En un juego de disparos con mira, normalmente queremos bloquear el cursor para que no salga de la pantalla.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        // Guardamos la rotación base inicial (ej: si empieza en 0, 180, 0) y calculamos nuestros límites relativos a ese punto.
        rotacionInicial = transform.localRotation;
    }

    void Update()
    {
        // Obtener la entrada del mouse
        float mouseX = Input.GetAxis("Mouse X") * sensibilidad;
        float mouseY = Input.GetAxis("Mouse Y") * sensibilidad;

        // Calcular la acumulación de movimiento
        rotacionAcumuladaY += mouseX;
        
        // Manejar el eje invertido
        if (invertirEjeY)
        {
            rotacionAcumuladaX += mouseY;
        }
        else
        {
            rotacionAcumuladaX -= mouseY;
        }

        // Limitar (clamp) los ángulos para que solo pueda desfasarse el límite especificado respecto al centro
        rotacionAcumuladaX = Mathf.Clamp(rotacionAcumuladaX, -anguloMaximoVertical, anguloMaximoVertical);
        rotacionAcumuladaY = Mathf.Clamp(rotacionAcumuladaY, -anguloMaximoHorizontal, anguloMaximoHorizontal);

        // Generar una rotación adicional basada en nuestro movimiento del ratón
        Quaternion rotacionAdicional = Quaternion.Euler(rotacionAcumuladaX, rotacionAcumuladaY, 0f);

        // Se usa la multiplicación de cuaterniones para sumar las rotaciones: Rotacion Inicial + Rotacion Adicional
        transform.localRotation = rotacionInicial * rotacionAdicional;
    }
}
