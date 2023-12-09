using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FvTGame
{ 
    public class SelectedUnit : MonoBehaviour {

        UnitData mData;

        public void Load(string _unitName, Sprite _sprite, AttackTypeIconGetter _getAttackTypeIcon)
        {
            mData = new UnitData();
            mData.LoadDefaultData(_unitName);

            //Update sprite
            gameObject.FindChild("UnitImage").GetComponent<Image>().sprite = _sprite;// Resources.Load<Sprite>("Games/FvT/Units/" + _unitName);

            //Load icon
            List<UnitData.UnitAttackType> list = mData.UnitAttackTypeList;

            for (int i = 0; i < list.Count; ++i)
            {
                UnitData.UnitAttackType type = list[i];

                //NOTE, there is only 3 game object, because we assume the game balance at most reach 3 unit types.
                gameObject.FindChild("Unit Type " + (i + 1)).SetActive(true);
                gameObject.FindChild("Unit Type " + (i + 1)).GetComponent<Image>().sprite = _getAttackTypeIcon(type);
            }
        }
    }
}