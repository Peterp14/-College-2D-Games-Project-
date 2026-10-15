using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;

public class GameManagerScript : MonoBehaviour
{
    public static GameManagerScript instance;

    public enum TimePeriod
    {
        Morning,
        Class,
        Chapel,
        Lunch,
        Afternoon,
        EndOfDay
    }

    public TimePeriod currentPeriod = TimePeriod.Morning;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        DialogueRunner dialogueRunner = FindFirstObjectByType<DialogueRunner>();

        if (dialogueRunner != null)
        {
            dialogueRunner.AddCommandHandler<string>("changeScene", ChangeScene);
            dialogueRunner.AddCommandHandler("nextPeriod", NextPeriod);
            dialogueRunner.AddFunction("isClassTime", IsClassTime);
        }
    }

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void NextPeriod()
    {
        if (currentPeriod < TimePeriod.EndOfDay)
        {
            currentPeriod++;
        }

        Debug.Log("Current Period: " + currentPeriod);
    }

    public bool IsClassTime()
    {
        return currentPeriod == TimePeriod.Class;
    }
}