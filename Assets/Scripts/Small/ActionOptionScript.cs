using UnityEngine;
using TMPro;

public class ActionOptionIcon : MonoBehaviour
{
    public GameObject actionIcon;
    public TMP_Text optionText;

    private const string marker = "[ACTION]";

    void LateUpdate()
    {
        if (optionText == null || actionIcon == null)
            return;

        string text = optionText.text;

        if (text.Contains(marker))
        {
            optionText.text = text.Replace(marker, "").TrimStart();
            actionIcon.SetActive(true);
        }
    }

    void OnDisable()
    {
        if (actionIcon != null)
            actionIcon.SetActive(false);
    }
}