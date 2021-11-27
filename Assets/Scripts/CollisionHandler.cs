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

    Movement movement;
    AudioSource audioSource;

    private int currentScene;

    private void Start()
    {
        movement = GetComponent<Movement>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision other)
    {
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
        // todo add particle system
        // todo add SFX on crash
        audioSource.PlayOneShot(rocketCrash);
        movement.enabled = false;
        Invoke("ReloadLevel", respawnDelay);
    }

    private void StartNewLevelSequence()
    {
        audioSource.PlayOneShot(success);
        movement.enabled = false;
        Invoke("LoadNextLevel", levelLoadDelay);
    }

    private void ReloadLevel()
    {
        currentScene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentScene);
    }

    private void LoadNextLevel()
    {
        currentScene = SceneManager.GetActiveScene().buildIndex;
        int nextScene = currentScene + 1;
        int lastScene = SceneManager.sceneCountInBuildSettings;

        if (nextScene == lastScene)
        {
            nextScene = 0;
        }

        SceneManager.LoadScene(nextScene);
    }
}
