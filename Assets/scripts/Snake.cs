using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Snake : MonoBehaviour {
	// Current Movement Direction
	// (by default it moves to the right)
	public Vector2 dir = Vector2.right;
    Vector2 newDir;
	// Keep Track of Tail
	public List<Transform> tail = new List<Transform>();
	// Grow in next movement?
	bool ate = false;
	bool alive = true;
	// Tail Prefab
	public GameObject tailPrefab;
	public GameObject gameOverPrefab;

    public AbstractInput inputScript;

    public float movementFrequency = 0.3f;

    float movementTimer = 0f;

	// Use this for initialization
	void Start () {
		// Move the Snake every 300ms
		//InvokeRepeating("Move", 0.3f, 0.3f);    
		//InvokeRepeating("Move", 0.1f, 0.1f);  
	}
	
	// Update is called once per frame
	void Update() {
		// Move in a new Direction?
		try{
	        Vector2 inp = inputScript.getDirection();
	        if (inp != Vector2.zero) newDir = inp;
	        
	        movementTimer += Time.deltaTime;
	        while(movementTimer >= movementFrequency)
	        {
	            Move();
	            movementTimer -= movementFrequency;
	        }
		}
		catch (System.Exception e)
		{
			Debug.Log (e.ToString ());
		}
	}

	void OnTriggerEnter2D(Collider2D coll) {
		// Food?
		if (coll.name.StartsWith("FoodPrefab")) {
			// Get longer in next Move call
			ate = true;
			
			// Remove the Food
			Destroy(coll.gameObject);
		}
		// Collided with Tail or Border
		else {
			// ToDo 'You lose' screen
			alive = false;
			Instantiate(gameOverPrefab,
			            new Vector2(0, 0),
			            Quaternion.identity);
		}
	}

	void Move() {
		if (alive) {
			// Save current position (gap will be here)
			Vector2 v = transform.position;

            if (!newDir.Equals(Vector2.zero) && !Vector2.zero.Equals(newDir + dir))
            {
                dir = newDir;
            }
            newDir = Vector2.zero;

			// Move head into new direction (now there is a gap)
			transform.Translate (dir);
			
			// Ate something? Then insert new Element into gap
			if (ate) {
				// Load Prefab into the world
				GameObject g = (GameObject)Instantiate (tailPrefab,
				                                      v,
				                                      Quaternion.identity);
				
				// Keep track of it in our tail list
				tail.Insert (0, g.transform);
				
				// Reset the flag
				ate = false;
			}
			// Do we have a Tail?
			else if (tail.Count > 0) {
				// Move last Tail Element to where the Head was
				tail.Last ().position = v;
				
				// Add to front of list, remove from the back
				tail.Insert (0, tail.Last ());
				tail.RemoveAt (tail.Count - 1);
			}
		}
		if (inputScript != null) {
			for (int i=0; i<AsynchronousClient.gameState.snakes.Count; i++) {
				if (AsynchronousClient.gameState.snakes[i].playerName == AsynchronousClient.playername)
				{
					List<Coordinate> newTail = new List<Coordinate>();
					for (int n=0; n<tail.Count(); n++) {
						newTail.Add (new Coordinate((int)tail[n].position.x, (int)tail[n].position.y));
					}
					AsynchronousClient.gameState.snakes[i].tail = newTail;
					Debug.Log (AsynchronousClient.gameState.snakes[i].tail.Count);
				}
			}
		}
	}
}