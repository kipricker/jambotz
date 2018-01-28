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
        public string orientation;
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
        public string[] actions;
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

public class Arena : MonoBehaviour
{
    public struct HitInfo
    {
        public bool hit;
        public float distance;
        public Bot bot;
    }

    public struct MoveInfo
    {
        public bool free;
        public Bot bot;
    }

    public float m_world_scale = 1.0f;

    private WorldData m_world;
    private Tiles m_tiles;
    private Addons m_addons;
    private float m_sx;
    private float m_sy;

    private float m_mouse_x;
    private float m_mouse_y;

    private string[] m_orientations;

    private void OnMouseDown()
    {
        m_mouse_x = Input.mousePosition.x;
        m_mouse_y = Input.mousePosition.y;
    }

    void OnMouseDrag()
    {
        float x = Input.mousePosition.x;
        float y = Input.mousePosition.y;

        Transform cam = Camera.main.transform;
        Vector3 delta = new Vector3(x - m_mouse_x, 0.0f, y - m_mouse_y);
        cam.localPosition = cam.localPosition - delta / 100.0f;

        m_mouse_x = x;
        m_mouse_y = y;
    }

    public void LoadMap(WorldData world)
    {
        m_orientations = new string[] { "west", "north", "east", "south" };
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

        BoxCollider collide = gameObject.AddComponent<BoxCollider>();
        collide.size = new Vector3(m_world_scale * w, 3.0f, m_world_scale * h);
        collide.isTrigger = true;

        GameObject tanks = new GameObject("tanks");
        tanks.transform.parent = gameObject.transform;
        tanks.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        for (int y = 0; y < h; ++y)
        {
            GameObject row = new GameObject(string.Format("Row{0}", y));
            row.transform.parent = gameObject.transform;
            row.transform.localPosition = new Vector3(0.0f, 0.0f, GridY(y));
            for (int x = 0; x < w; ++x)
            {
                GameObject col = new GameObject(string.Format("Col{0}", x));
                col.transform.parent = row.transform;
                col.transform.localPosition = new Vector3(GridX(x), 0.0f, 0.0f);
                for (int i = 0; i < 4; ++i)
                {
                    GameObject node = new GameObject(m_orientations[i]);
                    node.transform.parent = col.transform;
                    node.transform.localEulerAngles = new Vector3(0.0f, 90.0f * i, 0.0f);
                    node.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
                }
            }
        }

        foreach (WorldData.MapData tile_data in world.map_data)
        {
            int x = tile_data.x;
            int y = tile_data.y;
            int tile_type = FindTile(tile_data.tile);
            if (tile_type > 0)
            {
                float angle = 0.0f;
                switch (tile_data.orientation)
                {
                    case "north":
                        angle = 90.0f;
                        break;
                    case "east":
                        angle = 180.0f;
                        break;
                    case "south":
                        angle = 270.0f;
                        break;
                }

                Transform root = gameObject.transform.Find(string.Format("Row{0}/Col{1}", y, x));
                string asset = m_tiles.tiles[tile_type - 1].asset;
                GameObject tile = Instantiate(Resources.Load(asset) as GameObject);

                Animator anim = tile.GetComponent<Animator>();
                if (anim != null)
                {
                    anim.speed = 0.0f;
                }

                tile.transform.parent = root;
                tile.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
                tile.transform.localEulerAngles = new Vector3(0.0f, angle, 0.0f);
            }

            if (tile_data.tile_add_ons != null)
            {
                foreach (WorldData.Addon addon in tile_data.tile_add_ons)
                {
                    int addon_type = FindAddon(addon.name);
                    if (addon_type > 0)
                    {
                        string dir = addon.edge;
                        if (dir == "")
                            dir = "west";
                        Transform root = gameObject.transform.Find(string.Format("Row{0}/Col{1}/{2}", y, x, dir));

                        string asset = m_addons.tile_add_ons[addon_type - 1].asset;
                        GameObject wall = Instantiate(Resources.Load(asset) as GameObject);

                        wall.transform.parent = root;
                        wall.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
                        wall.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
                    }
                }
            }
        }
    }

    public Tiles.Tile GetTile(int x, int y)
    {
        int tile_type = -1;
        foreach (WorldData.MapData tile_data in m_world.map_data)
        {
            if (x == tile_data.x && y == tile_data.y)
            {
                tile_type = FindTile(tile_data.tile);
            }
        }
        if (tile_type < 0)
            tile_type = FindTile("blank");
        return m_tiles.tiles[tile_type - 1];
    }

    public string GetTileDir(int x, int y)
    {
        string dir = "west";
        foreach (WorldData.MapData tile_data in m_world.map_data)
        {
            if (x == tile_data.x && y == tile_data.y && tile_data.orientation != "")
                dir = tile_data.orientation;
        }
        return dir;
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

    public MoveInfo GridOcupied(int x, int y)
    {
        MoveInfo result = new MoveInfo();
        Transform root = gameObject.transform.Find("tanks");
        for (int i = 0; i < root.childCount; ++i)
        {
            Bot bot = root.GetChild(i).gameObject.GetComponent<Bot>();
            if (bot.At(x, y))
            {
                result.bot = bot;
                result.free = false;
                return result;
            }
        }
        result.free = true;
        return result;
    }

    public HitInfo Trace(int x0, int y0, int dir)
    {
        bool impact = false;
        HitInfo result = new HitInfo();

        int dist = 0;
        int dx = 0;
        int dy = 0;
        switch (dir)
        {
            case 0:
                dy = 1;
                break;
            case 1:
                dx = -1;
                break;
            case 2:
                dy = -1;
                break;
            case 3:
                dx = 1;
                break;
        }

        while (!impact)
        {
            int x = x0 + (dist + 1) * dx;
            int y = y0 + (dist + 1) * dy;

            if (x < 0 || y < 0 || x >= m_world.width || y >= m_world.height)
            {
                result.hit = false;
                result.distance = dist + 0.5f;
                impact = true;
            }
            if (!CanMove(x0, y0, x, y))
            {
                result.hit = false;
                result.distance = dist + 0.5f;
                impact = true;
            }
            else
            {
                Transform root = gameObject.transform.Find("tanks");
                for (int i = 0; i < root.childCount; ++i)
                {
                    Bot bot = root.GetChild(i).gameObject.GetComponent<Bot>();
                    if (bot.At(x, y))
                    {
                        result.hit = true;
                        result.distance = dist + 1.0f;
                        result.bot = bot;
                        impact = true;
                    }
                }
            }
            ++dist;
        }
        return result;
    }

    public bool CanMove(int x0, int y0, int x1, int y1)
    {
        int dx = (int)Mathf.Sign(x1 - x0);
        int dy = (int)Mathf.Sign(y1 - y0);
        if (x0 == x1)
            dx = 0;
        if (y0 == y1)
            dy = 0;
        if (dx != 0 && dy != 0)
            return false;
        int dir = 0;
        if (dx != 0)
            dir += 1;
        if (dx > 0)
            dir += 2;
        if (dy > 0)
            dir += 2;

        int x = x0;
        int y = y0;
        while (x != x1 || y != y1)
        {
            Transform root = gameObject.transform.Find(string.Format("Row{0}/Col{1}/{2}", y, x, m_orientations[dir]));
            if (root.childCount > 0)
                return false;

            x += dx;
            y += dy;
        }
        return true;
    }

    public bool GridSafe(int x, int y)
    {
        if (x < 0 || y < 0 || x >= m_world.width || y >= m_world.height)
            return false;
        Transform root = gameObject.transform.Find(string.Format("Row{0}/Col{1}", y, x));
        return root.childCount > 4;
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
