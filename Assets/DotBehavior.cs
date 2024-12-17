using System.Collections;
using UnityEngine;

public class DotBehavior : MonoBehaviour
{
    public enum DotState { Idle, Walking, Sick, Dead }
    public DotState currentState = DotState.Idle;

    public float moveSpeed = 3f;
    public float standTimeMin = 1f;
    public float standTimeMax = 4f;
    public float infectionRadius = 1.5f; // Radius to check for nearby dots
    public float deathTimer = 10f; // Time until dot dies after becoming dead

    private Vector3 targetPosition;

    void Start()
    {
        SetNewRandomTarget();
        StartCoroutine(RandomMovement());
    }

    void Update()
    {
        switch (currentState)
        {
            case DotState.Idle:
            case DotState.Sick:
                // Stay idle or sick and try to infect nearby dots
                CheckForInfection();
                break;

            case DotState.Walking:
                // Walk towards the target position
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                if (transform.position == targetPosition)
                {
                    SetNewRandomTarget();
                    StartCoroutine(StandStillAndMove());
                }
                break;

            case DotState.Dead:
                // If dead, stop movement and after 10 seconds, lose infection ability
                transform.position = transform.position; // Don't move
                break;
        }
    }

    // Set a new random target position within the map bounds
    void SetNewRandomTarget()
    {
        float randomX = Random.Range(-8.5f, 8.5f); // Map bounds
        float randomZ = Random.Range(-4.5f, 4.5f); // Map bounds
        targetPosition = new Vector3(randomX, 0f, randomZ);
    }

    // Stand still for a random time between standTimeMin and standTimeMax
    IEnumerator StandStillAndMove()
    {
        currentState = DotState.Idle;
        float standTime = Random.Range(standTimeMin, standTimeMax);
        yield return new WaitForSeconds(standTime);

        currentState = DotState.Walking;
        SetNewRandomTarget();
    }

    // Check for nearby dots and attempt to infect if necessary
    void CheckForInfection()
    {
        Collider[] nearbyDots = Physics.OverlapSphere(transform.position, infectionRadius);
        foreach (Collider col in nearbyDots)
        {
            DotBehavior otherDot = col.GetComponent<DotBehavior>();
            if (otherDot != null && otherDot != this)
            {
                if ((otherDot.currentState == DotState.Sick || otherDot.currentState == DotState.Idle) && currentState != DotState.Dead)
                {
                    TryInfect();
                }
            }
        }
    }

    // Try to infect this dot with a random chance
    void TryInfect()
    {
        if (currentState == DotState.Idle || currentState == DotState.Sick)
        {
            int roll = Random.Range(0, 100);
            if (roll < 10) // 1-10: infected
            {
                currentState = DotState.Sick;
                Debug.Log("Dot got infected!");
            }
            else if (roll > 90) // 90-99: immune
            {
                currentState = DotState.Idle;
                Debug.Log("Dot became immune!");
            }
        }
    }

    // Trigger death state for the dot
    public void Die()
    {
        currentState = DotState.Dead;
        Debug.Log("Dot has died!");
        StartCoroutine(DeathCountdown());
    }

    // After 10 seconds in the Dead state, stop infecting others
    IEnumerator DeathCountdown()
    {
        yield return new WaitForSeconds(deathTimer);
        Debug.Log("Dot can no longer infect others.");
    }

    // Randomly move the dot around
    IEnumerator RandomMovement()
    {
        currentState = DotState.Walking;
        SetNewRandomTarget();

        // Continuously move towards random targets
        while (currentState == DotState.Walking)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            if (transform.position == targetPosition)
            {
                SetNewRandomTarget();
                yield return new WaitForSeconds(Random.Range(standTimeMin, standTimeMax)); // Wait before moving again
            }
            yield return null; // Wait for the next frame
        }
    }
}
