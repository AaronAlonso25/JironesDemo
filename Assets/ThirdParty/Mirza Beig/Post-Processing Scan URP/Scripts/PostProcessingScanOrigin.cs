using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace NeonBlood
{
    public class PostProcessingScanOrigin : MonoBehaviour
    {
        public Material ScanMaterial;

        void Start()
        {

        }

        void OnEnable()
        {
            StartCoroutine(this.ActivatePulsarCo());
        }

        private IEnumerator ActivatePulsarCo()
        {
            this.ScanMaterial.SetColor("_Colour", Color.cyan);
            this.ScanMaterial.SetVector("_Position", this.transform.position);

            yield return StartCoroutine(ProgrammerTools.ScaleFromTo(this.gameObject, 0.25f, 1.2f, 1));

            this.ScanMaterial.SetColor("_Colour", new Color(0, 0, 0, 0));
            this.gameObject.SetActive(false);
        }
    }
}