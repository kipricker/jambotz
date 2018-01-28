using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardBehaviour : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void setCard(string name) {
		Text[] texts = gameObject.GetComponentsInChildren<Text>();
		texts [0].text = name;
	}
}
