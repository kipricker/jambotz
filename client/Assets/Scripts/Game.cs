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
	public GameObject m_player_hand;
	public GameObject m_network;

    private Actions m_actions;
    private Cards m_cards;
    private Player[] m_players;

    int m_action_player = 0;
    bool m_arena_acted;
    Stack<string> m_action_sequence;
    Stack<string> m_card_sequence;
    Stack<int> m_player_sequence;
    Bot m_target_bot;
    string m_target_dir;

	// Use this for initialization
	void Start ()
    {
        m_action_sequence = new Stack<string>();
        m_card_sequence = new Stack<string>();
        m_player_sequence = new Stack<int>();
        m_players = new Player[4];
        m_actions = Actions.FromJSON("json/actions");
        m_cards = Cards.FromJSON("json/cards");

        // Testing
        Arena arena = m_arena.GetComponent<Arena>();
        string json_map = Resources.Load<TextAsset>("json/maps/default").text;
        WorldData world = JsonUtility.FromJson<WorldData>(json_map);
        arena.LoadMap(world);

        {
            m_players[0].bot = Instantiate(Resources.Load("objects/obj_tank") as GameObject);
            Bot bot = m_players[0].bot.GetComponent<Bot>();
            bot.m_arena = m_arena;
            bot.Spawn(0);
        }
        {
            m_players[1].bot = Instantiate(Resources.Load("objects/obj_tank") as GameObject);
            Bot bot = m_players[1].bot.GetComponent<Bot>();
            bot.m_arena = m_arena;
            bot.Spawn(1);
        }

        string[] hand1 = new string[] { "move_1", "turn_right", "move_2", "look_right", "run_and_gun" };
        string[] hand2 = new string[] { "turn_left", "move_1", "turn_left", "move_3", "heal_2" };
        PlayHands(new string[][] { hand1, hand2 });
    }

    public Action GetAction(string name)
    {
        return m_actions.GetAction(name);
    }

    public void PlayHands(Card[][] cards)
    {
        m_card_sequence.Clear();
        m_player_sequence.Clear();
        for (int i = cards[0].Length - 1; i >= 0; --i)
        {
            for (int p = 0; p < cards.Length; ++p)
            {
                m_player_sequence.Push(p);
				m_card_sequence.Push(cards[p][i].name);
            }
        }
        Card card = m_cards.GetCard(m_card_sequence.Pop());
        ActionSequence(m_player_sequence.Pop(), card.actions);
    }

    void ActionSequence(int player, string[] actions)
    {
        m_action_sequence.Clear();
        for (int i = actions.Length - 1; i >= 0; --i)
        {
            m_action_sequence.Push(actions[i]);
        }
        m_action_player = player;
        m_arena_acted = false;
    }
	
	void FixedUpdate ()
    {
        if (m_action_sequence.Count > 0)
        {
            Bot bot = m_players[m_action_player].bot.GetComponent<Bot>();
            if (bot.GetStatus() == Bot.Status.Idle)
            {
                Action action = m_actions.GetAction(m_action_sequence.Pop());
                if (action.passive.actions != null)
                {
                    for (int i = action.passive.actions.Length - 1; i >= 0; --i)
                    {
                        m_action_sequence.Push(action.passive.actions[i]);
                    }
                }
                
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
                else if (action.passive.turn_turret > 0)
                {
                    bot.TurnTurretRight();
                }
                else if (action.passive.turn_turret < 0)
                {
                    bot.TurnTurretLeft();
                }
                else if (action.passive.align_turret_to_body)
                {
                    bot.AlignTurret();
                }
                else if (action.passive.target_modifier != 0)
                {
                    m_target_bot.Modifier(action.passive.target_modifier);
                }
                else if (action.passive.player_modifier != 0)
                {
                    bot.Modifier(action.passive.player_modifier);
                }
                else if (action.passive.target_move != 0)
                {
                    m_target_bot.ForcedMove(m_target_dir);
                }
                else if (action.name == "fire" || action.name == "vampiric_shot")
                {
                    Arena.HitInfo hitinfo = bot.Fire();
                    if (hitinfo.hit)
                    {
                        m_target_bot = hitinfo.bot;
                        if (action.on_hit.actions != null)
                        {
                            for (int i = action.on_hit.actions.Length - 1; i >= 0; --i)
                            {
                                m_action_sequence.Push(action.on_hit.actions[i]);
                            }
                        }
                    }
                }
            }
        }
        else if (!m_arena_acted)
        {
            Bot bot = m_players[m_action_player].bot.GetComponent<Bot>();
            if (bot.GetStatus() == Bot.Status.Idle)
            {
                m_arena_acted = true;
                Tiles.Tile tile = bot.GetTile();
                if (tile.actions != null)
                {
                    for (int i = tile.actions.Length - 1; i >= 0; --i)
                    {
                        m_action_sequence.Push(tile.actions[i]);
                    }
                    m_target_bot = bot;
                    m_target_dir = bot.GetTileDir();
                }
            }
        }
        else
        {
            Bot bot = m_players[m_action_player].bot.GetComponent<Bot>();
            if (m_card_sequence.Count > 0 && bot.GetStatus() == Bot.Status.Idle)
            {
                Card card = m_cards.GetCard(m_card_sequence.Pop());
                ActionSequence(m_player_sequence.Pop(), card.actions);
            }
        }
	}
}
