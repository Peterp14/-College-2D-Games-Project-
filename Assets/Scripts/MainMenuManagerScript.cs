using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManagerScript : MonoBehaviour
{
    [SerializeField] private GameObject chapelPass;

    private Toggle usingChapelPass;

    private void Start()
    {
        usingChapelPass = chapelPass.GetComponentInChildren<Toggle>();
        if (PlayerPrefs.GetInt("hasChapelPass") == 0)
        {
            chapelPass.SetActive(false);
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
        
        SceneManager.LoadScene("Hall");
    }
}
