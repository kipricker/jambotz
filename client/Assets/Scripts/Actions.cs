using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Action
{
    [System.Serializable]
    public struct Trigger
    {
        public string name;
        public string[] actions;
    }

    [System.Serializable]
    public struct Spawn
    {
        public string name;
        public string[] actions;
    }

    [System.Serializable]
    public struct Effect
    {
        public int target_modifier;
        public int player_modifier;
        public int target_move;
        public int player_move;
        public int turn_body;
        public int turn_turret;
        public bool align_turret_to_body;
        public bool align_body_to_turret;
        public int move;
        public bool trigger;
        public string state_change;
        public Spawn[] spawn;
        public string[] actions;
    }

    public string type;
    public string name;
    public string icon;
    public string sub_icon;
    public bool enabled;
    public string parent;
    public float priority_modifier;
    public int area_of_effect;

    public Trigger trigger;
    public Effect on_hit;
    public Effect on_miss;
    public Effect passive;
}

[System.Serializable]
public class Actions
{
    public Action[] actions;

    public Action GetAction(string name)
    {
        foreach(Action action in actions)
        {
            if (action.name == name)
                return action;
        }
        return new Action();
    }

    public static Actions FromJSON(string path)
    {
        string json = Resources.Load<TextAsset>(path).text;
        Actions acts = JsonUtility.FromJson<Actions>(json);
        return acts;
    }
}