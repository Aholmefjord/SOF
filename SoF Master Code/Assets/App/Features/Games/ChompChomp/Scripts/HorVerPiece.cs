using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("JMF/Pieces/HorVerPiece")]
public class HorVerPiece : PieceDefinition {

	// power call for HorVerPower (Hor)
	public override bool performPowerHor(int[] arrayRef){
		doHorizontalPower (arrayRef);
		return false; // default is nothing..
	}
	// power call for HorVerPower (Hor)
	public override bool performPowerVer(int[] arrayRef){
		doVerticalPower (arrayRef);
		return false; // default is nothing..
	}
	public override bool performPower(int[] arrayRef){
		Debug.LogError("PLAYSFX LINE CLEAR");
		AudioSystem.PlaySFX("buttons/JULES_CC_LINE_BURST_01");
		//doHorizontalPower(arrayRef); // match 4 line type power Horizontal line

		//////link when this piece is the one being dragged
		int slotNum =gm.iBoard(arrayRef).piece.slotNum;
		int iterTime = 0;
		// step 1 : check up/down (universal to both square & hex board
		foreach(Board _board in gm.iBoard(arrayRef).getAllBoardInDirection(BoardDirection.Top)){
			if ( _board.canBeMatched(_board.subPanelBottom) && _board.piece.slotNum == slotNum) {
				iterTime++;
			} else {
				break; // exit loop as no more match this side...
			}
		}
		foreach (Board _board in gm.iBoard(arrayRef).getAllBoardInDirection(BoardDirection.Bottom)) {
			if (_board.canBeMatched (_board.subPanelTop) && _board.piece.slotNum == slotNum) {
				iterTime++;
			} else {
				break; // exit loop as no more match this side...
			}
		}

		if (iterTime > 1) {
			doVerticalPower (arrayRef);
			return false;
		}
		iterTime = 0;
		foreach (Board _board in gm.iBoard(arrayRef).getAllBoardInDirection(BoardDirection.Right)) {
			if (_board.canBeMatched (_board.subPanelLeft) && _board.piece.slotNum == slotNum) {
					iterTime++;
			} else {
				break; // exit loop as no more match this side...
			}
		}
		foreach (Board _board in gm.iBoard(arrayRef).getAllBoardInDirection(BoardDirection.Left)) {
			if (_board.canBeMatched (_board.subPanelRight) && _board.piece.slotNum == slotNum) {
				iterTime++;
			} else {
				break; // exit loop as no more match this side...
			}
		}
		if (iterTime > 1) {
			doHorizontalPower (arrayRef);
			return false;
		}
		////////////////////////////
		/// 
		/// link when this piece is not the one getting dragged
		if (horLinked) {
			doHorizontalPower (arrayRef); // match 4 line type power Horizontal line
			return false;
		} else {
			doVerticalPower (arrayRef); // match 4 line type power Horizontal line
			return false;
		}

		return false;
	}
	public override bool powerMatched (int[] posA, int[] posB, bool execute, GamePiece thisGp, GamePiece otherGp){
		/*
		if(otherGp.pd is VerticalPiece || otherGp.pd is HorizontalPiece){
			if(execute) StartCoroutine( doPowerMergeHV(posA,posB,thisGp.master,otherGp.master) ); // do a power merge power
			return true; // <--- has power merge
		}*/
		return false; // <--- no power merge
	}
	
	public override bool matchConditions (int xPos, int yPos, List<Board> linkedCubesX, List<Board> linkedCubesY, List<Board> linkedCubesTRBL, List<Board> linkedCubesTLBR){
		if (linkedCubesY.Count > 2) { // 4 match in a Column
			gm.board[xPos,yPos].convertToSpecial(this); // makes the cube a special piece
			gm.board[xPos,yPos].panelHit();

			//lock the piece for just created power piece
			gm.lockJustCreated(xPos,yPos,0.3f);
			if (gm.tutorialLockDropPiece) {
				gm.boardBlurOut(gm.board[xPos,yPos]);
			}
			return true;
		}
		if (linkedCubesX.Count > 2) { // 4 match in a row
			gm.board[xPos,yPos].convertToSpecial(this); // makes the cube a special piece
			gm.board[xPos,yPos].panelHit();
			
			//lock the piece for just created power piece
			gm.lockJustCreated(xPos,yPos,0.3f);
			if (gm.tutorialLockDropPiece) {
				gm.boardBlurOut(gm.board[xPos,yPos]);
			}
			return true;
		}

		return false;
	}
	
	//
	// POWER DEFINITION
	//
	
	void doHorizontalPower(int[] arrayRef){
		
		float delay = 0f;
		float delayIncreament = 0.1f; // the delay of each piece being destroyed.

		//gm.animScript.doAnim(animType.ARROWH,arrayRef[0],arrayRef[1]); // perform anim
		//gm.audioScript.arrowSoundFx.play(); // play arrow sound fx
			
		// the left of this board...
		foreach(Board _board in gm.iBoard(arrayRef).getAllBoardInDirection(BoardDirection.Left) ){
			if(_board.pd is HorVerPiece){
				_board.pd.performPowerVer(_board.arrayRef);
				//delay += delayIncreament;
			}
			else{
				gm.destroyInTime(_board.arrayRef,delay,scorePerCube);
				//delay += delayIncreament;
			}
		}
		delay = 0f; // reset the delay
		// the right of this board...
		foreach(Board _board in gm.iBoard(arrayRef).getAllBoardInDirection(BoardDirection.Right) ){
			if(_board.pd is HorVerPiece){
				_board.pd.performPowerVer(_board.arrayRef);
				//delay += delayIncreament;
			}
			else{
				gm.destroyInTime(_board.arrayRef,delay,scorePerCube);
				//delay += delayIncreament;
			}
		}
		gm.destroyInTime (arrayRef, delay, scorePerCube);
	}

	void doVerticalPower(int[] arrayRef){
		
		float delay = 0f;
		float delayIncreament = 0.1f; // the delay of each piece being destroyed.
		//gm.animScript.doAnim(animType.ARROWV,arrayRef[0],arrayRef[1]); // perform anim
		//gm.audioScript.arrowSoundFx.play(); // play arrow sound fx
		
		// the top of this board...
		foreach(Board _board in gm.iBoard(arrayRef).getAllBoardInDirection(BoardDirection.Top) ){
			if(_board.pd is HorVerPiece){
				_board.pd.performPowerHor(_board.arrayRef);
				//delay += delayIncreament;
			}
			else{
				gm.destroyInTime(_board.arrayRef,delay,scorePerCube);
				//delay += delayIncreament;
			}
		}
		delay = 0f; // reset the delay
		// the bottom of this board...
		foreach(Board _board in gm.iBoard(arrayRef).getAllBoardInDirection(BoardDirection.Bottom) ){
			if(_board.pd is HorVerPiece){
				_board.pd.performPowerHor(_board.arrayRef);
				//delay += delayIncreament;
			}
			else{
				gm.destroyInTime(_board.arrayRef,delay,scorePerCube);
				//delay += delayIncreament;
			}
		}
		gm.destroyInTime (arrayRef, delay, scorePerCube);
	}
	
	// power merge ability code
	IEnumerator doPowerMergeHV(int[] posA, int[] posB, Board thisBd, Board otherBd){
		gm.mergePieces(posA,posB,false); // merge effect
		yield return new WaitForSeconds(gm.gemSwitchSpeed); // wait for merge effect
		
		gm.destroyInTimeMarked(thisBd,0.1f,scorePerCube);
		gm.destroyInTimeMarked(otherBd,0.1f,scorePerCube);
		
		float delay = 0f; // the delay variable we are using...
		float delayIncreament = 0.1f; // the delay of each piece being destroyed.
		gm.audioScript.arrowSoundFx.play(); // play arrow sound fx
		
		
		doHorizontalPower(posB); // perform basic power...
		
		gm.animScript.doAnim(animType.ARROWV,posB); // visuals for the v-power
		// then perform more power on top of basic power
		// the top of this board...
		foreach(Board _board in gm.iBoard(posB).getAllBoardInDirection(BoardDirection.Top) ){
			gm.destroyInTime(_board.arrayRef,delay,scorePerCube);
			delay += delayIncreament;
		}
		delay = 0f; // reset the delay
		// the bottom of this board...
		foreach(Board _board in gm.iBoard(posB).getAllBoardInDirection(BoardDirection.Bottom) ){
			gm.destroyInTime(_board.arrayRef,delay,scorePerCube);
			delay += delayIncreament;
		}
	}
}
