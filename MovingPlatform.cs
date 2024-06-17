using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Set in Inspector")]
    // Platform speed
    public float speed = 1f;

    // Platform boundaries
    public float leftEdge = -20f;  // left bound
    public float rightEdge = 20f;  // right bound

    // Chance to change directions
    public float chanceToChangeDirections = 0.1f;

    void Update()
    {
        // Basic movement
        Vector3 pos = transform.position;
        pos.x += speed * Time.deltaTime;
        transform.position = pos;

        // Changing direction based on independent left and right bounds
        if (pos.x < leftEdge)
        {
            speed = Mathf.Abs(speed); // Move right
        }
        else if (pos.x > rightEdge)
        {
            speed = -Mathf.Abs(speed); // Move left
        }
    }

    void FixedUpdate()
    {
        // Randomly change direction
        if (Random.value < chanceToChangeDirections)
        {
            speed *= -1; // Change direction
        }
    }
}