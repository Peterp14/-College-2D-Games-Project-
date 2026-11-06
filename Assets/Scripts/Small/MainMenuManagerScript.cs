using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManagerScript : MonoBehaviour
{
    [SerializeField] private GameObject chapelPass;
    [SerializeField] private GameObject personalSpeaker;

    private Toggle usingChapelPass;
    private Toggle usingSpeaker;


    public GameObject instructionScreen;
    public GameObject mainScreen; 

    private void Start()
    {
        usingChapelPass = chapelPass.GetComponentInChildren<Toggle>();
        if (PlayerPrefs.GetInt("hasChapelPass") == 0)
        {
            chapelPass.SetActive(false);
        }

        usingSpeaker = personalSpeaker.GetComponentInChildren<Toggle>();
        if (PlayerPrefs.GetInt("PersonalSpeaker") == 0)
        {
            personalSpeaker.SetActive(false);
        }
    }

    public void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Application.Quit();
        }
    }

    public void StartGame()
    {
        if (PlayerPrefs.GetInt("hasChapelPass") == 1 && usingChapelPass.isOn)
        {
            PlayerPrefs.SetInt("usingChapelPass", 1);
        }
        else
        {
            PlayerPrefs.SetInt("usingChapelPass", 0);
        }

        if (PlayerPrefs.GetInt("PersonalSpeaker") == 1 && usingSpeaker.isOn)
        {
            PlayerPrefs.SetInt("usingSpeaker", 1);
        }
        else
        {
            PlayerPrefs.SetInt("usingSpeaker", 0);
        }

        SceneManager.LoadScene("Hall");
    }

    public void Instructions()
    {
        mainScreen.SetActive(false);
        instructionScreen.SetActive(true);

    }
    public void Back()
    {
        instructionScreen.SetActive(false);
        mainScreen.SetActive(true);
    }


}
