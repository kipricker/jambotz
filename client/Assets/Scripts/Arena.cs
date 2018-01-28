using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct WorldData
{
    [System.Serializable]
    public struct Addon
    {
        public string name;
        public string edge;
    }

    [System.Serializable]
    public struct MapData
    {
        public int x;
        public int y;
        public string tile;
        public Addon[] tile_add_ons;
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

[System.Serializable]
public struct Tiles
{
    [System.Serializable]
    public struct Tile
    {
        public string name;
        public string asset;
    }

    public Tile[] tiles;
}

[System.Serializable]
public struct Addons
{
    [System.Serializable]
    public struct Addon
    {
        public string name;
        public string asset;
    }

    public Addon[] tile_add_ons;
}

public class Arena : MonoBehaviour {

    public struct Tile
    {
        public int type;
        public int orientation;
        public int[] walls;
    }

    public float m_world_scale = 1.0f;

    private WorldData m_world;
    private Tile[,] m_grid;
    private Tiles m_tiles;
    private Addons m_addons;
    private float m_sx;
    private float m_sy;

    public void LoadMap(WorldData world)
    {
        ClearMap();

        if (m_tiles.tiles == null)
        {
            string json = Resources.Load<TextAsset>("json/tiles").text;
            m_tiles = JsonUtility.FromJson<Tiles>(json);
        }
        if (m_addons.tile_add_ons == null)
        {
            string json = Resources.Load<TextAsset>("json/tile_add_ons").text;
            m_addons = JsonUtility.FromJson<Addons>(json);
        }

        m_world = world;
        int w = world.width;
        int h = world.height;

        m_sx = -(w - 1) * m_world_scale / 2.0f;
        m_sy = -(h - 1) * m_world_scale / 2.0f;

        m_grid = new Tile[w, h];
        for (int y = 0; y < h; ++y)
        {
            for (int x = 0; x < w; ++x)
            {
                m_grid[x, y].type = 0;
                m_grid[x, y].walls = new int[4];
                for (int i = 0; i < 4; ++i)
                    m_grid[x, y].walls[i] = 0;
            }
        }
        foreach (WorldData.MapData tile_data in world.map_data)
        {
            int x = tile_data.x;
            int y = tile_data.y;
            int tile_type = FindTile(tile_data.tile);
            if (tile_type > 0)
            {
                string asset = m_tiles.tiles[tile_type - 1].asset;
                GameObject tile = Instantiate(Resources.Load(asset) as GameObject);

                tile.transform.position = new Vector3(GridX(x), 0, GridY(y));
                tile.transform.parent = gameObject.transform;
            }

            m_grid[x, y].type = tile_type;
            if (tile_data.tile_add_ons != null)
            {
                foreach (WorldData.Addon addon in tile_data.tile_add_ons)
                {
                    float angle = 0.0f;
                    switch (addon.edge)
                    {
                        case "east":
                            angle = 180.0f;
                            if (addon.name == "wall")
                                m_grid[x, y].walls[0] = 1;
                            break;
                        case "south":
                            angle = 270.0f;
                            if (addon.name == "wall")
                                m_grid[x, y].walls[1] = 1;
                            break;
                        case "west":
                            angle = 0.0f;
                            if (addon.name == "wall")
                                m_grid[x, y].walls[2] = 1;
                            break;
                        case "north":
                            angle = 90.0f;
                            if (addon.name == "wall")
                                m_grid[x, y].walls[3] = 1;
                            break;
                    }

                    int addon_type = FindAddon(addon.name);
                    if (addon_type > 0)
                    {
                        string asset = m_addons.tile_add_ons[addon_type - 1].asset;
                        GameObject wall = Instantiate(Resources.Load(asset) as GameObject);

                        wall.transform.position = new Vector3(GridX(x), 0, GridY(y));
                        wall.transform.eulerAngles = new Vector3(0.0f, angle, 0.0f);
                        wall.transform.parent = gameObject.transform;
                    }
                }
            }
        }
    }

    int FindTile(string name)
    {
        for (int i = 0; i < m_tiles.tiles.Length; ++i)
        {
            if (m_tiles.tiles[i].name == name)
                return i + 1;
        }
        return 0;
    }

    int FindAddon(string name)
    {
        for (int i = 0; i < m_addons.tile_add_ons.Length; ++i)
        {
            if (m_addons.tile_add_ons[i].name == name)
                return i + 1;
        }
        return 0;
    }

    public bool CanMove(int x0, int y0, int x1, int y1)
    {
        return true;
    }

    public bool GridSafe(int x, int y)
    {
        if (x < 0 || y < 0 || x >= m_world.width || y >= m_world.height)
            return false;
        return m_grid[x, y].type > 0;
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
        
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
