using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

//This generates the "tower"
public class MapController : MonoBehaviour
{
    [SerializeField] int maxLength; //How big should the tower be?
    [SerializeField] float validThreshold; //How big should the threshold be in allowing similar exit/entrance levels?
    [SerializeField] Transform hazardGoal;
    private List<Transform> levels;

    private List<Transform> segments;

    void Start()
    {
        levels = Resources.LoadAll<Transform>("Prefabs/Levels").ToList(); // Put all levels in array.
        segments = new List<Transform>(); //The actual levels loaded in the game.

        FisherShuffle(levels); //Shuffle the level. This is so we do not have to keep randomly calling values.

        //Load the first level.
        LoadLevel(0);

        //Starting from the first level... (start at 1, as the first level is already loaded.)
        for(int curSeg = 1; curSeg < maxLength || levels.Count == 0; curSeg++) //From 1 to max length...
        {
            //At the previous segment, get the valid EXIT blocks.
            Tilemap[] prevTilemaps = GetLevelTileMaps(segments[curSeg - 1]);

            List<int> validExitTiles = CalculateValidTiles(prevTilemaps, true);

            //Now, go through EACH of the possible level. If the level entry matches most of the level exit, choose that level.
            int posLevel = 0;
            for(; posLevel < levels.Count; posLevel++) //loop through all levels, see which one can fit. (possibleLevel)
            {
                Transform level = levels[posLevel];
                Tilemap[] levelTilemaps = GetLevelTileMaps(level);

                List<int> validEntryTiles = CalculateValidTiles(levelTilemaps, false);
                float totalMatches = 0; //How many matches of entry and exit?

                foreach(int exitTileID in validExitTiles)
                {
                    if(validEntryTiles.Contains(exitTileID)) //both had the same valid
                    {
                        totalMatches++;
                    }
                }

                //Debug.Log(totalMatches/validExitTiles.Count);
                if(totalMatches > 0 && totalMatches/validExitTiles.Count >= validThreshold) //A majority of the tiles were matched, it is fair game!
                {
                    LoadLevel(posLevel);
                    break; //Go to next segment
                }
            }
            
            if(posLevel == levels.Count) //Level failed to load, cut the tower short.
            {
                break;
            }
        }

        //Last level has a bunch of weapons.
        Transform lastLevel = Instantiate(Resources.Load<Transform>("Prefabs/War"));
        segments.Add(lastLevel);
        lastLevel.position = new Vector2(0, 12 * (segments.Count));

        hazardGoal.position = new Vector3(hazardGoal.position.x, lastLevel.position.y - 6, hazardGoal.position.z);
    }

    List<int> CalculateValidTiles(Tilemap[] tileMaps, bool isExit) //Calculate the valid tiles. Exit is the one the player is coming out of (previous level)
    {
        var bounds = GetBounds(tileMaps);
        //-1 = empty. 0 = solid. 1 = one way, 2 = jumpable
        int[] exitTiles = new int[bounds.xMax - bounds.xMin];
        List<int> validTiles = new List<int>(); //The valid exit Tiles.
            
        for(int j = 0; j < exitTiles.Length; j++)
            exitTiles[j] = -1; //fill all array as "empty"

        //First, populate the array with information about the first row.
        //Debug.Log("xMin: " + bounds.xMin + " xMax: " + bounds.yMax);
        for(int i = 0; i < exitTiles.Length; i++) //At the final row of the level, find all valid blocks.
        {
            foreach(Tilemap tileMap in tileMaps)
            {
                TileBase tile = tileMap.GetTile(new Vector3Int(i + bounds.xMin, isExit ? bounds.yMax - 1 : bounds.yMin, 0));

                if(!tile) //no tile found for this tilemap, go to next tilemap to see if it has a tile.
                    continue;
                
                if(tileMap.name.Contains("One Way")) //tile is a one way. Mark it as 1.
                {
                    exitTiles[i] = 1;
                    validTiles.Add(i + bounds.xMin); //One way platform are automatically valid.
                }
                else
                    exitTiles[i] = 0; //Tile must be a platform. Mark it as 0.

                break; //Tile was found, break out of the for loop
            }
        }

        //Now, revisit the array. Mark which tile is marked as "valid" for the exit.
        //It goes all right. Because both empty and platforms check the rightmost, all cases will be satisfied.
        for(int i = 0; i < exitTiles.Length - 1; i++) //-1 length, as it is impossible for the last number to compare itself to others.
        {
            int status = exitTiles[i];
            
            if(status == 2) //Already mark as jumpable. This means a solid platform toggled it- add it to the list.
            {
                validTiles.Add(i + bounds.xMin);
            }
            else //Not marked as jumpable.
            {
                if(IsSolid(status)) //It is a solid! Is is there any empty to the right of it?
                {
                    if(!IsSolid(exitTiles[i + 1]))
                        exitTiles[i + 1] = 2;

                    if(i < exitTiles.Length - 2 && !IsSolid(exitTiles[i + 2]))
                        exitTiles[i + 2] = 2;
                }
                else //Is empty... is it next to any platforms to the right of it?
                {                                   //Need to make sure we do not go over array.
                    if(IsSolid(exitTiles[i + 1]) || (i < exitTiles.Length - 2 && IsSolid(exitTiles[i + 2])))
                    {
                        validTiles.Add(i + bounds.xMin); //The position of the valid tile.
                    }
                }
            }
        }

        /*
        for(int i = 0; i < validTiles.Count; i++)
            Debug.Log(validTiles[i]);
        */

        return validTiles;
    }

    bool IsSolid(int status) //simple function that check if it's equal to 0 or 1
    {
        return (status == 0 || status == 1);
    }

    void LoadLevel(int chosenLevel)
    {
        Transform level = Instantiate(levels[chosenLevel]); //create the level
        levels.RemoveAt(chosenLevel); // Not valid anymore.
        segments.Add(level);

        level.position = new Vector2(0, 12 * (segments.Count));
    }

    Tilemap[] GetLevelTileMaps(Transform level) //Get all of the tilemaps in a level.
    {
        Transform tileGrid = level.Find("Tilemap Grid");

        //Get all of the tilemaps for the level.
        Transform[] childs = Tools.GetChildren(tileGrid);
        Tilemap[] tileMaps = new Tilemap[childs.Length];

        for(int i = 0; i < tileMaps.Length; i++)
        {
            tileMaps[i] = childs[i].GetComponent<Tilemap>();
        }

        return tileMaps;
    }

    (int xMin, int xMax, int yMin, int yMax) GetBounds(Tilemap[] tileMaps) //returns a tuple of the bounds, as it may differ between tilemaps sometimes.
    {
        int xMin = tileMaps[0].cellBounds.min.x;
        int yMin = tileMaps[0].cellBounds.min.y;
        int xMax = tileMaps[0].cellBounds.max.x;
        int yMax = tileMaps[0].cellBounds.max.y;

        for(int i = 1; i < tileMaps.Length; i++)
        {
            if(xMin > tileMaps[i].cellBounds.min.x)
                xMin = tileMaps[i].cellBounds.min.x;

            if(yMin > tileMaps[i].cellBounds.min.y)
                yMin = tileMaps[i].cellBounds.min.y;

            if(xMax < tileMaps[i].cellBounds.max.x)
                xMax = tileMaps[i].cellBounds.max.x;

            if(yMax < tileMaps[i].cellBounds.max.y)
                yMax = tileMaps[i].cellBounds.max.y;
        }

        return (xMin, xMax, yMin, yMax);
    }

    void FisherShuffle(List<Transform> list) //Classic shuffle algorithm. Allow us to add some "randomization" quickly.
	{
		// Loops through array
		for (int i = list.Count - 1; i > 0; i--)
		{
			int toSwap = Random.Range(0,i);

			// Swap the new and old values
            Transform temp = list[i];
			list[i] = list[toSwap];
			list[toSwap] = temp;
		}
	}
}
