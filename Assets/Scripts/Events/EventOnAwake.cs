using UnityEngine;
using UnityEngine.Events;

namespace NeonBlood
{
    /// <summary>
    /// Clase que representa un evento al iniciar un objeto
    /// </summary>
    public class EventOnAwake : MonoBehaviour
    {
        public UnityEvent EventsOnAwake;

        void Awake()
        {
            EventsOnAwake.Invoke();
        }
    }
}