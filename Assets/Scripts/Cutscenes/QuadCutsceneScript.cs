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

    public GameObject peter;

    public Transform playerLunchSpawn;

    void Start()
    {
        bool peterEvent = GameManagerScript.instance.currentPeriod == GameManagerScript.TimePeriod.Afternoon
            && !GameManagerScript.instance.GetBool("hasChapelPass");

        // Only show Peter during his Afternoon event
        peter.SetActive(peterEvent);

        // Move the player to the new starting position for Peter's event
        if (peterEvent)
        {
            player.transform.position = playerLunchSpawn.position;
        }
    }

    public IEnumerator WillieDeath()
    {

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

    public IEnumerator PeterCreditCutscene()
    {
        player.inCutscene = true;

        Transform playerPosition = player.transform;
        Animator animator = player.GetComponent<Animator>();
        SpriteRenderer sprite = player.GetComponent<SpriteRenderer>();

        // Peter calls the player over
        dialogueRunner.StartDialogue("PeterCallsPlayer");

        yield return new WaitUntil(() => !dialogueRunner.IsDialogueRunning);

        yield return new WaitForSeconds(0.5f);

        sprite.flipX = true; 
        animator.SetBool("isMoving", true);

        Vector3 yDestination = new Vector3(playerPosition.position.x,  peter.transform.position.y,  playerPosition.position.z  );

        while (Vector2.Distance(playerPosition.position, yDestination) > 0.05f)
        {
            playerPosition.position = Vector3.MoveTowards( playerPosition.position, yDestination, 4f * Time.deltaTime );

            yield return null;
        }

        playerPosition.position = yDestination;

        Vector3 xDestination = new Vector3( peter.transform.position.x + 1f, peter.transform.position.y,  playerPosition.position.z );

        //sprite.flipX = xDestination.x < playerPosition.position.x;

        while (Vector2.Distance(playerPosition.position, xDestination) > 0.05f)
        {
            playerPosition.position = Vector3.MoveTowards(
                playerPosition.position,
                xDestination,
                4f * Time.deltaTime
            );

            yield return null;
        }

        playerPosition.position = xDestination;

        animator.SetBool("isMoving", false);

        //sprite.flipX = peter.transform.position.x < playerPosition.position.x;

        dialogueRunner.StartDialogue("PeterCredit");

        yield return new WaitUntil(() => !dialogueRunner.IsDialogueRunning);

        player.inCutscene = false;
    }
}