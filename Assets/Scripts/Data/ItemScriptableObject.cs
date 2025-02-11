using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace NeonBlood
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Item", order = 1)]
    public class ItemScriptableObject : ScriptableObject
    {
        public string ItemName;
        public int ItemAmount;
        public ITEMS ItemType;
        public float ItemPriceBuy;
        public float ItemPriceSell;
        public List<ITEMS_STATS> StatsToModify;
        public List<float> StatsMultipliers;
    }
}