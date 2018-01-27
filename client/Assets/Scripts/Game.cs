using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Player
{
    public string name;
    public bool local;
    public GameObject bot;
}

public class Game : MonoBehaviour
{
    public GameObject m_arena;

    private Actions m_actions;
    private Player[] m_players;

	// Use this for initialization
	void Start ()
    {
        m_players = new Player[4];
        m_actions = Actions.FromJSON("json/actions");

        // Testing
        Arena arena = m_arena.GetComponent<Arena>();
        string json_map = Resources.Load<TextAsset>("test_map").text;
        WorldData world = JsonUtility.FromJson<WorldData>(json_map);
        arena.LoadMap(world);

        m_players[0].bot = Instantiate(Resources.Load("objects/obj_tank") as GameObject);
        Bot bot = m_players[0].bot.GetComponent<Bot>();
        bot.m_arena = m_arena;
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
