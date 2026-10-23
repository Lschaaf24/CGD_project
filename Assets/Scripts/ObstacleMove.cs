using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class ObstacleMove : MonoBehaviour
{

    [SerializeField] private Transform startPos;
    [SerializeField] private Transform endPos;
    [SerializeField] private bool tip;

    public float minimumX = -1.0F;
    public float maximumX = 1.0F;
    public float minimumZ = -1.0F;
    public float maximumZ = 1.0F;
    static float t = 0.0f;

    void Start()
    {
        startPos = GameObject.Find("StartPos").GetComponentInChildren<Transform>();
        endPos = GameObject.Find("EndPos").GetComponentInChildren<Transform>();
        minimumX = startPos.transform.position.x;
        maximumX = endPos.transform.position.x;
        minimumZ = startPos.transform.position.z;
        maximumZ = endPos.transform.position.z;
    }

    void Update()
    {

        //transform.position = new Vector3(Mathf.Lerp(minimum, maximum, t), 0, 0);

        if (tip)
        {
            transform.position = new Vector3(Mathf.Lerp(minimumX, maximumX, t),this.gameObject.transform.position.y, Mathf.Lerp(minimumZ, maximumZ, t));

           // transform.position = new Vector3(0, 0, Mathf.Lerp(minimumZ, maximumZ, t));

            t += 0.5f * Time.deltaTime;


            if (t > 1.0f)
            {
                float tempx = maximumX;
                maximumX = minimumX;
                minimumX = tempx;

                float tempY = maximumZ;
                maximumZ = minimumZ;
                minimumZ = tempY;
                t = 0.0f;
            }
        }

    }
}
