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

    public GameObject normalPaper;
    public GameObject crumpledPaper;
    public GameObject yourSeat; 

    public Transform paperSpawn;
    public Transform paperTarget;

    public Transform paperTargetFloating; 

    public float throwDuration = 0.6f;
    public float throwHeight = 1.5f;

    public float floatDuration = 2f;
    public float swayAmount = 0.4f;
    public float swaySpeed = 4f;
    public float rockAngle = 25f;

    void Start()
    {
        if (FindFirstObjectByType<DialogueRunner>() != null)
        {
            dialogueRunner = FindFirstObjectByType<DialogueRunner>();
            dialogueRunner.AddCommandHandler("PaperDistraction", PaperDistraction);
            dialogueRunner.AddCommandHandler("ProfLeaves", ProfLeaves);
            dialogueRunner.AddCommandHandler("ThrowCrumpledPaper", ThrowCrumpledPaper);
            dialogueRunner.AddCommandHandler("ThrowNormalPaper", ThrowNormalPaper);

        }

        if (FindFirstObjectByType<GameManagerScript>() != null)
        {
            manager = FindFirstObjectByType<GameManagerScript>();
        }

        if (manager.currentPeriod == TimePeriod.Afternoon)
        {
            peter.SetActive(false);
            luke.SetActive(false);
            yourSeat.SetActive(false);
            //luca.SetActive(false);
            //sam.SetActive(false);

            if (manager.GetBool("profRan"))
            {
                teacher.SetActive(false);

                if (!manager.GetBool("hasEvidence"))
                    evidence.SetActive(true);
            }
        }

    }

    public IEnumerator ThrowCrumpledPaper()
    {
        crumpledPaper.transform.position = paperSpawn.position;
        crumpledPaper.transform.rotation = Quaternion.identity;
        crumpledPaper.SetActive(true);

        Vector3 start = paperSpawn.position;
        Vector3 end = paperTarget.position;

        float elapsed = 0f;

        while (elapsed < throwDuration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / throwDuration);

            Vector3 position = Vector3.Lerp(start, end, t);

            // Creates an arc above the normal path
            position.y += Mathf.Sin(t * Mathf.PI) * throwHeight;

            crumpledPaper.transform.position = position;

            // Spin while flying
            crumpledPaper.transform.Rotate(0, 0, 360f * Time.deltaTime);

            yield return null;
        }

        crumpledPaper.transform.position = end;
    }

    public IEnumerator ThrowNormalPaper()
    {
        normalPaper.transform.position = paperSpawn.position;
        normalPaper.transform.rotation = Quaternion.identity;
        normalPaper.SetActive(true);

        Vector3 start = paperSpawn.position;
        Vector3 end = paperTargetFloating.position;

        float elapsed = 0f;

        while (elapsed < floatDuration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / floatDuration);

            // Slowly move toward the floor
            Vector3 position = Vector3.Lerp(start, end, t);

            // Sway left and right
            position.x += Mathf.Sin(elapsed * swaySpeed) * swayAmount;

            normalPaper.transform.position = position;

            // Rock back and forth
            float angle = Mathf.Sin(elapsed * swaySpeed) * rockAngle;
            normalPaper.transform.rotation = Quaternion.Euler(0, 0, angle);

            yield return null;
        }

        normalPaper.transform.position = end;
        normalPaper.transform.rotation = Quaternion.identity;
    }
    public IEnumerator PaperDistraction()
    {
        Transform teacherTransform = teacher.transform;

        Animator animator = teacher.GetComponent<Animator>();
        animator.SetBool("Moving", true);

        while (Vector2.Distance(teacherTransform.position, teacherCorner.position) > 0.05f)
        {
            teacherTransform.position = Vector2.MoveTowards(teacherTransform.position, teacherCorner.position, moveSpeed * Time.deltaTime);

            yield return null;
        }

        teacherTransform.position = teacherCorner.position;
        animator.SetBool("Moving", false);

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

        crumpledPaper.SetActive(false);
        Animator animator = teacher.GetComponent<Animator>();
        animator.SetBool("Moving", true);

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

        animator.SetBool("Moving", false);

        manager.SetFlag("teacherDistracted", false);
        manager.SetFlag("teacherDistractedAt", -1);
        manager.SetFlag("teacherSeesPlayer", true);

        player.inCutscene = false;
    }

    public IEnumerator CharacterEnters(GameObject character, Transform[] path, string dialogueNode, bool turnTeacher)
    {
        player.inCutscene = true;

        BoxCollider2D characterbox = character.GetComponent<BoxCollider2D>();

        characterbox.enabled = false;

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


        characterbox.enabled = true;
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

        SpriteRenderer sr = teacher.GetComponent<SpriteRenderer>();

        sr.flipX = true;

        Animator animator = teacher.GetComponent<Animator>();
        animator.SetBool("Moving", true);

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

        
        Vector3 destination = new Vector3(teacherTransform.position.x, -8f, teacherTransform.position.z);

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
        teacher.SetActive(false);
        player.inCutscene = false;
        manager.SetFlag("profRan", true);

    }
}