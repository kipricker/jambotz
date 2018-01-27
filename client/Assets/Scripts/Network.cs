using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
struct JoinResponse {
	public string playerID;
	public string gameID;
}

[System.Serializable]
struct GameAction {
	public int move;
}

public class Network : MonoBehaviour {
	bool waiting = false;
	bool active = false;

	int lastSeen = -1;

	JoinResponse joinResponse = new JoinResponse();

	string host = "localhost:3000";

	Queue<GameAction> gameActionQueue = new Queue<GameAction>();

	void Start () {
		joinLobby();
//		StartCoroutine (heartbeat());
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
			joinResponse = JsonUtility.FromJson<JoinResponse> (www.downloadHandler.text);
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
				string json = www.downloadHandler.text;
				json = json.Substring (1, json.Length - 2);
				if (json.Length > 0) {
					string[] objects = json.Split (',');
					foreach (string obj in objects) {
						GameAction gameAction = JsonUtility.FromJson<GameAction> (obj);
						lastSeen++;
						gameActionQueue.Enqueue (gameAction);
					}
				}
			}

			yield return new WaitForSeconds(1);
		}
	}
}
