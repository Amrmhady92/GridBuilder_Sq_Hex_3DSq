using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBuilder : MonoBehaviour
{
    public int boardSizeX = 9;
    public int boardSizeY = 9;
    public int boardSizeZ = 9;

    public float separation = 0;
    public float stepX = 1;
    public float stepY = 1;
    public float stepZ = 1;
    public Pooler pooler;
    [Space(15)]

    [Tooltip("Step X must be 1.5 bigger and Step Z must be 0.5 smaller\n" +/*)]*/
    /*[Header(*/"For Hex shape of long Diagonal 1, short Diagonal will be 0.8660254\n"+//)]
    /*[Header(*/"So Step X = 1.5 and Step Z = 0.4330127")]
    public bool hexGrid = true;
    public float buildStepTime = 0.1f;
    private GameObject cursor;
    public static List<Tile> tiles;

    public static float tileSizeX;
    public static float tileSizeY;
    public static float tileSizeZ;


    private void Start()
    {
        if (pooler == null)
        {
            Debug.LogError("Pooler not referenced, make sure the pooler is referenced");
        }
        else
        {
            //CreateGrid();
           // mytiles = CreateGrid(mytiles); // instant build (returns the built list to mytiles or any tiles List outside of this script) (should use this , but not here)
        }
        tileSizeX = stepX;
        tileSizeY = stepY;
        tileSizeZ = stepZ;
    }


    public List<Tile> CreateGrid(List<Tile> tiles = null)
    {
        if(pooler == null)
        {
            Debug.LogError("Pooler not referenced, make sure the pooler is referenced on the Tile Builder Component");
            return null;
        }

        if (tiles != null)
        {
            for (int i = 0; i < tiles.Count; i++)
            {
                tiles[i].gameObject.SetActive(false);
            }
        }
        tiles = new List<Tile>();


        if (hexGrid)
        {
            stepX *= 1.5f;
            stepZ *= 0.5f;
        }

        cursor = new GameObject("cursor");
        cursor.transform.position = this.transform.position;
        float startPosX;
        float startPosY;
        float startPosZ;

        if (boardSizeX % 2 == 0) startPosX = ((boardSizeX - 1) / 2) * (stepX + separation) + (stepX / 2 + separation / 2);
        else startPosX = (boardSizeX / 2) * (stepX + separation);

        if (boardSizeY % 2 == 0) startPosY = ((boardSizeY - 1) / 2) * (stepY + separation) + (stepY / 2 + separation / 2);
        else startPosY = (boardSizeY / 2) * (stepY + separation);

        if (boardSizeZ % 2 == 0) startPosZ = ((boardSizeZ - 1) / 2) * (stepZ + separation) + (stepZ / 2 + separation / 2);
        else startPosZ = (boardSizeZ / 2) * (stepZ + separation);


        cursor.transform.position = new Vector3(cursor.transform.position.x - startPosX, cursor.transform.position.y + startPosY, cursor.transform.position.z + startPosZ);
        float startX = cursor.transform.position.x;
        float startY = cursor.transform.position.y;
        float startZ = cursor.transform.position.z;


        Tile currentTile;
        float step;
        if (hexGrid)
        {
            boardSizeX += 1;
            boardSizeZ -= 1;
        }

        for (int i = 0; i < boardSizeZ; i++)
        {
            for (int j = 0; j < boardSizeY; j++)
            {
                for (int k = 0; k < boardSizeX; k++)
                {
                    //if (applyHexShifting && (k == 0 /* boardSizeX - 1*/) && i % 2 == 0) continue;
                    if (hexGrid && (k == boardSizeX - 1) && i % 2 == 0) continue;

                    currentTile = pooler.Get(true).GetComponent<Tile>();
                    tiles.Add(currentTile);

                    currentTile.x = k;
                    currentTile.y = j;
                    currentTile.z = i;
                    currentTile.listIndex = tiles.Count - 1;

                    if (currentTile.coordText != null) currentTile.coordText.text = "x: " + k + "\nz: " + i;

                    currentTile.transform.position = cursor.transform.position;

                    step = stepX + separation;// applyHexShifting && i % 2 == 0 ? /*(stepX + separation) + */(stepX * 1.25f + separation * 1.25f) : stepX + separation;

                    cursor.transform.position = new Vector3(cursor.transform.position.x + step, cursor.transform.position.y, cursor.transform.position.z);

                }

                cursor.transform.position = new Vector3(startX, cursor.transform.position.y - (stepY + separation), cursor.transform.position.z);
            }
            step = hexGrid && i % 2 == 0 ? -1 * (stepX * 0.5f + separation * 0.5f) : 0;
            cursor.transform.position = new Vector3(startX + step, startY, cursor.transform.position.z - (stepZ + separation));
        }
        if (hexGrid)
        {
            boardSizeX -= 1;
            boardSizeZ += 1;
            stepX /= 1.5f;
            stepZ /= 0.5f;

        }
        Debug.Log(tiles.Count);
        for (int i = 0; i < tiles.Count; i++)
        {
            tiles[i].SetNeigbours(tiles);
        }
        TileBuilder.tiles = new List<Tile>(tiles);
        return tiles;
    }

    //Slow mo version , Only for show, or if needed as well
    //public IEnumerator CreateGridOneByOne(List<Tile> tiles , float time = 0.1f)
    //{

    //    if (pooler == null)
    //    {
    //        Debug.LogError("Pooler not referenced, make sure the pooler is referenced on the Tile Builder Component");
    //        yield return null;
    //    }

    //    if (tiles != null)
    //    {
    //        for (int i = 0; i < tiles.Count; i++)
    //        {
    //            tiles[i].gameObject.SetActive(false);
    //        }
    //    }
    //    tiles = new List<Tile>();


    //    if (hexGrid)
    //    {
    //        stepX *= 1.5f;
    //        stepZ *= 0.5f;
    //    }

    //    cursor = new GameObject("cursor");
    //    cursor.transform.position = this.transform.position;
    //    float startPosX;
    //    float startPosY;
    //    float startPosZ;

    //    if (boardSizeX % 2 == 0) startPosX = ((boardSizeX - 1) / 2) * (stepX + separation) + (stepX / 2 + separation / 2);
    //    else startPosX = (boardSizeX / 2) * (stepX + separation);

    //    if (boardSizeY % 2 == 0) startPosY = ((boardSizeY - 1) / 2) * (stepY + separation) + (stepY / 2 + separation / 2);
    //    else startPosY = (boardSizeY / 2) * (stepY + separation);

    //    if (boardSizeZ % 2 == 0) startPosZ = ((boardSizeZ - 1) / 2) * (stepZ + separation) + (stepZ / 2 + separation / 2);
    //    else startPosZ = (boardSizeZ / 2) * (stepZ + separation);


    //    cursor.transform.position = new Vector3(cursor.transform.position.x - startPosX, cursor.transform.position.y + startPosY, cursor.transform.position.z + startPosZ);
    //    float startX = cursor.transform.position.x;
    //    float startY = cursor.transform.position.y;
    //    float startZ = cursor.transform.position.z;


    //    Tile currentTile;
    //    float step;
    //    if (hexGrid)
    //    {
    //        boardSizeX += 1;
    //        boardSizeZ -= 1;
    //    }

    //    for (int i = 0; i < boardSizeZ; i++)
    //    {
    //        for (int j = 0; j < boardSizeY; j++)
    //        {
    //            for (int k = 0; k < boardSizeX; k++)
    //            {
    //                //if (applyHexShifting && (k == 0 /* boardSizeX - 1*/) && i % 2 == 0) continue;
    //                if (hexGrid && (k == boardSizeX - 1) && i % 2 == 0) continue;

    //                currentTile = pooler.Get(true).GetComponent<Tile>();
    //                tiles.Add(currentTile);

    //                currentTile.x = k;
    //                currentTile.y = j;
    //                currentTile.z = i;
    //                currentTile.listIndex = tiles.Count - 1;

    //                if (currentTile.coordText != null) currentTile.coordText.text = "x: " + k + "\nz: " + i;

    //                currentTile.transform.position = cursor.transform.position;

    //                step = stepX + separation;// applyHexShifting && i % 2 == 0 ? /*(stepX + separation) + */(stepX * 1.25f + separation * 1.25f) : stepX + separation;

    //                cursor.transform.position = new Vector3(cursor.transform.position.x + step, cursor.transform.position.y, cursor.transform.position.z);
                    
    //                yield return new WaitForSeconds(time);

    //            }
                
    //            cursor.transform.position = new Vector3(startX, cursor.transform.position.y - (stepY + separation), cursor.transform.position.z);
    //        }
    //        step = hexGrid && i % 2 == 0 ? -1 * (stepX * 0.5f + separation * 0.5f) : 0;
    //        cursor.transform.position = new Vector3(startX + step, startY, cursor.transform.position.z - (stepZ + separation));
    //    }
    //    if (hexGrid)
    //    {
    //        boardSizeX -= 1;
    //        boardSizeZ += 1;
    //        stepX /= 1.5f;
    //        stepZ /= 0.5f;

    //    }
    //    Debug.Log(tiles.Count);
    //    for (int i = 0; i < tiles.Count; i++)
    //    {
    //        tiles[i].SetNeigbours(tiles);
    //    }
    //    TileBuilder.tiles = new List<Tile>(tiles);


    //}

    
}

