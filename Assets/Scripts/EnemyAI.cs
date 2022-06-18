using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyAI : MonoBehaviour
{
    enum Statements
    {
        Seek,
        Dodge,
        Soot
    }
    public Transform[] Waypoints;
    public int RayCount;
    public int PredictionDepth = 1;

    private List<Ray2D> rays = new List<Ray2D>();
    private Player controlledPlayer;
    private Transform target;
    private Statements currentState = Statements.Seek;

    void Start()
    {
        controlledPlayer = GetComponent<Player>();
        float sector = 360f/RayCount;
        for (int i = 0; i < RayCount; i++)
        {
            Vector2 direction = Quaternion.Euler(0, 0, sector * i) * Vector2.up;
            Ray2D newRay = new Ray2D(transform.position,direction);
            rays.Add(newRay);
        }
    }

    void Update()
    {
        foreach (var ray in rays)
        {
            Debug.DrawRay((Vector2)transform.position,ray.direction*100,Color.black);
        }
    }

    private void FixedUpdate()
    {
        if (GameController.Instance.State!=GameController.GameStates.RoundIn)
        {
            return;
        }
        switch (currentState)
        {
            case Statements.Seek:
                Seek();
                break;
            case Statements.Dodge:
                break;
            case Statements.Soot:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    void Seek()
    {
        CheckEnemy();
        Move();
    }

    void Move()
    {
        if (target == null || Vector2.Distance(transform.position, target.position) <=
            controlledPlayer.MovementSpeed * Time.deltaTime || Random.Range(0f,1f)<0.2f/30)
        {
            target = Waypoints[Random.Range(0, Waypoints.Length)];
        }
        controlledPlayer.Move((target.position - transform.position).normalized);
        controlledPlayer.RotateTo(target.transform.position);
    }
    void CheckEnemy()
    {
        
    }
}
