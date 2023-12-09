using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TangramGridSpawn : MonoBehaviour {

    public static Vector2 GRID_SIZE;

    public int gridSize = 8;
    public GridLayoutGroup copyFromGridLayout;

	private IEnumerator Start () {
        yield return null;
	    
        if (copyFromGridLayout == null) {

            Vector2 size = gameObject.rectTransform ().rect.size;
            GetComponent<GridLayoutGroup> ().cellSize = size / gridSize;
            GRID_SIZE = size / gridSize;

        }

        yield return null;

        if (copyFromGridLayout != null) {
            gameObject.rectTransform ().sizeDelta = copyFromGridLayout.cellSize * gridSize;
            GetComponent<GridLayoutGroup> ().cellSize = GRID_SIZE;
        }

    }
	
}
