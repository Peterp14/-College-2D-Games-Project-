using UnityEngine;
using UnityEngine.EventSystems;
using Yarn.Unity;

public class DialogueSelectionReset : MonoBehaviour
{
    public DialogueRunner dialogueRunner;

    void LateUpdate()
    {
        var es = EventSystem.current;
        if (dialogueRunner == null || es == null) return;

        var selected = es.currentSelectedGameObject;
        if (selected == null) return;

        // Clear if dialogue ended OR the selected button was hidden
        // (e.g. between two sets of options)
        if (!dialogueRunner.IsDialogueRunning || !selected.activeInHierarchy)
        {
            es.SetSelectedGameObject(null);
        }
    }
}