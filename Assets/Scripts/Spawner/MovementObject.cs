using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace NeonBlood
{
    [System.Serializable]
    public struct MovementPoint
    {
        public Vector3 Target;
        public float Time;
        public bool DeactivateAfterMovement;
    }

    /// <summary>
    /// Clase que representa el movimiento de un objeto
    /// </summary>
    public class MovementObject : MonoBehaviour
    {
        [Header("Movement")]
        public MovementPoint[] Path;
        private int indexPoint = 0;

        //Indica si se mueve localmente
        public bool MoveLocal = false;
        //Indica si se mueve durante el camino o solo el primer punto
        public bool MovePath = false;

        IEnumerator Start()
        {
            if (this.MovePath)
            {
                while(this.indexPoint < this.Path.Length)
                {
                    yield return StartCoroutine(this.Move());
                    this.indexPoint++;
                }
            }
            else
                yield return StartCoroutine(this.Move());
        }

        /// <summary>
        /// Metodo para realizar el movimiento del objeto
        /// </summary>
        /// <returns></returns>
        public IEnumerator Move()
        {
            if (!this.MoveLocal)
                yield return StartCoroutine(
                    MMMovement.MoveFromTo(this.gameObject, this.transform.position, this.Path[this.indexPoint].Target, this.Path[this.indexPoint].Time, 0.01f));
            else
                yield return StartCoroutine(
                    MMMovement.MoveLocalFromTo(this.gameObject, this.transform.localPosition, this.Path[this.indexPoint].Target, this.Path[this.indexPoint].Time, 0.01f));
        }

        /// <summary>
        /// Metodo para que el objeto se mueva al siguiente punto
        /// </summary>
        public void MoveNextPoint()
        {
            this.indexPoint++;
            StartCoroutine(this.Move());
        }
    }
}