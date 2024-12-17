using UnityEngine;
using System.Collections;

public class RandomMovement : MonoBehaviour
{
    public enum State
    {
        Idle,
        Sick,
        Immune,
        Dead
    }

    public State currentState = State.Idle;

    public float idleSpeed = 3f;
    public float sickSpeed = 1.5f;
    public float immuneSpeed = 2.5f;
    private float moveSpeed;

    public Sprite IdleSprite;
    public Sprite SickSprite;
    public Sprite ImmuneSprite;
    public Sprite DeadSprite;

    private SpriteRenderer spriteRenderer;
    public float minDelay = 1f;
    public float maxDelay = 5f;
    private Vector2 targetPosition;
    private float minX = -13f;
    private float maxX = 13f;
    private float minY = -13f;
    private float maxY = 13f;
    private bool moving = false;

    private Coroutine infectionCoroutine;
    private bool infectionStarted = false;

    // Graveyard for dead players
    public Transform graveyard; // Drag your graveyard (anthill) GameObject here in the Inspector

    // Flag for dead players to prevent them from being picked up again
    private bool hasBeenPickedUp = false;

    private void Start()
    {
        // Start at a random position
        float randomX = Random.Range(minX, maxX);
        float randomY = Random.Range(minY, maxY);
        transform.position = new Vector2(randomX, randomY);

        spriteRenderer = GetComponent<SpriteRenderer>();

        // Randomly start as Idle or Sick
        float randomValue = Random.Range(0f, 1f);
        currentState = (randomValue < 0.9f) ? State.Idle : State.Sick;

        SetMovementSpeed(currentState);
        UpdateSprite(currentState);

        if (currentState != State.Dead)
        {
            StartCoroutine(MoveRandomly());
        }

        // Find the graveyard object
        graveyard = GameObject.Find("Graveyard").GetComponent<Transform>();
    }

    private void Update()
    {
        if (currentState != State.Dead)
        {
            // Move the player towards the target position
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            if ((Vector2)transform.position == targetPosition)
            {
                moving = false;
                StartCoroutine(MoveRandomly());
            }
        }

        // If the player is sick, check periodically for state transition
        if (currentState == State.Sick && !infectionStarted)
        {
            infectionStarted = true;
            StartCoroutine(HandleSickStateTransition());
        }
    }

    private IEnumerator HandleSickStateTransition()
    {
        while (currentState == State.Sick)
        {
            yield return new WaitForSeconds(5f); // Check every 5 seconds
            TriggerRandomEvent();  // Trigger a random event
        }
        infectionStarted = false; // Reset the flag when sick state ends
    }

    private IEnumerator MoveRandomly()
    {
        if (!moving)
        {
            float waitTime = Random.Range(minDelay, maxDelay);
            SetRandomTargetPosition();
            yield return new WaitForSeconds(waitTime);
        }
    }

    private void SetRandomTargetPosition()
    {
        float randomX = Random.Range(minX, maxX);
        float randomY = Random.Range(minY, maxY);

        targetPosition = new Vector2(randomX, randomY);
    }

    public void ChangeState(State newState)
    {
        // Check if changing to the new state is allowed
        if (currentState == State.Immune || currentState == State.Dead)
        {
            return; // Immune and Dead cannot change their states
        }

        currentState = newState;
        SetMovementSpeed(newState);
        UpdateSprite(newState);

        if (newState != State.Dead)
        {
            if (!moving)
            {
                StartCoroutine(MoveRandomly());
            }
        }
    }

    private void SetMovementSpeed(State state)
    {
        switch (state)
        {
            case State.Idle:
                moveSpeed = idleSpeed;
                break;
            case State.Sick:
                moveSpeed = sickSpeed;
                break;
            case State.Immune:
                moveSpeed = immuneSpeed;
                break;
            case State.Dead:
                moveSpeed = 0f;
                break;
        }
    }

    private void UpdateSprite(State state)
    {
        switch (state)
        {
            case State.Idle:
                spriteRenderer.sprite = IdleSprite;
                break;
            case State.Sick:
                spriteRenderer.sprite = SickSprite;
                break;
            case State.Immune:
                spriteRenderer.sprite = ImmuneSprite;
                break;
            case State.Dead:
                spriteRenderer.sprite = DeadSprite;
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        RandomMovement otherMovement = other.GetComponent<RandomMovement>();

        if (currentState == State.Idle && otherMovement != null && otherMovement.currentState != State.Sick)
        {
            Debug.Log("Player is near a sick player.");

            if (!infectionStarted)
            {
                infectionStarted = true;
                infectionCoroutine = StartCoroutine(StartInfectionTimer());
            }
        }

        // If the other player is dead and immune player finds them, move them to graveyard
        if (currentState == State.Immune && otherMovement != null && otherMovement.currentState == State.Dead && !otherMovement.hasBeenPickedUp)
        {
            Debug.Log("Immune player found a dead player!");
            StartCoroutine(CarryDeadPlayer(otherMovement.gameObject));
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        RandomMovement otherMovement = other.GetComponent<RandomMovement>();

        if (currentState == State.Idle && otherMovement != null && otherMovement.currentState == State.Sick)
        {
            Debug.Log("Player is no longer near a sick player.");
        }
    }

    private IEnumerator StartInfectionTimer()
    {
        float shortWaitTime = 5f;
        yield return new WaitForSeconds(shortWaitTime);

        // Transition to a random state
        TriggerRandomEvent();
        infectionStarted = false;
    }

    private void TriggerRandomEvent()
    {
        float randomValue = Random.Range(0f, 1f);

        if (currentState == State.Idle)
        {
            if (randomValue < 0.4f) // 40% chance
            {
                ChangeState(State.Sick);
            }
        }
        else if (currentState == State.Sick)
        {
            if (randomValue < 0.3f)
            {
                ChangeState(State.Idle); 
            }
            else if (randomValue < 0.9f)
            {
                ChangeState(State.Sick);
            }
            else if (randomValue < 0.95f)
            {
                ChangeState(State.Immune); 
            }
            else if (randomValue < 1f)
            {
                ChangeState(State.Dead);
            }
        }
    }

    private IEnumerator CarryDeadPlayer(GameObject deadPlayer)
    {
        moving = false;
        Vector2 deadPlayerPosition = deadPlayer.transform.position;

        while (Vector2.Distance(transform.position, deadPlayerPosition) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, deadPlayerPosition, immuneSpeed * Time.deltaTime);
            yield return null;
        }

        yield return new WaitForSeconds(2f);

        while (Vector2.Distance(transform.position, graveyard.position) > 0.1f)
        {
            deadPlayer.transform.position = transform.position; // Dead player follows immune player
            transform.position = Vector2.MoveTowards(transform.position, graveyard.position, immuneSpeed * Time.deltaTime);
            yield return null;
        }

        MoveDeadToGraveyard(deadPlayer);

        SetRandomTargetPosition();
        StartCoroutine(MoveRandomly());
    }

    private void MoveDeadToGraveyard(GameObject deadPlayer)
    {
        deadPlayer.transform.position = graveyard.position;
        RandomMovement deadMovement = deadPlayer.GetComponent<RandomMovement>();
        deadMovement.ChangeState(State.Dead);

        deadMovement.hasBeenPickedUp = true;
    }
}
