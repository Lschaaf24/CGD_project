using UnityEngine;

public class TipEvent : GameEvent
{
    float angleTarget = 10.0f;
    public float tipTime = 1f; 
    CameraRock cameraRock;
    GameObject camera;
    float elapsedTime = 0.0f;
    public override void StartEvent()
    {
        isActive = true;
        camera = Camera.main.gameObject;
        cameraRock = camera.GetComponent<CameraRock>();
        cameraRock.enabled = false;

    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;
        if (isActive) 
        {
            camera.transform.rotation = cameraRock.GetStartRot() * Quaternion.Euler(0, 0, Mathf.LerpAngle(camera.transform.eulerAngles.z, angleTarget, elapsedTime/tipTime));
        }
    }

    public override void EndEvent()
    {
        isActive = false;
    }
}
