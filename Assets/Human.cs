using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class Human : MonoBehaviour
{
    private enum State
    {
        Idle,
        Walk,
        Sick,
        Death
    }
    private State currentState = State.Idle;

    private Vector2 startpos = Vector2.zero;
    private Vector2 target = Vector2.zero;

    public Camera cam = null;

    // Start is called before the first frame update
    void Start()
    {
        startpos = new Vector2(Random.Range(-8f, 8f), Random.Range(-4.5f, 4.5f));
        transform.position = startpos;
        ChangeState(State.Idle);
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case State.Idle:
                break;
                
            case State.Walk:
                transform.position = Vector2.MoveTowards
                    (transform.position, target, 5 * Time.deltaTime);
                if ((Vector2)transform.position == target)
                {
                    ChangeState(State.Idle);
                }
                break;
            case State.Sick:
                break;
                case State.Death:
                break;
        }
    }
    private void ChangeState(State newState)
    {
        StopAllCoroutines();
        currentState = newState;

        switch (currentState)
        {
            case State.Idle:
                StartCoroutine(IdleStateRoutine());
                break;

                case State.Walk:
                startpos = new Vector2(Random.Range(-8.5f, 8.5f),
                    Random.Range(-4.5f, 4.5f));
                Debug.Log($"New Target: {target}");
                break;

                case State.Sick:
                Debug.Log("Agent is Sick.");
                break;
                case State.Death:
                Debug.Log("Agent had Died.");
                break;
        }
    }
    private IEnumerator IdleStateRoutine()
    {
        Debug.Log("Idle State");
        yield return new WaitForSeconds(1.0f);
        ChangeState(State.Walk);
    }
    public void SetSick()
    {
        ChangeState(State.Sick);
    }
    public void SetDeath()
    {
        ChangeState(State.Death);
    }
}
