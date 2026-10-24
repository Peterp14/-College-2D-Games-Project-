using UnityEngine;
using UnityEngine.EventSystems;
using Yarn.Unity;

public class DialogueSelectionReset : MonoBehaviour
{
    public DialogueRunner dialogueRunner;

    void Update()
    {
        if (dialogueRunner == null || EventSystem.current == null)
            return;

        if (!dialogueRunner.IsDialogueRunning &&
            EventSystem.current.currentSelectedGameObject != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}