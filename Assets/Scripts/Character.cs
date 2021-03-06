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

    [HideInInspector] public Rigidbody2D rb;
    private float timeToAttack;
    private Vector2 defaultPosition;
    private Collider2D collider;
    private Camera camera;

    private void OnRoundStart()
    {
        transform.position = defaultPosition;
    }

    private void Start()
    {
        camera = Camera.main;
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
        Vector2 nextPosition =
            (Vector2) transform.position + direction.normalized * (MovementSpeed * Time.fixedDeltaTime);
        Vector2 viewPort = camera.WorldToViewportPoint(nextPosition);
        if (viewPort.x < 0 || viewPort.x > 1)
        {
            nextPosition.x = transform.position.x;
        }

        if (viewPort.y < 0 || viewPort.y > 1)
        {
            nextPosition.y = transform.position.y;
        }

        rb.MovePosition(nextPosition);
    }

    public void Rotate(float angle)
    {
        float maxAngle = RotationSpeed * Time.fixedDeltaTime * 180;
        if (maxAngle < Mathf.Abs(angle))
        {
            angle = Mathf.Sign(angle) * maxAngle;
        }

        rb.MoveRotation(rb.rotation + angle);
    }


    public void Attack()
    {
        if (timeToAttack <= 0)
        {
            var bullet = Instantiate(Bullet, SourcePosition.transform.position, transform.rotation);
            var bulletCollider = bullet.GetComponent<Collider2D>();
            Physics2D.IgnoreCollision(bulletCollider, collider);
            timeToAttack = 1 / AttackPerSecornd;
        }
    }
}