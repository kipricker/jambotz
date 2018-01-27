using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot : MonoBehaviour {

    public enum Status
    {
        Idle = 0,
        Moving,
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
    private Status m_status = Status.Idle;
    private float m_animation_state = 0.0f;

	// Use this for initialization
	void Start ()
    {
        // Testing
        Spawn(0);
        Forward();
	}

    void Spawn(int n)
    {
        Arena arena = m_arena.GetComponent<Arena>();
        m_x_position = arena.GetSpawnX(0);
        m_y_position = arena.GetSpawnY(0);

        gameObject.transform.parent = m_arena.transform;
        gameObject.transform.position = new Vector3(arena.GridX(m_x_position), 0.5f, arena.GridY(m_y_position));
    }

    public Status GetStatus()
    {
        return m_status;
    }

    public void Forward()
    {
        switch(m_orientation)
        {
            case 0:
                m_target_x = m_x_position + 1;
                m_target_y = m_y_position;
                break;
            case 1:
                m_target_x = m_x_position;
                m_target_y = m_y_position + 1;
                break;
            case 2:
                m_target_x = m_x_position - 1;
                m_target_y = m_y_position;
                break;
            case 3:
                m_target_x = m_x_position;
                m_target_y = m_y_position - 1;
                break;
        }
        m_animation_state = 0.0f;
        m_status = Status.Moving;
    }
	
	// Update is called once per frame
	void Update ()
    {
        Arena arena = m_arena.GetComponent<Arena>();
        switch (m_status)
        {
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
                gameObject.transform.position = new Vector3(x, 0.5f, y);
                break;
        }
	}
}
