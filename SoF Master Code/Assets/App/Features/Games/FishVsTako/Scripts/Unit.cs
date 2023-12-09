using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FvTGame
{
    public class Unit : MonoBehaviour {

        UnitData mData;
        public UnitData GetData() { return mData; }
        
        public void Load(string _unitName, Sprite _sprite, AttackTypeIconGetter _getAttackTypeIcon)
        {
            mData = new UnitData();
            mData.LoadDefaultData(_unitName);

            //Update sprite
            transform.GetComponent<SpriteRenderer>().sprite = _sprite;// Resources.Load<Sprite>("Games/FvT/Units/" + _unitName);

            //Load icon
            List<UnitData.UnitAttackType> list = mData.UnitAttackTypeList;

            for (int i = 0; i < list.Count; ++i)
            {
                UnitData.UnitAttackType type = list[i];

                //NOTE, there is only 3 game object, because we assume the game balance at most reach 3 unit types.
                gameObject.FindChild("Unit Type " + (i + 1)).SetActive(true);
                gameObject.FindChild("Unit Type " + (i + 1)).GetComponent<SpriteRenderer>().sprite = _getAttackTypeIcon(type);
            }

            mData.IsFriendly = FindUnitFriendlyByName();

            //Set Location of Icons according to unit

            for (int i = 1; i <= 3; ++i)
            {
                Vector3 pos = gameObject.FindChild("Unit Type " + i).GetComponent<Transform>().localPosition;

                pos.x = mData.IsFriendly ? -0.8f : 0.8f;

                gameObject.FindChild("Unit Type " + i).GetComponent<Transform>().localPosition = pos;
            }
        }

        private bool FindUnitFriendlyByName()
        {
            bool result = false;

            for (int i = 1; i <= GameLogic.GameData.GetInt("player_unit_total"); ++i)
            {
                if (GameLogic.GameData.GetString("player_unit_" + i).ToLower().Equals(mData.Name.ToLower()))
                {
                    result = true;
                    break;
                }
            }

            return result;
        }

        public void SetGridLocation(int x, int y)
        {
            mData.GridLocation = new IntVector2(x, y);
        }

        public IntVector2 GetGridLocation()
        {
            return mData.GridLocation;
        }

        public bool IsFriendly()
        {
            return mData.IsFriendly;
        }

        public void SetLayerOrder(int _order)
        {
            gameObject.GetComponent<SpriteRenderer>().sortingOrder = _order;
            gameObject.FindChild("highlight").GetComponent<SpriteRenderer>().sortingOrder = _order - 1;
            gameObject.FindChild("Shadow").GetComponent<SpriteRenderer>().sortingOrder = _order - 2;
            gameObject.FindChild("Unit Type 3").GetComponent<SpriteRenderer>().sortingOrder = _order + 1;
            gameObject.FindChild("Unit Type 2").GetComponent<SpriteRenderer>().sortingOrder = _order + 2;
            gameObject.FindChild("Unit Type 1").GetComponent<SpriteRenderer>().sortingOrder = _order + 3;
        }
    }
}