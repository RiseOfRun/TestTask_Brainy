using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Player player;

    private Vector2 movement;
    private float angle;
    void Start()
    {
        player = GetComponent<Player>();
    }

    void Update()
    {
        if (GameController.Instance.State!=GameController.GameStates.RoundIn)
        {
            return;
        }
        angle = Input.GetAxisRaw("Horizontal")*180;
        movement = new Vector2(0,Input.GetAxisRaw("Vertical"));
        if (Input.GetKey(KeyCode.Space))
        {
            player.Attack();
        }
        
    }

    private void FixedUpdate()
    {
        if (angle != 0)
        {
            player.Rotate(angle);
        }
        if (movement !=Vector2.zero)
        {
            player.Move(player.transform.rotation*movement);
        }

        
    }
}
