﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Reflection.Emit;
using System.Xml.Linq;

namespace TextRPG
{
    public class Player:Unit
    {
        // 플레이어 경험치
        private int[] levelExp = new int[10] { 5, 7, 10, 12, 15, 20, 25, 30, 40, 50 }; // 레벨 별 경험치 통

        [JsonProperty] public List<EquipItem> PlayerEquipItems { get; private set; }
        [JsonProperty] public Dictionary<string, int> PlayerConsumableItems { get; private set; } 
        public EUnitType ePlayerClass { get;  set; }
        public int Gold { get; set; }
        [JsonProperty] public int Exp { get; private set; }
        public float EquipAtkItem { get; set; }
        public float EquipDefItem { get; set; }    
        public EEquipItemType EquipItemFlag { get; set; }
              

        public Player(string name)
        {
            Name = name;
            Level = 1;
            Atk = 10000;
            Def = 5;
            MaxHealth = 10;
            Health = MaxHealth;
            Gold = 10000;
            AvoidChance = 10;
            CriticalChance = 16;
            CriticalDamage = 1.6f;
            MaxMana = 100;
            Mana = MaxMana;
            Phase = 0;
            base.Skills = new List<Skill>();
            base.DeBuffs = new List<DeBuff>();
            PlayerEquipItems = new List<EquipItem>();
            PlayerConsumableItems = new Dictionary<string, int>();
        }


        public void ChangePlayerClass(EUnitType ePlayerClass)
        {
            // 플레이어의 직업에 따라 추가스탯 배정
            this.ePlayerClass = ePlayerClass;

            switch (ePlayerClass)
            {
                case EUnitType.WARRIOR:
                    MaxHealth += 50; /// 수정이 필요함
                    Health += 50;
                    Def += 5;
                    Skills.Add(new Skill(0));                    
                    Skills.Add(new Skill(1));
                    Skills.Add(new CrisisEvasion(2));                    
                    break;
                case EUnitType.ARCHER:
                    CriticalChance += 9;
                    CriticalDamage += 0.9f;
                    Skills.Add(new Skill(3));
                    Skills.Add(new Skill(4));
                    Skills.Add(new WeaknessSniping(5));
                    break;
                case EUnitType.THIEF:
                    AvoidChance += 10;
                    Skills.Add(new Skill(6));
                    Skills.Add(new Skill(7));
                    Skills.Add(new Assassination(8));
                    break;
                case EUnitType.MAGICIAN:
                    MaxMana += 50;
                    Mana += 50;
                    Skills.Add(new Skill(9));
                    Skills.Add(new Skill(10));
                    Skills.Add(new ChainLighting(11));
                    break;
            }
        }

        public string GetPlayerClass(EUnitType ePlayerClass) // 플레이어의 직업 별 이름 반환 
        {
            string playerClass = ePlayerClass switch
            {
                EUnitType.WARRIOR => "전사",
                EUnitType.ARCHER => "궁수",
                EUnitType.THIEF => "도적",
                EUnitType.MAGICIAN => "마법사",
                _ => "직업이 존재하지 않습니다." // default
            };

            return playerClass;
        }

        public void SwitchingEquipItem(EquipItem equipItem)
        {
            if (equipItem.IsEquip)
            {
                EquipAtkItem += equipItem.AtkValue;
                EquipDefItem += equipItem.DefValue;
            }
            else
            {
                EquipAtkItem -= equipItem.AtkValue;
                EquipDefItem -= equipItem.DefValue;
            }
            
        }

        public float GetAtkValue() // 전체 공격력 반환
        {
            if (EquipAtkItem == 0)
            {
                return Atk;
            }
            else
            {
                return Atk + EquipAtkItem;
            }
        }
        public float GetDefValue() // 전체 방어력 반환
        {
            if (EquipDefItem == 0)
            {
                return Def;
            }
            else
            {
                return Def + EquipDefItem;
            }
        }

        public void ExpUp(int exp) // 경험치 상승
        {
            if(Level >= 10) // 예외처리
            {
                return;
            }

            // 4.30 J => 경험치 상승 수정
            Exp += exp;

            if(Exp >= levelExp[Level - 1])
            {
                Exp-= levelExp[Level-1];
                Level++;
                LevelUp();
            }
        }

        private void LevelUp() // 레벨업 
        {
            if(Level >= 10)
            {
                Exp = 0;
            }
            Atk += 0.5f;
            Def += 1;
            UnlockSkills();
        }        
        private void UnlockSkills()
        {
            if (Level >= 7) Phase = 3;
            else if (Level >= 4) Phase = 2;
            else if (Level >= 2) Phase = 1;
        }

        public override string OnDamaged(int damage) // 회피시 0
        {
            int endDamage = Convert.ToInt32(Math.Round(damage - GetDefValue()));
            endDamage = endDamage > 0 ? endDamage : 1;

            Health -= endDamage;
            return $"[데미지 {endDamage}] ";
        }

        public override int GetDamagePerHit() // bool 반환형 = 치명타 여부
        {
            int atkRange = random.Next(0, 21); // 공격력 오차범위
            float damage = GetAtkValue() * (100 + (atkRange - 10)) * 0.01f; // 오차범위 적용한 데미지 

            return Convert.ToInt32(Math.Round(damage));
        }

        public override string Attack(Unit target)
        {
            int avoidRange = random.Next(0, 101); // 회피 범위
            string result;
            string? critStr = IsCriticalHit();
            float critRate = critStr != null ? CriticalDamage : 1f;
            int damage = GetDamagePerHit();

            if (CheckCrowdControl(ECrowdControlType.BLIND) || avoidRange <= target.AvoidChance) // 회피 시 리턴
            {
                return "Miss!! ";
            }

            damage = Convert.ToInt32(Math.Round(damage * critRate));
            result = target.OnDamaged(damage);
            result += critStr;

            return result;
        }

        //  5.3 J => 플레이어 보유 아이템 리스트 리팩토링
        public void AddEquipItem(EquipItem equipItem)
        {
            equipItem.IsSell = true;
            PlayerEquipItems.Add(equipItem);
        }
        public void RemoveEquipItem(EquipItem equipItem)
        {

            PlayerEquipItems.Remove(equipItem);
        }

        // 소비 아이템 추가 및 삭제
        public void AddConsumableItem(string consumableItemName)
        {
            if (PlayerConsumableItems.ContainsKey(consumableItemName))
            {
                PlayerConsumableItems[consumableItemName]++;
            }
            else
            {
                PlayerConsumableItems.Add(consumableItemName, 1);
            }
        }   
        public void RemoveConsumableItem(string consumableItemName)
        {
            if (PlayerConsumableItems.ContainsKey(consumableItemName) && PlayerConsumableItems[consumableItemName] > 1)
            {
                PlayerConsumableItems[consumableItemName]--;
            }
            else
            {
                PlayerConsumableItems.Remove(consumableItemName);
            }
        }
    }
}
