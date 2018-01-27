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
    private int m_x_position;
    private int m_y_position;

    private int m_orientation = 0;
    private Status m_status = Status.Idle;

	// Use this for initialization
	void Start () {
        Arena arena = m_arena.GetComponent<Arena>();
        m_x_position = arena.GetSpawnX(0);
        m_y_position = arena.GetSpawnY(0);

        gameObject.transform.parent = m_arena.transform;
        gameObject.transform.position = new Vector3(arena.GridX(m_x_position), 50.0f, arena.GridY(m_y_position));
	}

    public Status GetStatus()
    {
        return m_status;
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
