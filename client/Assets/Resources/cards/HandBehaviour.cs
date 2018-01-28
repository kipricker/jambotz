using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandBehaviour : MonoBehaviour {

	List<string> cards = new List<string> (new string[] {
		"1",
		"2",
		"3",
		"4",
		"5",
	});

	List<GameObject> cardObjs = new List<GameObject>();

	// Use this for initialization
	void Start () {
		int cardNumber = 0;
//		GameObject tile = Instantiate(Resources.Load("cards/Card") as GameObject);
		foreach (Transform child in transform) {
			GameObject childObj = child.gameObject;
			GameObject card = Instantiate(Resources.Load("cards/Card") as GameObject);
			card.transform.SetParent(childObj.transform, false);
			card.transform.localPosition = Vector3.zero;
			cardObjs.Add (card);

			childObj.SendMessage ("setSlotNumber", cardNumber);
			card.SendMessage ("setCard", cards[cardNumber]);
			cardNumber++;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnItemPlace(DragAndDropCell.DropDescriptor desc)
	{
		int srcSlotNumber = desc.sourceCell.slotNumber;
		int destSlotNumber = desc.destinationCell.slotNumber;
		Debug.Log (destSlotNumber);

		GameObject cardObj = cardObjs [srcSlotNumber];
		cardObjs.RemoveAt (srcSlotNumber);
		cardObjs.Insert (destSlotNumber, cardObj);

		string card = cards [srcSlotNumber];
		cards.RemoveAt (srcSlotNumber);
		cards.Insert (destSlotNumber, card);

		int i = 0;
		foreach (Transform child in transform) {
			GameObject childObj = child.gameObject;
			cardObjs [i].transform.SetParent (childObj.transform, false);
			cardObjs [i].transform.localPosition = Vector3.zero;
			i++;
		}
	}
}
