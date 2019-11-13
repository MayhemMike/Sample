using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Assumptions: Prefabs are 1x1 units big
///              Prefab names are unique to each other
///              
/// Class uses a multidimensional array as a virtual grid to determine tile positions.
/// Tiles carry a copy of their array position within their Tile component.
/// The idea is to avoid lookup operations on the array.
/// 
/// Execution sequence:
/// Entry: Update -> Detect Mouseevent on Tile
///     ↓
///     Destroy valid Tile(s) in case of a click or a match and move column(s) down
///     ↓ (recursive loop ↑)
///     Check column for matching neighbors
/// </summary>
public class GameController : MonoBehaviour {

    [SerializeField]
    private int xAxisElements = 10;
    [SerializeField]
    private int yAxisElements = 10;
    private Tile[,] tileGrid;

    void Start() {
        tileGrid = MapBuilder.instance.CreateMap(xAxisElements, yAxisElements);
    }

    /// <summary>
    /// Detects if a Tile was selected and triggers iteration if the Tile is not moving currently.
    /// </summary>
    void Update() { 
        if(Input.GetMouseButtonDown(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out RaycastHit hit)) {
                Tile found = hit.collider.GetComponent<Tile>();
                if(!found.moving) {
                    UpdateColumn(found.xPosition, found.yPosition);
                }
            }
        }
    }

    /// <summary>
    /// Removes Tile and triggers column movement if necessary.
    /// </summary>
    private void UpdateColumn(int x, int y) {
        Destroy(tileGrid[x, y].gameObject);
        tileGrid[x, y] = null;

        //if last in column, no further action, otherwise move items
        if(y == (tileGrid.GetLength(1) - 1)) {
            return;
        }
        else {
            StartCoroutine(MoveColumn(x, y));
        }
    }

    /// <summary>
    /// Relocates tiles within the array grid and triggers move order on Tiles.
    /// Is Coroutine to allow the movement to play out.
    /// </summary>
     private IEnumerator MoveColumn(int x, int y) {
        for(int i = y; i < (tileGrid.GetLength(1) - 1); i++) {
            if(tileGrid[x, (i + 1)] != null) {
                tileGrid[x, i] = tileGrid[x, (i + 1)];
                tileGrid[x, i].yPosition = i;

                if(i < (tileGrid.GetLength(1) - 1)) {
                    tileGrid[x, i + 1] = null;
                }
                tileGrid[x, i].Move();
            }
        }

        yield return new WaitForSeconds(0.5f);

        CheckColumnForNeighbors(x);
    }

    /// <summary>
    /// Checks neighbor tiles of a given coordinate for a match and returns indices in case
    /// 3 or more tiles are a match.
    /// </summary>
    private List<int[]> CheckNeighbors(int x, int y) {
        List<Tile> matchedTiles = new List<Tile> {
            tileGrid[x, y]
        };

        int t_x = x;
        int t_y = y;

        //check for viable neighbors on the left side
        for(int i = (t_x - 1); i >= 0; i--) {
            if(IsTileOfSameType(i, t_y, tileGrid[x, y].name)) {
                matchedTiles.Add(tileGrid[i, t_y]);
            }
            else {
                break;
            }
        }

        //check for viable neighbors on the right side
        for(int j = (t_x + 1); j < tileGrid.GetLength(0); j++) {
            if(IsTileOfSameType(j, t_y, tileGrid[x, y].name)) {
                matchedTiles.Add(tileGrid[j, t_y]);
            }
            else {
                break;
            }
        }

        //Pack results and return
        if(matchedTiles.Count >= 3) {
            List<int[]> results = new List<int[]>();
            for(int k = 0; k < matchedTiles.Count; k++) {
                results.Add(new int[]{matchedTiles[k].xPosition, matchedTiles[k].yPosition});
            }
            return results;
        }
        matchedTiles.Clear();
        return null;
    }
    
    private bool IsTileOfSameType(int x, int y, string sourceName) {
        if(tileGrid[x, y] == null) {
            return false;
        }
        if(tileGrid[x, y].name == sourceName) {
            return true;
        }
        else {
            return false;
        }
    }

    /// <summary>
    /// Loops through a given column and collects index positions of matching tiles.
    /// Initiates tile removal in case of a match.
    /// </summary>
    private void CheckColumnForNeighbors(int x) {
        for(int y = 0; y < tileGrid.GetLength(1); y++) {
            if(tileGrid[x, y] != null) {
                var result = CheckNeighbors(x, y);
                if(result != null) {
                    for(int i = 0; i < result.Count; i++) {
                        UpdateColumn(result[i][0], result[i][1]);
                    }
                    return;
                }
            }
        }
    }
}

