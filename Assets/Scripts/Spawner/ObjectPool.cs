﻿using UnityEngine;
using System.Collections.Generic;

namespace NeonBlood
{
    /// <summary>
    /// Clase que controla el pooling de objetos
    /// </summary>
    public class ObjectPool : MonoBehaviour
    {
        public static ObjectPool Instance;

        //The object prefabs which the pool can handle.
        public GameObject[] objectPrefabs;

        //The pooled objects currently available.
        public List<GameObject>[] pooledObjects;

        //The amount of objects of each type to buffer.
        public int[] amountToBuffer;

        public int defaultBufferAmount = 3;

        void Awake()
        {
            Instance = this;

            //Loop through the object prefabs and make a new list for each one.
            //We do this because the pool can only support prefabs set to it in the editor,
            //so we can assume the lists of pooled objects are in the same order as object prefabs in the array
            pooledObjects = new List<GameObject>[objectPrefabs.Length];

            int i = 0;
            foreach (GameObject objectPrefab in objectPrefabs)
            {
                pooledObjects[i] = new List<GameObject>();

                int bufferAmount;

                if (i < amountToBuffer.Length) bufferAmount = amountToBuffer[i];
                else bufferAmount = defaultBufferAmount;

                for (int n = 0; n < bufferAmount; n++)
                {
                    GameObject newObj = Instantiate(objectPrefab) as GameObject;
                    newObj.name = objectPrefab.name;
                    PoolObject(newObj);
                }

                i++;
            }
        }

        /// <summary>
        /// Gets a new object for the name type provided.  If no object type exists or if onlypooled is true and there is no objects of that type in the pool
        /// then null will be returned.
        /// </summary>
        /// <returns>The object for type.</returns>
        /// <param name='objectType'> Object type.</param>
        /// <param name='onlyPooled'>If true, it will only return an object if there is one currently pooled.</param>
        public GameObject GetObjectForType(string objectType, bool onlyPooled)
        {
            for (int i = 0; i < objectPrefabs.Length; i++)
            {
                GameObject prefab = objectPrefabs[i];
                if (prefab.name == objectType)
                {
                    if (pooledObjects[i].Count > 0)
                    {
                        GameObject pooledObject = pooledObjects[i][0];
                        pooledObjects[i].RemoveAt(0);
                        pooledObject.transform.parent = null;
                        pooledObject.SetActive(true);

                        return pooledObject;
                    }
                    else if (!onlyPooled)
                    {
                        Debug.Log("Instantiate");
                        return Instantiate(objectPrefabs[i]);
                    }

                    break;
                }
            }

            //If we have gotten here either there was no object of the specified type or non were left in the pool with onlyPooled set to true
            return null;
        }

        /// <summary>
        /// Pools the object specified.  Will not be pooled if there is no prefab of that type.
        /// </summary>
        /// <param name='obj'>Object to be pooled.</param>
        public void PoolObject(GameObject obj)
        {
            for (int i = 0; i < objectPrefabs.Length; i++)
            {
                if (objectPrefabs[i].name == obj.name)
                {
                    obj.SetActive(false);
                    obj.transform.parent = this.transform;
                    pooledObjects[i].Add(obj);
                    return;
                }
            }
        }
    }
}