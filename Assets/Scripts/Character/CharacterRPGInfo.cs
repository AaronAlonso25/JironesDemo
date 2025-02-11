using UnityEngine;
using System.Collections.Generic;

namespace NeonBlood
{
    public enum ITEMS { CONSUMABLES, EQUIPMENT, KEYS, VARIOUS };
    public enum ITEMS_STATS { HEALTH, INITIATIVE, EVASION, ARMOR };

    [System.Serializable]
    public class Dice
    {
        public Vector2Int Value;
        public List<double> Probabilities;

        private System.Random random;

        public Dice(int x, int y)
        {
            this.Value.x = x;
            this.Value.y = y;
            this.random = new System.Random();

            this.SetProbabilities();
        }

        public Dice(Vector2Int value)
        {
            this.Value = value;
            this.random = new System.Random();

            this.SetProbabilities();
        }

        public int DiceRoll()
        {
            double roll = this.random.NextDouble();

            for (int i = 0; i < this.Probabilities.Count; i++)
            {
                if (roll < this.Probabilities[i])
                {
                    return i + 1; // Regresa la cara del dado (1-indexed)
                }
            }

            return this.Probabilities.Count; // En caso de falla, retorna la última cara
        }

        private void SetProbabilities()
        {
            double min = 10;
            double max = 10;
            double mid = (100 - min - max) / (this.Value.y - 2);

            List<double> weights = new List<double>();
            weights.Add(min);
            for (int i = 1; i < this.Value.y - 1; i++)
                weights.Add(mid);
            weights.Add(max);

            double total = 0;
            this.Probabilities = new List<double>();

            foreach (double weight in weights)
            {
                total += weight;
                this.Probabilities.Add(total);
            }

            // Normalizar para que la suma sea 1
            for (int i = 0; i < this.Probabilities.Count; i++)
                this.Probabilities[i] /= total;
        }

        public override string ToString()
        {
            return this.Value.x + "d" + this.Value.y;
        }
    }

    [System.Serializable]
    public class CharacterRPGInfo
    {
        public string CharacterID;
        public string CharacterName;
        public string CharacterPrefab;
        public CharacterRPGStats CharacterStats;
        public CharacterRPGStates CharacterStates;
        public WeaponRPGInfo CharacterWeapon;
        public List<WeaponRPGInfo> CharacterSpecialWeapons;
        public List<AbilityRPGInfo> CharacterAbilities;

        //Combat
        public GameObject CharacterCombatObject { get; set; }
        public GameObject CharacterSelectorObject { get; set; }
        public GameObject CharacterStatsObject { get; set; }
        public GameObject CharacterHealthBarObject { get; set; }
        public GameObject CharacterPriorityObject { get; set; }
        public Sprite CharacterPrioritySpriteOff { get; set; }
        public Sprite CharacterPrioritySpriteOn { get; set; }

        //Default Stats
        public CharacterRPGStats CharacterDefaultStats { get; set; }

        public CharacterRPGInfo(
            string id, 
            string name,
            string prefab,
            CharacterRPGStats stats,
            CharacterRPGStates states,
            WeaponRPGInfo weapon,
            List<WeaponRPGInfo> specialWeapons,
            List<AbilityRPGInfo> abilities)
        {
            this.CharacterID = id;
            this.CharacterName = name;
            this.CharacterPrefab = prefab;
            this.CharacterStats = stats;
            this.CharacterStates = states;
            this.CharacterWeapon = weapon;
            this.CharacterSpecialWeapons = specialWeapons;
            this.CharacterAbilities = abilities;

            this.CharacterDefaultStats = new CharacterRPGStats(
                stats.CharacterHealth,
                stats.CharacterTotalHealth,
                stats.CharacterInitiative,
                stats.CharacterEvasion,
                stats.CharacterArmor
                );
        }

        public CharacterRPGInfo(CharacterRPGInfo ch)
        {
            this.CharacterID = ch.CharacterID;
            this.CharacterName = ch.CharacterName;
            this.CharacterPrefab = ch.CharacterPrefab;

            this.CharacterStats = new CharacterRPGStats(
                ch.CharacterStats.CharacterHealth,
                ch.CharacterStats.CharacterTotalHealth,
                ch.CharacterStats.CharacterInitiative,
                ch.CharacterStats.CharacterEvasion,
                ch.CharacterStats.CharacterArmor
                );

            this.CharacterStates = new CharacterRPGStates(
                ch.CharacterStates.IsDefend,
                ch.CharacterStates.IsBlind
                );

            this.CharacterWeapon = new WeaponRPGInfo(
                    ch.CharacterWeapon.WeaponID,
                    ch.CharacterWeapon.WeaponName,
                    ch.CharacterWeapon.WeaponAttack,
                    ch.CharacterWeapon.WeaponRecharge,
                    ch.CharacterWeapon.WeaponUses
                    );

            this.CharacterSpecialWeapons = new List<WeaponRPGInfo>();
            foreach (WeaponRPGInfo wp in ch.CharacterSpecialWeapons)
            {
                this.CharacterSpecialWeapons.Add(new WeaponRPGInfo(
                    wp.WeaponID,
                    wp.WeaponName,
                    wp.WeaponAttack,
                    wp.WeaponRecharge,
                    wp.WeaponUses)
                    );
            }

            this.CharacterAbilities = new List<AbilityRPGInfo>();
            foreach (AbilityRPGInfo ab in ch.CharacterAbilities)
            {
                this.CharacterAbilities.Add(new AbilityRPGInfo(
                    ab.AbilityID, 
                    ab.AbilityName)
                    );
            }                

            this.CharacterDefaultStats = new CharacterRPGStats(
                ch.CharacterStats.CharacterHealth,
                ch.CharacterStats.CharacterTotalHealth,
                ch.CharacterStats.CharacterInitiative,
                ch.CharacterStats.CharacterEvasion,
                ch.CharacterStats.CharacterArmor
                );
        }

        public void CharacterResetStats()
        {
            this.CharacterStats.CharacterInitiative = this.CharacterDefaultStats.CharacterInitiative;

            this.CharacterStates.IsDefend = false;
            this.CharacterStates.DefendValue = 0;

            this.CharacterStates.IsBlind = false;
        }

        public void ApplyDamage(float damage)
        {
            this.CharacterStats.CharacterHealth = Mathf.Clamp(this.CharacterStats.CharacterHealth - damage, 0, this.CharacterStats.CharacterTotalHealth);
        }
    }

    [System.Serializable]
    public class CharacterRPGStats
    {
        public float CharacterHealth;
        public float CharacterTotalHealth;
        public int CharacterInitiative;
        public Dice CharacterEvasion;
        public int CharacterArmor;

        public CharacterRPGStats(
            float health,
            float totalHealth,
            int iniciative,
            Dice evasion,
            int armor
            )
        {
            this.CharacterHealth = health;
            this.CharacterTotalHealth = totalHealth;
            this.CharacterInitiative = iniciative;
            this.CharacterEvasion = evasion;
            this.CharacterArmor = armor;
        }

        public CharacterRPGStats(CharacterRPGStats stats)
        {
            this.CharacterHealth = stats.CharacterHealth;
            this.CharacterTotalHealth = stats.CharacterTotalHealth;
            this.CharacterInitiative = stats.CharacterInitiative;
            this.CharacterEvasion = stats.CharacterEvasion;
            this.CharacterArmor = stats.CharacterArmor;
        }
    }

    [System.Serializable]
    public class CharacterRPGStates
    {
        public bool IsDefend;
        public int DefendValue;

        public bool IsBlind;

        public CharacterRPGStates(
            bool defend, 
            bool blind
            )
        {
            this.IsDefend = defend;
            this.IsBlind = blind;
        }

        public CharacterRPGStates()
        {
            this.IsDefend = false;
            this.IsBlind = false;
        }
    }

    //[System.Serializable]
    //public class EquipmentRPGInfo
    //{
    //    public string EquipmentName;
    //    public List<STATS> StatsToModify;
    //    public List<float> StatsMultipliers;

    //    public EquipmentRPGInfo(string name, List<STATS> stats, List<float> multipliers)
    //    {
    //        this.EquipmentName = name;
    //        this.StatsToModify = stats;
    //        this.StatsMultipliers = multipliers;
    //    }
    //}

    [System.Serializable]
    public class WeaponRPGInfo
    {
        public string WeaponID;
        public string WeaponName;
        public Dice WeaponAttack;
        public int WeaponRecharge;
        public int WeaponUses;

        public int WeaponActualRecharge;

        public WeaponRPGInfo(string id, string name, Dice dice, int recharge, int uses)
        {
            this.WeaponID = id;
            this.WeaponName = name;
            this.WeaponAttack = dice;
            this.WeaponRecharge = recharge;
            this.WeaponUses = uses;
        }

        public WeaponRPGInfo(WeaponRPGInfo weapon)
        {
            this.WeaponID = weapon.WeaponID;
            this.WeaponName = weapon.WeaponName;
            this.WeaponAttack = weapon.WeaponAttack;
            this.WeaponRecharge = weapon.WeaponRecharge;
            this.WeaponUses = weapon.WeaponUses;
        }
    }

    [System.Serializable]
    public class AbilityRPGInfo
    {
        public string AbilityID;
        public string AbilityName;

        public AbilityRPGInfo(string id, string name)
        {
            this.AbilityID = id;
            this.AbilityName = name;
        }

        public AbilityRPGInfo(AbilityRPGInfo ab)
        {
            this.AbilityID = ab.AbilityID;
            this.AbilityName = ab.AbilityName;
        }
    }

    [System.Serializable]
    public class ItemRPGInfo
    {
        public string ItemName;
        public int ItemAmount;
        public ITEMS ItemType;
        public float ItemPriceBuy;
        public float ItemPriceSell;
        public List<ITEMS_STATS> StatsToModify;
        public List<float> StatsMultipliers;

        public ItemRPGInfo(string name, int amount, ITEMS type, float buyPrice, float sellPrice, List<ITEMS_STATS> stats, List<float> multipliers)
        {
            this.ItemName = name;
            this.ItemAmount = amount;
            this.ItemType = type;
            this.ItemPriceBuy = buyPrice;
            this.ItemPriceSell = sellPrice;
            this.StatsToModify = stats;
            this.StatsMultipliers = multipliers;
        }

        public ItemRPGInfo(ItemRPGInfo item)
        {
            this.ItemName = item.ItemName;
            this.ItemAmount = item.ItemAmount;
            this.ItemType = item.ItemType;
            this.ItemPriceBuy = item.ItemPriceBuy;
            this.ItemPriceSell = item.ItemPriceSell;
            this.StatsToModify = item.StatsToModify;
            this.StatsMultipliers = item.StatsMultipliers;
        }

        public ItemRPGInfo(ItemRPGInfo item, int amount)
        {
            this.ItemName = item.ItemName;
            this.ItemAmount = amount;
            this.ItemType = item.ItemType;
            this.ItemPriceBuy = item.ItemPriceBuy;
            this.ItemPriceSell = item.ItemPriceSell;
            this.StatsToModify = item.StatsToModify;
            this.StatsMultipliers = item.StatsMultipliers;
        }

        public ItemRPGInfo(ItemScriptableObject itemSO)
        {
            this.ItemName = itemSO.ItemName;
            this.ItemAmount = itemSO.ItemAmount;
            this.ItemType = itemSO.ItemType;
            this.ItemPriceBuy = itemSO.ItemPriceBuy;
            this.ItemPriceSell = itemSO.ItemPriceSell;
            this.StatsToModify = itemSO.StatsToModify;
            this.StatsMultipliers = itemSO.StatsMultipliers;
        }
    }
}