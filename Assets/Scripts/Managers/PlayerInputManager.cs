using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;

    [SerializeField] private List<Transform> spawnPoints;

    private HashSet<Gamepad> joinedGamePads = new HashSet<Gamepad>();

    [SerializeField] List<Color> hatColors;

    
    
    int playerCount = 0;
    private int spawn = 0;

    void Update()
    {
        foreach (var gamepad in Gamepad.all)
        {
            if (gamepad.buttonSouth.wasPressedThisFrame && spawn < spawnPoints.Count && !joinedGamePads.Contains(gamepad))
            {
               
                var player = PlayerInput.Instantiate(playerPrefab, controlScheme: "Gamepad", pairWithDevice: gamepad);       
                AssignHatColour(player.GetComponent<PlayerController>());
                player.transform.position = spawnPoints[spawn].position;
                player.SwitchCurrentActionMap("Player");
                joinedGamePads.Add(gamepad);

                spawn++;

                player.GetComponent<PlayerController>().AssignControllerType(gamepad);

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
