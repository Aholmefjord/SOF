using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using AutumnInteractive.SimpleJSON;

public class TangramPieceConstruction : MonoBehaviour {

    private UIManager uiManager;
    public bool[,] grid = new bool[10, 10];

    private void Awake () {
        uiManager = new UIManager (gameObject);
    }

    public string pieceName {
        get {
            return (large ? string.Empty : "s") + pieceNumber;
        }
    }

    public bool large = false;
    public int pieceNumber;

    private GameObject piece;

    private void OnEnable () {
        var jsonText = PlayerPrefs.GetString ("tangram_pieces", "{}");
        json = JSON.Parse<JSONClass> (jsonText);
    }

    private void Update () {

        if (piece == null || piece.name != pieceName) {
            Destroy (piece);
            piece = TangramGame.SpawnTangramPiece (pieceName);
            if (piece != null) {
                piece.transform.SetParent (uiManager["Inventory Pieces"].transform);
                piece.transform.position = uiManager["Inventory Grid"].transform.GetChild (90).transform.position;
                grid = new bool[10, 10];
                foreach (JSONArray piecePos in json.GetJson (pieceName, new JSONArray ())) {
                    grid[piecePos[0].AsInt, 9 - piecePos[1].AsInt] = true;
                }
            }
        }


        if (Input.GetMouseButtonUp (0)) {
            var ped = new PointerEventData (EventSystem.current);
            ped.position = Input.mousePosition;
            var raycastResults = new List<RaycastResult> ();
            EventSystem.current.RaycastAll (ped, raycastResults);

            foreach (var result in raycastResults) {
                if (result.gameObject.transform.IsChildOf (uiManager["Inventory Grid"].transform)) {
                    var index = result.gameObject.transform.GetSiblingIndex ();
                    var i = index % 10;
                    var j = index / 10;

                    grid[i, j] = !grid[i, j];
                    break;
                }
            }
        }

        for (var j = 0; j < 10; ++j) {
            for (var i = 0; i < 10; ++i) {
                var index = j * 10 + i;
                var cellGo = uiManager["Inventory Grid"].transform.GetChild (index).gameObject;
                cellGo.SetColor (grid[i, j] ? Color.blue : Color.white);
            }
        }

        if (Input.GetKeyUp (KeyCode.RightArrow)) {
            if (Input.GetKey (KeyCode.LeftControl) || Input.GetKey (KeyCode.RightControl)) {
                pieceNumber += 10;
            } else {
                pieceNumber++;
            }
        }
        if (Input.GetKeyUp (KeyCode.LeftArrow)) {
            if (Input.GetKey (KeyCode.LeftControl) || Input.GetKey (KeyCode.RightControl)) {
                pieceNumber -= 10;
            } else {
                pieceNumber--;
            }
        }
        if (Input.GetKeyUp (KeyCode.S)) {
            large = !large;
        }
        pieceNumber = Mathf.Clamp (pieceNumber, 0, 50);

        if (piece != null) {
            if (Input.GetKeyUp (KeyCode.Space)) {
                var pieceJson = new JSONArray ();
                for (var i = 0; i < 10; ++i) {
                    for (var j = 0; j < 10; ++j) {
                        if (grid[i, j]) {
                            var posJson = new JSONArray ();
                            posJson.Add (new JSONData (i));
                            posJson.Add (new JSONData (9 - j));
                            pieceJson.Add (posJson);
                        }
                    }
                }
                json.Add (pieceName, pieceJson);
                Debug.Log (json.ToString ());
            }
        }
    }

    private void OnDisable () {
        PlayerPrefs.SetString ("tangram_pieces", json.ToString ());
        Debug.Log (json.ToString ());
    }

    private JSONClass json;

}
