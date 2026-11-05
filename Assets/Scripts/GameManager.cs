using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{

    public static GameManager instance;

    [Header("Refs")]
    [SerializeField] private LevelSelector levelSelector;
    private States currentState;
    [SerializeField] private LevelDataSO currentLevel;
    [SerializeField] private WinCondition winChecker;
    [SerializeField] private QuotaManager quotaManager;

    [Header("Quota timing Settings")]
    [SerializeField] private float startDelay = 2f; 
    [SerializeField] private float minDelay = 8f; 
    [SerializeField] private float maxDelay = 12f;

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
        //Debug.Log("Completed Quota");
        //quotaManager.TryRandomGenerateQuota();
    }

    private void OnLevelFinished()
    {
        Debug.Log("level finish Reached");
        levelSelector.AddLevelToCompletedList(currentLevel);



        SceneManager.LoadScene("LevelSelector");
        levelSelector.SetCurrentLevelData(null);
    }

    private void StartQuotaRoutine()
    {
        if (quotaRoutine != null) return;

        quotaRoutine = StartCoroutine(QuotaScheduleRoutine());

    }

    private void StopQuotaRoutine()
    {
        if(quotaRoutine == null) return;

        StopCoroutine(QuotaScheduleRoutine());
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

}
