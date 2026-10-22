using UnityEngine;

public class LevelData : MonoBehaviour
{


    [Header("Level Completion Goals")]
    [SerializeField] private int TimeLimit;
    [SerializeField] private int QuotaLimit;
    [SerializeField] private uint ScoreReq;
    [SerializeField] private int BoatRoll_Intensty;



    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public LevelData getCurrerntLevelData()
    {
        return this;
    }


}
