using UnityEngine;

public class ActionUIScript : MonoBehaviour
{
    public GameObject[] actionIcons;

    void Start()
    {
        UpdateActions();
    }

    void Update()
    {
        UpdateActions();
    }

    void UpdateActions()
    {
        int remaining = GameManagerScript.instance.actionsRemaining;

        for (int i = 0; i < actionIcons.Length; i++)
        {
            actionIcons[i].SetActive(i < remaining);
        }
    }
}