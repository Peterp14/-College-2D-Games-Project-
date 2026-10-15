using UnityEngine;
using Yarn.Unity;

public class DoorScript : InteractionManagerScript
{
    public string dialogueNode;
    public DialogueRunner dialogueRunner;

    public override void Interact()
    {
        if (!dialogueRunner.IsDialogueRunning)
        {
            dialogueRunner.StartDialogue(dialogueNode);
        }
    }
}