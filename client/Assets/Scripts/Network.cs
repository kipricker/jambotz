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
struct GameState {
	
}

public class Network : MonoBehaviour {
	bool waiting = false;
	bool active = false;

	int lastSeen = -1;

	JoinResponse joinResponse = new JoinResponse();

	string host = "localhost:3000";

	Queue transactionQueue = new Queue();

	void Start () {
//		joinLobby();
//		StartCoroutine (heartbeat());
	}
	
	void Update () {
		
	}

	public void joinLobby() {
		StartCoroutine (iJoinLobby());
	}

	IEnumerator iJoinLobby()
	{
		WWWForm form = new WWWForm();
		form.AddField("data", "");

		UnityWebRequest www = UnityWebRequest.Post (host + "/lobby/join", form);
		yield return www.SendWebRequest();

		if (www.isNetworkError || www.isHttpError) {
			Debug.Log (www.error);
		} else {
			// TODO: deserialize JSON to object and enqueue
			Debug.Log (www.downloadHandler.text);
			joinResponse = JsonUtility.FromJson<JoinResponse> (www.downloadHandler.text);
			Debug.Log (joinResponse.gameID);
//			transactionQueue.Enqueue("Hello");

			StartCoroutine (heartbeat());
		}
	}

	IEnumerator heartbeat()
	{
		while (true) {
			WWWForm form = new WWWForm();
			form.AddField("lastSeen", lastSeen);
			form.AddField("playerID", joinResponse.playerID);
			form.AddField("gameID", joinResponse.gameID);

			UnityWebRequest www = UnityWebRequest.Post(host + "/game/heartbeat", form);
			yield return www.SendWebRequest();

			if (www.isNetworkError || www.isHttpError) {
				Debug.Log (www.error);
			} else {
				// TODO: deserialize JSON to object and enqueue
				Debug.Log (www.downloadHandler.text);
//				transactionQueue.Enqueue("Hello");
			}

			yield return new WaitForSeconds(1);
		}
	}
}