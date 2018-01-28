using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardBehaviour : MonoBehaviour {
	public Text m_title;
	public Text m_flavor;
	public Text m_priority;

	public Image[] m_attacks;
	public Image[] m_heal;
	public Image[] m_move;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void setCard(Card card) {
		m_title.text = card.title;
		m_flavor.text = card.flavor;
		m_priority.text = card.priority.ToString();
	}
}

