using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Movement : MonoBehaviour
{
    [SerializeField] private float thrustForce = 1000f;
    [SerializeField] private float thrustRotation = 1f;
    [SerializeField] public float fuel = 100f;

    [SerializeField] Slider slider;

    [SerializeField] AudioClip mainEngine;

    [SerializeField] ParticleSystem mainThrustParticles;
    [SerializeField] ParticleSystem leftSideThrustParticles;
    [SerializeField] ParticleSystem rightSideThrustParticles;

    Rigidbody rb;
    AudioSource audioSource;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        ProcessThrust();
        ProcessRotate();
        UpdateSliderValue();
    }

    private void ProcessThrust()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            StartThrusting();
        }
        else
        {
            StopThrusting();
        }
    }

    private void StartThrusting()
    {
        fuel = fuel - 10 * Time.deltaTime;

        if (fuel > 0)
        {
            // Move the rocket relatively to his top position
            rb.AddRelativeForce(Vector3.up * thrustForce * Time.deltaTime);

            // if the audio is currently not playing play the one shot of a mainengine sound
            if (!audioSource.isPlaying) { audioSource.PlayOneShot(mainEngine); }

            // if the mainthrustparticles are not playing play them
            if (!mainThrustParticles.isPlaying) { mainThrustParticles.Play(); }

            // to be observed, might invoke some bugs
            if (fuel < 0.1) { StopThrusting(); } 
        }
        else
        {
            fuel = 0;
        }
    }

    private void ProcessRotate()
    {
        if (Input.GetKey(KeyCode.A)) { RotateLeft(); }
        else if (Input.GetKey(KeyCode.D)) { RotateRight(); }
        else { StopRotating(); }
    }

    private void StopThrusting()
    {
        // stop the audo of thrusting and the particles
        audioSource.Stop();
        mainThrustParticles.Stop();
    }

    private void RotateLeft()
    {
        // particles to right thrust
        if (!rightSideThrustParticles.isPlaying) { rightSideThrustParticles.Play(); }

        ApplyRotation(thrustRotation);
    }

    private void RotateRight()
    {
        // particles to left thrust
        if (!leftSideThrustParticles.isPlaying) { leftSideThrustParticles.Play(); }

        ApplyRotation(-thrustRotation);
    }

    private void StopRotating()
    {
        // stop particles from left side thrust and the right one
        rightSideThrustParticles.Stop();
        leftSideThrustParticles.Stop();
    }

    private void ApplyRotation(float rotationDirection)
    {
        rb.freezeRotation = true; // freezing rotation so we can manually rotate
        transform.Rotate(Vector3.forward * rotationDirection * Time.deltaTime);
        rb.freezeRotation = false; // unfreezing rotation so the physics system can take over
    }

    private void UpdateSliderValue() 
    {
        slider.value = fuel;
    }
}
