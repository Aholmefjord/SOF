using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ResourceController : MonoBehaviour {
	public static ResourceController current;
	public Text CurrencyText;
    public Text JulesText;
	void Awake()
	{
		current = this;
		
	}

	void Update()
	{
		UpdateCurrencyDisplay();
	}

	public void UpdateCurrencyDisplay(int newAmount)
	{
        CurrencyText.text = string.Format("{0:n0}", GameState.me.inventory.Coins);
        JulesText.text = string.Format("{0:n0}", GameState.me.inventory.Jewels);
    }

	public void UpdateCurrencyDisplay()
	{
		if (GameState.me != null)
		{
			CurrencyText.text = string.Format("{0:n0}", GameState.me.inventory.Coins);
            JulesText.text = string.Format("{0:n0}", GameState.me.inventory.Jewels);
        }
	}

}
