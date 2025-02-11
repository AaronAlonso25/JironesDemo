using UnityEngine;
using UnityEngine.Events;

namespace NeonBlood
{
    /// <summary>
    /// Clase que representa un evento al iniciar un objeto
    /// </summary>
    public class EventOnStart : MonoBehaviour
    {
        public UnityEvent EventsOnStart;

        void Start()
        {
            EventsOnStart.Invoke();
        }
    }
}