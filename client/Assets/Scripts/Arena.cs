using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arena : MonoBehaviour {

    public float m_world_scale = 100.0f;

    private List<GameObject> m_objects;
    private float m_sx;
    private float m_sy;

    void LoadMap(int w, int h, byte[] world)
    {
        ClearMap();

        m_sx = -(w - 1) * m_world_scale / 2.0f;
        m_sy = -(h - 1) * m_world_scale / 2.0f;
        for (int y = 0; y < h; ++y)
        {
            for (int x = 0; x < w; ++x)
            {
                byte tile_data = world[w * y + x];
                byte tile_type = (byte)(tile_data & 0xf);

                bool nwall = (tile_data & 0x80) > 0;
                bool ewall = (tile_data & 0x40) > 0;

                if (tile_type > 0)
                {
                    GameObject tile = Instantiate(Resources.Load("tileset/tile_blank") as GameObject);
                    //GameObject tile = Instantiate(Resources.Load("tile_blank") as GameObject);
                    // m_objects.Add(tile);
                    // tile.AddComponent<Rigidbody>();
                    tile.transform.position = new Vector3(GridX(x), 0, GridY(y));
                    tile.transform.localScale = new Vector3(100f, 100f, 100f);
                    tile.transform.parent = gameObject.transform;

                    //Renderer rend = tile.GetComponent<Renderer>();
                    //rend.material = new Material(Shader.Find("Diffuse"));
                }

                for (int i = 0; i < 4; ++i)
                {
                    if (i == 0 && (y > 0 || tile_type == 0))
                        continue;
                    if (i == 1 && (x > 0 || tile_type == 0))
                        continue;
                    if (i == 2 && !ewall)
                        continue;
                    if (i == 3 && !nwall)
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

    void ClearMap()
    {

    }

	// Use this for initialization
	void Start ()
    {
        // Testing crap.
        byte[] world = new byte[100];
        for (int i = 0; i < 100; ++i) world[i] = (byte)((i % 16) + 64 * (i % 4));
        LoadMap(10, 10, world);
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
