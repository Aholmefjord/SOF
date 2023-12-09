using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FvTGame
{ 
    public class UnitData
    {
        public enum UnitAttackType
        {
            SCISSOR = 3,
            PAPER = 2,
            STONE = 1
        }

        public string Name;
        public List<UnitAttackType> UnitAttackTypeList;

        public bool IsFriendly;

        public IntVector2 GridLocation;

        public int Power { get { return UnitAttackTypeList.Count;  } }

        public void LoadDefaultData(string name)
        {
            //Load data from game data
            Name = name;

            int total = 1;

            UnitAttackTypeList = new List<UnitAttackType>();
            string type = GameLogic.GameData.GetString(name + "_unit_type_" + total);
            
            while (!type.Equals(""))
            { 
                if (type.ToLower().Equals("scissor"))
                {
                    UnitAttackTypeList.Add(UnitAttackType.SCISSOR);
                }
                else if (type.ToLower().Equals("paper"))
                {
                    UnitAttackTypeList.Add(UnitAttackType.PAPER);
                }
                else if (type.ToLower().Equals("stone"))
                {
                    UnitAttackTypeList.Add(UnitAttackType.STONE);
                }

                total++;
                type = GameLogic.GameData.GetString(name + "_unit_type_" + total);
            }
        }
        /*
        public static Sprite GenerateAttackTypeIcon(UnitAttackType _type)
        {
            string fileName = "";

            switch (_type)
            {
                case UnitData.UnitAttackType.PAPER:
                    fileName = "paper";
                    break;
                case UnitData.UnitAttackType.SCISSOR:
                    fileName = "scissor";
                    break;
                case UnitData.UnitAttackType.STONE:
                    fileName = "stone";
                    break;
            }

            return Resources.Load<Sprite>("Games/FvT/UI/" + fileName);
        }
        //*/
    }
}