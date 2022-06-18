using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float Speed = 5f;
    public int BounceCount = 3;
    private Rigidbody2D rb;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.AddForce(transform.rotation*Vector2.up*Speed,ForceMode2D.Impulse);
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        Player p = other.gameObject.GetComponent<Player>();
        if (p!=null)
        {
            GameController.Instance.PlayerHit(p);
            Destroy(gameObject);
            return;
        }
        
        if (BounceCount==0)
        {
            Destroy(gameObject);
            return;
        }
        BounceCount -= 1;
        var contact = other.contacts[0];
        var direction = transform.rotation * Vector2.up;
        var reflection = Vector2.Reflect(direction,contact.normal);
        rb.velocity = Vector2.zero;
        rb.AddForce(reflection.normalized*Speed,ForceMode2D.Impulse);
        float angle = -Vector2.SignedAngle(reflection,Vector2.up);
        rb.rotation = angle;
    }
}
