using UnityEngine;

namespace NeonBlood
{
    /// <summary>
    /// Clase que destruye todos los objetos que pasen por el
    /// </summary>
    public class Destroyer : MonoBehaviour
    {
        public ObjectPool Pool;

        void OnTriggerEnter(Collider col)
        {
            this.Pool.PoolObject(col.gameObject);
        }
    }
}