using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;

using static GameManagerScript;
public class GameManagerScript : MonoBehaviour
{
    //Variables for functionality and game design 
    public static GameManagerScript instance;
    public PlayerScript player;
    public enum TimePeriod
    {
        Morning, Class, Chapel, Lunch, Afternoon, EndOfDay
    }
    //public int maxActions;
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
    private bool eventRunning = false;
    private bool checkpointPending = false;
    private bool restartingPeriod = false;

    //variables for flags, items, etc that change throughout the game
    public GameObject paper;
    //flags in dictionary for organization
    public Dictionary<string, object> flags = new Dictionary<string, object>
    {
        { "hasChapelPass", false },
        { "hasItem1", false },
        { "hasItem2", false },
        { "hasPaper", 0 },
        { "teacherDistracted", false },
        { "teacherDistractedAt", -1 },
        { "teacherSeesPlayer", true },
        { "scheduledMeeting", true }, //should be false
        { "friendedLuke", false },
        { "guyhasLunch", false },
        { "starving", true },
        { "peterCredit", true }, //should be false 
        { "hasEvidence", false } //should be false
    };


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

    // Flag helpers
    public bool GetBool(string key)
    {
        return flags.TryGetValue(key, out object value) && value is bool b && b;
    }

    public int GetInt(string key)
    {
        return flags.TryGetValue(key, out object value) && value is int i ? i : 0;
    }

    public void SetFlag(string key, object value)
    {
        flags[key] = value;
    }

    //Grab neccisary elements on a scene load, gives yarn spinner methods and vars to use
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
            dialogueRunner.AddFunction<string, bool>("getBool", GetBool);
            dialogueRunner.AddFunction<string, int>("getInt", GetInt);
            dialogueRunner.AddCommandHandler<string, bool>("setBool", (key, value) => SetFlag(key, value));
            dialogueRunner.AddCommandHandler<string, int>("setInt", (key, value) => SetFlag(key, value));
            dialogueRunner.AddCommandHandler("PaperTook", takenPaper);
            dialogueRunner.AddCommandHandler("PaperCrumpleTook", takenPaperCrumple);
            dialogueRunner.AddCommandHandler<string>("gameOver", GameOver);
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
            actionsRemaining = 4;


            if (currentPeriod == TimePeriod.Lunch || currentPeriod == TimePeriod.Afternoon)
            {
                actionsRemaining = 5;
            }

            checkpointPending = true;
        }

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

    //conditions throughout the game that cause events to happen. Method is called at beginning of a scene or after an action was used
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
            StartCoroutine(WaitForEvent(classCutscenes.CharacterEnters(classCutscenes.sam, classCutscenes.samPath, "SamLate", !GetBool("teacherSeesPlayer"))));
        }

        if (currentPeriod == TimePeriod.Class && GetBool("teacherDistracted") && actionsRemaining < GetInt("teacherDistractedAt"))
        {
            StartCoroutine(WaitForEvent(classCutscenes.TeacherReturns()));
        }

        if (currentPeriod == TimePeriod.Chapel && !GetBool("hasChapelPass") && actionsRemaining == 3 && SceneManager.GetActiveScene().name == "Quad")
        {
            StartCoroutine(WaitForEvent(WillieDeath()));
        }

        if (currentPeriod == TimePeriod.Lunch && actionsRemaining == 5 && !GetBool("hasChapelPass"))
        {
            dialogueRunner.StartDialogue("EnterLunch");
        }

        if (currentPeriod == TimePeriod.Lunch && actionsRemaining == 4)
        {
            StartCoroutine(WaitForEvent(lunchCutscenes.CharacterGotHisFood()));
            SetFlag("guyhasLunch", true);
        }

        if (currentPeriod == TimePeriod.Lunch && actionsRemaining == 0 && GetBool("starving"))
        {
            StartCoroutine(WaitForEvent(Starve()));
        }

        if (currentPeriod == TimePeriod.Afternoon && !GetBool("hasChapelPass") && GetBool("peterCredit") && SceneManager.GetActiveScene().name == "Quad" && actionsRemaining == 5)
        {
            StartCoroutine(WaitForEvent(quadCutscenes.PeterCreditCutscene()));
        }

    }

    //waits for a current event to be finished before starting another
    private IEnumerator WaitForEvent(IEnumerator nextEvent)
    {
        // Wait until no other event, dialogue, or cutscene is running
        yield return new WaitUntil(() => !eventRunning && !dialogueRunner.IsDialogueRunning && !player.inCutscene );

        // Set true so the event that is waiting can then run with no interuptions 
        eventRunning = true;

        // Play the next event
        yield return StartCoroutine(nextEvent);

        // Wait for any dialogue 
        if (dialogueRunner != null)
        {
            yield return new WaitUntil(() => !dialogueRunner.IsDialogueRunning);
        }

        //set false so other events can start 
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

    public void takenPaper()
    {
        Destroy(paper);
        SetFlag("hasPaper", 1);
        //Debug.Log(GetInt("hasPaper"));
    }

    public void takenPaperCrumple()
    {
        Destroy(paper);
        SetFlag("hasPaper", 2);
        //Debug.Log(GetInt("hasPaper"));
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

    //If specific events happen at a time change put conditions here
    private IEnumerator TimeChanges()
    {
        if (currentPeriod == TimePeriod.Class)
        {
            dialogueRunner.StartDialogue("ClassOver");

            yield return new WaitUntil(() => !dialogueRunner.IsDialogueRunning);
        }

        if (currentPeriod == TimePeriod.Lunch && GetBool("starving") == false)
        {
            dialogueRunner.StartDialogue("LunchOver");

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

    //Saves the current game state at the beginning of a time period
    public void SavePeriodCheckpoint()
    {
        checkpoint = new PeriodCheckpoint
        {
            startingScene = SceneManager.GetActiveScene().name,
            period = currentPeriod,
            actionsRemaining = actionsRemaining,

            //Make a copy of all flags so future changes do not affect the checkpoint
            flags = new Dictionary<string, object>(flags)
        };

        //Debug.Log("Checkpoint saved for " + currentPeriod + " in scene " + checkpoint.startingScene);
    }

    //restores the game to the last time SavePeriodCheckpoint was called 
    public void RestorePeriodCheckpoint()
    {
        if (checkpoint == null)
            return;

        currentPeriod = checkpoint.period;
        actionsRemaining = checkpoint.actionsRemaining;

        //Restore a copy of the saved flags without changing the checkpoint itself
        flags = new Dictionary<string, object>(checkpoint.flags);
    }

    //Restarts the current time period using the saved checkpoint
    public void RestartPeriod()
    {
        //checkpoint must exist
        if (checkpoint == null)
        {
            //Debug.LogWarning("No checkpoint available!");
            return;
        }

        Time.timeScale = 1f;

        // Hide the Game Over screen
        gameOver.SetActive(false);

        //checkpoint isn't overwritten on the reload
        restartingPeriod = true;

        RestorePeriodCheckpoint();

        SceneManager.LoadScene(checkpoint.startingScene);
    }
}

//Stores the information needed to restart a time period
[System.Serializable]
public class PeriodCheckpoint
{
    public string startingScene;
    public TimePeriod period;
    public int actionsRemaining;

    public Dictionary<string, object> flags;
}