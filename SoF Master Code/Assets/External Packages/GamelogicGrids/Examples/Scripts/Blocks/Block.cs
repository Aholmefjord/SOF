//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

using UnityEngine;

public class Block : GLMonoBehaviour
{
	public void SetColor(Color color)
	{
		GetComponent<Renderer>().material.color = color;
	}
}
