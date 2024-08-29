using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class WorldGenerator
{
    private static FastNoiseLite noise;

    private static void SetupNoise()
    {
        noise = new FastNoiseLite();
        noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
        noise.SetSeed(1008);
    }

    public static void Save(Chunk chunk)
    {
        string json = JsonUtility.ToJson(chunk);
        File.WriteAllText(Application.persistentDataPath + $"/{chunk.x}-{chunk.y}.chunk", json);
    }

    private static Chunk TryLoad(Vector2Int chunkPosition)
    {
        try
        {
            string chunkJson = File.ReadAllText(Application.persistentDataPath + $"/{chunkPosition.x}-{chunkPosition.y}.chunk");
            return JsonUtility.FromJson<Chunk>(chunkJson);
        } 
        catch
        {
            return null;
        }
    }

    public static Chunk GenerateChunk(Vector2Int position)
    {
        if (noise == null) SetupNoise();

        Chunk preloaded = TryLoad(position);
        if (preloaded != null) return preloaded;

        Chunk chunk = new Chunk();
        chunk.position = position;

        Vector2Int chunkOffset = new Vector2Int(position.x * Numbers.ChunkSize, position.y * Numbers.ChunkSize);

        for (int x = 0; x < Numbers.ChunkSize; x++)
            for (int y = 0; y < Numbers.ChunkSize; y++)
            {
                float noiseValue = noise.GetNoise(x + chunkOffset.x, y + chunkOffset.y);

                Tile tile = new Tile();
                tile.position = new Vector2Int(x, y) + chunkOffset;
                tile.type = (byte)(noiseValue < -0.5 ? 1 : 0);

                chunk.tiles.Add(tile);
            }

        Save(chunk);
        return chunk;
    }
}
