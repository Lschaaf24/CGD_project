using System;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class CameraRock : MonoBehaviour
{
    [SerializeField] private float amptitude = 0.5f;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float curve = 0.3f;
    [SerializeField] private float tiltDegrees = 10f;

    private Vector3 startPos;
    private Quaternion startRot;

    private Material skyboxMat;

    float elapsedTime = 0;

    void Start()
    {
        startPos = transform.position;
        startRot = transform.rotation;

        skyboxMat = RenderSettings.skybox;
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;
        float s = Mathf.Sin(elapsedTime * speed);
        float x = amptitude * s;
        float y = curve * x * x;

        transform.position = startPos + new Vector3(x, y, 0f);

        transform.rotation = startRot * Quaternion.Euler(0, -tiltDegrees * s, -tiltDegrees * s);

        if (skyboxMat != null)
        {
            Quaternion inverseCamRot = Quaternion.Inverse(transform.rotation);

            skyboxMat.SetMatrix("_CamCounterRotation", Matrix4x4.Rotate(inverseCamRot));
        }

    }

    private void OnEnable()
    {
        //startPos = transform.position;
        //startRot = transform.rotation;
    }

    public Quaternion GetStartRot() 
    {
        return startRot;
    }
}
