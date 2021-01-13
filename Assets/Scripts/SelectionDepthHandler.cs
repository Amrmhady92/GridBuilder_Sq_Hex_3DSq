using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SelectionDepthHandler : MonoBehaviour
{

    public int depth = 1;
    public TileBuilder builder;
    public TextMeshProUGUI textUI;
    List<Tile> tiles;

    bool started = false;
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1);
        tiles = builder.CreateGrid(tiles);
        depth = 1;
        SetDepth(depth);
        started = true;
        textUI.text = "Selection Depth : " + depth;

    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow) && started)
        {
            if (depth > 0)
            {
                depth--;
                OnValueUpdated(depth);
            }
        }
        if (Input.GetKeyDown(KeyCode.UpArrow) && started)
        {
            depth++;
            OnValueUpdated(depth);
        }
    }
    public void OnValueUpdated(int val)
    {
        depth = val;
        SetDepth(depth);
        textUI.text = "Selection Depth : " + depth;

    }

    private void SetDepth(int depth)
    {
        //for (int i = 0; i < tiles.Count; i++)
        //{
        //    if (tiles[i] != null) tiles[i].lightDepth = depth;
        //}
        Tile.depth = depth;
    }
}
