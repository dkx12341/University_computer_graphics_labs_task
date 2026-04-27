using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Terrain))]
public class Terrain_generator : MonoBehaviour
{
    public float scale = 20f;
    public float heightMultiplier = 0.2f;

    public int octaves = 4;
    public float persistence = 0.5f;
    public float lacunarity = 2f;

    public Vector2 offset;


    public bool animateTerrain = false;
    public float animationSpeed = 5f;
    public float updateInterval = 0.1f; // co ile sekund aktualizowa

    private float timer;

    private Terrain terrain;
    private TerrainData terrainData;

    void Start()
    {
        terrain = GetComponent<Terrain>();
        terrainData = terrain.terrainData;

        GenerateTerrain();
    }

    void Update()
    {
        if (!animateTerrain) return;

        timer += Time.deltaTime;

        if (timer >= updateInterval)
        {
            timer = 0f;

            
            offset.x += Time.deltaTime * animationSpeed;
            offset.y += Time.deltaTime * animationSpeed;

            GenerateTerrain();
        }
    }

    void GenerateTerrain()
    {
        int width = terrainData.heightmapResolution;
        int height = terrainData.heightmapResolution;

        float[,] heights = new float[width, height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                heights[x, y] = CalculateHeight(x, y);
            }
        }

        terrainData.SetHeights(0, 0, heights);
    }

    float CalculateHeight(int x, int y)
    {
        float amplitude = 1;
        float frequency = 1;
        float noiseHeight = 0;

        for (int i = 0; i < octaves; i++)
        {
            float sampleX = (x / scale) * frequency + offset.x;
            float sampleY = (y / scale) * frequency + offset.y;

            float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
            noiseHeight += perlinValue * amplitude;

            amplitude *= persistence;
            frequency *= lacunarity;
        }

        return Mathf.Clamp01(noiseHeight * heightMultiplier + 0.5f);
    }
}