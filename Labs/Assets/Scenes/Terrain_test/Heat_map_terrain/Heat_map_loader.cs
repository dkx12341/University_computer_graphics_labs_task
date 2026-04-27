using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[RequireComponent(typeof(Terrain))]
public class Heat_map_loader : MonoBehaviour
{
    // Start is called before the first frame update

    public string filePath = "Assets/Scenes/Terrain_test/Heat_map_terrain/heightmap.jpg";
    public float heightMultiplier = 0.2f;

    private Terrain terrain;
    private TerrainData terrainData;

    void Start()
    {
        terrain = GetComponent<Terrain>();
        terrainData = terrain.terrainData;

        LoadHeightmap();
    }

    void LoadHeightmap()
    {
        string path = filePath;

        if (!File.Exists(path))
        {
            Debug.LogError("Plik nie istnieje: " + path);
            return;
        }

        byte[] fileData = File.ReadAllBytes(path);

        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(fileData);

        ApplyHeightmap(texture);
    }

    void ApplyHeightmap(Texture2D texture)
    {
        int width = terrainData.heightmapResolution;
        int height = terrainData.heightmapResolution;

        float[,] heights = new float[width, height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float u = (float)x / (width - 1);
                float v = (float)y / (height - 1);

                Color pixel = texture.GetPixelBilinear(u, v);

                float grayscale = pixel.grayscale;

                heights[y, x] = grayscale * heightMultiplier;
            }
        }

        terrainData.SetHeights(0, 0, heights);
    }
}

