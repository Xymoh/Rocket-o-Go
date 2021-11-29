using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CollisionHandler : MonoBehaviour
{
    [SerializeField] private float respawnDelay = 1f;
    [SerializeField] private float levelLoadDelay = 3f;

    [SerializeField] AudioClip rocketCrash;
    [SerializeField] AudioClip success;
    [SerializeField] AudioClip fuelPickup;

    [SerializeField] ParticleSystem crashParticles;
    [SerializeField] ParticleSystem successParticles;

    [SerializeField] GameObject audioController;

    Movement movement;
    AudioSource audioSource;
    AudioSource fuelAudioSource;
    CapsuleCollider capsuleCollider;

    private int currentScene;
    private int nextScene;
    private bool isTransitioning = false;
    private bool collisionDisabled = false;

    void Start()
    {
        currentScene = SceneManager.GetActiveScene().buildIndex;
        nextScene = currentScene + 1;

        movement = GetComponent<Movement>();
        audioSource = GetComponent<AudioSource>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        fuelAudioSource = audioController.GetComponent<AudioSource>();
    }

    void Update()
    {
        DebugOptions();
    }

    // Simple Debug
    private void DebugOptions()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("Changing Scene...");

            SceneManager.LoadScene(nextScene);
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            if (collisionDisabled == true) { Debug.Log("Collision enabled"); }
            else { Debug.Log("Collision disabled"); }

            collisionDisabled = !collisionDisabled; // toggle collision
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (isTransitioning || collisionDisabled) { return; }

        switch (other.gameObject.tag)
        {
            case "Friendly":
                Debug.Log("You are on starting platform");
                break;
            case "Finish":
                StartNewLevelSequence();
                break;
            default:
                StartCrashSequence();
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isTransitioning || collisionDisabled) { return; }

        switch (other.gameObject.tag)
        {
            case "Fuel":
                StartFuelUpSequence(other);
                break;
        }
    }

    private void StartCrashSequence()
    {
        isTransitioning = true;
        crashParticles.Play();
        audioSource.Stop();
        audioSource.PlayOneShot(rocketCrash);
        movement.enabled = false;
        Invoke("ReloadLevel", respawnDelay);
    }

    private void StartNewLevelSequence()
    {
        isTransitioning = true;
        successParticles.Play();
        audioSource.Stop();
        audioSource.PlayOneShot(success);
        movement.enabled = false;
        Invoke("LoadNextLevel", levelLoadDelay);
    }

    private void ReloadLevel()
    {
        SceneManager.LoadScene(currentScene);
    }

    private void LoadNextLevel()
    {
        int lastScene = SceneManager.sceneCountInBuildSettings;

        if (nextScene == lastScene) { nextScene = 0; }

        SceneManager.LoadScene(nextScene);
    }

    private void StartFuelUpSequence(Collider other) 
    {
        movement.fuel = 100;
        fuelAudioSource.PlayOneShot(fuelPickup);
        other.gameObject.SetActive(false);
    }
}
