using UnityEngine;
using AutumnInteractive.SimpleJSON;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// Tom: seems to be another level editor that works runtime; not converting to work with assetbundle
/// </summary>
public class TangramPuzzleBuilder : MonoBehaviour {

    public int timer;
    public int difficulty;

    public bool large;
    public int pieceNumber;

    private UIManager uiManager;

    public string pieceName {
        get {
            return (large ? string.Empty : "s") + pieceNumber;
        }
    }

    private void Awake () {
        uiManager = new UIManager (gameObject);
    }

    private void OnEnable () {
        var jsonText = PlayerPrefs.GetString ("tangram_levels", "{}");
        json = JSON.Parse<JSONClass> (jsonText);

        StartCoroutine (LoadSuggestionLevel ());
    }


    private bool loaded = false;
    private float loadProgression = 0;
    private IEnumerator LoadSuggestionLevel () {
        // TOM: seems to be deprecated; 28th Aug 2017
        var suggestions = CachedResources.Load<TextAsset> ("json/tangram/puzzle_suggestions").text;
        var suggestionArray = suggestions.Split ('\n');
        int i = 0;
        foreach (var s in suggestionArray) {
            puzzleSuggestions.Add (JSON.Parse<JSONClass> (s));
            ++i;
            if (i == 100) {
                i = 0;
                loadProgression += 100f / suggestionArray.Length;
                yield return null;
            }
        }

        uiManager["Mouse Blocker"].SetActive (false);
        loaded = true;
    }

    private JSONClass GetSuggestionForPuzzle (string puzzle) {
        try {
            int puzzleId = int.Parse (puzzle);
            var i = 0;
            for (i = 0; i < puzzleSuggestions.Count; ++i) {
                if (puzzleSuggestions[i].GetEntry ("puzzle", string.Empty) == puzzle) break;
            }
            int min = i;
            int max = i + 1000;
            return puzzleSuggestions[Random.Range (min, max)];
        } catch (System.Exception) {
            for (var i = 0; i < 10000; ++i) {
                var result = (JSONClass) puzzleSuggestions[Random.Range (0, puzzleSuggestions.Count)];
                if (result.GetEntry ("puzzle", string.Empty) == puzzle) return result;
            }
            return null;
        }
    }


    private GameObject blueprintPiece;
    private GameObject selectedPiece, lastSelectedPiece;
    private Vector3 offset;

    private TangramGameLogic.TangramGrid inventory = new TangramGameLogic.TangramGrid (new Vector2 (10, 10));

    private void Update () {
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

        if (Input.GetKeyUp (KeyCode.Return)) {
            var piece = TangramGame.SpawnTangramPiece (pieceName);
            if (piece != null) {
                if (large) {
                    uiManager["Workstation Pieces"].Clear ();
                    blueprintPiece = piece;
                    piece.transform.SetParent (uiManager["Workstation Pieces"].transform);
                    piece.transform.position = uiManager["Workstation Grid"].transform.GetChild (8 * 8 - 1).transform.position;
                } else {
                    piece.transform.SetParent (uiManager["Inventory Pieces"].transform);
                    piece.transform.position = uiManager["Inventory Grid"].transform.GetChild (90).transform.position;
                }

                piece.AddEventTrigger (EventTriggerType.PointerDown, () => {
                    lastSelectedPiece = selectedPiece = piece;
                    offset = Input.mousePosition - Camera.main.WorldToScreenPoint (piece.transform.position);
                });
                piece.AddEventTrigger (EventTriggerType.PointerUp, () => {
                    selectedPiece = null;
                });
            }
        }

        if (selectedPiece != null) {
            selectedPiece.transform.position = Camera.main.ScreenToWorldPoint (Input.mousePosition - offset);
        }

        if (Input.GetKeyDown (KeyCode.Delete)) {
            if (lastSelectedPiece != null) {
                Destroy (lastSelectedPiece);
            }
        }

        if (Input.GetKeyUp (KeyCode.Space)) {
            Save ();
        }

        if (Input.GetKeyUp (KeyCode.R)) {
            RandomSolution ();
        }
    }

    private void RandomSolution () {
        Debug.Log ("Try solve...");
        var solution = GetSuggestionForPuzzle (blueprintPiece.name);
        Debug.Log ("Done solving");

        uiManager["Inventory Pieces"].Clear ();

        inventory.Clear ();

        foreach (JSONClass piece in solution.GetJson ("solution", new JSONArray ())) {
            var name = piece.GetEntry ("piece", "");
            var pieceGo = TangramGame.SpawnTangramPiece (name);
            pieceGo.transform.SetParent (uiManager["Inventory Pieces"].transform);
            var pieceTangram = TangramGameLogic.TangramPiece.CreateNew (name);
            for (var i = 0; i < 100; ++i) {
                var x = Random.Range (0, 10);
                var y = Random.Range (0, 10);
                pieceTangram.position = new Vector2 (x, y);
                if (inventory.CheckPieceFit (pieceTangram)) {
                    inventory.AddPiece (pieceTangram);
                    break;
                }
            }
            var index = (int) pieceTangram.position.y * 10 + (int) pieceTangram.position.x;
            pieceGo.transform.position = uiManager["Inventory Grid"].transform.GetChild (index).transform.position;

            pieceGo.AddEventTrigger (EventTriggerType.PointerDown, () => {
                lastSelectedPiece = selectedPiece = pieceGo;
                offset = Input.mousePosition - Camera.main.WorldToScreenPoint (pieceGo.transform.position);
            });
            pieceGo.AddEventTrigger (EventTriggerType.PointerUp, () => {
                selectedPiece = null;
            });
        }
    }

    private void Save () {
        var levelJson = new JSONClass ();
        var blueprint = new JSONClass ();
        var blueprintPieces = new JSONArray ();

        levelJson.Add ("fish", new JSONData (1));
        levelJson.Add ("timer", new JSONData (timer));
        levelJson.Add ("difficulty", new JSONData (difficulty));

        levelJson.Add ("blueprint", blueprint);
        blueprint.Add ("size", JSON.Parse ("{'x':8,'y': 8}"));
        blueprint.Add ("pieces", blueprintPieces);

        for (var i = 0; i < uiManager["Workstation Pieces"].transform.childCount; ++i) {
            var piece = uiManager["Workstation Pieces"].transform.GetChild (i).gameObject;
            var ped = new PointerEventData (EventSystem.current);
            ped.position = Camera.main.WorldToScreenPoint (piece.transform.position);
            var raycastResults = new List<RaycastResult> ();
            EventSystem.current.RaycastAll (ped, raycastResults);

            foreach (var result in raycastResults) {
                if (result.gameObject.transform.IsChildOf (uiManager["Workstation Grid"].transform)) {
                    var index = result.gameObject.transform.GetSiblingIndex ();
                    var x = index % 8;
                    var y = index / 8;
                    var pieceJson = new JSONClass ();
                    pieceJson.Add ("name", piece.name);
                    pieceJson.Add ("position", JSON.Parse ("[" + x + "," + y + "]"));
                    blueprintPieces.Add (pieceJson);
                    break;
                }
            }
        }

        var inventory = new JSONClass ();
        var inventoryPieces = new JSONArray ();

        levelJson.Add ("inventory", inventory);
        inventory.Add ("size", JSON.Parse ("{'x':10,'y': 10}"));
        inventory.Add ("pieces", inventoryPieces);

        for (var i = 0; i < uiManager["Inventory Pieces"].transform.childCount; ++i) {
            var piece = uiManager["Inventory Pieces"].transform.GetChild (i).gameObject;
            var ped = new PointerEventData (EventSystem.current);
            ped.position = Camera.main.WorldToScreenPoint (piece.transform.position);
            var raycastResults = new List<RaycastResult> ();
            EventSystem.current.RaycastAll (ped, raycastResults);

            foreach (var result in raycastResults) {
                if (result.gameObject.transform.IsChildOf (uiManager["Inventory Grid"].transform)) {
                    var index = result.gameObject.transform.GetSiblingIndex ();
                    var x = index % 10;
                    var y = index / 10;
                    var pieceJson = new JSONClass ();
                    pieceJson.Add ("name", piece.name);
                    pieceJson.Add ("position", JSON.Parse ("[" + x + "," + y + "]"));
                    inventoryPieces.Add (pieceJson);
                    break;
                }
            }
        }

        json = levelJson;

        Debug.Log (levelJson.ToString ());
    }

    private void OnDisable () {
        PlayerPrefs.SetString ("tangram_levels", json.ToString ());
        Debug.Log (json.ToString ());
    }

    private void OnGUI () {
        string text = string.Empty;

        if (!loaded) {
            text += string.Format ("Loading {0}%... Please wait!", Mathf.FloorToInt ((loadProgression * 100f)));
        } else {
            text += "Last Selected: " + (lastSelectedPiece == null ? "empty" : lastSelectedPiece.name) + "\n";
            text += "Spawn: " + pieceName + "\n\n";
            text += "HELP SHEET: \n";
            text += "[<-] [->] to change piece number\n";
            text += "[s] to toggle spawning puzzle pieces\n";
            text += "[ENTER] to spawn pieces\n";
            text += "[DEL] to delete the last selected piece\n";
            text += "[LMR] to roughtly reposition pieces\n";
            text += "[R] to spawn a random minimum solution\n";
            text += "[SPACE] to save current configuration\n";
        }

        GUI.Label (new Rect (10, 10, 800, 800), text);
    }

    private JSONClass json;
    private List<JSONClass> puzzleSuggestions = new List<JSONClass> ();

}
