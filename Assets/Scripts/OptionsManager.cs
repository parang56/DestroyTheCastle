using UnityEngine;
using UnityEngine.UI;
using TMPro; // Add this line to use TextMeshPro

public class OptionsManager : MonoBehaviour
{
    public Slider volumeSlider;
    public TMP_Dropdown qualityDropdown; // Change Dropdown to TMP_Dropdown

    private void Start() {
        // Load preferences or set default values
        volumeSlider.value = PlayerPrefs.GetFloat("volume", 0.5f); // Default to 50% if not set
        qualityDropdown.value = PlayerPrefs.GetInt("quality", 1); // Default to 'Medium' if not set
        ApplyQuality(qualityDropdown.value);
    }

    public void OnVolumeChange() {
        PlayerPrefs.SetFloat("volume", volumeSlider.value);
    }

    public void OnQualityChange() {
        PlayerPrefs.SetInt("quality", qualityDropdown.value);
        ApplyQuality(qualityDropdown.value);
    }

    private void ApplyQuality(int index) {
        QualitySettings.SetQualityLevel(index, true);
    }
}