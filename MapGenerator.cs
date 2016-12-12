using UnityEngine;
using System.Collections;
using System;

public class MapGenerator : MonoBehaviour
{
    // set the size of the level/ dungeon
    public int width;
    public int height;

    // seed for the RNG
    public string seed;
    public bool useRandomSeed;

    // scale for how much of the area is was and floor
    [Range(0, 100)]
    public int randomFillPercent;

    // the grid/ map
    int[,] map;

    /// <summary>
    /// is call at the start, when 'run' is pressed
    /// </summary>
    void Start()
    {
        GenerateMap();
    }

    /// <summary>
    /// called every 'tick' 
    /// use fixedUpdate to get it to run at 60fps
    /// </summary>
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GenerateMap();
        }
    }

    void GenerateMap()
    {
        // inits the map to the set size
        map = new int[width, height];
        RandomFillMap();

        // the amount of iterations we want to apply the SmoothMap function
        for (int i = 0; i < 5; i++)
        {
            SmoothMap();
        }
    }

    void RandomFillMap()
    {
        // test to enable the map to look the same each time
        // or not to re-RNG and new map
        if (useRandomSeed)
        {
            // if a new seed is to be used them use the current time
            seed = Time.time.ToString();
        }

        System.Random pseudoRandom = new System.Random(seed.GetHashCode());

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // looping through and if it is the edge of the map then set that value to 1, which is a wall
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                {
                    map[x, y] = 1;
                }
                else
                {
                    // if the RNG is below the set parameter for the fill level then set it to a wall, else a empty tile
                    map[x, y] = (pseudoRandom.Next(0, 100) < randomFillPercent) ? 1 : 0;
                }
            }
        }
    }

    /// <summary>
    /// altering the map to generate a more realistic look
    /// if a tile is surounded by wall then set the tile to a wall
    /// this stops/ reduces the apperance of single emplty tiles on the map
    /// but if the tile is not surrounded by walls then make it a empty tile
    /// it makes use of the 'GetSurroundingWallCount(x, y)' function to find 
    /// the amount of walls
    /// </summary>
    void SmoothMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int neighbourWallTiles = GetSurroundingWallCount(x, y);
                
                if (neighbourWallTiles > 4)
                    map[x, y] = 1;
                else if (neighbourWallTiles < 4)
                    map[x, y] = 0;

            }
        }
    }

    /// <summary>
    /// This function looks at the surrounding 3 x 3 grid of the passed in tile
    /// and calculates the number of wall tiles
    /// </summary>
    /// <param name="gridX"> x position of the tile </param>
    /// <param name="gridY"> y position of the tile </param>
    /// <returns> the number of wall tiles in the 3x3 grid </returns>
    int GetSurroundingWallCount(int gridX, int gridY)
    {
        int wallCount = 0;
        for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
        {
            for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
            {
                // this is a error check to prevent the alorithium from looking a tiles outside of the map
                if (neighbourX >= 0 && neighbourX < width && neighbourY >= 0 && neighbourY < height)
                {
                    // check so it doesnt look at itself
                    if (neighbourX != gridX || neighbourY != gridY)
                    {
                        wallCount += map[neighbourX, neighbourY];
                    }
                }
                else
                {
                    wallCount++;
                }
            }
        }
        return wallCount;
    }

    /// <summary>
    /// this in a unity only function
    /// it draws a visual aid tot he screen, which is used to represent the generated map
    /// it loop through the grid array and if the stored number is 
    /// 0 -- it is a wall therefore a black tile
    /// 1 -- is is a floor therefore a white tile
    /// it also uses a vector3 to set the position of the tile based on the loop position and the size of the map
    /// </summary>
    void OnDrawGizmos()
    {
        if (map != null)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Gizmos.color = (map[x, y] == 1) ? Color.black : Color.white;
                    Vector3 pos = new Vector3(width / 2 + x + .5f, height / 2 + y + .5f, 0);
                    Gizmos.DrawCube(pos, Vector3.one);
                }
            }
        }
    }

}
