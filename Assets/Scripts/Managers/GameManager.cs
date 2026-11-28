using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{

    public static GameManager instance;

    [Header("Refs")]
    [SerializeField] private LevelSelector levelSelector;
    [SerializeField] private GameObject feedbackUI;
    [SerializeField] private GameObject quotaPanel;
    [SerializeField] private GameObject quotaCompleteText;
    [SerializeField] private GameObject timerText;
    private States currentState;
    [SerializeField] private LevelDataSO currentLevel;
    [SerializeField] private WinCondition winChecker;
    [SerializeField] private QuotaManager quotaManager;
    [SerializeField] private GameObject endScreen;
    [SerializeField] private TMP_Text endScreen_text;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private Image endScreen_image;
    [SerializeField] private Sprite endScreen_winSprite;
    [SerializeField] private Sprite endScreen_loseSprite;
    [SerializeField] private TMP_Text inGame_Score_Text;


    [Header("Quota timing Settings")]
    [SerializeField] private float startDelay = 4f; 
    [SerializeField] private float minDelay = 8f; 
    [SerializeField] private float maxDelay = 12f;

    //[Header("TrackerBools")]

    private Coroutine quotaRoutine;

    enum States
    {
        GamePlay,
        Pause,
    }

    private void Awake()
    {
        instance = this;
        TryGetLevelSelector();
    }


    void Start()
    {
        /*quotaManager.TryRandomGenerateQuota();
        //quotaManager.TryRandomGenerateQuota();*/
        StartQuotaRoutine();
    }



    void Update()
    {
        

    }

    private void TryGetLevelSelector()
    {
        if (LevelSelector.Instance != null)
        {
            levelSelector = LevelSelector.Instance;
            currentLevel = levelSelector.GetCurrentLevelData();

            Debug.Log("LevelSelector successfully assigned!");
        }
    }

    private LevelSelector LevelSelector
    {
        get
        {
            if (levelSelector == null)
                levelSelector = LevelSelector.Instance;

            return levelSelector;
        }
    }
   
    public LevelDataSO GetLevelDataSO()
    {
        return currentLevel;
    }

    private void SetState(States newState)
    {
        if (currentState == newState) return;

        currentState = newState;

        switch (newState)
        {
            case States.Pause:
                Time.timeScale = 0f;
                break;
            case States.GamePlay:
                Time.timeScale = 1f;
                break;
        }
    }

    void OnEnable()
    {
        winChecker.OnEndGame += OnLevelFinished;
        quotaManager.OnQuotaCompleted += HandleQuotaCompleted;
        quotaManager.OnQuotaFailed += HandleQuotaFailed;
    }


    void OnDisable()
    {
        winChecker.OnEndGame -= OnLevelFinished;
        quotaManager.OnQuotaCompleted -= HandleQuotaCompleted;
        quotaManager.OnQuotaFailed -= HandleQuotaFailed;
    }
    private void HandleQuotaFailed(QuotaDataSO sO)
    {
        Debug.Log("Failed Quota");
        //quotaManager.TryRandomGenerateQuota();

    }

    private void HandleQuotaCompleted(QuotaDataSO sO)
    {
        Debug.LogWarning("Completed Quota");
        StartCoroutine(QuotaCompleteText());
        //Debug.Log("Completed Quota");
        //quotaManager.TryRandomGenerateQuota();
    }

    private IEnumerator QuotaCompleteText()
    {
        quotaCompleteText.SetActive(true);
        yield return new WaitForSeconds(2);
        quotaCompleteText.SetActive(false);

    }

    private void OnLevelFinished()
    {
        Debug.Log("level finish Reached");
        levelSelector.AddLevelToCompletedList(currentLevel);

        feedbackUI.SetActive(false);
        quotaPanel.SetActive(false);
        timerText.SetActive(false);
        inGame_Score_Text.gameObject.SetActive(false);
        StopQuotaRoutine();
        quotaManager.ClearAll();
        TimeChecker.Instance.SetTimerOff(false);

        endScreen.SetActive(true);

        scoreText.text =  winChecker.GetScore().ToString();

        if (winChecker.GetScoreReqReached())
        {
            endScreen_text.text = "WIN";
            endScreen_image.sprite = endScreen_winSprite;
        }
        else
        {
            endScreen_text.text = "LOSE";
            endScreen_image.sprite = endScreen_loseSprite;

        }


        //SceneManager.LoadScene("LevelSelector");
        //levelSelector.SetCurrentLevelData(null);
    }

    private void StartQuotaRoutine()
    {
        if (quotaRoutine != null) return;

        quotaRoutine = StartCoroutine(QuotaScheduleRoutine());

    }

    private void StopQuotaRoutine()
    {
        if(quotaRoutine == null) return;

        StopCoroutine(quotaRoutine);
        quotaRoutine = null;
    }

    private IEnumerator QuotaScheduleRoutine()
    {

        yield return new WaitForSeconds(startDelay);

        while (true)
        {
            quotaManager.TryRandomGenerateQuota();
            yield return new WaitForSeconds(Random.Range(minDelay, maxDelay));
        }


    }

    public void ExitButtonPressed()
    {
        SceneManager.LoadScene("LevelSelector");
    }

}
