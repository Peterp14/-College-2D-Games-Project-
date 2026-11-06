using UnityEngine;
using System.Collections;
using Yarn.Unity;

using UnityEngine.UIElements;


public class LunchCutsceneScript : MonoBehaviour
{
    public GameObject guyGettingFood;
    public Transform[] guyGettingFoodPath;
    public DialogueRunner dialogueRunner;
    private GameManagerScript manager;

    public float moveSpeed = 2f;
    public CameraScript cameraScript;
    public PlayerScript player;

    public GameObject luke;
    public GameObject purpleGuy;
    public GameObject sam;


    void Start()
    {
        if (FindFirstObjectByType<DialogueRunner>() != null)
        {
            dialogueRunner = FindFirstObjectByType<DialogueRunner>();
            dialogueRunner.AddCommandHandler("PunchWithoutLuke", PunchWithoutLuke);
            dialogueRunner.AddCommandHandler("PunchWithLuke", PunchWithLuke);

        }

        if (FindFirstObjectByType<GameManagerScript>() != null)
        {
            manager = FindFirstObjectByType<GameManagerScript>();
        }

    }
    public IEnumerator CharacterGotHisFood()
    {
        player.inCutscene = true;

        Transform characterPosition = guyGettingFood.transform;
        //Animator animator = guyGettingFood.GetComponent<Animator>();
        SpriteRenderer sprite = guyGettingFood.GetComponent<SpriteRenderer>();

        cameraScript.SetTarget(characterPosition);

        yield return new WaitForSeconds(0.5f);


        yield return new WaitForSeconds(0.5f);

        //animator.SetBool("Moving", true);

        foreach (Transform point in guyGettingFoodPath)
        {
            if (point.position.x < characterPosition.position.x)
            {
                sprite.flipX = true;
            }
            else if (point.position.x > characterPosition.position.x)
            {
                sprite.flipX = false;
            }

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

        yield return StartCoroutine(cameraScript.ReturnToPlayer());

        player.inCutscene = false;
    }


    public IEnumerator PunchWithoutLuke()
    {
        player.inCutscene = true;

        // Player falls over
        Quaternion startRotation = player.transform.rotation;
        Quaternion endRotation = Quaternion.Euler(0, 0, -90);

        float elapsed = 0f;

        while (elapsed < 0.5f)
        {
            elapsed += Time.deltaTime;

            player.transform.rotation = Quaternion.Lerp(
                startRotation,
                endRotation,
                elapsed / 0.5f
            );

            yield return null;
        }

        player.transform.rotation = endRotation;

        yield return new WaitForSeconds(1f);

        GameManagerScript.instance.GameOver("You picked a fight you couldn't win! Come next time with backup!");
    }

    public IEnumerator PunchWithLuke()
    {
        player.inCutscene = true;

        // Luke approaches PurpleGuy
        Vector3 destination = new Vector3(
            purpleGuy.transform.position.x - .3f,
            purpleGuy.transform.position.y + .3f,
            luke.transform.position.z
        );

        Animator animator = luke.GetComponent<Animator>();
        SpriteRenderer sprite = luke.GetComponent<SpriteRenderer>();

        if (animator != null)
            animator.SetBool("Moving", true);

        while (Vector3.Distance(luke.transform.position, destination) > 0.05f)
        {
            if (sprite != null)
                sprite.flipX = destination.x < luke.transform.position.x;

            luke.transform.position = Vector3.MoveTowards(
                luke.transform.position,
                destination,
                moveSpeed * Time.deltaTime
            );

            yield return null;
        }

        if (animator != null)
            animator.SetBool("Moving", false);

        // Luke knocks PurpleGuy over
        purpleGuy.transform.rotation = Quaternion.Euler(0, 0, -90);

        yield return new WaitForSeconds(1f);

        player.inCutscene = false;
    }

    public IEnumerator LukeLunchChat()
    {
        player.inCutscene = true;
        dialogueRunner.StartDialogue("LukeLunchChat");

        player.inCutscene = false;
        yield return null;
    }

    public IEnumerator GingerBicker()
    {
        player.inCutscene = true;
        BoxCollider2D box = purpleGuy.GetComponent<BoxCollider2D>();
        SpriteRenderer gregSprite = guyGettingFood.GetComponent<SpriteRenderer>();
        box.enabled = false;
        gregSprite.flipX = false;

        Vector3 destination = new Vector3(
            guyGettingFood.transform.position.x + 1.2f,
            guyGettingFood.transform.position.y,
            guyGettingFood.transform.position.z
        );

        Animator animator = purpleGuy.GetComponent<Animator>();
        SpriteRenderer sprite = purpleGuy.GetComponent<SpriteRenderer>();

        if (animator != null)
            animator.SetBool("Moving", true);

        while (Vector3.Distance(purpleGuy.transform.position, destination) > 0.05f)
        {
            if (sprite != null)
                sprite.flipX = destination.x < purpleGuy.transform.position.x;

            purpleGuy.transform.position = Vector3.MoveTowards(
                purpleGuy.transform.position,
                destination,
                moveSpeed * Time.deltaTime
            );

            yield return null;
        }

        if (animator != null)
            animator.SetBool("Moving", false);
        dialogueRunner.StartDialogue("GingerBicker");
        box.enabled = true;
        yield return new WaitForSeconds(0.5f);

        player.inCutscene = false;
    }

    public IEnumerator SamSpeakerBack()
    {
        player.inCutscene = true;

        Animator animator = sam.GetComponent<Animator>();
        SpriteRenderer sprite = sam.GetComponent<SpriteRenderer>();
        BoxCollider2D box = sam.GetComponent<BoxCollider2D>();

        box.enabled = false;

        Vector3 destination = new Vector3(
            guyGettingFood.transform.position.x + 1f,
            guyGettingFood.transform.position.y,
            sam.transform.position.z
        );

        animator.SetBool("Moving", true);

        while (Vector2.Distance(sam.transform.position, destination) > 0.05f)
        {
            sprite.flipX = destination.x < sam.transform.position.x;

            sam.transform.position = Vector2.MoveTowards(
                sam.transform.position,
                destination,
                moveSpeed * Time.deltaTime
            );

            yield return null;
        }

        sam.transform.position = destination;
        animator.SetBool("Moving", false);

        // Sam falls onto the ground
        Quaternion startRotation = sam.transform.rotation;
        Quaternion endRotation = Quaternion.Euler(0, 0, -90);

        float elapsed = 0f;

        while (elapsed < 0.5f)
        {
            elapsed += Time.deltaTime;

            sam.transform.rotation = Quaternion.Lerp(
                startRotation,
                endRotation,
                elapsed / 0.5f
            );

            yield return null;
        }

        sam.transform.rotation = endRotation;

        player.inCutscene = false;


        dialogueRunner.StartDialogue("LucaGivesMilk");
        yield return null;
    }

}