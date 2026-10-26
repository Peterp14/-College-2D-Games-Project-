using UnityEngine;
using System.Collections;
using Yarn.Unity;
using UnityEngine.UIElements;

public class ChapelCutsceneScript : MonoBehaviour
{
    public DialogueRunner dialogueRunner;
    private GameManagerScript manager;

    public CameraScript cameraScript;
    public PlayerScript player;


    void Start()
    {
        if (FindFirstObjectByType<DialogueRunner>() != null)
        {
            dialogueRunner = FindFirstObjectByType<DialogueRunner>();
            dialogueRunner.AddCommandHandler("cameraToOpitz", CameraToOpitz);
            dialogueRunner.AddCommandHandler("cameraToPlayer", CameraToPlayer);
        }

        if (FindFirstObjectByType<GameManagerScript>() != null)
        {
            manager = FindFirstObjectByType<GameManagerScript>();
        }

    }

    public IEnumerator CameraToOpitz()
    {
        player.inCutscene = true;
        Camera cam = Camera.main;
        
        Vector3 start = cam.transform.position;
        Vector3 end = new Vector3(0, 3, start.z);

        float duration = 1.5f;
        float elapsed = 0f;

        // Temporarily disable camera following
        cameraScript.enabled = false;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / duration);
            t = Mathf.SmoothStep(0f, 1f, t);

            cam.transform.position = Vector3.Lerp(start, end, t);

            yield return null;
        }

        cam.transform.position = end;

        player.inCutscene = false;

    }

    public IEnumerator CameraToPlayer()
    {
        yield return StartCoroutine(cameraScript.ReturnToPlayer());
    }


}