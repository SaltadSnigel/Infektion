using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotManager : MonoBehaviour
{
    public GameObject dotPrefab;
    public int dotCount = 10;

    private List<GameObject> dots = new List<GameObject>();

    void Start()
    {
        SpawnDots();
    }

    void SpawnDots()
    {
        for (int i = 0; i < dotCount; i++)
        {
            GameObject dot = Instantiate(dotPrefab, new Vector3(Random.Range(-8.5f, 8.5f), 0f, Random.Range(-4.5f, 4.5f)), Quaternion.identity);
            dots.Add(dot);
        }

        // Randomly infect one dot to start the infection
        int infectedDotIndex = Random.Range(0, dots.Count);
        DotBehavior dotBehavior = dots[infectedDotIndex].GetComponent<DotBehavior>();
        dotBehavior.currentState = DotBehavior.DotState.Sick;
    }
}
