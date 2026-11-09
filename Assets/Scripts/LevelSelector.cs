using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class LevelSelector : MonoBehaviour
{

    public static LevelSelector Instance;

    /*[SerializeField] private List<Level> levels;
    [SerializeField] private string Game_Scene_Name;*/

    [SerializeField] private Level currentLevel;
    [SerializeField] private LevelDataSO currentLevelDataSO;

    [SerializeField] private List<LevelDataSO> CompletedLevels;



    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //currentLevel = levels[0];
        //Debug.Log(currentLevel.GetLevelData().getTimeLimit());

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Level GetCurrentLevel()
    {
        if(currentLevel == null)
            return null;
        return currentLevel; 
    }

    public void AddLevelToCompletedList(LevelDataSO levelDataSO)
    {
        if (levelDataSO == null || CompletedLevels.Contains(levelDataSO))
            return;

        CompletedLevels.Add(levelDataSO);
    }

    public LevelDataSO GetCurrentLevelData()
    {
        if(currentLevelDataSO == null)
            return null;
        return currentLevelDataSO;
    }

    public void SetCurrentLevel(Level level)
    {
        currentLevel = level;
    }

    public void SetCurrentLevelData(LevelDataSO levelDataSO)
    {
        currentLevelDataSO = levelDataSO;
    }

    public string GetLevelName()
    {
        if (currentLevel == null)
            return null;
        return currentLevel.GetLevelData().getLevelScene().name;
    }


    public void LoadLevel(Level level)
    {
        SetCurrentLevel(level);

        Debug.Log(GetCurrentLevel().GetLevelData().getLevelScene());

        currentLevelDataSO = currentLevel.GetLevelData();
        SceneManager.LoadScene(currentLevelDataSO.getLevelScene().name);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
