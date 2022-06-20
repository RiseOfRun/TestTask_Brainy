using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    public Logger PlayerLogger;
    private Character character;
    private Vector2 movement;
    private float angle;
    private bool shoot;

    void Start()
    {
        character = GetComponent<Character>();
    }

    void Update()
    {
        movement = Vector2.zero;
        angle = 0;
        if (GameController.Instance.State != GameController.GameStates.RoundIn)
        {
            return;
        }

        movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (Input.GetKey(KeyCode.Space))
        {
            shoot = true;
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
            if (!PlayerLogger.PerformedActions.Contains(Logger.Actions.Rotate))
            {
                PlayerLogger.PerformedActions.Add(Logger.Actions.Rotate);
            }
        }

        if (movement != Vector2.zero)
        {
            character.Move(movement);
            if (!PlayerLogger.PerformedActions.Contains(Logger.Actions.Move))
            {
                PlayerLogger.PerformedActions.Add(Logger.Actions.Move);
            }
        }

        if (shoot)
        {
            if (character.CanAttack)
            {
                PlayerLogger.PerformedActions.Add(Logger.Actions.Shoot);
            }

            character.Attack();
            shoot = false;
        }
    }
}