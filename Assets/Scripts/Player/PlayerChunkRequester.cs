using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Tilemaps;
using System.Linq;

public class PlayerChunkRequester : NetworkBehaviour
{
    [Header("Client controlled")]
    public Tilemap tilemap;
    public TileBase tile0, tile1;
    [Header("Server controlled")]
    public Dictionary<Vector2Int, Chunk> chunksLoaded = new Dictionary<Vector2Int, Chunk>();
    public Vector2Int chunkPosition;
    public Vector2Int chunkViewMin;
    public Vector2Int chunkViewMax;

    public override void OnStartClient()
    {
        base.OnStartClient(); 
        tilemap = GameObject.Find("Tilemap").GetComponent<Tilemap>();
    }

    [Server]
    private void ServerUnloadUnusedChunks()
    {
        List<Vector2Int> chunksNeeded = new List<Vector2Int>();

        foreach (PlayerChunkRequester player in FindObjectsOfType<PlayerChunkRequester>())
            for (int x = player.chunkViewMin.x; x <= player.chunkViewMax.x; x++)
                for (int y = player.chunkViewMin.y; y <= player.chunkViewMax.y; y++)
                {
                    Vector2Int key = new Vector2Int(x, y);
                    if (!chunksNeeded.Contains(key)) chunksNeeded.Add(key);
                }

        List<Vector2Int> toUnload = chunksLoaded.Keys.Except(chunksNeeded).ToList();
        foreach (Vector2Int key in toUnload)
        {
            RcpUnload(key);
            WorldGenerator.Save(chunksLoaded[key]);
            chunksLoaded.Remove(key);
        }
    }

    [Server]
    private void ServerRequestChunks()
    {
        for (int x = chunkViewMin.x; x <= chunkViewMax.x; x++)
            for (int y = chunkViewMin.y; y <= chunkViewMax.y; y++)
            {
                Vector2Int key = new Vector2Int(x, y);

                if (!chunksLoaded.ContainsKey(key)) chunksLoaded.Add(key, WorldGenerator.GenerateChunk(new Vector2Int(x, y)));

                TargetDrawChunk(chunksLoaded[key]);
            }

        ServerUnloadUnusedChunks();
    }

    [ClientRpc]
    private void RcpUnload(Vector2Int position)
    {
        Vector2Int chunkOffset = new Vector2Int(position.x * Numbers.ChunkSize, position.y * Numbers.ChunkSize);

        for (int x = 0; x < Numbers.ChunkSize; x++)
            for (int y = 0; y < Numbers.ChunkSize; y++)
                tilemap.SetTile(new Vector3Int(x, y) + (Vector3Int)chunkOffset, null);
    }

    [TargetRpc]
    private void TargetDrawChunk(Chunk chunk)
    {
        foreach (Tile item in chunk.tiles)
        {
            tilemap.SetTile((Vector3Int)item.position, item.type == 0 ? tile0 : tile1);
        }
    }

    private void Update()
    {
        if (!isServer) return;
        chunkPosition = Numbers.ChunkPosition(transform.position);

        Vector2Int oldMin = chunkViewMin;
        Vector2Int oldMax = chunkViewMax;
        chunkViewMin = chunkPosition - new Vector2Int(Numbers.ChunkDrawDistance, Numbers.ChunkDrawDistance);
        chunkViewMax = chunkPosition + new Vector2Int(Numbers.ChunkDrawDistance, Numbers.ChunkDrawDistance);

        if (oldMin != chunkViewMin || oldMax != chunkViewMax) ServerRequestChunks();
    }
}
