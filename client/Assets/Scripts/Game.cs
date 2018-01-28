using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct Player
{
    public string name;
    public bool local;
    public GameObject bot;
}

public class Game : MonoBehaviour
{
	public Button m_trade_button;
	public Button m_transmit_button;

    public GameObject m_arena;
	public GameObject m_player_hand;
	public GameObject m_network;
    public GameObject m_p0_health;
    public GameObject m_p1_health;

    private Actions m_actions;
    private Cards m_cards;
    private Player[] m_players;
    private bool m_game_running = false;

    int m_action_player = 0;
    bool m_arena_acted;
    Stack<string> m_action_sequence;
    Stack<string> m_card_sequence;
    Stack<int> m_player_sequence;
    Bot m_target_bot;
    string m_target_dir;

    private int m_player_local = 0;

    void SpawnBots()
    {
        for (int i = 0; i < m_players.Length; ++i)
        {
            m_players[i].bot = Instantiate(Resources.Load("objects/obj_tank") as GameObject);
            Bot bot = m_players[i].bot.GetComponent<Bot>();
            bot.m_arena = m_arena;
            bot.Spawn(i);
        }
    }

    public void SetLocal(int n)
    {
        m_player_local = n;
        Bot bot0 = m_players[n].bot.GetComponent<Bot>();
        bot0.m_health_text = m_p0_health;
        bot0.Modifier(0);
        Bot bot1 = m_players[(n + 1) % 2].bot.GetComponent<Bot>();
        bot1.m_health_text = m_p1_health;
        bot1.Modifier(0);
    }

	// Use this for initialization
	void Start ()
    {
        m_action_sequence = new Stack<string>();
        m_card_sequence = new Stack<string>();
        m_player_sequence = new Stack<int>();
        m_players = new Player[2];
        m_actions = Actions.FromJSON("json/actions");
        m_cards = Cards.FromJSON("json/cards");

        // Testing
        Arena arena = m_arena.GetComponent<Arena>();
        string json_map = Resources.Load<TextAsset>("json/maps/ggjam").text;
        WorldData world = JsonUtility.FromJson<WorldData>(json_map);
        arena.LoadMap(world);

        SpawnBots();
        m_game_running = true;

        /*string[] hand1 = new string[] { "move_2", "turn_right", "move_2", "heal_2", "fire_3", "fire_2" };
        string[] hand2 = new string[] { "turn_right", "move_2", "turn_left", "move_3", "move_2", "fire_1" };
        Card[] chand1 = new Card[hand1.Length];
        for (int i = 0; i < hand1.Length; ++i)
            chand1[i] = m_cards.GetCard(hand1[i]);
        Card[] chand2 = new Card[hand2.Length];
        for (int i = 0; i < hand2.Length; ++i)
            chand2[i] = m_cards.GetCard(hand2[i]);
        PlayHands(new Card[][] { chand1, chand2 });*/
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
            if (cards[1][i].priority >= cards[0][i].priority)
            {
                m_player_sequence.Push(0);
				m_card_sequence.Push(cards[0][i].name);
                m_player_sequence.Push(1);
                m_card_sequence.Push(cards[1][i].name);
            }
            else
            {
                m_player_sequence.Push(1);
                m_card_sequence.Push(cards[1][i].name);
                m_player_sequence.Push(0);
                m_card_sequence.Push(cards[0][i].name);
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

    void RestartGame()
    {
        for (int i = 0; i < m_players.Length; ++i)
        {
            GameObject.Destroy(m_players[i].bot);
        }

        SpawnBots();

        // Reconnect network here!

        m_game_running = true;
    }
	
	void FixedUpdate ()
    {
		int currentStage = m_network.GetComponent<Network>().m_stage;
		switch (currentStage) {
		case 0: 
			m_trade_button.gameObject.SetActive(true);
			m_transmit_button.gameObject.SetActive(false);
			break;
		case 2: 
			m_trade_button.gameObject.SetActive(false);
			m_transmit_button.gameObject.SetActive(true);
			break;
		case 1:
		case 3:
		default:
			m_trade_button.gameObject.SetActive(false);
			m_transmit_button.gameObject.SetActive(false);
			break;
		}

        if (!m_game_running)
            return;

        for (int i = 0; i < m_players.Length; ++i)
        {
            Bot bot = m_players[i].bot.GetComponent<Bot>();
            if (bot.GetStatus() == Bot.Status.Dead)
            {
                m_game_running = false;
                Invoke("RestartGame", 3.0f);
				m_network.GetComponent<Network>().leaveGame ();
                return;
            }
        }

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
