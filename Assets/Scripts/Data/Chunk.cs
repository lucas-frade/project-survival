using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Chunk
{
    public int x;
    public int y;
    public List<Tile> tiles = new List<Tile>();
    public Vector2Int position
    {
        get { return new Vector2Int(x, y); }
        set { this.x = value.x; this.y = value.y; }
    }
}
