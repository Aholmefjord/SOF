using UnityEngine;
using DG.Tweening;

public class ProgressBar : MonoBehaviour
{
    float CurrentValue = 1.0f;

    //Function for calling from button press since it only allows 1 variable
    public void CallFromButton(float percent)
    {
        SetValue(percent, 1.0f);
    }

    public void SetValue(float percent, float animationduration)
    {
        //Clamp
        if (percent < 0f) percent = 0f;
        else if (percent > 1f) percent = 1f;

        //Animate progress bar
        transform.GetChild(0).DOScaleY(percent, animationduration);

        //Animate number
        /*float temppercent = CurrentValue;
        DOTween.To(()=> temppercent, (x)=>temppercent=x, percent, animationduration).OnUpdate (() => {
            transform.GetChild(1).GetComponent<UnityEngine.UI.Text>().text = Mathf.RoundToInt(temppercent * 100f).ToString()+"%";
        });*/

        //Set value
        CurrentValue = percent;
    }

    public float GetValue()
    {
        return CurrentValue;
    }
}
