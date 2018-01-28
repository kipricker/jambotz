using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandBehaviour : MonoBehaviour {
	public GameObject m_network;
	public GameObject m_game;
	public List<Card> m_cards = new List<Card> (); 
	List<GameObject> cardObjs = new List<GameObject>();
	List<int> selectedCards = new List<int>();

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void SendCards() {
		List<Card> cards = new List<Card> (); 
		selectedCards.Sort ();
		selectedCards.Reverse ();
		foreach (int i in selectedCards) {
			cards.Add (m_cards [i]);
			Destroy (cardObjs [i]);
			cardObjs.RemoveAt (i);
			m_cards.RemoveAt (i);
		}
		selectedCards.Clear ();
		m_network.GetComponent<Network> ().sendCards (cards.ToArray ());
	}

	public void ProgramJambox() {
		selectedCards.Clear ();
		m_network.GetComponent<Network> ().playHand (m_cards.ToArray ());
		m_cards.Clear ();

		foreach (GameObject card in cardObjs) {
			Destroy (card);
		}

		cardObjs.Clear ();
	}

	public void SetHand(Card[] cards) {
		m_cards.Clear ();
		foreach (GameObject card in cardObjs) {
			Destroy (card);
		}

		cardObjs.Clear ();

		m_cards.AddRange(cards);
		int cardNumber = 0;
		foreach (Transform child in transform) {
			GameObject childObj = child.gameObject;
			GameObject card = Instantiate(Resources.Load("cards/Card") as GameObject);
			card.transform.SetParent(childObj.transform, false);
			card.transform.localPosition = Vector3.zero;
			cardObjs.Add (card);

			childObj.SendMessage ("setSlotNumber", cardNumber);
			card.SendMessage ("setCard", m_cards[cardNumber]);
			cardNumber++;
		}
	}

	void ToggleCardSelect(GameObject cardObj, bool selected, bool leftSide) 
	{
		Vector3 vec3 = cardObj.GetComponent<RectTransform> ().sizeDelta;
		float xPos = vec3.x / 2;
		if (selected) 
		{
			xPos = vec3.x / 4;
			if (leftSide) {
				xPos *= 3;
			}
		}
		cardObj.GetComponent<RectTransform> ().anchoredPosition = new Vector3 (xPos, -vec3.y / 2, 0);
	}

	void OnItemSelected(DragAndDropCell.DropDescriptor desc)
	{
		int srcSlotNumber = desc.sourceCell.slotNumber;
		GameObject cardObj = cardObjs [srcSlotNumber];

		if (selectedCards.Contains (srcSlotNumber)) {
			ToggleCardSelect(cardObj, false, true);

			selectedCards.Remove (srcSlotNumber);
		} else if (selectedCards.Count < 2) {
			ToggleCardSelect(cardObj, true, true);

			selectedCards.Add (srcSlotNumber);
		}
	}

	void OnItemPlace(DragAndDropCell.DropDescriptor desc)
	{
		selectedCards.Clear ();

		int srcSlotNumber = desc.sourceCell.slotNumber;
		int destSlotNumber = desc.destinationCell.slotNumber;

		GameObject cardObj = cardObjs [srcSlotNumber];
		cardObjs.RemoveAt (srcSlotNumber);
		cardObjs.Insert (destSlotNumber, cardObj);

		Card card = m_cards [srcSlotNumber];
		m_cards.RemoveAt (srcSlotNumber);
		m_cards.Insert (destSlotNumber, card);

		int i = 0;
		foreach (Transform child in transform) {
			GameObject childObj = child.gameObject;
			cardObjs [i].transform.SetParent (childObj.transform, false);
			Vector3 vec3 = cardObjs [i].GetComponent<RectTransform> ().sizeDelta;
			cardObjs [i].GetComponent<RectTransform> ().anchoredPosition = new Vector3 (vec3.x / 2, -vec3.y / 2, 0);;
			i++;
		}
	}
}
