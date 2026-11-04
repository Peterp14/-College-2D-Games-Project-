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
    public Transform playerChapelSpawn;
    public Transform playerKetRecSpawn;


    public GameObject ChapelSkipper;

    public GameObject ParticleObject;


    private ParticleSystem ChapelSkipperParticles;

    private BoxCollider2D willieBox;

    public Transform[] williePath;

    public float moveSpeed = 10f;

    void Start()
    {

        if (FindFirstObjectByType<DialogueRunner>() != null)
        {
            dialogueRunner = FindFirstObjectByType<DialogueRunner>();
            dialogueRunner.AddCommandHandler("Snitching", Snitching);
        }

        if (FindAnyObjectByType<GameManagerScript>() != null)
        {
            manager = FindFirstObjectByType<GameManagerScript>();

        }

        ChapelSkipperParticles = ParticleObject.GetComponent<ParticleSystem>();

        willieBox = willie.GetComponent<BoxCollider2D>();

        bool peterEvent = GameManagerScript.instance.currentPeriod == GameManagerScript.TimePeriod.Afternoon
            && !GameManagerScript.instance.GetBool("hasChapelPass");

        //// Only show Peter during his Afternoon event
        //peter.SetActive(peterEvent);

        //// Move the player to the new starting position for Peter's event
        //if (peterEvent)
        //{
        //    player.transform.position = playerLunchSpawn.position;
        //}

        if (GameManagerScript.instance.currentPeriod == GameManagerScript.TimePeriod.Chapel)
        {
            ChapelSkipper.transform.position = new Vector2(3.42f, 1.19f);
        }

        if (GameManagerScript.instance.currentPeriod == GameManagerScript.TimePeriod.Afternoon)
        {
            ChapelSkipper.transform.position = new Vector2(4.77f, -16.31f);

            if (manager.GetBool("skipperDead"))
                ChapelSkipper.SetActive(false); 
           // player.transform.position = playerLunchSpawn.position;
        }

        if (GameManagerScript.instance.previousScene == "Chapel")
        {
            player.transform.position = playerChapelSpawn.position;

        }
        else if (GameManagerScript.instance.previousScene == "Ketrec")
        {
            player.transform.position = playerKetRecSpawn.position; 
        }
    }

    public IEnumerator WillieDeath()
    {
        Animator animator = willie.GetComponent<Animator>();
        animator.SetBool("Moving", true);

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

    public IEnumerator Snitching()
    {
        player.inCutscene = true;

        willie.transform.position = new Vector3(
            ChapelSkipper.transform.position.x,
            willie.transform.position.y,
            willie.transform.position.z
        );

        willie.SetActive(true);
        willieBox.enabled = false;
        Animator animator = willie.GetComponent<Animator>();
        animator.SetBool("Moving", true);

        // Move Willie to just below the Chapel Skipper
        Vector3 destination = new Vector3(
            ChapelSkipper.transform.position.x,
            ChapelSkipper.transform.position.y + 2.25f,
            willie.transform.position.z
        );

        while (Vector3.Distance(willie.transform.position, destination) > 0.05f)
        {
            willie.transform.position = Vector3.MoveTowards(
                willie.transform.position,
                destination,
                10f * Time.deltaTime
            );
            if (ChapelSkipper.activeSelf && willie.transform.position.y > ChapelSkipper.transform.position.y - 0.5f)
            {
                if (ChapelSkipper.activeSelf)
                {
                    ChapelSkipperParticles.Play();
                }

                ChapelSkipper.SetActive(false);
            }

            yield return null;
        }

        willie.transform.position = destination;
        animator.SetBool("Moving", false);

        //yield return new WaitUntil(() => !dialogueRunner.IsDialogueRunning);
        willieBox.enabled = true;
        player.inCutscene = false;
        manager.SetFlag("skipperDead",true);
    }

    public IEnumerator WillieToChapel()
    {
        player.inCutscene = true;
        willieBox.enabled = false;


        Transform characterPosition = willie.transform;
        Animator animator = willie.GetComponent<Animator>();
        SpriteRenderer sprite = willie.GetComponent<SpriteRenderer>();

        cameraScript.SetTarget(characterPosition);

        yield return new WaitForSeconds(0.5f);

        willie.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        animator.SetBool("Moving", true);

        sprite.flipX = true;

        foreach (Transform point in williePath)
        {
            //if (point.position.x < characterPosition.position.x)
            //{
            //    sprite.flipX = false;
            //}
            //else if (point.position.x > characterPosition.position.x)
            //{
            //    sprite.flipX = true;
            //}
            //else if (point.position.y < characterPosition.position.y)
            //{
            //    sprite.flipX = false;
            //}

            while (Vector2.Distance(characterPosition.position, point.position) > 0.05f)
            {
                characterPosition.position = Vector2.MoveTowards(
                    characterPosition.position,
                    point.position,
                    moveSpeed * Time.deltaTime
                );

                yield return null;
            }

            characterPosition.position = point.position;
        }

        //animator.SetBool("Moving", false);

        yield return new WaitForSeconds(0.5f);

        //dialogueRunner.StartDialogue(dialogueNode);


        yield return new WaitUntil(() => !dialogueRunner.IsDialogueRunning);
        willie.SetActive(false);
        yield return StartCoroutine(cameraScript.ReturnToPlayer());
        willieBox.enabled = true;
        player.inCutscene = false;
    }
}