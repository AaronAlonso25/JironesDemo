using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class EscEndManager : MonoBehaviour
{
void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("¡Has presionado Escape! Ejecutando acción...");
            SceneManager.LoadScene("Escena 0");
        }
    }
}
