using System.Collections;
using UnityEngine;
using Yarn.Unity;

public class HallEndOfDayCutsceneScript : MonoBehaviour
{
    public DialogueRunner dialogueRunner;
    private GameManagerScript manager;

    public CameraScript cameraScript;
    public PlayerScript player;

    public GameObject sam;

    public GameObject willie;
    public ParticleSystem playerDeathParticles;

    public Transform playerDespawn;

    public GameObject completeScreen; 




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

        if (manager.GetBool("samKnockedOut"))
        {
            sam.SetActive(false);
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
        Vector3 destination = new Vector3(player.transform.position.x - 2.5f, player.transform.position.y, characterPosition.position.z  );

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
                7f * Time.deltaTime
            );

            yield return null;
        }

        characterPosition.position = destination;
        animator.SetBool("Moving", false);

        // Start Sam's dialogue
        dialogueRunner.StartDialogue("SamEndOfDay");

        yield return new WaitUntil(() => !dialogueRunner.IsDialogueRunning);

        // Walk back to original position
        animator.SetBool("Moving", true);

        sprite.flipX = originalPosition.x < characterPosition.position.x;

        while (Vector2.Distance(characterPosition.position, originalPosition) > 0.05f)
        {
            characterPosition.position = Vector3.MoveTowards(
                characterPosition.position,
                originalPosition,
                7f * Time.deltaTime
            );

            yield return null;
        }

        characterPosition.position = originalPosition;
        animator.SetBool("Moving", false);

        player.inCutscene = false;
    }

    public IEnumerator EndWillieDeath()
    {
        player.inCutscene = true;

        Transform playerTransform = player.transform;
        Animator playerAnimator = player.GetComponent<Animator>();
        SpriteRenderer playerSprite = player.GetComponent<SpriteRenderer>();

        Animator willieAnimator = willie.GetComponent<Animator>();
        SpriteRenderer willieSprite = willie.GetComponent<SpriteRenderer>();

        playerSprite.flipX = true;
        willieSprite.flipX = false;

        playerAnimator.SetBool("isMoving", true);
        willieAnimator.SetBool("Moving", true);

        float playerSpeed = 8f;
        float willieSpeed = 10f;

        // Both move until Willie catches the player
        while (Vector2.Distance(willie.transform.position, playerTransform.position) > 3f)
        {
            // Player walks left
            playerTransform.position += Vector3.left * playerSpeed * Time.deltaTime;

            // Willie chases the player
            willie.transform.position = Vector3.MoveTowards(
                willie.transform.position,
                playerTransform.position,
                willieSpeed * Time.deltaTime
            );

            yield return null;
        }

        willieAnimator.SetBool("Moving", false);
        playerAnimator.SetBool("isMoving", false);

        if (playerDeathParticles != null)
        {
            playerDeathParticles.transform.position = playerTransform.position;
            playerDeathParticles.Play();
        }

        player.gameObject.SetActive(false);

        yield return new WaitForSeconds(1f);
    }

    public IEnumerator EndOfDayComplete()
    {
        player.inCutscene = true;


        Transform playerTransform = player.transform;
        Animator playerAnimator = player.GetComponent<Animator>();
        SpriteRenderer playerSprite = player.GetComponent<SpriteRenderer>();

        playerSprite.flipX = true;

        playerAnimator.SetBool("isMoving", true);

        float playerSpeed = 4f;
        completeScreen.SetActive(true);

        while (Vector2.Distance(playerDespawn.position, playerTransform.position) > 1f)
        {
            // Player walks left
            playerTransform.position += Vector3.left * playerSpeed * Time.deltaTime;

            yield return null;
        }

        playerAnimator.SetBool("isMoving", false);


        yield return new WaitForSeconds(1f);

        player.gameObject.SetActive(false);
        yield return new WaitForSeconds(1f);

    }

    public void End()
    {
        manager.BackToMainMenu();
    }
}