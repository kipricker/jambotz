using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot : MonoBehaviour {

    public enum Status
    {
        Idle = 0,
        Dead,
        Moving,
        Pushing,
        Falling,
        Turning,
        TurretTurning,
        Firing,
        Action
    };

    public GameObject m_arena;
    public float m_animation_rate = 0.01f;

    private int m_health = 5;

    private int m_x_position;
    private int m_y_position;

    private int m_target_x;
    private int m_target_y;
    private Bot m_target_bot;
    private Status m_target_status;
    private Animator m_tile_anim;

    private float m_pro_dist;
    private GameObject m_pro_obj;

    private int m_orientation = 0;
    private int m_turret_orientation = 0;
    private int m_target_orientation;
    private int m_target_turret_orientation;
    private Status m_status = Status.Idle;
    private float m_animation_state = 0.0f;

    // Use this for initialization
    void Start()
    {

    }

    public void Spawn(int n)
    {
        Arena arena = m_arena.GetComponent<Arena>();
        m_x_position = arena.GetSpawnX(n);
        m_y_position = arena.GetSpawnY(n);

        gameObject.transform.parent = m_arena.transform.Find("tanks");
        gameObject.transform.position = new Vector3(arena.GridX(m_x_position), 0.0f, arena.GridY(m_y_position));
        gameObject.transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);

        Animator anim = gameObject.GetComponent<Animator>();
        if (anim != null)
        {
            anim.speed = 0.0f;
        }
    }

    public Status GetStatus()
    {
        return m_status;
    }

    public Tiles.Tile GetTile()
    {
        Arena arena = m_arena.GetComponent<Arena>();
        return arena.GetTile(m_x_position, m_y_position);
    }

    public string GetTileDir()
    {
        Arena arena = m_arena.GetComponent<Arena>();
        return arena.GetTileDir(m_x_position, m_y_position);
    }

    public bool ForcedMove(string dir)
    {
        m_target_x = m_x_position;
        m_target_y = m_y_position;
        switch (dir)
        {
            case "west":
                --m_target_y;
                break;
            case "north":
                ++m_target_x;
                break;
            case "east":
                ++m_target_y;
                break;
            case "south":
                --m_target_x;
                break;
        }

        Arena arena = m_arena.GetComponent<Arena>();
        if (!arena.CanMove(m_x_position, m_y_position, m_target_x, m_target_y))
            return false;

        Transform root = arena.transform.Find(string.Format("Row{0}/Col{1}", m_y_position, m_x_position));
        m_tile_anim = root.GetChild(root.childCount - 1).gameObject.GetComponent<Animator>();
        if (m_tile_anim != null)
        {
            m_tile_anim.speed = 1.0f;
        }

        Arena.MoveInfo info = arena.GridOcupied(m_target_x, m_target_y);
        if (!info.free)
        {
            return false;
        }

        m_animation_state = 0.0f;
        m_status = Status.Moving;
        return true;
    }

    public bool Move(int n)
    {
        m_target_x = m_x_position;
        m_target_y = m_y_position;
        switch (m_orientation)
        {
            case 0:
                m_target_y += n;
                break;
            case 1:
                m_target_x -= n;
                break;
            case 2:
                m_target_y -= n;
                break;
            case 3:
                m_target_x += n;
                break;
        }

        Arena arena = m_arena.GetComponent<Arena>();
        if (!arena.CanMove(m_x_position, m_y_position, m_target_x, m_target_y))
            return false;

        Animator anim = gameObject.GetComponent<Animator>();
        m_animation_state = 0.0f;
        if (!arena.GridSafe(m_target_x, m_target_y))
        {
            if (anim != null)
            {
                anim.speed = 1.0f;
            }

            m_status = Status.Falling;
            return true;
        }

        Arena.MoveInfo info = arena.GridOcupied(m_target_x, m_target_y);
        if (!info.free)
        {
            int ttx = 2 * m_target_x - m_x_position;
            int tty = 2 * m_target_y - m_y_position;

            if (!arena.CanMove(m_target_x, m_target_y, ttx, tty))
                return false;

            m_target_bot = info.bot;
            m_status = Status.Pushing;
            m_target_bot.m_target_x = ttx;
            m_target_bot.m_target_y = tty;
            m_target_bot.m_animation_state = 0.0f;
            if (arena.GridSafe(ttx, tty))
            {
                m_target_status = Status.Moving;
            }
            else
            {
                m_target_status = Status.Falling;
            }
            if (anim != null)
            {
                anim.speed = 1.0f;
            }
            return true;
        }

        if (anim != null)
        {
            anim.speed = 1.0f;
        }
        m_status = Status.Moving;
        return true;
    }

    public void TurnLeft()
    {
        m_target_orientation = (m_orientation + 1) % 4;
        m_animation_state = 0.0f;
        m_status = Status.Turning;

        Animator anim = gameObject.GetComponent<Animator>();
        if (anim != null)
        {
            anim.speed = 1.0f;
        }
    }

    public void TurnRight()
    {
        m_target_orientation = (m_orientation + 3) % 4;
        m_animation_state = 0.0f;
        m_status = Status.Turning;

        Animator anim = gameObject.GetComponent<Animator>();
        if (anim != null)
        {
            anim.speed = 1.0f;
        }
    }

    public void TurnTurretLeft()
    {
        m_target_turret_orientation = (m_turret_orientation + 1) % 4;
        m_animation_state = 0.0f;
        m_status = Status.TurretTurning;
    }

    public void TurnTurretRight()
    {
        m_target_turret_orientation = (m_turret_orientation + 3) % 4;
        m_animation_state = 0.0f;
        m_status = Status.TurretTurning;
    }

    public void AlignTurret()
    {
        m_target_turret_orientation = 0;
        m_animation_state = 0.0f;
        m_status = Status.TurretTurning;
    }

    public void Modifier(int n)
    {
        if (m_status == Status.Dead)
            return;

        m_health += n;
        if (m_health <= 0)
        {
            m_status = Status.Dead;
            GameObject.Destroy(gameObject.transform.Find("turret").gameObject);
        }
    }

    public Arena.HitInfo Fire()
    {
        Arena arena = m_arena.GetComponent<Arena>();
        Arena.HitInfo hit = arena.Trace(m_x_position, m_y_position, (m_orientation + m_turret_orientation) % 4);

        m_pro_dist = hit.distance;
        m_animation_state = 0.0f;
        m_status = Status.Firing;

        m_pro_obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        m_pro_obj.transform.parent = gameObject.transform.Find("turret");
        m_pro_obj.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        m_pro_obj.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

        return hit;
    }

    public bool At(int x, int y)
    {
        return (x == m_x_position && y == m_y_position);
    }

    void FixedUpdate ()
    {
        Arena arena = m_arena.GetComponent<Arena>();
        float z = 0.0f;
        if (m_status == Status.Falling && m_animation_state > 0.5f)
        {
            float t = m_animation_state - 0.5f;
            z = -10.0f * t * t;
            Vector3 axis = new Vector3(m_y_position - m_target_y, 0.0f, m_target_x - m_x_position);
            gameObject.transform.Rotate(axis, -180.0f * m_animation_rate, Space.World);
        }
        else if (m_status == Status.Pushing && m_animation_state >= 0.4 && m_animation_state < 1.0f)
        {
            m_target_bot.m_status = m_target_status;
            if (m_target_status == Status.Falling)
                m_status = Status.Moving;
        }
        switch (m_status)
        {
            case Status.Falling:
            case Status.Pushing:
            case Status.Moving:
                float x = (1.0f - m_animation_state) * arena.GridX(m_x_position) + m_animation_state * arena.GridX(m_target_x);
                float y = (1.0f - m_animation_state) * arena.GridX(m_y_position) + m_animation_state * arena.GridX(m_target_y);
                m_animation_state += m_animation_rate;

                if (m_status == Status.Pushing)
                {
                    if (m_target_bot.m_status == Status.Idle && m_animation_state > 1.0f)
                    {
                        m_x_position = 2 * m_target_x - m_x_position;
                        m_y_position = 2 * m_target_y - m_y_position;
                        m_animation_state = 0.6f;
                        m_status = Status.Moving;
                    }
                }
                else
                {
                    if (m_animation_state >= 1.0f)
                    {
                        m_x_position = m_target_x;
                        m_y_position = m_target_y;
                        x = arena.GridX(m_x_position);
                        y = arena.GridX(m_y_position);
                        if (m_status == Status.Falling)
                        {
                            m_status = Status.Dead;
                        }
                        else
                        {
                            m_status = Status.Idle;
                            if (m_tile_anim != null)
                            {
                                m_tile_anim.speed = 0.0f;
                            }
                            Animator anim = gameObject.GetComponent<Animator>();
                            if (anim != null)
                            {
                                anim.speed = 0.0f;
                            }
                        }
                    }
                }
                gameObject.transform.position = new Vector3(x, z, y);
                break;
            case Status.Turning:
                float a0 = m_orientation * 90.0f;
                float a1 = m_target_orientation * 90.0f;
                if (a1 - a0 > 180.0f)
                {
                    a0 += 360.0f;
                }
                if (a0 - a1 > 180.0f)
                {
                    a1 += 360.0f;
                }
                float a = (1.0f - m_animation_state) * a0 + m_animation_state * a1;
                m_animation_state += m_animation_rate;
                if (m_animation_state >= 1.0f)
                {
                    m_orientation = m_target_orientation;
                    a = m_orientation * 90.0f;
                    m_status = Status.Idle;

                    Animator anim = gameObject.GetComponent<Animator>();
                    if (anim != null)
                    {
                        anim.speed = 0.0f;
                    }
                }
                gameObject.transform.localEulerAngles = new Vector3(0.0f, -a, 0.0f);
                break;
            case Status.TurretTurning:
                float ta0 = m_turret_orientation * 90.0f;
                float ta1 = m_target_turret_orientation * 90.0f;
                if (ta1 - ta0 > 180.0f)
                {
                    ta0 += 360.0f;
                }
                if (ta0 - ta1 > 180.0f)
                {
                    ta1 += 360.0f;
                }
                float ta = (1.0f - m_animation_state) * ta0 + m_animation_state * ta1;
                m_animation_state += m_animation_rate;
                if (m_animation_state >= 1.0f)
                {
                    m_turret_orientation = m_target_turret_orientation;
                    ta = m_turret_orientation * 90.0f;
                    m_status = Status.Idle;
                }
                Transform root = gameObject.transform.Find("turret");
                root.localEulerAngles = new Vector3(0.0f, -ta, 0.0f);
                break;
            case Status.Firing:
                float pos = m_animation_state;
                m_animation_state += m_animation_rate * 10.0f;
                if (m_animation_state > m_pro_dist)
                {
                    GameObject.Destroy(m_pro_obj);
                    m_status = Status.Idle;
                }
                m_pro_obj.transform.localPosition = new Vector3(0.0f, 0.0f, pos);
                break;
        }
	}
}
