using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class LevelSelector : MonoBehaviour
{

    public static LevelSelector Instance { get; private set; }

    [SerializeField] private List<Level> levels;

    public Level currentLevel;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentLevel = levels[0];
        Debug.Log(currentLevel.GetLevelData().getTimeLimit());

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Level GetCurrentLevel()
    {
        return currentLevel; 
    }


}
