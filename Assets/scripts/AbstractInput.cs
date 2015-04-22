using UnityEngine;
using System.Collections;

public abstract class AbstractInput : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    abstract public Vector2 getDirection();
}
