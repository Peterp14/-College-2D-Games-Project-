using UnityEngine;
using System.Collections;
using Yarn.Unity;

public class ClassroomCutsceneScript : MonoBehaviour
{
    public Transform teacher;
    public Transform teacherCorner;

    public float moveSpeed = 2f;

    void Start()
    {
        DialogueRunner dialogueRunner = FindFirstObjectByType<DialogueRunner>();

        if (dialogueRunner != null)
        {
            dialogueRunner.AddCommandHandler("PaperDistraction",PaperDistraction );
        }
    }

    public IEnumerator PaperDistraction()
    {
        while (Vector2.Distance(teacher.position, teacherCorner.position) > 0.05f)
        {
            teacher.position = Vector2.MoveTowards( teacher.position,teacherCorner.position, moveSpeed * Time.deltaTime  );

            yield return null;
        }

        teacher.position = teacherCorner.position;

        GameManagerScript.instance.teacherDistracted = true;
    }
}