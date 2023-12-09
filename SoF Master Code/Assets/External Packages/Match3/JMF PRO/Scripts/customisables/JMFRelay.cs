using UnityEngine;
using System.Collections;

/// <summary>
/// JMF Relay static class. 
/// WARNING~! Do not call JMFRelay.onXXX(); explicitly... it is not meant to be called!
/// *** already called by fixed coding positions in GameManager. ***
/// </summary>


public static class JMFRelay {

	static MatchGameManager gm {get{return JMFUtils.gm;}} // getter methods for gameManager reference
	static WinningConditions wc {get{return JMFUtils.wc;}} // getter methods for WinningConditions reference
	public delegate void onEventDelegate();
	public static onEventDelegate dlgOnGameStart;
	public static onEventDelegate dlgOnPlayerMove;
	public static onEventDelegate dlgOnBoardStabilize;
	public static onEventDelegate dlgOnCombo;
	public static onEventDelegate dlgOnComboEnd;
	public static onEventDelegate dlgOnNoMoreMoves;
	public static onEventDelegate dlgOnBoardReset;

	public delegate int onEventDelegateScore(int num,int x, int y);
	public static onEventDelegateScore dlgOnScoreIssue;

	public delegate void onEventDelegateClick(int x, int y);
	public static onEventDelegateClick dlgOnPieceClick;
	public static onEventDelegateClick dlgOnPanelClick;

	public static void onPreGameStart (){ // called before GameManager does anything... NOTHING IS SET UP YET
		dlgOnGameStart = null;
		dlgOnPlayerMove = null;
		dlgOnBoardStabilize = null;
		dlgOnCombo = null;
		dlgOnComboEnd = null;
		dlgOnNoMoreMoves = null;
		dlgOnBoardReset = null;
		dlgOnPieceClick = null;
		dlgOnPanelClick = null;
		dlgOnScoreIssue = null;

		// -----------------------------------
		// your own stuff here...
		//
		// WARNING : board HAS NOT been set up yet... you can do final board modifications here... 
		// stuff like abilities that modify the current GameManager set up before board inits
		// e.g., board size, board width/height etc...
		// -----------------------------------
	}

	// when the board has been finalized, and are being initiated ( GUI output of pieces and panels )
	public static void onGameStart (){

		foreach(Board _board in gm.board){ // call the gameStart for the board objects
			_board.onGameStart();
		}
		foreach(SubBoardPanel sbp in gm.subPanelH){ // call the gameStart for the subPanelH objects
			if(sbp.spnd != null) sbp.spnd.onGameStart(sbp);
		}
		foreach(SubBoardPanel sbp in gm.subPanelV){ // call the gameStart for the subPanelV objects
			if(sbp.spnd != null) sbp.spnd.onGameStart(sbp);
		}

		// -----------------------------------
		// your own stuff here...
		// -----------------------------------

		if(dlgOnGameStart != null) dlgOnGameStart.Invoke();
	}

	public static void onPlayerMove() { // called when player makes a move

		// custom piece / panels onPlayerMove function call
		foreach(Board _board in gm.board){
			if(_board.isFilled){
				_board.piece.pd.onPlayerMove(_board);
			}
			_board.panel.pnd.onPlayerMove(_board.panel);
		}
		foreach(SubBoardPanel sbp in gm.subPanelH){
			if(sbp.spnd != null) sbp.spnd.onPlayerMove(sbp);
		}
		foreach(SubBoardPanel sbp in gm.subPanelV){
			if(sbp.spnd != null) sbp.spnd.onPlayerMove(sbp);
		}

		// -----------------------------------
		// your own stuff here...
		// -----------------------------------

		if(dlgOnPlayerMove != null) dlgOnPlayerMove.Invoke();
	}

	// called when all pieces stop moving and suggestion is being calculated
	public static void onBoardStabilize (){
        //onComboEnd(); // end the combo when board stabilizes

        //reset combo time counter
        gm.comboCountReset();

        if (MatchGameManager.canChange)
			MatchGameManager.shouldChange = true;
		// custom piece / panels onBoardStabilize function call
		foreach(Board _board in gm.board){
			if(_board.isFilled){
				_board.piece.pd.onBoardStabilize(_board);
			}
			_board.panel.pnd.onBoardStabilize(_board.panel);
		}
		foreach(SubBoardPanel sbp in gm.subPanelH){
			if(sbp.spnd != null) sbp.spnd.onBoardStabilize(sbp);
		}
		foreach(SubBoardPanel sbp in gm.subPanelV){
			if(sbp.spnd != null) sbp.spnd.onBoardStabilize(sbp);
		}

		// -----------------------------------
		// your own stuff here...
		// -----------------------------------

		if(dlgOnBoardStabilize != null) dlgOnBoardStabilize.Invoke();
	}

	public static void onCombo(){ // called directly after combo+1, but before GUI output 
		// -----------------------------------
		// your own stuff here...
		// -----------------------------------

		if(dlgOnCombo != null) dlgOnCombo.Invoke();
	}
	public static void onComboEnd(){
		if(gm.currentCombo > 25){
			gm.audioScript.comboHighFx.play(); // play sound for hi combo
		} else if(gm.currentCombo > 15){
			gm.audioScript.comboMidFx.play(); // play sound for mid combo
		} else if(gm.currentCombo > 5){
			gm.audioScript.comboLowFx.play(); // play sound for low combo
		}
		gm.currentCombo = 0; // reset combo counter...

		// -----------------------------------
		// your own stuff here...
		// -----------------------------------

		if(dlgOnComboEnd != null) dlgOnComboEnd.Invoke();
	}

	public static void onNoMoreMoves(){ // called before board reset happens
		// -----------------------------------
		// your own stuff here...
		// -----------------------------------

		if(dlgOnNoMoreMoves != null) dlgOnNoMoreMoves.Invoke();
	}

	public static void onBoardReset(){ // called after board reset happens
		// -----------------------------------
		// your own stuff here...
		// -----------------------------------

		if(dlgOnBoardReset != null) dlgOnBoardReset.Invoke();
	}

	public static void onPieceClick(int x, int y){
		gm.board[x,y].pd.onPieceClicked(gm.board[x,y].piece); // call PieceDefinition's onPieceClick
		
		// the panel click is here because the panel will be next to be clicked after the piece
		onPanelClick(x,y); // call PanelDefinition's onPanelClick

		// -----------------------------------
		// your own stuff here...
		// x / y is the board position of which the piece located was clicked.
		// e.g., JMFUtils.gm.board[x,y] ....
		// -----------------------------------

		if(dlgOnPieceClick != null) dlgOnPieceClick.Invoke(x,y);
	}

	public static void onPanelClick(int x, int y){
		gm.board[x,y].panel.pnd.onPanelClicked(gm.board[x,y].panel); // call PanelDefinition's onPanelClick
		// -----------------------------------
		// your own stuff here...
		// x / y is the board position of which the piece located was clicked.
		// e.g., JMFUtils.gm.board[x,y] ....
		// -----------------------------------

		if(dlgOnPanelClick != null) dlgOnPanelClick.Invoke(x,y);
	}

	// the "RAW" score given for destroyed pieces / matches of an individual box
	// the score HAS NOT been multiplied by combo bonus yet~!
	public static int onScoreIssue(int scoreGain,int x, int y){
		int modifiedGains = scoreGain;
		// -----------------------------------
		// your own stuff here...
		// -----------------------------------
		// modifiedGains = something else?? ;

		if(dlgOnScoreIssue != null) modifiedGains = dlgOnScoreIssue.Invoke(modifiedGains,x,y);
		return modifiedGains;
	}
}
