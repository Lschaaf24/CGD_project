using UnityEngine;

public class ScreenSpaceIcon : MonoBehaviour
{
    public Transform targetObject;
    public Vector3 worldOffset = new Vector3(0f, 1.5f, 0f);

    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
        transform.SetParent(mainCam.GetComponentInChildren<Canvas>().transform);
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }

    void LateUpdate()
    {
        if (targetObject == null) return;
        Vector3 screenPos = mainCam.WorldToScreenPoint(targetObject.position + worldOffset);
        transform.position = screenPos;
    }
}