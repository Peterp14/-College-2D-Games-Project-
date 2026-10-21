using UnityEngine;
using Yarn.Unity;

public class ItemScript : InteractionManagerScript
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
