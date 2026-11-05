using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ThrowIndication : MonoBehaviour
{
    [SerializeField] private Image chargeMeter;
    [SerializeField] private RectTransform arrow;

    public void UpdateChargeMeter(float chargeRatio)
    {
        chargeMeter.fillAmount = chargeRatio;
    }

    public void UpdateArrowDirection(Vector3 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        arrow.rotation = Quaternion.Euler(0, 0, angle);
    }


}
