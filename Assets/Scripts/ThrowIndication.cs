using UnityEngine;
using UnityEngine.UI;

public class ThrowIndication : MonoBehaviour
{
    [SerializeField] private Image chargeMeter;
    [SerializeField] private RectTransform arrow;

    [SerializeField] private float arrowDistance = 300f;

    private Vector3 currentDirection = Vector3.forward;

    public void UpdateChargeMeter(float chargeRatio)
    {
        chargeMeter.fillAmount = chargeRatio;
    }

    public void UpdateArrowDirection(Vector3 targetDirection)
    {
        if (targetDirection.sqrMagnitude < 0.01f)
            return;

        // Keep direction horizontal.
        targetDirection.y = 0f;
        targetDirection.Normalize();

        currentDirection = Vector3.Slerp(
            currentDirection,
            targetDirection,
            Time.deltaTime * 10f).normalized;

        Vector2 arrowPosition = new Vector2(currentDirection.x, currentDirection.z) * arrowDistance;

        arrow.anchoredPosition = arrowPosition;

        float angle = Mathf.Atan2(
            currentDirection.z,
            currentDirection.x
        ) * Mathf.Rad2Deg;

        arrow.localRotation = Quaternion.Euler(0f, 0f, angle - 90f);

    }

    public Vector3 GetCurrentDirection()
    {
        return currentDirection;
    }

}
