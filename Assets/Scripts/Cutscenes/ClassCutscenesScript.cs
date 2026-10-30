using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using Yarn.Unity;
using static GameManagerScript;

public class ClassCutsceneScript : MonoBehaviour
{
    public GameObject teacher;
    public Transform teacherCorner;
    public Transform teacherPath; 
    public Transform desk;

    public GameObject evidence;

    public GameObject luca;
    public Transform[] lucaPath;
    public GameObject sam;
    public Transform[] samPath;

    public GameObject luke;
    public GameObject peter;

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
            dialogueRunner.AddCommandHandler("PaperDistraction", PaperDistraction);
            dialogueRunner.AddCommandHandler("ProfLeaves", ProfLeaves);

        }

        if (FindFirstObjectByType<GameManagerScript>() != null)
        {
            manager = FindFirstObjectByType<GameManagerScript>();
        }

        if (manager.currentPeriod == TimePeriod.Afternoon)
        {
            peter.SetActive(false);
            luke.SetActive(false);
            //luca.SetActive(false);
            //sam.SetActive(false);
        }
    }

    public IEnumerator PaperDistraction()
    {
        Transform teacherTransform = teacher.transform;
        while (Vector2.Distance(teacherTransform.position, teacherCorner.position) > 0.05f)
        {
            teacherTransform.position = Vector2.MoveTowards(teacherTransform.position, teacherCorner.position, moveSpeed * Time.deltaTime);

            yield return null;
        }

        teacherTransform.position = teacherCorner.position;

        manager.SetFlag("teacherDistracted", true);
        manager.SetFlag("teacherDistractedAt", manager.actionsRemaining);
    }

    public IEnumerator TeacherReturns()
    {
        Transform teacherTransform = teacher.transform;

        if (!manager.GetBool("teacherSeesPlayer"))
            TurnTeacher();

        player.inCutscene = true;

        cameraScript.SetTarget(teacherTransform);

        yield return new WaitForSeconds(0.5f);

        while (Vector2.Distance(teacherTransform.position, desk.position) > 0.05f)
        {
            teacherTransform.position = Vector2.MoveTowards(
                teacherTransform.position,
                desk.position,
                moveSpeed * Time.deltaTime
            );

            yield return null;
        }

        teacherTransform.position = desk.position;

        yield return new WaitForSeconds(0.5f);

        cameraScript.SetTarget(player.transform);

        manager.SetFlag("teacherDistracted", false);
        manager.SetFlag("teacherDistractedAt", -1);
        manager.SetFlag("teacherSeesPlayer", true);

        player.inCutscene = false;
    }

    public IEnumerator CharacterEnters(GameObject character, Transform[] path, string dialogueNode, bool turnTeacher)
    {
        player.inCutscene = true;

        Transform characterPosition = character.transform;
        Animator animator = character.GetComponent<Animator>();
        SpriteRenderer sprite = character.GetComponent<SpriteRenderer>();

        cameraScript.SetTarget(characterPosition);

        yield return new WaitForSeconds(0.5f);

        character.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        animator.SetBool("Moving", true);

        foreach (Transform point in path)
        {
            if (point.position.x < characterPosition.position.x)
            {
                sprite.flipX = false;
            }
            else if (point.position.x > characterPosition.position.x)
            {
                sprite.flipX = true;
            }
            else if (point.position.y < characterPosition.position.y)
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



        if (turnTeacher && !manager.GetBool("teacherDistracted"))
        {
            TurnTeacher();
            manager.SetFlag("teacherSeesPlayer", !manager.GetBool("teacherSeesPlayer"));

        }


        dialogueRunner.StartDialogue(dialogueNode);


        yield return new WaitUntil(() => !dialogueRunner.IsDialogueRunning);
        yield return StartCoroutine(cameraScript.ReturnToPlayer());

        player.inCutscene = false;
    }

    public void TurnTeacher()
    {
        SpriteRenderer _renderer = teacher.GetComponent<SpriteRenderer>();

        _renderer.flipX = !_renderer.flipX;
    }

    public IEnumerator ProfLeaves()
    {
        player.inCutscene = true;

        Transform teacherTransform = teacher.transform;

        // Move to TeacherPath
        while (Vector2.Distance(teacherTransform.position, teacherPath.position) > 0.05f)
        {
            teacherTransform.position = Vector2.MoveTowards(
                teacherTransform.position,
                teacherPath.position,
                moveSpeed * Time.deltaTime
            );

            yield return null;
        }

        teacherTransform.position = teacherPath.position;

        // Move straight down to Y = -20
        Vector3 destination = new Vector3(teacherTransform.position.x, -20f, teacherTransform.position.z);

        while (Vector2.Distance(teacherTransform.position, destination) > 0.05f)
        {
            teacherTransform.position = Vector2.MoveTowards(
                teacherTransform.position,
                destination,
                moveSpeed * Time.deltaTime
            );

            yield return null;
        }

        teacherTransform.position = destination;
        evidence.SetActive(true);
        player.inCutscene = false;

    }
}