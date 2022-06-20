using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Character : MonoBehaviour
{
    public float MovementSpeed = 5f;
    public float RotationSpeed = 5f;
    public float AttackPerSecornd = 1f;
    public bool CanAttack => timeToAttack <= 0;
    public GameObject Bullet;
    public GameObject SourcePosition;
    public int Score;
    
    [HideInInspector]public Rigidbody2D rb;
    private float timeToAttack = 0;
    private Vector2 defaultPosition;
    private Collider2D collider;

    private void OnRoundStart()
    {
        transform.position = defaultPosition;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        defaultPosition = transform.position;
        collider = GetComponentInChildren<Collider2D>();
        GameController.Instance.StartRound += OnRoundStart;
    }

    public void GetHit()
    {
        if (GameController.Instance.State == GameController.GameStates.RoundIn)
        {
            gameObject.SetActive(false);
        }
    }
    private void Update()
    {
        if (timeToAttack > 0)
        {
            timeToAttack -= Time.deltaTime;
        }
    }

    public void Move(Vector2 direction)
    {
        rb.MovePosition((Vector2) transform.position + direction.normalized * (MovementSpeed * Time.fixedDeltaTime));
    }

    public void Rotate(float angle)
    {
        float MaxAngle = RotationSpeed * Time.fixedDeltaTime * 180;
        if (MaxAngle < Mathf.Abs(angle))
        {
            angle = Mathf.Sign(angle) * MaxAngle;
        }
        rb.MoveRotation(rb.rotation + angle);
    }

    public void RotateTo(Vector2 target)
    {
        float angle = -Vector2.SignedAngle((target - (Vector2) transform.position).normalized,
            (transform.rotation * Vector2.up).normalized);
        float MaxAngle = RotationSpeed * Time.fixedDeltaTime * 180;
        if (MaxAngle < Mathf.Abs(angle))
        {
            angle = Mathf.Sign(angle) * MaxAngle;
        }
        rb.MoveRotation(rb.rotation + angle);
    }

    
    public void Attack()
    {
        if (timeToAttack <= 0)
        {
            var bullet = Instantiate(Bullet, SourcePosition.transform.position, transform.rotation);
            var bulletCollider = bullet.GetComponent<Collider2D>();
            Physics2D.IgnoreCollision(bulletCollider,collider);
            timeToAttack = 1 / AttackPerSecornd;
        }
    }
}