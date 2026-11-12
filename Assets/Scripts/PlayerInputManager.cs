using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerInputManager : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;

    [SerializeField] private Transform spawnPoint;

    private HashSet<Gamepad> joinedGamePads = new HashSet<Gamepad>();

    void Update()
    {
        foreach (var gamepad in Gamepad.all)
        {
           if(gamepad.buttonSouth.wasPressedThisFrame && !joinedGamePads.Contains(gamepad))
            {
                var player = PlayerInput.Instantiate(playerPrefab, controlScheme: "Gamepad", pairWithDevice: gamepad);

                MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
                materialPropertyBlock.SetColor("Color", Random.ColorHSV());
                player.GetComponent<MeshRenderer>().SetPropertyBlock(materialPropertyBlock);

                player.transform.position = spawnPoint.position;
                player.SwitchCurrentActionMap("Player");
                joinedGamePads.Add(gamepad);

            }
        }
    }
}
