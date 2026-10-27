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
    public IEnumerator CharacterGotHisFood()
    {
        player.inCutscene = true;

        Transform characterPosition = guyGettingFood.transform;
        Animator animator = guyGettingFood.GetComponent<Animator>();
        SpriteRenderer sprite = guyGettingFood.GetComponent<SpriteRenderer>();

        cameraScript.SetTarget(characterPosition);

        yield return new WaitForSeconds(0.5f);


        yield return new WaitForSeconds(0.5f);

        animator.SetBool("Moving", true);

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

        animator.SetBool("Moving", false);

        yield return new WaitForSeconds(0.5f);

        yield return StartCoroutine(cameraScript.ReturnToPlayer());

        player.inCutscene = false;
    }
}