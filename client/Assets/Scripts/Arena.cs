﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct WorldData
{
    [System.Serializable]
    public struct MapData
    {
        public int x;
        public int y;
        public string tile;
    }

    [System.Serializable]
    public struct Position
    {
        public int x;
        public int y;
    }

    public int width;
    public int height;
    public Position[] starting_locations;
    public MapData[] map_data;
}

public class Arena : MonoBehaviour {

    public struct Tile
    {
        public int type;
        public int orientation;
        public int[] walls;
    }

    public float m_world_scale = 100.0f;

    private WorldData m_world;
    private Tile[,] m_grid;
    private float m_sx;
    private float m_sy;

    void LoadMap(WorldData world)
    {
        ClearMap();

        m_world = world;
        int w = world.width;
        int h = world.height;

        m_grid = new Tile[w, h];
        foreach(WorldData.MapData tile in world.map_data)
        {
            int x = tile.x;
            int y = tile.y;
            m_grid[x, y].type = 1;
            m_grid[x, y].walls = new int[4];
        }

        for (int y = 0; y < h; ++y)
        {
            for (int x = 0; x < w; ++x)
            {
                m_grid[x, y].walls = new int[4];
                for (int i = 0; i < 4; ++i)
                    m_grid[x, y].walls[i] = 0;
            }
        }

        m_sx = -(w - 1) * m_world_scale / 2.0f;
        m_sy = -(h - 1) * m_world_scale / 2.0f;
        for (int y = 0; y < h; ++y)
        {
            for (int x = 0; x < w; ++x)
            {
                int tile_type = m_grid[x, y].type;

                if (tile_type > 0)
                {
                    GameObject tile = Instantiate(Resources.Load("tileset/tile_blank") as GameObject);

                    tile.transform.position = new Vector3(GridX(x), 0, GridY(y));
                    tile.transform.localScale = new Vector3(100f, 100f, 100f);
                    tile.transform.parent = gameObject.transform;
                }

                for (int i = 0; i < 4; ++i)
                {
                    if (m_grid[x, y].walls[i] == 0)
                        continue;

                    float dx = 0.0f;
                    float dy = 0.0f;
                    if ((i & 1) > 0)
                    {
                        dx = (i & 2) / 2.0f - 0.5f;
                    }
                    else
                    {
                        dy = (i & 2) / 2.0f - 0.5f;
                    }

                    GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    wall.transform.position = new Vector3(GridX(x) + dx * m_world_scale, m_world_scale / 2.0f, GridY(y) + dy * m_world_scale);
                    if ((i & 1) > 0)
                        wall.transform.Rotate(0.0f, 90.0f, 0.0f);
                    wall.transform.localScale = new Vector3(100, 100, 1);
                    wall.transform.parent = gameObject.transform;

                    Renderer rend = wall.GetComponent<Renderer>();
                    rend.material = new Material(Shader.Find("Diffuse"));
                }
            }
        }
    }

    public float GridX(int x)
    {
        return m_sx + x * m_world_scale;
    }

    public float GridY(int y)
    {
        return m_sy + y * m_world_scale;
    }

    public int GetSpawnX(int n)
    {
        return m_world.starting_locations[n].x;
    }

    public int GetSpawnY(int n)
    {
        return m_world.starting_locations[n].y;
    }

    void ClearMap()
    {

    }

	// Use this for initialization
	void Start ()
    {
        // Testing crap.
        string json_map = Resources.Load<TextAsset>("test_map").text;
        WorldData world = JsonUtility.FromJson<WorldData>(json_map);
        LoadMap(world);
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
