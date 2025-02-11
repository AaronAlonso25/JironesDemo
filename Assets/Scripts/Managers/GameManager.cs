using UnityEngine;

/// <summary>
/// Clase GameManager para gestionar el estado global del juego.
/// Implementa un patrón Singleton para garantizar que solo exista una instancia de esta clase.
/// Controla el volumen del juego y la navegación a la escena del menú principal.
/// </summary>
public class GameManager : MonoBehaviour
{
    /// <summary>
    /// Instancia Singleton de la clase GameManager.
    /// Permite acceder a esta instancia desde cualquier otro script.
    /// </summary>
    public static GameManager Instance { get; private set; }

    /// <summary>
    /// Variable que controla el volumen del juego.
    /// El valor del volumen está comprendido entre 0.0 (silencio) y 1.0 (máximo).
    /// </summary>
    private float gameVolume = 1.0f;

    /// <summary>
    /// Método Awake se llama cuando el script de la instancia es cargado.
    /// Implementa el patrón Singleton y se asegura de que esta instancia no se destruya al cargar nuevas escenas.
    /// </summary>
    private void Awake()
    {
        // Implementación del Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // Esto hará que el objeto no se destruya al cargar una nueva escena
        }
        else
        {
            Destroy(gameObject);  // Si ya existe una instancia, destruye el objeto duplicado
        }
    }

    /// <summary>
    /// Obtiene el valor actual del volumen del juego.
    /// </summary>
    /// <returns>Un valor float que representa el volumen del juego, entre 0.0 y 1.0.</returns>
    public float GetVolume()
    {
        return gameVolume;
    }

    /// <summary>
    /// Establece un nuevo valor para el volumen del juego.
    /// Clampea el valor para asegurarse de que esté en el rango entre 0.0 y 1.0.
    /// </summary>
    /// <param name="newVolume">El nuevo valor del volumen a establecer, debe estar entre 0.0 y 1.0.</param>
    public void SetVolume(float newVolume)
    {
        gameVolume = Mathf.Clamp(newVolume, 0.0f, 1.0f);  // Asegura que el volumen esté entre 0 y 1
        AudioListener.volume = gameVolume;  // Ajusta el volumen global del juego
    }

    
   
}