using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{

    public static GameManager instance;

    [SerializeField] private LevelSelector levelSelector;
    private States currentState;
    [SerializeField] private LevelDataSO currentLevel;
    [SerializeField] private WinCondition winChecker;
    [SerializeField] private QuotaManager quotaManager;


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
        quotaManager.RandomGenerateQuota();
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
        quotaManager.RandomGenerateQuota();

    }

    private void HandleQuotaCompleted(QuotaDataSO sO)
    {
        Debug.LogWarning("Completed Quota");
        //Debug.Log("Completed Quota");
        quotaManager.RandomGenerateQuota();
    }

    private void OnLevelFinished()
    {
        Debug.Log("level finish Reached");
        levelSelector.AddLevelToCompletedList(currentLevel);



        SceneManager.LoadScene("LevelSelector");
        levelSelector.SetCurrentLevelData(null);
    }
}
