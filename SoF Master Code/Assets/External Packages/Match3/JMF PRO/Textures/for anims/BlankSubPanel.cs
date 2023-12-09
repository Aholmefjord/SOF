using UnityEngine;
using System.Collections;

public class BlankSubPanel : SubPanelDefinition {

	public override bool allowsGravity (SubBoardPanel bp)
	{
		return true;
	}

	public override bool isMatchable (SubBoardPanel bp)
	{
		return true;
	}

	public override bool isSwitchable (SubBoardPanel bp)
	{
		return true;
	}

	public override bool isSolid (SubBoardPanel bp)
	{
		return false;
	}

	public override void playAudioVisuals (SubBoardPanel bp)
	{
		// nothing
	}
}
