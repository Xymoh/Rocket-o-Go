using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oscillator : MonoBehaviour
{
    Vector3 startingPos;
    [SerializeField] Vector3 movementVector;
    float movementFactor;
    [SerializeField] float period = 2f;

    void Start()
    {
        startingPos = transform.position;
    }

    void Update()
    {
        // The smallest float is Mathf.Epsilon
        // it's safer to compare it rather than to 0
        if (period <= Mathf.Epsilon) { return; } // to avoid dividing by 0

        float cycles = Time.time / period; // continually growing over time
        
        const float tau = Mathf.PI * 2; // constant value of 6.283
        float rawSinWave = Mathf.Sin(cycles * tau); // going from -1 to 1

        movementFactor = (rawSinWave + 1f) / 2; // recalculation to go from 0 to 1 (just to make it cleaner)

        Vector3 offset = movementVector * movementFactor;
        transform.position = startingPos + offset;
    }
}
