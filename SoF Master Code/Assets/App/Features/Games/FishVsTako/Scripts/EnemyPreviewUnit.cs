using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FvTGame
{ 
    public class EnemyPreviewUnit : MonoBehaviour {
        
        public void Load(UnitData data, Sprite _sprite , AttackTypeIconGetter _getAttackTypeIcon)
        {
            gameObject.FindChild("Fish Image").SetActive(true);

            //Replace Sprite
            gameObject.FindChild("Fish Image").GetComponent<Image>().sprite = _sprite;// Resources.Load<Sprite>("Games/FvT/Units/" + data.Name);
            gameObject.FindChild("Fish Image").GetComponent<Image>().SetNativeSize();

            List<UnitData.UnitAttackType> list = data.UnitAttackTypeList;

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