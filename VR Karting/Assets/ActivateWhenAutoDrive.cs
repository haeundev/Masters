using UnityEngine;

public class ActivateWhenAutoDrive : MonoBehaviour
{
    private void Awake()
    {
        gameObject.SetActive(false);
    }

    private void Start()
    {
        var gameController = FindObjectOfType<GameController>();
        if (gameController == null)
        {
            Debug.LogError("GameController not found");
            return;
        }
        
        gameObject.SetActive(gameController.driveMode == DriveMode.Auto);
    }
}