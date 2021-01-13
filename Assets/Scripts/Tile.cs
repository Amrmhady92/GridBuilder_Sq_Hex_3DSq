using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Unity;

public class Tile : MonoBehaviour
{
    public int x;
    public int y;
    public int z;
    public int listIndex = 0;
    public bool hex = false;
    public bool lit = false;
    public TMPro.TextMeshProUGUI coordText;

    public int lightDepth = -1;
    bool selected = false;

    private readonly Tile[] neighbours = new Tile[8];

    public static List<Tile> litTiles;

    public static int depth = 1;
    private List<Tile> tiles;

    private void Awake()
    {
        MeshRenderer mr = this.GetComponentInChildren<MeshRenderer>();

        if (mr)
        {
            Vector3 col;
            col.x = Random.Range(0, 1f);
            col.y = Random.Range(0, 1f);
            col.z = Random.Range(0, 1f);

            mr.material.color = new Color(col.x, col.y, col.z);
        }
    }

    public enum NeighborDirection
    {
        N,
        NE,
        E,
        SE,
        S,
        SW,
        W,
        NW
    }



    public void SetNeigbours(List<Tile> tiles)
    {
        if (hex) // no E or W tiles
        {
            neighbours[(int)NeighborDirection.N] = tiles.Find(t => t.x == x && t.z == (z - 2));
            neighbours[(int)NeighborDirection.NE] = tiles.Find(t => t.x == ((z % 2 == 0 ? (x + 1) : x)) && t.z == (z - 1));
            neighbours[(int)NeighborDirection.SE] = tiles.Find(t => t.x == ((z % 2 == 0 ? (x + 1) : x)) && t.z == (z + 1));
            neighbours[(int)NeighborDirection.S] = tiles.Find(t => t.x == x && t.z == (z + 2));
            neighbours[(int)NeighborDirection.SW] = tiles.Find(t => t.x == ((z % 2 == 0 ? x : (x - 1))) && t.z == (z + 1));
            neighbours[(int)NeighborDirection.NW] = tiles.Find(t => t.x == ((z % 2 == 0 ? x : (x - 1))) && t.z == (z - 1));
            neighbours[(int)NeighborDirection.E]  = null;
            neighbours[(int)NeighborDirection.W] = null;
        }
        else
        {
            neighbours[(int)NeighborDirection.N]  = tiles.Find(t => t.x == x && t.z == (z - 1));
            neighbours[(int)NeighborDirection.NE] = tiles.Find(t => t.x == (x + 1)  && t.z == (z - 1));
            neighbours[(int)NeighborDirection.E]  = tiles.Find(t => t.x == (x + 1)&& t.z == (z));
            neighbours[(int)NeighborDirection.SE] = tiles.Find(t => t.x == (x + 1) && t.z == (z + 1));
            neighbours[(int)NeighborDirection.S]  = tiles.Find(t => t.x == x && t.z == (z + 1));
            neighbours[(int)NeighborDirection.SW] = tiles.Find(t => t.x == (x - 1) && t.z == (z + 1));
            neighbours[(int)NeighborDirection.W]  = tiles.Find(t => t.x == (x - 1) && t.z == (z));
            neighbours[(int)NeighborDirection.NW] = tiles.Find(t => t.x == (x - 1) && t.z == (z - 1));
        }
    }




    public Tile GetNeigbour(NeighborDirection dir)
    {
        return neighbours[(int)dir];
    }


    private void OnMouseEnter()
    {
        selected = true;
        if(litTiles == null) litTiles = new List<Tile>();
        else litTiles.Clear();

        //use lightDepth instead of depth, for individual tile , ie for tile view range or something
        LightNeighbours(true, depth);
        //Debug.Log("Lit tiles count  = " + litTiles.Count); //for debug or other things

    }
    private void OnMouseExit()
    {
        selected = false;
        if (depth <= 0) return;
        if(tiles != null)
        {
            for (int i = 0; i < tiles.Count; i++)
            {
                if(tiles[i] != null)
                {
                    tiles[i].Light(false);
                }
            }
        }
    }




    //        ///////////
    //        //
    //        //
    //        //
    //        //
    //        //   ECOS
    //        //
    //        //
    //        //
    //        //
    //        //


    public void LightNeighbours(bool onoff, int depth)
    {
        if (depth <= 0) return; // may use lightDepth for individual
        NewLight(onoff);
    }
    public void NewLight(bool onoff)
    {

        if (depth < 0)
        {
            depth = 0;
            return;
        }

        if (hex)
        {
            Collider[] hits = Physics.OverlapSphere(this.transform.position + new Vector3(-0.5f, 0, 0), TileBuilder.tileSizeZ * depth);
            if (hits.Length > 0)
            {
                if (tiles != null) tiles.Clear();
                else tiles = new List<Tile>();

                Tile hitTile;
                for (int i = 0; i < hits.Length; i++)
                {
                    hitTile = hits[i].gameObject.GetComponent<Tile>();
                    if (hitTile != null && hitTile != this) tiles.Add(hitTile);
                }

                //for (int i = 0; i < tiles.Count; i++)
                //{
                //    if (tiles[i] == null) continue;
                //    tiles[i].Light(onoff);
                //}
            }
        }
        else // square
        {
            int minX = x - Mathf.Max(depth, 0);
            int maxX = x + Mathf.Max(depth, 0);
            int minZ = z - Mathf.Abs(depth);
            int maxZ = z + Mathf.Abs(depth);
            tiles = TileBuilder.tiles.FindAll(t => t.x >= minX && t.x <= maxX && t.z >= minZ && t.z <= maxZ && t.x != x && t.z != z);


        }
        if (tiles != null)
            for (int i = 0; i < tiles.Count; i++)
            {
                if (tiles[i] == null) continue;
                tiles[i].Light(onoff);
            }

    }

    public void Light(bool onoff)
    {
        if (selected) return;

        if (onoff) litTiles.Add(this);

        MeshRenderer mr = this.GetComponentInChildren<MeshRenderer>();
        Vector3 col;

        if (mr)
        {
            col.x = onoff ? Mathf.Min(1, mr.material.color.r + 0.2f) : Mathf.Max(0, mr.material.color.r - 0.2f);
            col.y = onoff ? Mathf.Min(1, mr.material.color.g + 0.2f) : Mathf.Max(0, mr.material.color.g - 0.2f);
            col.z = onoff ? Mathf.Min(1, mr.material.color.b + 0.2f) : Mathf.Max(0, mr.material.color.b - 0.2f);
            mr.material.color = new Color(col.x, col.y, col.z);
        }

        col = this.transform.position;
        col.y = onoff ? col.y - 0.2f : col.y + 0.2f;
        this.transform.position = col;
    }

}
