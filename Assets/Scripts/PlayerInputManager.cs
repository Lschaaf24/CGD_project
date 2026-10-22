using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerInputManager : MonoBehaviour
{

    [SerializeField] private GameObject playerPrefab;

    [SerializeField] private Transform spawnPoint;

    private HashSet<Gamepad> joinedGamePads = new HashSet<Gamepad>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var gamepad in Gamepad.all)
        {
           if(gamepad.buttonSouth.wasPressedThisFrame && !joinedGamePads.Contains(gamepad))
            {
                var player = PlayerInput.Instantiate(playerPrefab, controlScheme: "Gamepad", pairWithDevice: gamepad);
                player.transform.position = spawnPoint.position;
                joinedGamePads.Add(gamepad);

            }
        }
    }
}
