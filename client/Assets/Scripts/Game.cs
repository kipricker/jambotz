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

    int m_action_player = -1;
    int m_action_current;
    Action[] m_action_sequence;

	// Use this for initialization
	void Start ()
    {
        m_players = new Player[4];
        m_actions = Actions.FromJSON("json/actions");

        // Testing
        Arena arena = m_arena.GetComponent<Arena>();
        string json_map = Resources.Load<TextAsset>("json/maps/default").text;
        WorldData world = JsonUtility.FromJson<WorldData>(json_map);
        arena.LoadMap(world);

        m_players[0].bot = Instantiate(Resources.Load("objects/obj_tank") as GameObject);
        Bot bot = m_players[0].bot.GetComponent<Bot>();
        bot.m_arena = m_arena;
        bot.Spawn(0);
        ActionSequence(0, new string[] { "move", "turn_right", "backup" });
        //ActionSequence(0, new string[] { "move", "turn_right", "move" });
        //ActionSequence(0, new string[] { "move", "move" });
    }

    public Action GetAction(string name)
    {
        return m_actions.GetAction(name);
    }

    void ActionSequence(int player, string[] actions)
    {
        m_action_sequence = new Action[actions.Length];
        for (int i = 0; i < actions.Length; ++i)
        {
            m_action_sequence[i] = GetAction(actions[i]);
        }
        m_action_current = -1;
        m_action_player = player;
    }
	
	void FixedUpdate ()
    {
        if (m_action_player > -1)
        {
            Bot bot = m_players[m_action_player].bot.GetComponent<Bot>();
            if (bot.GetStatus() == Bot.Status.Idle)
            {
                ++m_action_current;
                if (m_action_current < m_action_sequence.Length)
                {
                    Action action = m_action_sequence[m_action_current];
                    if (action.passive.player_move != 0)
                    {
                        bot.Move(action.passive.player_move);
                    }
                    else if (action.passive.turn_body > 0)
                    {
                        bot.TurnRight();
                    }
                    else if (action.passive.turn_body < 0)
                    {
                        bot.TurnLeft();
                    }
                }
                else
                {
                    m_action_player = -1;
                }
            }
        }
	}
}
