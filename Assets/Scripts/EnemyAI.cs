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
        Aim,
        Move,
        Soot
    }

    public Transform[] Waypoints;
    public int RayCount;
    public int PredictionDepth = 1;
    public float PlayerDetectionDeviation = 10;

    private readonly List<Ray2D> rays = new List<Ray2D>();
    private Character controlledCharacter;
    private Character enemyCharacter;
    private Transform target;
    private Statements currentState = Statements.Seek;

    private Vector2 lastDetectionPosition;
    private Vector2 attackDirection;
    private Vector2 movement;
    private float angle;
    private bool shoot;

    void Start()
    {
        controlledCharacter = GetComponent<Character>();
        Character[] players = FindObjectsOfType<Character>();
        foreach (Character p in players)
        {
            if (p != this)
            {
                enemyCharacter = p;
                break;
            }
        }

        float sector = 360f / RayCount;
        for (int i = 0; i < RayCount; i++)
        {
            Vector2 direction = Quaternion.Euler(0, 0, sector * i) * Vector2.up;
            Ray2D newRay = new Ray2D(transform.position, direction);
            rays.Add(newRay);
        }

        GameController.Instance.StartRound += OnRoundStart;
    }

    void Update()
    {
        movement = Vector2.zero;
        angle = 0f;

        foreach (var ray in rays)
        {
            Debug.DrawRay((Vector2) transform.position, ray.direction * 100, Color.black);
        }

        if (GameController.Instance.State != GameController.GameStates.RoundIn)
        {
            return;
        }

        if (currentState == Statements.Soot)
        {
            if (Vector2.Distance(lastDetectionPosition, enemyCharacter.transform.position) >
                PlayerDetectionDeviation)
            {
                currentState = Statements.Seek;
                shoot = false;
                return;
            }
        }

        if (currentState == Statements.Seek)
        {
            if (!controlledCharacter.CanAttack)
            {
                currentState = Statements.Move;
            }
            else
            {
                Seek();
            }
        }

        if (currentState == Statements.Move)
        {
            Move();
            currentState = Statements.Seek;
            return;
        }

        if (currentState == Statements.Aim)
        {
            angle = Vector2.SignedAngle(controlledCharacter.transform.rotation * Vector2.up, attackDirection -
                (Vector2) controlledCharacter.transform.position);
            if (Mathf.Abs(angle) <= controlledCharacter.RotationSpeed * Time.fixedDeltaTime)
            {
                currentState = Statements.Soot;
                shoot = true;
            }
        }
    }

    private void FixedUpdate()
    {
        if (movement != Vector2.zero)
        {
            controlledCharacter.Move(movement);
        }

        if (angle != 0)
        {
            controlledCharacter.Rotate(angle);
        }

        if (shoot)
        {
            controlledCharacter.Attack();
            currentState = Statements.Seek;
            shoot = false;
        }
    }

    void Seek()
    {
        CheckEnemy();
    }

    void Move()
    {
        if (target == null || Vector2.Distance(transform.position, target.position) <=
            controlledCharacter.MovementSpeed * Time.fixedDeltaTime || Random.Range(0f, 1f) < 0.2f / 30)
        {
            target = Waypoints[Random.Range(0, Waypoints.Length)];
        }

        movement = target.position - transform.position;
        var transform1 = controlledCharacter.transform;
        float a = Vector2.SignedAngle(transform1.rotation * Vector2.up,
            target.transform.position - transform1.position);
        if (Mathf.Abs(a) > Mathf.Abs(a) * controlledCharacter.RotationSpeed * Time.fixedDeltaTime)
        {
            angle = a;
        }
    }

    void CheckEnemy()
    {
        attackDirection = Vector2.zero;
        Vector2 origin = controlledCharacter.SourcePosition.transform.position;
        Vector2 direction = (Vector2) enemyCharacter.transform.position - origin;
        Ray2D r = new Ray2D(origin, direction);
        if (CheckDirection(r))
        {
            currentState = Statements.Aim;
            attackDirection = enemyCharacter.transform.position;
            lastDetectionPosition = enemyCharacter.transform.position;
            return;
        }

        foreach (var ray in rays)
        {
            if (CheckDirection(ray, PredictionDepth))
            {
                currentState = Statements.Aim;
                attackDirection = (Vector2) transform.position + ray.direction;
                lastDetectionPosition = enemyCharacter.transform.position;
                return;
            }
        }

        currentState = Statements.Move;
    }

    bool CheckDirection(Ray2D r, int predictionDepth = 1)
    {
        Vector2 origin = transform.position;
        Vector2 direction = r.direction.normalized;
        RaycastHit2D hit;
        for (int i = 0; i < predictionDepth; i++)
        {
            hit = Physics2D.Raycast(origin, direction, float.MaxValue, LayerMask.GetMask("Wall", "Player"));
            if (hit.collider != null)
            {
                if (i != 0 || predictionDepth == 1)
                {
                    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Player") &&
                        hit.collider.transform.parent.gameObject != gameObject)
                    {
                        return true;
                    }

                    Debug.DrawRay(origin, direction * 3, Color.magenta);
                }

                var reflection = Vector2.Reflect(hit.point - origin, hit.normal).normalized;
                origin = hit.point;
                direction = reflection;
            }
            else
            {
                return false;
            }
        }

        return false;
    }

    private void OnRoundStart()
    {
        target = null;
    }
}