using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using TMPro;

namespace NeonBlood
{
    public class ElevatorCanvas : MMSingleton<ElevatorCanvas>
    {
        [Header("Main Panels")]
        public GameObject ElevatorPanel;

        [Header("Buttons")]
        public GameObject ElevatorFirstFloorButton;

        private ElevatorEvent ee;

        protected override void Awake()
        {
            base.Awake();
        }

        void Start()
        {

        }

        void Update()
        {
            if (Input.GetButtonDown("Circle_UI") && this.ElevatorPanel.activeSelf)
                this.ExitElevatorPanel();
        }

        public IEnumerator ShowElevatorPanel(ElevatorEvent ee)
        {
            //Comprobar que el panel de Mapa no esta activo
            if (MapCanvas.Instance.ContainerPanel.activeSelf ||
                PauseCanvas.Instance.MainPanel.activeSelf)
                yield break;

            yield return new WaitForEndOfFrame();

            MenuCanvas.Instance.Pause(true);
            this.ElevatorPanel.SetActive(true);
            PauseCanvas.Instance.CanPause = false;

            this.ee = ee;

            EventSystem.current.SetSelectedGameObject(this.ElevatorFirstFloorButton);
        }

        public void ExitElevatorPanel()
        {
            MenuCanvas.Instance.Pause(false);
            this.ElevatorPanel.SetActive(false);
            PauseCanvas.Instance.CanPause = true;

            this.ee.ReactiveEvent();
            this.ee = null;

            EventSystem.current.SetSelectedGameObject(null);
        }

        public void GoToFloor(int indexFloor)
        {
            if (this.ee.CurrentFloor == indexFloor)
                return;
            
            this.ee.GoToFloor(indexFloor);

            //Cierro el panel del ascensor
            MenuCanvas.Instance.Pause(false);
            this.ElevatorPanel.SetActive(false);
            PauseCanvas.Instance.CanPause = true;

            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}