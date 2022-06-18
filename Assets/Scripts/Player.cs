using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float MovementSpeed = 5f;
    public float RotationSpeed = 5f;
    public float AttackPerSecornd = 1f;
    
    public GameObject Bullet;
    public Rigidbody2D rb;
    public GameObject SourcePosition;
    public int Score; 
    private float timeToAttack = 0;
    private Vector2 defaultPosition;

    private void OnRoundStart()
    {
        transform.position = defaultPosition;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        defaultPosition = transform.position;
        GameController.Instance.StartRound += OnRoundStart;
    }

    private void Update()
    {
        Aim();
        if (timeToAttack > 0)
        {
            timeToAttack -= Time.deltaTime;
        }
    }

    public void Move(Vector2 direction)
    {
        var transform1 = transform;
        rb.MovePosition((Vector2) transform1.position + direction * (MovementSpeed * Time.fixedDeltaTime));
    }

    public void Rotate(float angle)
    {
        rb.MoveRotation(rb.rotation + angle * -RotationSpeed * Time.deltaTime);
    }

    public void RotateTo(Vector2 target)
    {
        float angle = -Vector2.SignedAngle((target - (Vector2) transform.position).normalized,
            (transform.rotation * Vector2.up).normalized);
        float MaxAngle = RotationSpeed * Time.deltaTime * 180;
        if (MaxAngle < Mathf.Abs(angle))
        {
            angle = Mathf.Sign(angle) * MaxAngle;
        }

        rb.MoveRotation(rb.rotation + angle);
    }

    private void Aim()
    {    
        var rotation = transform.rotation;
        Vector2 origin = transform.position + rotation * Vector2.up;
        var hit  = Physics2D.Raycast(origin,rotation*Vector2.up,float.MaxValue);
        if (hit!=null && hit.collider!=null)
        {
            Debug.DrawLine(origin,hit.point,Color.green);
            Vector2 reflection = Vector2.Reflect(hit.point-origin, hit.normal);
            Debug.DrawRay(hit.point,reflection.normalized*3,Color.red);
        }
    }
    public void Attack()
    {
        if (timeToAttack <= 0)
        {
            Instantiate(Bullet, SourcePosition.transform.position, transform.rotation);
            timeToAttack = 1 / AttackPerSecornd;
        }
    }
}