using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class TipEvent : GameEvent
{
    CameraRock cameraRock;
    GameObject camera;

    [SerializeField] private float amptitude = 0.5f;
    [SerializeField] private float speed = 2.5f;
    [SerializeField] private float curve = 0.3f;

    [SerializeField] float tipDegrees;
    [SerializeField] float holdTime = 0;
    [SerializeField] GameObject warningText;
    public override void StartEvent()
    {
        isActive = true;
        camera = Camera.main.gameObject;
        cameraRock = camera.GetComponent<CameraRock>();
        cameraRock.enabled = false;
        StartCoroutine(LerpTip(tipDegrees));
        warningText.SetActive(true);
    }


    public override void EndEvent()
    {
        isActive = false;
        warningText.SetActive(false);

    }

    IEnumerator LerpTip(float degrees) 
    {
        yield return new WaitForSeconds(2.0f);
        float elapsedTime = 0.0f;
        Quaternion startRot = camera.transform.rotation;
        Vector3 startPos = camera.transform.position;
        while(elapsedTime < duration) 
        {
            float s = Mathf.Lerp(0, degrees, elapsedTime / duration);
            float x = amptitude * s;
            float y = curve * (x * x);

            camera.transform.position = startPos + new Vector3(x, y, 0f);

            camera.transform.rotation = startRot * Quaternion.Euler(0, -s, -s);

            elapsedTime += Time.deltaTime;
        
            yield return null;
        }
        StartCoroutine(ResetTip(degrees));
    }

    IEnumerator ResetTip(float degrees) 
    {
        yield return new WaitForSeconds(holdTime);
        float elapsedTime = 0.0f;
        Quaternion startRot = camera.transform.rotation;
        Vector3 startPos = camera.transform.position;
        while (elapsedTime < duration)
        {



            float s = Mathf.Lerp(0, -degrees, elapsedTime / duration);
            float x = amptitude * s;
            float y = curve * (x * x);

            camera.transform.position = startPos + new Vector3(x, -y, 0f);

            camera.transform.rotation = startRot * Quaternion.Euler(0, -s, -s);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        EndEvent(); 
    }
}
