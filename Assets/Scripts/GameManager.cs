using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;
using static GameManagerScript;
public class GameManagerScript : MonoBehaviour
{
    public static GameManagerScript instance;

    public PlayerScript player;
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
    public int maxActions;
    public int actionsRemaining;
    public TimePeriod currentPeriod = TimePeriod.Morning;
    private ClassCutsceneScript classCutscenes;
    private QuadCutsceneScript quadCutscenes;
    private HallCutsceneScript hallCutscenes;

    private LunchCutsceneScript lunchCutscenes;

    private DialogueRunner dialogueRunner;

    public GameObject gameOver;
    public TMP_Text reason;

    private PeriodCheckpoint checkpoint;
    private bool checkpointPending = false;
    private bool restartingPeriod = false;



    public GameObject paper;
    public int hasPaper = 0;

    public bool teacherDistracted = false;
    public int teacherDistractedAt = -1;
    private bool eventRunning = false;
    public bool teacherSeesPlayer = true;

    public bool scheduledMeeting = false;

    public bool friendedLuke = false;
    public bool guyhasLunch = false;

    public bool starving = true;


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
        if (FindFirstObjectByType<DialogueRunner>() != null)
        {
            dialogueRunner = FindFirstObjectByType<DialogueRunner>();
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
            dialogueRunner.AddFunction("GetTeacherSeesYou", GetTeacherSeesYou);
            dialogueRunner.AddCommandHandler<string>("gameOver", GameOver);
            dialogueRunner.AddCommandHandler("scheduleMeeting", ScheduleMeeting);
            dialogueRunner.AddFunction("hasScheduledMeeting", HasScheduledMeeting);
            dialogueRunner.AddFunction("GetGuyHasLunch", GetGuyHasLunch);
            dialogueRunner.AddFunction("GetFriendedLuke", GetFriendedLuke);
            dialogueRunner.AddCommandHandler("FriendedLuke", FriendedLuke);
            dialogueRunner.AddCommandHandler("EatMeal", EatMeal);
        }


        if (FindFirstObjectByType<ClassCutsceneScript>() != null)
            classCutscenes = FindFirstObjectByType<ClassCutsceneScript>();

        if (FindFirstObjectByType<PlayerScript>() != null)
            player = FindFirstObjectByType<PlayerScript>();

        if (FindFirstObjectByType<QuadCutsceneScript>() != null)
            quadCutscenes = FindFirstObjectByType<QuadCutsceneScript>();

        if (FindFirstObjectByType<HallCutsceneScript>() != null)
            hallCutscenes = FindFirstObjectByType<HallCutsceneScript>();

        if (FindFirstObjectByType<LunchCutsceneScript>() != null)
            lunchCutscenes = FindFirstObjectByType<LunchCutsceneScript>();


        if (restartingPeriod)
        {
            restartingPeriod = false;
        }
        // Save when entering the first scene of a new period
        else if (checkpointPending)
        {
            SavePeriodCheckpoint();
            checkpointPending = false;
        }
        // Create the initial Morning checkpoint
        else if (checkpoint == null)
        {
            SavePeriodCheckpoint();
        }

        EventChecks();

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


            if (currentPeriod == TimePeriod.Lunch)
            {
                actionsRemaining = 5;
            }

            checkpointPending = true; 
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
            StartCoroutine(WaitForEvent(TimeChanges()));
        }

        EventChecks();
    }

    private void EventChecks()
    {
        if (currentPeriod == TimePeriod.Morning && actionsRemaining == 4)
        {
            StartCoroutine(WaitForEvent(hallCutscenes.SamHello()));
        }


        if (currentPeriod == TimePeriod.Class && actionsRemaining == 3)
        {
            StartCoroutine(WaitForEvent(classCutscenes.CharacterEnters(classCutscenes.luca, classCutscenes.lucaPath, "LucaLate", true)));

        }

        if (currentPeriod == TimePeriod.Class && actionsRemaining == 2)
        {
            StartCoroutine(WaitForEvent(classCutscenes.CharacterEnters(classCutscenes.sam, classCutscenes.samPath, "SamLate", !teacherSeesPlayer)));
        }

        if (currentPeriod == TimePeriod.Class && teacherDistracted && actionsRemaining < teacherDistractedAt)
        {
            StartCoroutine(WaitForEvent(classCutscenes.TeacherReturns()));
        }

        if (currentPeriod == TimePeriod.Chapel && !hasChapelPass && actionsRemaining == 3 && SceneManager.GetActiveScene().name == "Quad")
        {
            StartCoroutine(WaitForEvent(WillieDeath()));
        }

        if (currentPeriod == TimePeriod.Lunch && actionsRemaining == 5 && !hasChapelPass)
        {
            dialogueRunner.StartDialogue("EnterLunch");
        }

        if (currentPeriod == TimePeriod.Lunch && actionsRemaining == 4)
        {
            StartCoroutine(WaitForEvent(lunchCutscenes.CharacterGotHisFood()));
            guyhasLunch = true;
        }

        if (currentPeriod == TimePeriod.Lunch && actionsRemaining == 0 && starving)
        {
            StartCoroutine(WaitForEvent(Starve()));
        }


    }

    private IEnumerator WaitForEvent(IEnumerator nextEvent)
    {
        // Wait until no other event, dialogue, or cutscene is running
        yield return new WaitUntil(() =>
            !eventRunning && !dialogueRunner.IsDialogueRunning && !player.inCutscene
        );

        // Reserve the event slot
        eventRunning = true;

        // Play the cutscene and wait for it to finish
        yield return StartCoroutine(nextEvent);

        // Wait for any dialogue started by the cutscene
        if (dialogueRunner != null)
        {
            yield return new WaitUntil(() => !dialogueRunner.IsDialogueRunning);
        }

        eventRunning = false;
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

    public bool GetTeacherSeesYou()
    {
        return teacherSeesPlayer;
    }
    public void ScheduleMeeting()
    {
        scheduledMeeting = true;
    }

    public bool HasScheduledMeeting()
    {
        return scheduledMeeting;
    }


    public bool GetGuyHasLunch()
    {
        return guyhasLunch;
    }

    public bool GetFriendedLuke()
    {
        return friendedLuke;
    }
    public void FriendedLuke()
    {
        friendedLuke = true;
    }
    public void EatMeal()
    {
        starving = false;
    }


    public void GameOver()
    {
        GameOver("You died!");
    }

    public void GameOver(string deathMessage)
    {
        reason.text = deathMessage;
        gameOver.SetActive(true);
        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;

        Destroy(gameObject);

        // Clear the singleton reference
        instance = null;

        SceneManager.LoadScene("Hall");
    }

    public void RestartRoom()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private IEnumerator TimeChanges()
    {
        if (currentPeriod == TimePeriod.Class)
        {
            dialogueRunner.StartDialogue("ClassOver");

            yield return new WaitUntil(() => !dialogueRunner.IsDialogueRunning);
        }

    }

    private IEnumerator WillieDeath()
    {

        yield return StartCoroutine(quadCutscenes.WillieDeath());

        GameOver("Willie caught you skipping chapel!");
    }

    private IEnumerator Starve()
    {
        yield return StartCoroutine(lunchCutscenes.PunchWithoutLuke());

        GameOver("You forgot to eat lunch and starved to death!");
    }


    public void SavePeriodCheckpoint()
    {
        checkpoint = new PeriodCheckpoint
        {
            startingScene = SceneManager.GetActiveScene().name,
            period = currentPeriod,
            actionsRemaining = actionsRemaining,

            hasChapelPass = hasChapelPass,
            hasItem1 = hasItem1,
            hasItem2 = hasItem2,
            hasPaper = hasPaper,

            teacherDistracted = teacherDistracted,
            teacherDistractedAt = teacherDistractedAt,
            teacherSeesPlayer = teacherSeesPlayer,

            scheduledMeeting = scheduledMeeting,
            friendedLuke = friendedLuke,
            guyhasLunch = guyhasLunch,
            starving = starving
        };

        Debug.Log("Checkpoint saved for " + currentPeriod +
                  " in scene " + checkpoint.startingScene);
    }

    public void RestorePeriodCheckpoint()
    {
        if (checkpoint == null)
            return;

        currentPeriod = checkpoint.period;
        actionsRemaining = checkpoint.actionsRemaining;

        hasChapelPass = checkpoint.hasChapelPass;
        hasItem1 = checkpoint.hasItem1;
        hasItem2 = checkpoint.hasItem2;
        hasPaper = checkpoint.hasPaper;

        teacherDistracted = checkpoint.teacherDistracted;
        teacherDistractedAt = checkpoint.teacherDistractedAt;
        teacherSeesPlayer = checkpoint.teacherSeesPlayer;

        scheduledMeeting = checkpoint.scheduledMeeting;
        friendedLuke = checkpoint.friendedLuke;
        guyhasLunch = checkpoint.guyhasLunch;
        starving = checkpoint.starving;
    }

    public void RestartPeriod()
    {
        if (checkpoint == null)
        {
            Debug.LogWarning("No checkpoint available!");
            return;
        }

        Time.timeScale = 1f;

        // Hide the Game Over screen
        gameOver.SetActive(false);

        restartingPeriod = true;

        RestorePeriodCheckpoint();

        SceneManager.LoadScene(checkpoint.startingScene);
    }
}

[System.Serializable]
public class PeriodCheckpoint
{
    public string startingScene;
    public TimePeriod period;
    public int actionsRemaining;

    public bool hasChapelPass;
    public bool hasItem1;
    public bool hasItem2;
    public int hasPaper;

    public bool teacherDistracted;
    public int teacherDistractedAt;
    public bool teacherSeesPlayer;

    public bool scheduledMeeting;
    public bool friendedLuke;
    public bool guyhasLunch;
    public bool starving;
}