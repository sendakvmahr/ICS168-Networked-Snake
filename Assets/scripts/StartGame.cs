using System.Collections;
using UnityEngine;
using System;
using System.Text;

public class StartGame : MonoBehaviour {

	private string username = "";
	private string password = "";
	private string IP = "";
	public void ChangeToGame () {
		string status = AsynchronousClient.StartClient (username, password);
		Debug.Log (" *** " + status);
		if (status != "Incorrect") {
			Application.LoadLevel ("MainScene");
		} 
		else {
		}
	}

	public void SetUsername(string un) {
		Debug.Log (un);
		username = un;
	}

	public void SetPassword(string pw) {
		Debug.Log (pw);
		password = pw;
	}

	public void SetIP(string ip) {
		Debug.Log (ip);
		IP = ip;
	}
}
		