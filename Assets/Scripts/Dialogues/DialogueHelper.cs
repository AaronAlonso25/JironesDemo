using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Yarn.Unity;

namespace NeonBlood
{
    public class DialogueHelper : MonoBehaviour
    {
        public DialogueRunner DialogueRunner;

        void Awake()
        {
            //Player
            this.DialogueRunner.AddCommandHandler<bool>("stop_player", StopPlayer);
            this.DialogueRunner.AddCommandHandler<bool>("set_player_detective_mode", SetPlayerDetectiveMode);
            //Data
            this.DialogueRunner.AddCommandHandler<string>("set_npc_state", SetNPCState);
            this.DialogueRunner.AddCommandHandler<string>("set_check_point", SetCheckPoint);
            this.DialogueRunner.AddCommandHandler<int>("set_current_chapter", SetCurrentChapter);
            this.DialogueRunner.AddCommandHandler("save_data", SaveData);
            //Combats
            this.DialogueRunner.AddCommandHandler<string>("start_combat", StartCombat);
            //Fade In Out
            this.DialogueRunner.AddCommandHandler<float>("fade", FadeInOutFunction);
            //Items
            this.DialogueRunner.AddCommandHandler<string>("add_item_to_inventory", AddItemToInventory);
            this.DialogueRunner.AddCommandHandler<string>("remove_item_from_inventory", RemoveItemFromInventory);
            this.DialogueRunner.AddCommandHandler<string>("check_item_from_inventory", CheckItemFromInventory);
            //Levels
            this.DialogueRunner.AddCommandHandler<string>("play_level", PlayLevel);
            //Trophies
            this.DialogueRunner.AddCommandHandler<int>("activate_trophy", ActivateTrophy);
            //Vibration
            this.DialogueRunner.AddCommandHandler("activate_vibration", ActivateVibration);
        }

        //Player
        public void StopPlayer(bool stop)
        {
            CharacterController.Instance.StopPlayer(stop);
        }

        public void SetPlayerDetectiveMode(bool active)
        {
            CharacterController.Instance.CanDetective = active;
        }

        //Data
        public void SetNPCState(string state)
        {
            LevelManager.Instance.SetNPCState(state);
        }

        public void SetCheckPoint(string checkPoint)
        {
            LevelManager.Instance.SetCheckPoint(checkPoint);
        }

        public void SetCurrentChapter(int chapterIndex)
        {
            DataManager.Instance.CurrentChapter = chapterIndex;
        }

        public void SaveData()
        {
            DataManager.Instance.SaveData();
        }

        //Combats
        public void StartCombat(string combatName)
        {
            LevelManager.Instance.StartCombat(combatName);
        }

        //Fade In Out
        public void FadeInOutFunction(float timeDelay)
        {
            MenuCanvas.Instance.FadeInOutFunction(timeDelay);
        }

        //Items
        public void AddItemToInventory(string itemName)
        {
            if(DataManager.Instance.CharactersItemsTemplates.Any(x => x.ItemName == itemName))
            {
                ItemRPGInfo item = new ItemRPGInfo(DataManager.Instance.CharactersItemsTemplates.Single(x => x.ItemName == itemName));
                DataManager.Instance.CharactersItems.Add(item);
            }           

            //@TODO: Mirar si hace falta comprobar si el objeto ya existe en el inventario para aumentar la cantidad
        }

        public void RemoveItemFromInventory(string itemName)
        {
            if (DataManager.Instance.CharactersItems.Any(x => x.ItemName == itemName))
            {
                ItemRPGInfo item = DataManager.Instance.CharactersItems.Single(x => x.ItemName == itemName);
                DataManager.Instance.CharactersItems.Remove(item);

                //@TODO: Mirar si hace falta comprobar la cantidad del objeto en el inventario para reducir su cantidad
            }
            else
                Debug.LogWarning("No existe el objeto en el inventario del jugador");
        }

        public void CheckItemFromInventory(string itemName)
        {
            if (DataManager.Instance.CharactersItems.Any(x => x.ItemName == itemName))
                Debug.Log("Object " + itemName + " Found");
            else
                Debug.Log("Object " + itemName + " Not Found");
        }

        //Levels
        public void PlayLevel(string nameLevel)
        {
#if UNITY_PS4
            MenuCanvas.Instance.PlayLevel(nameLevel);
#else
            MenuCanvas.Instance.PlayLevelDirectly(nameLevel);
#endif
        }

        //Trophies
        public void ActivateTrophy(int trophyID)
        {
            GameObject te = new GameObject("TrophyEvent_" + trophyID);
            te.AddComponent<TrophyEvent>().ActivateTrophy(trophyID);
        }

        //Vibration
        public void ActivateVibration()
        {
#if UNITY_PS4
            ControllerUtilitiesManager.Instance.SetControllerVibration(50, 50, 3);
#endif

#if UNITY_PS5
            ControllerUtilitiesManager.Instance.SetControllerVibration(50, 50, 3);
#endif

#if UNITY_SWITCH
            SwitchControllerVibrationManager.Instance.MakeVibration(3, 0.15f, 0);
#endif
        }
    }
}