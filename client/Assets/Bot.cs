using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot : MonoBehaviour {

    public GameObject m_arena;
    public int m_x_position;
    public int m_y_position;

    private int m_orientation = 0;

	// Use this for initialization
	void Start () {
        Arena arena = m_arena.GetComponent<Arena>();

        gameObject.transform.parent = m_arena.transform;
        gameObject.transform.position = new Vector3(arena.GridX(m_x_position), 50.0f, arena.GridY(m_y_position));
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
