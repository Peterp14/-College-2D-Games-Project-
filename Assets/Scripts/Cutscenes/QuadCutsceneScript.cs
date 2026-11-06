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

    public GameObject luke; 

    public Transform playerLunchSpawn;
    public Transform playerChapelSpawn;
    public Transform playerKetRecSpawn;


    public GameObject ChapelSkipper;

    public GameObject ParticleObject;


    private ParticleSystem ChapelSkipperParticles;

    private BoxCollider2D willieBox;

    public Transform[] williePath;

    public float moveSpeed = 10f;

    [Header("Spikeball")]
    public GameObject spikeball;
    public Transform samT;
    public Transform samBallPosition;
    public Transform samStartPosition;
    public Transform samExitPosition;

    public GameObject speaker; 




    public GameObject sam;

    public Transform spikeballPlayer;

    public float ballSpeed = 2f;
    public float bounceHeight = 1.5f;

    private bool spikeballPaused = false;

    void Start()
    {

        if (FindFirstObjectByType<DialogueRunner>() != null)
        {
            dialogueRunner = FindFirstObjectByType<DialogueRunner>();
            dialogueRunner.AddCommandHandler("Snitching", Snitching);
            dialogueRunner.AddCommandHandler("PauseSpikeball", PauseSpikeball);
            dialogueRunner.AddCommandHandler("ResumeSpikeball", ResumeSpikeball);
            dialogueRunner.AddCommandHandler("SamGetsBall", SamGetsBall);
            dialogueRunner.AddCommandHandler("removeSpeaker", removeSpeaker);
        }

        if (FindAnyObjectByType<GameManagerScript>() != null)
        {
            manager = FindFirstObjectByType<GameManagerScript>();

        }

        ChapelSkipperParticles = ParticleObject.GetComponent<ParticleSystem>();

        willieBox = willie.GetComponent<BoxCollider2D>();

        bool peterEvent = GameManagerScript.instance.currentPeriod == GameManagerScript.TimePeriod.Afternoon
            && !GameManagerScript.instance.GetBool("hasChapelPass");


        if (peterEvent)
        {
            peter.SetActive(true);
        }

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
        else if (GameManagerScript.instance.previousScene == "Lunch")
        {
            player.transform.position = playerLunchSpawn.position;
        }


        if (GameManagerScript.instance.currentPeriod == GameManagerScript.TimePeriod.Chapel)
        {
            spikeball.SetActive(true);
            StartCoroutine(SpikeballLoop());
        }
        else
        {
            spikeball.SetActive(false);
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

    public IEnumerator LukePartyQuad()
    {
        player.inCutscene = true;

        Transform playerPosition = player.transform;
        Animator animator = player.GetComponent<Animator>();
        SpriteRenderer sprite = player.GetComponent<SpriteRenderer>();

        // Luke calls the player over
        dialogueRunner.StartDialogue("LukeCallsPlayer");

        yield return new WaitUntil(() => !dialogueRunner.IsDialogueRunning);

        yield return new WaitForSeconds(0.5f);

        sprite.flipX = true;
        animator.SetBool("isMoving", true);

        Vector3 yDestination = new Vector3(playerPosition.position.x, luke.transform.position.y, playerPosition.position.z);

        while (Vector2.Distance(playerPosition.position, yDestination) > 0.05f)
        {
            playerPosition.position = Vector3.MoveTowards(playerPosition.position, yDestination, 4f * Time.deltaTime);

            yield return null;
        }

        playerPosition.position = yDestination;

        Vector3 xDestination = new Vector3(luke.transform.position.x + 1f, luke.transform.position.y, playerPosition.position.z);

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

        dialogueRunner.StartDialogue("LukeQuadPlan");

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

    private IEnumerator SpikeballLoop()
    {
        Transform start = samT;
        Transform end = spikeballPlayer;

        while (true)
        {
            float progress = 0f;

            while (progress < 1f)
            {
                // Pause whenever dialogue is running
                if (dialogueRunner.IsDialogueRunning || spikeballPaused)
                {
                    yield return null;
                    continue;
                }

                progress += Time.deltaTime * ballSpeed;
                progress = Mathf.Clamp01(progress);

                // Move horizontally between players
                Vector3 position = Vector3.Lerp(
                    start.position,
                    end.position,
                    progress
                );

                // Create a bouncing arc
                float arc = -Mathf.Sin(progress * Mathf.PI) * bounceHeight;

                position.y += arc;

                spikeball.transform.position = position;

                yield return null;
            }

            // Swap the players so the ball travels back
            Transform temp = start;
            start = end;
            end = temp;
        }
    }

    public void PauseSpikeball()
    {
        spikeballPaused = true;
    }

    public void ResumeSpikeball()
    {
        spikeballPaused = false;
    }

    public IEnumerator SamGetsBall()
    {
        player.inCutscene = true;

        // Stop the normal Spikeball animation
        PauseSpikeball();

        BoxCollider2D bc = sam.GetComponent<BoxCollider2D>();
        bc.enabled = false;

        // Move the ball from its current position to the left
        Vector3 ballStart = spikeball.transform.position;
        Vector3 ballEnd = samBallPosition.position;

        float duration = 0.7f;
        float elapsed = 0f;
        float arcHeight = 2f;

        // Animate the ball flying away
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float progress = Mathf.Clamp01(elapsed / duration);

            Vector3 position = Vector3.Lerp(ballStart, ballEnd, progress);

            // Arc upward as the ball flies away
            position.y += Mathf.Sin(progress * Mathf.PI) * arcHeight;

            spikeball.transform.position = position;

            yield return null;
        }

        spikeball.transform.position = ballEnd;

        // Sam reacts and runs after it
        yield return new WaitForSeconds(0.4f);

        Transform samTransform = sam.transform;
        Animator animator = sam.GetComponent<Animator>();
        SpriteRenderer sprite = sam.GetComponent<SpriteRenderer>();

        sprite.flipX = true;
        animator.SetBool("Moving", true);

        while (Vector2.Distance(samTransform.position, samBallPosition.position) > 0.05f)
        {
            samTransform.position = Vector2.MoveTowards(
                samTransform.position,
                samBallPosition.position,
                moveSpeed * Time.deltaTime
            );

            yield return null;
        }

        samTransform.position = samBallPosition.position;
        animator.SetBool("Moving", false);

        // Hide the ball once Sam picks it up
        spikeball.SetActive(false);

        manager.SetFlag("samGettingBall", true);
        manager.SetFlag("samGettingBallAt", manager.actionsRemaining);

        bc.enabled = true;

        player.inCutscene = false;
    }

    public IEnumerator SamReturns()
    {
        player.inCutscene = true;

        Transform samTransform = sam.transform;
        Animator animator = sam.GetComponent<Animator>();
        SpriteRenderer sprite = sam.GetComponent<SpriteRenderer>();

        sprite.flipX = false; // Face right

        animator.SetBool("Moving", true);

        while (Vector2.Distance(samTransform.position, samStartPosition.position) > 0.05f)
        {
            samTransform.position = Vector2.MoveTowards(
                samTransform.position,
                samStartPosition.position,
                moveSpeed * Time.deltaTime
            );

            yield return null;
        }

        samTransform.position = samStartPosition.position;

        animator.SetBool("Moving", false);

        // Resume the Spikeball animation
        spikeball.transform.position = samT.position;
        spikeball.SetActive(true);
        ResumeSpikeball();

        manager.SetFlag("samGettingBall", false);
        manager.SetFlag("samGettingBallAt", -1);

        if (manager.GetBool("speakerStolen"))
        {
            dialogueRunner.StartDialogue("SamLostSpeaker");
            yield return new WaitUntil(() => !dialogueRunner.IsDialogueRunning);
            sprite.flipX = false;
            animator.SetBool("Moving", true);
            spikeball.SetActive(false);


            while (Vector2.Distance(samTransform.position, samExitPosition.position) > 0.05f)
            {
                samTransform.position = Vector2.MoveTowards(
                    samTransform.position,
                    samExitPosition.position,
                    moveSpeed * Time.deltaTime
                );

                yield return null;
            }

            samTransform.position = samExitPosition.position;
            animator.SetBool("Moving", false);
            sam.SetActive(false);

            dialogueRunner.StartDialogue("GregHelps");
            yield return new WaitUntil(() => !dialogueRunner.IsDialogueRunning);

        }


        player.inCutscene = false;
    }

    public void removeSpeaker()
    {
        Destroy(speaker); 
    }
}