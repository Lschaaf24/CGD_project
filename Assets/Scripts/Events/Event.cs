using UnityEngine;

public abstract class GameEvent : MonoBehaviour
{
    [Header("Event Settings")]
    public string eventName;
    [Range(0, 1)]
    public float minIntensity;
    protected float intensityModifier = 0;
    public float cooldown;
    public float duration;
    public int budgetCost;
    public int baseWeight;

    public bool isActive;
    public float lastTriggeredTime;

    public abstract void StartEvent();
    public abstract void EndEvent();

    public bool IsCoolingDown()
    {
        return Time.time < lastTriggeredTime + cooldown;
    }

    public float GetRequiredIntensity()
    {
        return minIntensity + intensityModifier;
    }
}