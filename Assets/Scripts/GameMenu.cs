using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    [SerializeField] private string Game_Scene_Name;

    public void PlayGame()
    // Void for Loading Chosen Scene
    {
        SceneManager.LoadScene(Game_Scene_Name);
    }





}
