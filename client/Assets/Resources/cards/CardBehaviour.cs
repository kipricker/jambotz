using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardBehaviour : MonoBehaviour {
	public Text m_title;
	public Text m_flavor;
	public Text m_priority;

	public Image[] m_action_images;

	private Actions m_actions;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void setCard(Card card) {
		if (m_actions == null) {
			m_actions = Actions.FromJSON ("json/actions");
		}

		m_title.text = card.title;
		m_flavor.text = card.flavor;
		m_priority.text = card.priority.ToString();

		int actionCount = 0;
		foreach (string actionName in card.actions) {
			Action action = m_actions.GetAction (actionName);
			m_action_images [actionCount].enabled = true;
			m_action_images [actionCount].sprite = Resources.Load<Sprite>("UI/bitmaps/" + action.icon);
			actionCount++;
		}
	}
}

