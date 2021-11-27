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

    [SerializeField] ParticleSystem crashParticles;
    [SerializeField] ParticleSystem successParticles;

    Movement movement;
    AudioSource audioSource;
    CapsuleCollider capsuleCollider;

    private int currentScene;
    private int nextScene;
    private bool isTransitioning = false;
    private bool collisionDisabled = false;

    private void Start()
    {
        currentScene = SceneManager.GetActiveScene().buildIndex;
        nextScene = currentScene + 1;

        movement = GetComponent<Movement>();
        audioSource = GetComponent<AudioSource>();
        capsuleCollider = GetComponent<CapsuleCollider>();
    }

    private void Update()
    {
        DebugOptions();
    }

    // Simple Debug
    private void DebugOptions()
    {
        // currentScene = SceneManager.GetActiveScene().buildIndex;
        // int nextScene = currentScene + 1;

        if (Input.GetKey(KeyCode.L))
        {
            SceneManager.LoadScene(nextScene);
        }
        else if (Input.GetKey(KeyCode.C))
        {
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
            case "Fuel":
                Debug.Log("You collected additional fuel");
                break;
            case "Finish":
                StartNewLevelSequence();
                break;
            default:
                StartCrashSequence();
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
        // currentScene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentScene);
    }

    private void LoadNextLevel()
    {
        // currentScene = SceneManager.GetActiveScene().buildIndex;
        // int nextScene = currentScene + 1;
        int lastScene = SceneManager.sceneCountInBuildSettings;

        if (nextScene == lastScene)
        {
            nextScene = 0;
        }

        SceneManager.LoadScene(nextScene);
    }
}
