using UnityEngine;

public class ForceField : MonoBehaviour
{
    [SerializeField] private Vector2 forceDirection = Vector2.right;
    [SerializeField] private float forceMagnitude = 300f;

    private Rigidbody2D landerRb;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Lander _))
        {
            landerRb = other.GetComponent<Rigidbody2D>();
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // Continuously apply force while inside
        if (landerRb != null)
        {
            landerRb.AddForce(forceDirection.normalized * forceMagnitude * Time.fixedDeltaTime);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out Lander _))
        {
            landerRb = null;
        }
    }

    // Draws the force direction in the Scene view for easy setup
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, forceDirection.normalized * 2f);
    }
}