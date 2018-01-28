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

    private float m_pro_dist;
    private GameObject m_pro_obj;

    private int m_orientation = 0;
    private int m_target_orientation;
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
    }

    public Status GetStatus()
    {
        return m_status;
    }

    public bool Move(int n)
    {
        switch(m_orientation)
        {
            case 0:
                m_target_x = m_x_position;
                m_target_y = m_y_position + n;
                break;
            case 1:
                m_target_x = m_x_position - n;
                m_target_y = m_y_position;
                break;
            case 2:
                m_target_x = m_x_position;
                m_target_y = m_y_position - n;
                break;
            case 3:
                m_target_x = m_x_position + n;
                m_target_y = m_y_position;
                break;
        }

        Arena arena = m_arena.GetComponent<Arena>();
        if (!arena.CanMove(m_x_position, m_y_position, m_target_x, m_target_y))
            return false;

        m_animation_state = 0.0f;
        if (!arena.GridSafe(m_target_x, m_target_y))
        {
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
            return true;
        }

        m_status = Status.Moving;
        return true;
    }

    public void TurnLeft()
    {
        m_target_orientation = (m_orientation + 1) % 4;
        m_animation_state = 0.0f;
        m_status = Status.Turning;
    }

    public void TurnRight()
    {
        m_target_orientation = (m_orientation + 3) % 4;
        m_animation_state = 0.0f;
        m_status = Status.Turning;
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
        Arena.HitInfo hit = arena.Trace(m_x_position, m_y_position, m_orientation);

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
        }
        switch (m_status)
        {
            case Status.Falling:
            case Status.Pushing:
            case Status.Moving:
                float x = (1.0f - m_animation_state) * arena.GridX(m_x_position) + m_animation_state * arena.GridX(m_target_x);
                float y = (1.0f - m_animation_state) * arena.GridX(m_y_position) + m_animation_state * arena.GridX(m_target_y);
                m_animation_state += m_animation_rate;

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
                    else if (m_status == Status.Pushing)
                    {
                        if (m_target_bot.m_status == Status.Idle)
                            m_status = Status.Idle;
                    }
                    else
                    {
                        m_status = Status.Idle;
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
                }
                gameObject.transform.eulerAngles = new Vector3(0.0f, -a, 0.0f);
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
