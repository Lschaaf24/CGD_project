using UnityEngine;

public class Level : MonoBehaviour
{

    public static Level instance;

    [SerializeField] private LevelDataSO data;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public LevelDataSO GetLevelData()
    {
        return data;
    }

    public void SetLevelData(LevelDataSO Newdata)
    {
        data = Newdata;
    }

    public void OnClick()
    {
        LevelSelector.Instance.LoadLevel(this);
    }

}
