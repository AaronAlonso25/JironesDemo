using UnityEngine;
using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;

namespace NeonBlood
{
    public class LevelManager : MMSingleton<LevelManager>
    {
        public CharacterController Player { get; set; }

        private List<CheckPoint> _checkpoints;
        private int _currentCheckPointIndex;        

        void Start()
        {
            this.ConfigCheckPoints();
            this.ConfigPlayerPosition();
        }

        private void ConfigPlayerPosition()
        {
            //Busco a Axel
            this.Player = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>();

            //Si tengo un combate asociado, volvemos a colocar a Axel en su posicion original
            if (DataManager.Instance.CombatPosition != Vector3.zero)
            {
                if (FindObjectsByType<CombatEvent>(FindObjectsSortMode.None).Any(x => x.CombatName == DataManager.Instance.CombatName))
                {
                    CombatEvent _combat = FindObjectsByType<CombatEvent>(FindObjectsSortMode.None).Single(x => x.CombatName == DataManager.Instance.CombatName);

                    //Posicionar a Axel
                    this.Player.transform.position = DataManager.Instance.CombatPosition + _combat.CombatOffset;

                    //Ejecutamos el evento de final de combate del que hemos venido
                    _combat.EndCombat(DataManager.Instance.CombatResult);
                }
            }
            //Si es FastTravel
            else if (DataManager.Instance.CurrentLevelDoor == "FastTravel")
            {                
                FastTravelEvent _fast = FindFirstObjectByType<FastTravelEvent>();

                //Posicionar a Axel
                this.Player.transform.position = _fast.transform.position + _fast.FastTravelOffset;
            }
            //Si tengo una puerta asociada, colocamos a Axel en la puerta
            else if (DataManager.Instance.CurrentLevelDoor != "")
            {
                //Posicionar a Axel
                if (FindObjectsByType<DoorEvent>(FindObjectsSortMode.None).Any(x => x.name == DataManager.Instance.CurrentLevelDoor))
                {
                    DoorEvent _door = FindObjectsByType<DoorEvent>(FindObjectsSortMode.None).Single(x => x.name == DataManager.Instance.CurrentLevelDoor);
                    this.Player.transform.position = _door.transform.position + _door.DoorOffset;
                }
            }
        }

        #region Combats

        public void StartCombat(string combatName)
        {
            if (FindObjectsByType<CombatEvent>(FindObjectsSortMode.None).Any(x => x.CombatName == combatName))
                FindObjectsByType<CombatEvent>(FindObjectsSortMode.None).Single(x => x.CombatName == combatName).InitializeCombat();
        }

        #endregion

        #region CheckPoints

        private void ConfigCheckPoints()
        {
            //CheckPoints
            _checkpoints = FindObjectsByType<CheckPoint>(FindObjectsSortMode.None).Where(x => int.Parse(x.name.Split('_')[1]) == DataManager.Instance.CurrentChapter).ToList();
            _checkpoints = _checkpoints.OrderBy(x => int.Parse(x.name.Split('_')[2])).ToList();

            //Ejecuto el Check Point correspondiente
            _currentCheckPointIndex = DataManager.Instance.GetCheckPoint().CurrentCheckPointIndex;
            if (_currentCheckPointIndex != -1)
            {
                try
                {
                    _checkpoints[_currentCheckPointIndex].CheckPointEvent.Invoke();
                }
                catch (ArgumentOutOfRangeException)
                {
                    Debug.LogError("Recuerda meter el CheckPoint " + _currentCheckPointIndex + " en esta escena en el capítulo " + DataManager.Instance.CurrentChapter);
                }
            }
        }

        public void NextCheckPoint()
        {
            DataManager.Instance.GetCheckPoint().CurrentCheckPointIndex++;
        }

        /// <summary>
        /// Metodo para avanzar el CheckPoint de la escena indicada
        /// </summary>
        /// <param name="nameScene_CheckPointIndex">Ejemplo: Mercadoblind v7,1</param>
        public void SetCheckPoint(string nameScene_CheckPointIndex)
        {
            SaveDataLevel sdl = DataManager.Instance.GetCheckPoint(nameScene_CheckPointIndex.Split(',')[0]);
            int currentIndex = int.Parse(nameScene_CheckPointIndex.Split(',')[1]);

            //Si el checkPoint es anterior al que asocio, lo asocio
            if (sdl.CurrentCheckPointIndex <= currentIndex)
                sdl.CurrentCheckPointIndex = currentIndex;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nameScene_nameNPC_StateIndex">Ejemplo: MercadoBlind v7,Basilio,2</param>
        public void SetNPCState(string nameScene_nameNPC_StateIndex)
        {
            SaveDataLevel sdl = DataManager.Instance.GetCheckPoint(nameScene_nameNPC_StateIndex.Split(',')[0]);
            string nameNPC = nameScene_nameNPC_StateIndex.Split(',')[1];
            int stateNPC = int.Parse(nameScene_nameNPC_StateIndex.Split(',')[2]);

            NPCInfo npci = sdl.CurrentPlayerNPCs.Single(x => x.NameNPC == nameNPC);
            npci.StateNPC = stateNPC;

            //Si el estado es anterior al que asocio, lo asocio
            if (npci.StateNPC <= stateNPC)
                npci.StateNPC = stateNPC;
        }

        #endregion
    }
}