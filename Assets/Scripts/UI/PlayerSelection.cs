using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class PlayerSelection : MonoBehaviour
{
    Gamepad assignedGamepad;

    [SerializeField] private GameObject playerModel;
    [SerializeField] private Image fishLeft;
    [SerializeField] private Image fishRight;
    [SerializeField] private GameObject textJoin;
    [SerializeField] private Image selectedColour;

    [SerializeField] public int playerNumber;
    [SerializeField] private Color[] coloursArray;
    private Color chosenColour;
    int colourNumber = 0;

    void Start()
    {
        ChangeColour(playerNumber);

    }

    public void AssignPlayerController(Gamepad gamepad)
    {
        assignedGamepad = gamepad;
        textJoin.SetActive(false);
        fishLeft.gameObject.SetActive(true);
        fishRight.gameObject.SetActive(true);
        playerModel.SetActive(true);
        selectedColour.gameObject.SetActive(true);
        selectedColour.color = chosenColour;

    }
    
    private void ChangeColour(int inColour)
    {
        if (inColour == -1)
        {
            inColour = coloursArray.Length - 1;
            
        }
       
        if (inColour == coloursArray.Length)
        {
            inColour = 0;

        }

        if (inColour == 0)
        {
            fishLeft.color = coloursArray[coloursArray.Length - 1];
        }
        else 
        {
            fishLeft.color = coloursArray[inColour - 1];
        }
        if (inColour == coloursArray.Length - 1)
        {
            fishRight.color = coloursArray[0];
        }
        else
        {
            fishRight.color = coloursArray[inColour + 1];
        }



        colourNumber = inColour;






        chosenColour = coloursArray[colourNumber];
        selectedColour.color = chosenColour;

    }
    
    
    // Update is called once per frame
    void Update()
    {
        foreach (var gamepad in Gamepad.all)
        {
            if (gamepad == assignedGamepad)
            {
                if (gamepad.leftStick.left.wasPressedThisFrame)
                {
                    print("Left on " + gamepad.name);
                    ChangeColour(colourNumber - 1);

                }


                if (gamepad.leftStick.right.wasPressedThisFrame)
                {
                    print("Right on " + gamepad.name);
                    ChangeColour(colourNumber + 1);
                }
            }  
        }
    
    
    
    
    }

}
