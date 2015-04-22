using UnityEngine;
using System.Collections;

public class KeyboardInput : AbstractInput {
    public override Vector2 getDirection()
    {
        if (Input.GetKey(KeyCode.RightArrow))
            return Vector2.right;
        else if (Input.GetKey(KeyCode.DownArrow))
            return -Vector2.up;    // '-up' means 'down'
        else if (Input.GetKey(KeyCode.LeftArrow))
            return -Vector2.right; // '-right' means 'left'
        else if (Input.GetKey(KeyCode.UpArrow))
            return Vector2.up;
        else
            return Vector2.zero;
    }
}
