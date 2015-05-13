// Imported by default
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// JSON handling
using Newtonsoft.Json;

using UnityEngine;

[Serializable]
public class Coordinate
{
	public int x;
	public int y;
	public Coordinate(int x_coordinate, int y_coordinate)
	{
		x = x_coordinate;
		y = y_coordinate;
	}
}


public class PlayerSnake
{
	public string playerName;
	public Coordinate head;
	public List<Coordinate> tail = new List<Coordinate>();
	public string direction;
	public PlayerSnake(string name, Coordinate c, string d)
	{
		playerName = name;
		head = c;
		tail = new List<Coordinate>();
		direction = d;
	}
}

public class GameState
{
	public List<PlayerSnake> snakes;
	public List<Coordinate> food;
	public bool finished = false;
	public string currentPlayer;

	public GameState(string player) {
		snakes = new List<PlayerSnake>();
		food = new List<Coordinate> ();
		currentPlayer = player;
		food.Add(new Coordinate(0,0));
	}

	public GameState() {
		snakes = new List<PlayerSnake>();
		food = new List<Coordinate> ();
		currentPlayer = "";
		food.Add(new Coordinate(0,0));
	}

	public void update(string json) {
		Debug.Log ("in update");
		var newgame = JsonConvert.DeserializeObject<GameState> (json);
		if (newgame != null) {
			food = newgame.food;
			snakes = newgame.snakes;
			//Debug.Log (snakes);
		}
		Debug.Log ("exit update");
	}

	public string ToJSON()
	{
		return JsonConvert.SerializeObject(this);
	}
}

