using UnityEngine;

/// <summary>
/// This makes an object move randomly in a set of directions, with some random time delay in between each decision
/// </summary>
public class Wanderer : MonoBehaviour
{
    Animator animator;
    Meeple meeple;
    Rigidbody2D rigidBody;

    // A minimum and maximum time delay for taking a decision, choosing a direction to move in
    public Vector2 decisionTime = new Vector2(1, 4);
    internal float decisionTimeCount = 0;

    // The possible directions that the object can move int, right, left, up, down, and zero for staying in place. I added zero twice to give a bigger chance if it happening than other directions
    internal Vector2[] moveDirections = new Vector2[] { Vector2.right, Vector2.left, Vector2.up, Vector2.down, Vector2.zero, Vector2.zero };
    internal int currentMoveDirection;

    // Use this for initialization
    void Start()
    {
        animator = GetComponent<Animator>();
        meeple = GetComponent<Meeple>();
        rigidBody = GetComponent<Rigidbody2D>();

        // Set a random time delay for taking a decision ( changing direction, or standing in place for a while )
        decisionTimeCount = Random.Range(decisionTime.x, decisionTime.y);

        // Choose a movement direction, or stay in place
        ChooseMoveDirection();
    }

    void FixedUpdate()
    {
        // Move the object in the chosen direction at the set speed
        Vector2 direction = moveDirections[currentMoveDirection];
        float xDir = direction.x;
        float yDir = direction.y;

        Vector2 position = rigidBody.position;
        position += meeple.Speed * Time.deltaTime * direction;
        rigidBody.MovePosition(position);

        if (animator)
        {
            animator.SetFloat("MoveX", xDir * meeple.Speed);
            animator.SetFloat("MoveY", yDir * meeple.Speed);
        }

        if (decisionTimeCount > 0) decisionTimeCount -= Time.deltaTime;
        else
        {
            // Choose a random time delay for taking a decision ( changing direction, or standing in place for a while )
            decisionTimeCount = Random.Range(decisionTime.x, decisionTime.y);

            // Choose a movement direction, or stay in place
            ChooseMoveDirection();
        }
    }

    void ChooseMoveDirection()
    {
        // Choose whether to move sideways or up/down
        currentMoveDirection = Mathf.FloorToInt(Random.Range(0, moveDirections.Length));
    }
}