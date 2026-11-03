
using UnityEngine;

public class SoundTest : MonoBehaviour
{
    private void Start()
    {
        SoundManager.PlaySound(SoundType.Footsteps);
    }
}
