using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelInfoManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text levelNameText;
    public TMP_Text difficultyText;
    public TMP_Text starsEarnedText;
    public Transform starsContainer; // Container holding the stars
    public Texture goldStarTexture;
    public Texture blackStarTexture;

    public PlayerController playerController; // Reference to the PlayerController
    public Button[] levelButtons; // Array of level buttons in the UI

    private Dictionary<int, LevelInfo> levelsInfo = new Dictionary<int, LevelInfo>();

    void Start() {
        // Load level data
        LoadLevelData();
        
        // Sample data Init
        levelsInfo.Add(1, new LevelInfo("1", "Easy", PlayerPrefs.GetInt("Level1Stars", 0), 3, true)); // 3 balls for level 1, accessible
        levelsInfo.Add(2, new LevelInfo("2", "Medium", PlayerPrefs.GetInt("Level2Stars", 0), 4, false)); // 4 balls for level 2, not accessible initially
        levelsInfo.Add(3, new LevelInfo("3", "Hard", PlayerPrefs.GetInt("Level3Stars", 0), 5, false)); // 5 balls for level 3, not accessible initially
    
        // Initialize stars as inactive
        SetStarsActive(false);

        // Check level accessibility based on loaded data
        foreach (var level in levelsInfo.Keys) {
            CheckLevelAccessibility(level);
        }

        // Update level buttons based on accessibility
        UpdateLevelButtons();
    }

    // Show level info such as level name, level difficulty, and stars earned for that level.
    public void ShowLevelInfo(TMP_Text levelText) {
        if (int.TryParse(levelText.text, out int level)) {
            if (levelsInfo.ContainsKey(level)) {
                LevelInfo levelInfo = levelsInfo[level];
                levelNameText.text = $"Level: {levelInfo.levelName}";
                difficultyText.text = $"Difficulty: {levelInfo.difficulty}";
                starsEarnedText.text = $"Stars Earned: ";

                // Save the current level to PlayerController
                playerController.currentLevel = level;

                // Update stars
                for (int i = 0; i < starsContainer.childCount; i++) {
                    RawImage starImage = starsContainer.GetChild(i).GetComponent<RawImage>();
                    if (i < levelInfo.starsEarned) {
                        starImage.texture = goldStarTexture;
                    }
                    else {
                        starImage.texture = blackStarTexture;
                    }
                }

                // Activate stars container
                SetStarsActive(true);
            }
            else {
                Debug.LogWarning("Level not found in the dictionary.");
            }
        }
        else {
            Debug.LogError("Invalid level text.");
        }
    }

    // Function to save stars earned to player prefs.
    public void SaveStars(int level, int stars) {
        if (levelsInfo.ContainsKey(level)) {
            int currentStars = levelsInfo[level].starsEarned;
            if (stars > currentStars) {
                levelsInfo[level].starsEarned = stars;
                PlayerPrefs.SetInt($"Level{level}Stars", stars);
                PlayerPrefs.Save();
                CheckLevelAccessibility(level);
            }
        }
    }

    // Load saved stars or set to 0 if no data exists
    private void LoadLevelData() {
        foreach (var level in levelsInfo.Keys) {
            levelsInfo[level].starsEarned = PlayerPrefs.GetInt($"Level{level}Stars", 0);
        }
    }

    // Set stars active
    private void SetStarsActive(bool isActive) {
        for (int i = 0; i < starsContainer.childCount; i++) {
            starsContainer.GetChild(i).gameObject.SetActive(isActive);
        }
    }

    // Set balls (ammo) for each level
    public int GetBallsForLevel(int level) {
        if (levelsInfo.ContainsKey(level)) {
            return levelsInfo[level].balls;
        }
        return 3; // Default to totalBalls if level not found
    }

    // If no stars were earned for previous level, level is not unlocked.
    private void CheckLevelAccessibility(int level) {
        if (levelsInfo.ContainsKey(level)) {
            LevelInfo levelInfo = levelsInfo[level];
            if (levelInfo.starsEarned > 0 && levelsInfo.ContainsKey(level + 1)) {
                levelsInfo[level + 1].isAccessible = true;
            }
        }
        UpdateLevelButtons();
    }

    // Make the level buttons interactable true or false
    private void UpdateLevelButtons() {
        for (int i = 0; i < levelButtons.Length; i++) {
            int level = i + 1;
            if (levelsInfo.ContainsKey(level)) {
                levelButtons[i].interactable = levelsInfo[level].isAccessible;
            }
            else {
                levelButtons[i].interactable = false;
            }
        }
    }
}

[System.Serializable]
public class LevelInfo
{
    public string levelName;
    public string difficulty;
    public int starsEarned;
    public int balls;
    public bool isAccessible;

    public LevelInfo(string levelName, string difficulty, int starsEarned, int balls, bool isAccessible)
    {
        this.levelName = levelName;
        this.difficulty = difficulty;
        this.starsEarned = starsEarned;
        this.balls = balls;
        this.isAccessible = isAccessible;
    }
}
