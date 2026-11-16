using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using Unity.VisualScripting;

public class PlayerInputManager : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;

    [SerializeField] private Transform spawnPoint;

    private HashSet<Gamepad> joinedGamePads = new HashSet<Gamepad>();

    [SerializeField] List<Color> hatColors;
    int playerCount = 0;

    void Update()
    {
        foreach (var gamepad in Gamepad.all)
        {
            if (gamepad.buttonSouth.wasPressedThisFrame && !joinedGamePads.Contains(gamepad))
            {
                var player = PlayerInput.Instantiate(playerPrefab, controlScheme: "Gamepad", pairWithDevice: gamepad);

                AssignHatColour(player.GetComponent<PlayerController>());
                

                player.transform.position = spawnPoint.position;
                player.SwitchCurrentActionMap("Player");
                joinedGamePads.Add(gamepad);

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
            propertyBlock.SetColor("_BaseColor", Random.ColorHSV());
        }
      
        player.GetHatRenderer().SetPropertyBlock(propertyBlock);
        
    }
}
