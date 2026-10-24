using UnityEngine;
using Yarn.Unity;

public class InteractionScript : MonoBehaviour
{
    public string dialogueNode;
    public DialogueRunner dialogueRunner;

    public void Interact()
    {
        if (dialogueRunner != null && !dialogueRunner.IsDialogueRunning)
        {
            dialogueRunner.StartDialogue(dialogueNode);
        }
    }
}