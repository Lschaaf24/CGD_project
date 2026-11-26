using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class PlayerSelection : MonoBehaviour
{
    Gamepad assignedGamepad;

    //Serialized Feilds for Objects and Images
    [SerializeField] private GameObject playerModel;
    [SerializeField] private Image fishLeft;
    [SerializeField] private Image fishRight;
    [SerializeField] private GameObject textJoin;
    [SerializeField] private Image selectedColour;
    [SerializeField] public int playerNumber;

    //If you change "coloursArray" Name Go into the prefab "Join" and re add the colours you want
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
        
        //Sets The join Text Inactive and then Sets all player choosening Icons and model to Active
               
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
        }   //Checks if the Colour is = to zero (the first colour) then sets the colour to the last colour in the array
            //this is to stop the array going out of bounds

        else
        {
            fishLeft.color = coloursArray[inColour - 1];
        }   //else move up one in the array

        if (inColour == coloursArray.Length - 1)
        {
            fishRight.color = coloursArray[0];
        }   //Checks if the Colour is = to the last colour in the array then sets the colour to the first colour in the array
            //this is to stop the array going out of bounds
        {
            fishRight.color = coloursArray[inColour + 1];
        }   //else move up one in the array


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
                    //If the player Presses Left it runs the changeColour Function and then removes one from the colour number 
                }


                if (gamepad.leftStick.right.wasPressedThisFrame)
                {
                    print("Right on " + gamepad.name);
                    ChangeColour(colourNumber + 1);
                    //If the player Presses Right it runs the changeColour Function and then Adds one to the colour number 
                }
            }  
        }
    
    
    
    
    }

}
