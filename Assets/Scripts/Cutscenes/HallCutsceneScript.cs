using System.Collections;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;
using Yarn.Unity;

public class HallCutsceneScript : MonoBehaviour
{
    public DialogueRunner dialogueRunner;
    private GameManagerScript manager;

    public CameraScript cameraScript;
    public PlayerScript player;

    public GameObject sam; 



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

    public IEnumerator SamHello()
    {
        player.inCutscene = true;

        Transform characterPosition = sam.transform;
        Animator animator = sam.GetComponent<Animator>();
        SpriteRenderer sprite = sam.GetComponent<SpriteRenderer>();

        // Remember Sam's original position
        Vector3 originalPosition = characterPosition.position;

        // Position directly in front of the player
        Vector3 destination = new Vector3(player.transform.position.x + 2.5f, player.transform.position.y, characterPosition.position.z  );

        yield return new WaitForSeconds(0.5f);
        sprite.flipX = destination.x < characterPosition.position.x;
        yield return new WaitForSeconds(0.5f);

        // Walk toward the player
        animator.SetBool("Moving", true);

        while (Vector2.Distance(characterPosition.position, destination) > 0.05f)
        {
            characterPosition.position = Vector3.MoveTowards(
                characterPosition.position,
                destination,
                4f * Time.deltaTime
            );

            yield return null;
        }

        characterPosition.position = destination;
        animator.SetBool("Moving", false);

        // Start Sam's dialogue
        dialogueRunner.StartDialogue("Sam");

        yield return new WaitUntil(() => !dialogueRunner.IsDialogueRunning);

        // Walk back to original position
        animator.SetBool("Moving", true);

        sprite.flipX = originalPosition.x < characterPosition.position.x;

        while (Vector2.Distance(characterPosition.position, originalPosition) > 0.05f)
        {
            characterPosition.position = Vector3.MoveTowards(
                characterPosition.position,
                originalPosition,
                3f * Time.deltaTime
            );

            yield return null;
        }

        characterPosition.position = originalPosition;
        animator.SetBool("Moving", false);

        player.inCutscene = false;
    }
}