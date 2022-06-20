using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Character character;

    private Vector2 movement;
    private float angle;
    void Start()
    {
        character = GetComponent<Character>();
    }

    void Update()
    {
        movement = Vector2.zero;
        angle = 0;
        if (GameController.Instance.State!=GameController.GameStates.RoundIn)
        {
            return;
        }
        movement = new Vector2(Input.GetAxisRaw("Horizontal"),Input.GetAxisRaw("Vertical"));
        if (Input.GetKey(KeyCode.Space))
        {
            character.Attack();
        }
        if (Input.GetKey(KeyCode.E))
        {
            angle = -180;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            angle = 180;
        }
        
    }

    private void FixedUpdate()
    {
        if (angle != 0)
        {
            character.Rotate(angle);
        }
        if (movement !=Vector2.zero)
        {
            character.Move(movement);
        }

        
    }
}
