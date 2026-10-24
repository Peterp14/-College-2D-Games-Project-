using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;

public class GameManagerScript : MonoBehaviour
{
    public static GameManagerScript instance;


    public bool hasChapelPass = false;
    public bool hasItem1 = false;
    public bool hasItem2 = false;
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


    public bool teacherDistracted = false;
    public GameObject paper;
    public int hasPaper = 0;

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
            dialogueRunner.AddFunction("hasChapelPass", HasChapelPass);
            dialogueRunner.AddFunction("hasItem1", HasItem1);
            dialogueRunner.AddFunction("hasItem2", HasItem2);
            dialogueRunner.AddCommandHandler("PaperTook", takenPaper);
            dialogueRunner.AddCommandHandler("PaperCrumpleTook", takenPaperCrumple);
            dialogueRunner.AddFunction("GetPaper", GetPaper);
            dialogueRunner.AddFunction("isTeacherDistracted", IsTeacherDistracted);
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

    public bool HasChapelPass()
    {
        return hasChapelPass;
    }

    public bool HasItem1()
    {
        return hasItem1;
    }
    public bool HasItem2()
    {
        return hasItem2;
    }

    public void takenPaper()
    {
        Destroy(paper);
        hasPaper = 1;
        //Debug.Log(hasPaper);
    }

    public void takenPaperCrumple()
    {
        Destroy(paper);
        hasPaper = 2;
        //Debug.Log(hasPaper);
    }
    public int GetPaper()
    {
        return hasPaper;
    }

    public bool IsTeacherDistracted()
    {
        return teacherDistracted;
    }

}