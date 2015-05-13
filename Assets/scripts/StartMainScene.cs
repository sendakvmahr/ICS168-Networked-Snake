using UnityEngine;
using System.Collections;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// JSON handling
using Newtonsoft.Json;


public class StartMainScene : MonoBehaviour {
	public GameObject foodPrefab;
	public GameObject player;
	public GameObject tailPrefab;
	private GameObject food;
	public GameObject otherPlayer;
	public List<GameObject> othertail;
	void Start () {
		AsynchronousClient.gameStarted = true;
		food = (GameObject) Instantiate(foodPrefab,
		                               new Vector2(0, 0),
		                               Quaternion.identity);
		Debug.Log ("started");
	}

	// Update is called once per frame
	void Update () {
		// Destroys the food in case it moved
		UpdateFood ();
		//send json to server
		UpdatePlayerPosition ();
		UpdateEnemyPosition ();
	}

	void UpdatePlayerPosition() {
		for (int i=0; i<AsynchronousClient.gameState.snakes.Count; i++) {
			if (AsynchronousClient.gameState.snakes[i].playerName == AsynchronousClient.playername)
			{
				AsynchronousClient.gameState.snakes[i].head = new Coordinate((int)player.transform.position.x,(int)player.transform.position.y);
			}
		}
		// head is done in snake class, annoying to do it here
	}

	void UpdateEnemyPosition() {
		Destroy (otherPlayer);
		for (int i=0; i<othertail.Count; i++) {
			Destroy(othertail[i]);
		}
		for (int i=0; i<AsynchronousClient.gameState.snakes.Count; i++) {
			if (AsynchronousClient.gameState.snakes[i].playerName != AsynchronousClient.playername)
			{
				Debug.Log (AsynchronousClient.gameState.snakes[i].playerName );

				otherPlayer = (GameObject) Instantiate(tailPrefab,
				                                       new Vector2(AsynchronousClient.gameState.snakes[i].head.x, AsynchronousClient.gameState.snakes[i].head.y),
				                         Quaternion.identity);
				for (int n=0; n<AsynchronousClient.gameState.snakes[i].tail.Count; n++) {
					othertail.Add((GameObject) Instantiate(tailPrefab,
                               new Vector2(AsynchronousClient.gameState.snakes[i].tail[n].x, AsynchronousClient.gameState.snakes[i].tail[n].y),
					            Quaternion.identity));
				}
			}
		}
	}

	void UpdateFood() {
		Destroy (food);
		food = (GameObject) Instantiate(foodPrefab,
		            new Vector2(AsynchronousClient.gameState.food[0].x, AsynchronousClient.gameState.food[0].y),
		            Quaternion.identity); // default rotation
	}
}
