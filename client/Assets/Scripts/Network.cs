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

[System.Serializable]
struct GameUpdate {
	public bool exists;
	public bool active;
	public GameAction[] latestActions;
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
				GameUpdate gameUpdate = JsonUtility.FromJson<GameUpdate> (json);
				foreach (GameAction gameAction in gameUpdate.latestActions) {
					gameActionQueue.Enqueue (gameAction);
					lastSeen++;
				}
			}

			yield return new WaitForSeconds(1);
		}
	}
}
