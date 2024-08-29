using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Numbers
{
    public static int ChunkSize = 30;
    public static int ChunkDrawDistance = 2;

    public static Vector2Int ChunkPosition(Vector2 position)
    {
        return new Vector2Int(Mathf.FloorToInt(position.x / ChunkSize), Mathf.FloorToInt(position.y / ChunkSize));
    }

    public static Vector2Int TilePosition(Vector2 position)
    {
        return new Vector2Int(Mathf.FloorToInt(position.x), Mathf.FloorToInt(position.y));
    }
}
