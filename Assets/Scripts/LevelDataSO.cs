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

}
