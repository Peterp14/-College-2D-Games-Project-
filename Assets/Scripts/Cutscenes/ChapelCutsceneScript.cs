using System.Collections;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;
using Yarn.Unity;

public class ChapelCutsceneScript : MonoBehaviour
{
    public DialogueRunner dialogueRunner;
    private GameManagerScript manager;

    public CameraScript cameraScript;
    public PlayerScript player;

    public GameObject willie;
    public GameObject opitz;
    public GameObject conversationBox;
    public GameObject lastCredit; 


    void Start()
    {
        if (FindFirstObjectByType<DialogueRunner>() != null)
        {
            dialogueRunner = FindFirstObjectByType<DialogueRunner>();
            dialogueRunner.AddCommandHandler("cameraToOpitz", CameraToOpitz);
            dialogueRunner.AddCommandHandler("cameraToPlayer", CameraToPlayer);
            dialogueRunner.AddCommandHandler("OpitzRemoved", OpitzRemoved);
            dialogueRunner.AddCommandHandler("CreditTaken", CreditTaken);
        }

        if (FindFirstObjectByType<GameManagerScript>() != null)
        {
            manager = FindFirstObjectByType<GameManagerScript>();
        }

        manager.UseAction();

        if (manager.currentPeriod == GameManagerScript.TimePeriod.Afternoon && !(manager.GetBool("willieSeenEvidence")))
        {
            conversationBox.SetActive(false);
            willie.SetActive(false);
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

    public IEnumerator OpitzRemoved()
    {
        player.inCutscene = true;

        Transform willieTransform = willie.transform;
        Transform opitzTransform = opitz.transform;
        Animator animator = willie.GetComponent<Animator>();


        willie.GetComponent<BoxCollider2D>().enabled = false;
        opitz.GetComponent<BoxCollider2D>().enabled = false;

        yield return new WaitForSeconds(1f);

        // Move Willie slightly left toward Opitz
        Vector3 leftDestination = willieTransform.position + new Vector3(-0.75f, 0, 0);

        while (Vector3.Distance(willieTransform.position, leftDestination) > 0.05f)
        {
            willieTransform.position = Vector3.MoveTowards( willieTransform.position, leftDestination, 2f * Time.deltaTime );

            yield return null;
        }

        willieTransform.position = leftDestination;

        yield return new WaitForSeconds(0.5f);

        opitzTransform.rotation = Quaternion.Euler(0, 0, 90);

        opitzTransform.SetParent(willieTransform, true);

        // Position Opitz in Willie's arms
        opitzTransform.localPosition = new Vector3(-0.5f, 0.5f, 0);

        yield return new WaitForSeconds(1f);

        // Move Willie down the aisle
        Vector3 destination = new Vector3(  willieTransform.position.x,willieTransform.position.y - 15f,  willieTransform.position.z);

        animator.SetBool("Moving", true);

        while (Vector3.Distance(willieTransform.position, destination) > 0.05f)
        {
            willieTransform.position = Vector3.MoveTowards(willieTransform.position, destination, 4f * Time.deltaTime );

            if (willieTransform.position.y <= -1f && !lastCredit.activeSelf)
                lastCredit.SetActive(true);

            yield return null;
        }

        willieTransform.position = destination;
        animator.SetBool("Moving", false);

        opitzTransform.SetParent(null);
        opitz.SetActive(false);
        willie.SetActive(false);
        conversationBox.SetActive(false);
        player.inCutscene = false;
    }


    public void CreditTaken()
    {
        Destroy(lastCredit); 
    }


}