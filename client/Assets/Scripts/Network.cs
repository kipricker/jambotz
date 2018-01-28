﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
struct JoinResponse {
	public string playerID;
	public string gameID;
}

[System.Serializable]
struct PlayedHand {
	public int playerNumber;
	public Card[] hand;
}

[System.Serializable]
struct GameAction {
	public bool gameStarted;
	public bool handsDealt;
	public bool player_cards_sent;
	public PlayedHand player_played_hand;
}

[System.Serializable]
struct GameUpdate {
	public bool exists;
	public bool active;
	public GameAction[] latestActions;
}

public class Network : MonoBehaviour {
	public GameObject m_game;

	bool m_active = false;
	int m_lastSeen = -1;

	JoinResponse m_joinResponse = new JoinResponse();

	string m_host = "localhost:3000";

	Queue<GameAction> m_gameActionQueue = new Queue<GameAction>();

	void Start () {
		joinLobby();
	}

	public void joinLobby() {
		StartCoroutine (iJoinLobby());
	}

	public void sendCards() {
		
	}

	public void playHand() {

	}

	IEnumerator iGetHand()
	{
		WWWForm form = new WWWForm();
		form.AddField("playerID", m_joinResponse.playerID);
		form.AddField("gameID", m_joinResponse.gameID);

		UnityWebRequest www = UnityWebRequest.Post (m_host + "/game/get-hand", form);
		yield return www.SendWebRequest();

		if (www.isNetworkError || www.isHttpError) {
			Debug.Log (www.error);
		} else {
			Debug.Log (www.downloadHandler.text);
			Game game = m_game.GetComponent<Game>();
			HandBehaviour hand = game.m_player_hand.GetComponent<HandBehaviour> ();
			hand.SetHand(JsonUtility.FromJson<PlayedHand> (www.downloadHandler.text).hand);
		}
	}

	IEnumerator iJoinLobby()
	{
		WWWForm form = new WWWForm();
		UnityWebRequest www = UnityWebRequest.Post (m_host + "/lobby/join", form);
		yield return www.SendWebRequest();

		if (www.isNetworkError || www.isHttpError) {
			Debug.Log (www.error);
		} else {
			m_joinResponse = JsonUtility.FromJson<JoinResponse> (www.downloadHandler.text);
			StartCoroutine (heartbeat());
		}
	}

	IEnumerator heartbeat()
	{
		while (true) {
			WWWForm form = new WWWForm();
			form.AddField("lastSeen", m_lastSeen);
			form.AddField("playerID", m_joinResponse.playerID);
			form.AddField("gameID", m_joinResponse.gameID);

			UnityWebRequest www = UnityWebRequest.Post(m_host + "/game/heartbeat", form);
			yield return www.SendWebRequest();

			if (www.isNetworkError || www.isHttpError) {
				Debug.Log (www.error);
			} else {
				string json = www.downloadHandler.text;
				GameUpdate gameUpdate = JsonUtility.FromJson<GameUpdate> (json);
				if (!m_active && gameUpdate.active) {
					m_active = true;
					yield return iGetHand ();
				}

				foreach (GameAction gameAction in gameUpdate.latestActions) {
					m_gameActionQueue.Enqueue (gameAction);
					m_lastSeen++;
				}
			}

			yield return new WaitForSeconds(1);
		}
	}
}
