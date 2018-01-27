using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public GameObject m_arena;

    private Actions m_actions;

	// Use this for initialization
	void Start ()
    {
        m_actions = Actions.FromJSON("json/actions");
	}

    public Action GetAction(string name)
    {
        return m_actions.GetAction(name);
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
