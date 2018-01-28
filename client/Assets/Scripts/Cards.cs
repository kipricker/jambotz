using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Card
{
    public string name;
    public string flavor;
    public string title;
	public int priority;
	public int rarity;
	public int id;
    public string[] actions;
}

[System.Serializable]
public class Cards
{
    public Card[] cards;

    public Card GetCard(string name)
    {
        foreach (Card card in cards)
        {
            if (card.name == name)
                return card;
        }
        return new Card();
    }

    public static Cards FromJSON(string path)
    {
        string json = Resources.Load<TextAsset>(path).text;
        Cards cds = JsonUtility.FromJson<Cards>(json);
        return cds;
    }
}