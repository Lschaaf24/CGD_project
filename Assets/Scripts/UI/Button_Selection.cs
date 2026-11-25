using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

//Alice's Scrpit

public class Button_Selection : MonoBehaviour
{
    private Button menuButton;
    //float Size = 1.0f;
    float Target_Size = 1.0f;


    void Start()
    {
        menuButton = GetComponentInChildren<Button>();


    }

    public void Selected()
    {
        //default Size of the Text when Selected
        print("ButtonSelected");
        Target_Size = 1.5f;
        
    }

    public void UnSelected()
    {
        //default Size of the Text when not Selected
        print("Unselected");
        Target_Size = 1.0f;
    }

    void Update()
    {
        //Gets the Menu Buttons Child (TPMText) and its local scale then sets it using a Lerp between the Texts local size and a vector one by the target size
        //Change the last set of numbers "0.05f" to change the speed of the growth

        menuButton.transform.GetChild(0).localScale = Vector2.Lerp(menuButton.transform.GetChild(0).localScale, Vector2.one * Target_Size, 0.01f);
    }

}
