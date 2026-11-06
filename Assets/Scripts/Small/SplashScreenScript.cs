using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SplashScreenScript : MonoBehaviour
{
    public float splashDuration = 3f;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(splashDuration);

        SceneManager.LoadScene("MainMenu");
    }
}