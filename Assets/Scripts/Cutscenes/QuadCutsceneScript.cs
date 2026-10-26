using UnityEngine;
using System.Collections;
using Yarn.Unity;
using UnityEngine.UIElements;

public class QuadCutsceneScript : MonoBehaviour
{
    public DialogueRunner dialogueRunner;
    private GameManagerScript manager;

    public CameraScript cameraScript;
    public PlayerScript player;

    public GameObject willie; 


    void Start()
    {
        if (FindFirstObjectByType<DialogueRunner>() != null)
        {
            dialogueRunner = FindFirstObjectByType<DialogueRunner>();

        }

        if (FindFirstObjectByType<GameManagerScript>() != null)
        {
            manager = FindFirstObjectByType<GameManagerScript>();
        }

    }

    public IEnumerator WillieDeath()
    {
        Debug.Log("DORA");

        player.inCutscene = true;
        // Match the player's X position, keeping Willie's current Y
        willie.transform.position = new Vector3(
            player.transform.position.x,
            willie.transform.position.y,
            willie.transform.position.z
        );

        willie.SetActive(true);

        // Move Willie to just below the player
        Vector3 destination = new Vector3(
            player.transform.position.x,
            player.transform.position.y - 1.5f,
            willie.transform.position.z
        );

        while (Vector3.Distance(willie.transform.position, destination) > 0.05f)
        {
            willie.transform.position = Vector3.MoveTowards(
                willie.transform.position,
                destination,
                10f * Time.deltaTime
            );

            yield return null;
        }

        willie.transform.position = destination;

        // Start dialogue
        //dialogueRunner.StartDialogue("WillieDeath");

        yield return new WaitUntil(() => !dialogueRunner.IsDialogueRunning);
        player.inCutscene = true;

    }
}