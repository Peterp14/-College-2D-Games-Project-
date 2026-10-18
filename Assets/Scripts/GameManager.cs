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

    public int maxActions = 5;
    public int actionsRemaining = 5;

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
            dialogueRunner.AddFunction<string, bool>("isPeriod", IsPeriod);
            dialogueRunner.AddCommandHandler("useAction", UseAction);
            dialogueRunner.AddFunction("hasActions", HasActions);
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
            actionsRemaining = maxActions;
        }

        Debug.Log("Current Period: " + currentPeriod);
        Debug.Log("Actions Remaining: " + actionsRemaining);
    }

    public void UseAction()
    {
        if (actionsRemaining <= 0)
            return;

        actionsRemaining--;

        Debug.Log("Actions Remaining: " + actionsRemaining);

        if (actionsRemaining == 0)
        {
            Debug.Log("No actions left! Time to move on.");
        }
    }

    public bool IsPeriod(string period)
    {
        return currentPeriod.ToString() == period;
    }

    public bool HasActions()
    {
        return actionsRemaining > 0;
    }
}