using UnityEngine;

public class Durability : MonoBehaviour
{
    public int durability = 1; // Default durability value

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the collided object has the tag "Ball"
        if (collision.gameObject.CompareTag("Ball"))
        {
            // Get the Durability component of the other object (the ball)
            Durability ballDurability = collision.gameObject.GetComponent<Durability>();

            if (ballDurability != null)
            {
                // Reduce the ball's durability by 1
                ballDurability.durability -= 1;

                // If the ball's durability is 0 or less, destroy it
                if (ballDurability.durability <= 0)
                {
                    Destroy(collision.gameObject);
                }

                // Reduce self's durability by 1
                durability -= 1;

                // If self's durability is 0 or less, notify the PlaceOnPlane and destroy self
                if (durability <= 0)
                {
                    FindObjectOfType<PlaceOnPlane>().OnBlockDestroyed();
                    Destroy(gameObject);
                }
            }
        }
    }
}