using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot : MonoBehaviour {

    public enum Status
    {
        Idle = 0,
        Moving,
        Falling,
        Turning,
        Action
    };

    public GameObject m_arena;
    public float m_animation_rate = 0.01f;

    private int m_x_position;
    private int m_y_position;

    private int m_target_x;
    private int m_target_y;

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
        m_x_position = arena.GetSpawnX(0);
        m_y_position = arena.GetSpawnY(0);

        gameObject.transform.parent = m_arena.transform;
        gameObject.transform.position = new Vector3(arena.GridX(m_x_position), 0.0f, arena.GridY(m_y_position));
        gameObject.transform.eulerAngles = new Vector3(0.0f, 90.0f, 0.0f);
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
                m_target_x = m_x_position + n;
                m_target_y = m_y_position;
                break;
            case 1:
                m_target_x = m_x_position;
                m_target_y = m_y_position + n;
                break;
            case 2:
                m_target_x = m_x_position - n;
                m_target_y = m_y_position;
                break;
            case 3:
                m_target_x = m_x_position;
                m_target_y = m_y_position - n;
                break;
        }

        Arena arena = m_arena.GetComponent<Arena>();
        if (!arena.CanMove(m_x_position, m_y_position, m_target_x, m_target_y))
            return false;

        m_animation_state = 0.0f;
        if (arena.GridSafe(m_target_x, m_target_y))
        {
            m_status = Status.Moving;
        }
        else
        {
            m_status = Status.Falling;
        }
        return true;
    }

    public void TurnLeft()
    {
        m_target_orientation = (m_orientation + 3) % 4;
        m_animation_state = 0.0f;
        m_status = Status.Turning;
    }

    public void TurnRight()
    {
        m_target_orientation = (m_orientation + 1) % 4;
        m_animation_state = 0.0f;
        m_status = Status.Turning;
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
        switch (m_status)
        {
            case Status.Falling:
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
                    m_status = Status.Idle;
                }
                gameObject.transform.position = new Vector3(x, z, y);
                break;
            case Status.Turning:
                float a0 = (1 - m_orientation) * 90.0f;
                float a1 = (1 - m_target_orientation) * 90.0f;
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
                    a = (1 - m_orientation) * 90.0f;
                    m_status = Status.Idle;
                }
                gameObject.transform.eulerAngles = new Vector3(0.0f, a, 0.0f);
                break;
        }
	}
}
