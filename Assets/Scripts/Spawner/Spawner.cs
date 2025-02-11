using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace NeonBlood
{
    /// <summary>
    /// Clase que controla la creacion objetos mediante un pool
    /// </summary>
    public class Spawner : MonoBehaviour
    {
        //Lista de objetos a crear
        public List<GameObject> Pieces;
        //Pool
        public ObjectPool Pool;

        [Header("Object")]
        public Vector2 LoopTime = new Vector2(0.5f, 1.5f);
        public Vector2 Time = new Vector2(1, 1);
        public Vector3 Scale = new Vector3(1, 1, 1);
        public float Distance = 100;

        IEnumerator Start()
        {
            yield return StartCoroutine(this.Spawn());
        }

        /// <summary>
        /// Metodo que crea los objetos
        /// </summary>
        private IEnumerator Spawn()
        {
            while (true)
            {
                int index = Random.Range(0, this.Pieces.Count);
                GameObject piece = this.Pool.GetObjectForType(this.Pieces[index].name, false);

                piece.transform.position = this.transform.position;
                piece.transform.localScale = this.Scale;
                piece.transform.parent = this.transform;

                float time = Random.Range(this.Time.x, this.Time.y);

                MovementObject mo = piece.GetComponent<MovementObject>();

                MovementPoint mp = new MovementPoint();
                mp.Target = this.transform.position + new Vector3(Distance, 0, 0);
                mp.Time = time;

                mo.Path = new MovementPoint[] { mp };           

                float loopTime = Random.Range(this.LoopTime.x, this.LoopTime.y);
                yield return new WaitForSeconds(loopTime);
            }
        }
    }
}