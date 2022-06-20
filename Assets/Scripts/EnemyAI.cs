using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public int RayCount;
    public int PredictionDepth = 1;
    public float PlayerDetectionDeviation = 10;
    public GameObject Waypoints;

    private readonly List<Ray2D> rays = new List<Ray2D>();
    private Character controlledCharacter;
    private Character enemyCharacter;
    private Transform target;
    private Statements currentState = Statements.Seek;
    private readonly List<Transform> waypoints = new List<Transform>();

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
            if (p != controlledCharacter)
            {
                enemyCharacter = p;
                break;
            }
        }

        foreach (Transform waypoint in Waypoints.transform)
        {
            waypoints.Add(waypoint);
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

    void GetWaypoint()
    {
        if (target == null || Vector2.Distance(transform.position, target.position) <=
            controlledCharacter.MovementSpeed * Time.fixedDeltaTime || Random.Range(0f, 1f) < 0.2f / 30)
        {
            for (int i = 0; i < waypoints.Count; i++)
            {
                int point = Random.Range(0, waypoints.Count);
                (waypoints[i], waypoints[point]) = (waypoints[point], waypoints[i]);
            }

            foreach (var waypoint in waypoints)
            {
                if (!Physics2D.Raycast(transform.position, waypoint.position - transform.position,
                    Vector2.Distance(waypoint.position, transform.position), LayerMask.GetMask("Wall")))
                {
                    target = waypoint;
                    break;
                }
            }
        }
    }

    void Move()
    {
        GetWaypoint();
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
        for (int i = 0; i < predictionDepth; i++)
        {
            RaycastHit2D hit =
                Physics2D.Raycast(origin, direction, float.MaxValue, LayerMask.GetMask("Wall", "Player"));
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