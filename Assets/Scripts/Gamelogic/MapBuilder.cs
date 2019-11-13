using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBuilder : MonoBehaviour {
    public static MapBuilder instance;

    public GameObject backgroundObject;
    public GameObject cameraObject;
    public GameObject[] prefabs;

    void Awake() {
        instance = this;
    }

    public Tile[,] CreateMap(int xAxisElements, int yAxisElements) {
        if(xAxisElements <= 0 || yAxisElements <= 0) {
            Debug.LogError("Grid size must be bigger than 0");
        }

        var map = BuildGrid(xAxisElements, yAxisElements);
        SetCameraPosition(xAxisElements, yAxisElements);
        AdjustBackground(xAxisElements, yAxisElements);

        return map;
    }

    /// <summary>
    /// Creates the multidimensional array and populates it with gameobjects
    /// </summary>
    private Tile[,] BuildGrid(int xAxisElements, int yAxisElements) {
        GameObject emptyFolder = new GameObject {
            name = "MapContainer"
        };
        emptyFolder.transform.parent = this.transform;

        Tile[,] tileGrid = new Tile[xAxisElements, yAxisElements];

        for(int y = 0; y < yAxisElements; y++) {
            List<int> previousTileTypes = new List<int>();

            for(int x = 0; x < xAxisElements; x++) {
                int selection = GenerateTrippleFreeNumber(previousTileTypes);
                previousTileTypes.Add(selection);

                GameObject item = (GameObject)Instantiate(prefabs[selection], new Vector3(x, y, 0), prefabs[selection].transform.rotation);
                item.transform.parent = emptyFolder.transform;
                item.name = prefabs[selection].name;
                Tile sc = item.AddComponent(typeof(Tile)) as Tile;
                sc.yPosition = y;
                sc.xPosition = x;
                tileGrid[x, y] = sc;
            }
        }
        return tileGrid;
    }

    /// <summary>
    /// changes a random number in case that it occurred a 3rd time.
    /// </summary>
    private int GenerateTrippleFreeNumber(List<int> previousTileTypes) {
        int randomNumber = Random.Range(0, prefabs.Length);

        if(previousTileTypes.Count >= 2) {
            int counter = 0;
            for(int i = previousTileTypes.Count - 1; i >= 0; i--) {
                if(randomNumber == previousTileTypes[i]) {
                    counter++;
                }
                else {
                    break;
                }
            }
            if(counter >= 2) {
                bool foundGoodNeighbor = false;
                int prevNumber = randomNumber;

                while(!foundGoodNeighbor) {
                    randomNumber = Random.Range(0, prefabs.Length);
                    if(prevNumber != randomNumber) {
                        foundGoodNeighbor = true;
                    }
                }
            }
        }
        return randomNumber;
    }

    private void SetCameraPosition(int xAxisElements, int yAxisElements) {
        int cameraOffset = 0;
        if(yAxisElements >= xAxisElements) {
            cameraOffset = -(yAxisElements + 5);
        }
        else {
            cameraOffset = -(xAxisElements / 2);
        }
        cameraObject.transform.localPosition = new Vector3(cameraObject.transform.localPosition.x + ((xAxisElements-1f) / 2f), cameraObject.transform.localPosition.y + ((yAxisElements-1f) / 2f), cameraOffset);
    }

    private void AdjustBackground(int xAxisElements, int yAxisElements) {
        backgroundObject.transform.localScale = new Vector3(xAxisElements, yAxisElements, backgroundObject.transform.localScale.z);
        backgroundObject.transform.localPosition = new Vector3(backgroundObject.transform.localPosition.x + ((xAxisElements - 1f) / 2f), backgroundObject.transform.localPosition.y + ((yAxisElements - 1f) / 2f), backgroundObject.transform.localPosition.z);
    }
}
