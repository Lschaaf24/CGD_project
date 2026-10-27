using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelDataSO", menuName = "Scriptable Objects/LevelDataSO")]
public class LevelDataSO : ScriptableObject
{
    [Header("Level Completion Goals")]
    [SerializeField] private int TimeLimit;
    [SerializeField] private int QuotaLimit;
    [SerializeField] private uint ScoreReq;
    [SerializeField] private int BoatRoll_Intensty;
    [SerializeField] private SceneAsset LevelScene;


    public int getTimeLimit()
    {
        return TimeLimit;
    }

    public int getQuotaLimit()
    {
        return QuotaLimit;
    }
    public uint getScoreReq()
    {
        return ScoreReq;
    }
    public int getBoatRoll_Intensty()
    {
        return BoatRoll_Intensty;
    }
    public SceneAsset getLevelScene()
    {
        return LevelScene;
    }
}
