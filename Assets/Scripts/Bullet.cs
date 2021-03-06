using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float Speed = 5f;
    public int BounceCount = 3;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.AddForce(transform.rotation * Vector2.up * Speed, ForceMode2D.Impulse);
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        Character p = other.gameObject.GetComponent<Character>();
        if (p != null)
        {
            p.GetHit();
            GameController.Instance.PlayerHit(p);
            Destroy(gameObject);
            return;
        }

        if (BounceCount == 0)
        {
            Destroy(gameObject);
            return;
        }

        BounceCount -= 1;
        ContactPoint2D contact = other.contacts[0];
        Vector3 direction = transform.rotation * Vector2.up;
        Vector2 reflection = Vector2.Reflect(direction, contact.normal);
        if (rb == null) return;
        rb.velocity = Vector2.zero;
        rb.AddForce(reflection.normalized * Speed, ForceMode2D.Impulse);
        float angle = -Vector2.SignedAngle(reflection, Vector2.up);
        rb.rotation = angle;
    }
}