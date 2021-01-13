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


    private UnityEvent<bool> lightActivatorListener;
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
        lightActivatorListener = new UnityEvent<bool>();
        for (int i = 0; i < neighbours.Length; i++)
        {
            if (neighbours[i] == null) continue;
            lightActivatorListener.AddListener(neighbours[i].Light);
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
        //LightNeighbours(false, lightDepth);
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

    public void LightNeighbours(bool onoff, int depth)
    {
        if (depth <= 0) return;
        //lightActivatorListener.Invoke(onoff, depth - 1);
        //for (int i = 0; i < neighbours.Length; i++)
        //{
        //    if (neighbours[i] != null)
        //    {
        //        neighbours[i].Light(onoff, depth - 1);
        //if (depth > 0)
        //{
        //    neighbours[i].LightNeighbours(onoff, depth);
        //}
        //    }
        //}


        NewLight(onoff);
    }
    public void NewLight(bool onoff)
    {
        /*List<Tile> */

        //if (lightDepth != depth)
        //{
        if (depth < 0)
        {
            depth = 0;
            return;
        }
        Collider[] hits = Physics.OverlapSphere(this.transform.position + new Vector3(-0.5f, 0, 0), TileBuilder.tileSizeZ * depth);
        if(hits.Length > 0)
        {
            if (tiles != null) tiles.Clear();
            else tiles = new List<Tile>();

            Tile hitTile;
            for (int i = 0; i < hits.Length; i++)
            {
                hitTile = hits[i].gameObject.GetComponent<Tile>();
                if (hitTile != null && hitTile != this) tiles.Add(hitTile);
            }

            for (int i = 0; i < tiles.Count; i++)
            {
                if (tiles[i] == null) continue;
                tiles[i].Light(onoff);
            }
        }

        //int minX = z % 2 == 0 ? x - Mathf.Max(depth - 1, 0) : ;//x - Mathf.Abs(depth) : x - Mathf.Max(Mathf.Abs(depth) - 1, 0);
        //int maxX = z % 2 == 0 ? x + Mathf.Max(depth - 1, 0) : x + depth;// x + Mathf.Max(Mathf.Abs(depth) - 1, 0) : x + Mathf.Abs(depth);
        //int minZ = z - Mathf.Abs(depth) + 1;
        //int maxZ = z + Mathf.Abs(depth) + 1;
        //tiles = TileBuilder.tiles.FindAll(t => t.x >= minX && t.x <= maxX && t.z >= minZ && t.z <= maxZ && t.x != x && t.z != z);
        //    tiles = TileBuilder.tiles.FindAll(t =>
        //(t.x >= (x - Mathf.Abs(depth)) &&
        //(t.x < x + Mathf.Abs(depth)) &&
        //(t.z >= z - Mathf.Abs(depth)) &&
        //(t.z < z + Mathf.Abs(depth))
        //));
        //lightDepth = depth;
        //}

        //if (tiles != null)
        //for (int i = 0; i < tiles.Count; i++)
        //{
        //    if (tiles[i] == null) continue;
        //        tiles[i].Light(onoff);
        //}
    }
    public void Light(bool onoff)
    {
        if (selected) return;

        //if(/*(lit && onoff == false) || (lit == false && onoff)*/ lit != onoff)
       // {
           // lit = onoff;
            if(onoff) litTiles.Add(this);

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

            //Collider coll = this.gameObject.GetComponent<Collider>();
            //if (coll != null) coll.enabled = !onoff;
       // }


    }

    //private void OnEnable()
    //{
        
    //}
}
