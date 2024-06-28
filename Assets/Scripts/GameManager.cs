using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public PlayerController playerController; // Reference to the PlayerController
    public PlaceOnPlane placeOnPlane; // Reference to the PlaceOnPlane script
    public int totalBalls = 3; // Default number of balls
    public TextMeshProUGUI remainingAmmoText; // Reference to the TextMeshProUGUI text element

    public GameObject resultsPanel; // Reference to the Results panel
    public TextMeshProUGUI winText; // Reference to the Win text
    public TextMeshProUGUI loseText; // Reference to the Lose text
    public TextMeshProUGUI percentageText; // Reference to the Percentage text
    public Transform starsContainer; // Container holding the stars
    public Texture goldStarTexture; // Texture for achieved stars
    public Texture blackStarTexture; // Texture for not achieved stars

    public Button continueButton; // Reference to the continue button for Results
    public Button backButton; //Reference to back button in game

    private int remainingBalls; // Number of remaining balls
    private int initialBlockCount; // Initial number of blocks
    private int destroyedBlockCount; // Destroyed number of blocks

    void Start() {
        placeOnPlane.confirmBtn.onClick.AddListener(StartLevel);
        UpdateRemainingAmmoText(); // Update text at start
        resultsPanel.SetActive(false); // Hide results panel at start
        
        continueButton.onClick.AddListener(ResetGame);

        backButton.onClick.AddListener(ResetGame);
    }

    void Update() {
        if (playerController.ShotBallsCount() != totalBalls) return;
        
        CalculateScore();
        CompleteLevel();
        playerController.EnableTouch(false);
    }

    // Start the level(round)
    public void StartLevel() {
        // Initialize the level
        remainingBalls = playerController.levelInfoManager.GetBallsForLevel(playerController.currentLevel);
        totalBalls = remainingBalls;
        initialBlockCount = placeOnPlane.GetBlockCount();
        destroyedBlockCount = 0;
        playerController.EnableTouch(true); // Can shoot ammo now
        UpdateRemainingAmmoText(); // Update text when the level starts
    }

    // Decrement balls and update remaining balls ammo
    public void DecrementBallCount() {
        remainingBalls--;
        UpdateRemainingAmmoText(); // Update text when a ball is shot
    }

    // Update destroyed block counts
    public void IncrementDestroyedBlockCount() {
        destroyedBlockCount++;
    }

    // Function to return stars based on the percentage of broken blocks
    private void CalculateScore() {
        float destructionPercentage = (float)destroyedBlockCount / initialBlockCount;
        if (destructionPercentage >= 1.0f) {
            playerController.starsEarned = 3;
        }
        else if (destructionPercentage >= 0.75f) {
            playerController.starsEarned = 2;
        }
        else if (destructionPercentage >= 0.5f) {
            playerController.starsEarned = 1;
        }
        else {
            playerController.starsEarned = 0;
        }
    }

    // After ammo has been all used, save stars earned and show results
    private void CompleteLevel() {
        playerController.levelInfoManager.SaveStars(playerController.currentLevel, playerController.starsEarned);

        // Show results
        ShowResults();
    }

    // Function to show how many stars were earned and percentage of blocks were broken,
    // And updates victory or fail text accordingly
    private void ShowResults() {
        backButton.gameObject.SetActive(false);
        resultsPanel.SetActive(true);

        // Set winText or loseText according to stars earned
        if (playerController.starsEarned > 0) {
            winText.gameObject.SetActive(true);
            loseText.gameObject.SetActive(false);
        }
        else {
            winText.gameObject.SetActive(false);
            loseText.gameObject.SetActive(true);
        }

        // Show percentage of blocks broken
        float destructionPercentage = (float)destroyedBlockCount / initialBlockCount;
        percentageText.text = "Percentage Broken: " + ((int)(destructionPercentage * 100)).ToString("F2") + "%";

        // Update stars image respective to how many stars earned previously. 0, if none.
        for (int i = 0; i < starsContainer.childCount; i++) {
            RawImage starImage = starsContainer.GetChild(i).GetComponent<RawImage>();
            if (i < playerController.starsEarned) {
                starImage.texture = goldStarTexture;
            }
            else {
                starImage.texture = blackStarTexture;
            }
        }
    }

    private void UpdateRemainingAmmoText() {
        remainingAmmoText.text = "Remaining Ammo: " + remainingBalls;
    }
    
    // Function to reset game (as in removing the game that was played, not renewing it immediately)
    private void ResetGame() {
        // Destroy the spawned prefab
        if (placeOnPlane._spawnedObject != null) {
            Destroy(placeOnPlane._spawnedObject);
        }
        // Reset the shot balls list in PlayerController
        playerController.ResetShotBalls();
        playerController.forceField.gameObject.SetActive(false);

        // Reset game state
        remainingBalls = totalBalls;
        destroyedBlockCount = 0;
        initialBlockCount = 0;
        UpdateRemainingAmmoText();
    }
}
