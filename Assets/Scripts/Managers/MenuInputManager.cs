using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuInputManager : MonoBehaviour
{
   

    private HashSet<Gamepad> joinedGamePads = new HashSet<Gamepad>();

    [SerializeField] List<Color> hatColors;

    [SerializeField] private PlayerSelection[] PlayerJoin;
    //For player Selection Scene. Looks for list of Player Selection, Player Join
    //Alice
    
    
    int playerCount = 0;
    private int playerNumber = 0;

    void Update()
    {
        foreach (var gamepad in Gamepad.all)
        {
            
            if (gamepad.buttonSouth.wasPressedThisFrame && playerNumber < 4 && !joinedGamePads.Contains(gamepad))
            {
                print("TestButtonPressed");
                joinedGamePads.Add(gamepad);
              

                if (PlayerJoin[playerNumber] != null)
                {
                    PlayerJoin[playerNumber].AssignPlayerController(gamepad);
                }
              
                playerNumber++;

          



            }
        }
    }

    




    private void AssignHatColour(PlayerController player) 
    {
        if (!player.GetHatRenderer()) return;


            MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
        if (playerCount < hatColors.Count)
        {
            propertyBlock.SetColor("_BaseColor", hatColors[playerCount]);
            playerCount++;
        }
        else
        {
            propertyBlock.SetColor("_BaseColor", UnityEngine.Random.ColorHSV());
        }
      
        player.GetHatRenderer().SetPropertyBlock(propertyBlock);
        
    }
}
