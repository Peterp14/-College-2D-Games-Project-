using TMPro;
using TMPro.EditorUtilities;
using UnityEngine;

public class ActionUIScript : MonoBehaviour
{
    public GameObject[] actionIcons;
    public TMP_Text time; 

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
        // GameManager may briefly be null during restart
        if (GameManagerScript.instance == null)
            return;


        int remaining = GameManagerScript.instance.actionsRemaining;

        for (int i = 0; i < actionIcons.Length; i++)
        {
            if (actionIcons[i] != null)
            {
                actionIcons[i].SetActive(i < remaining);
            }
        }

        time.text = GameManagerScript.instance.currentPeriod.ToString();


    }
}