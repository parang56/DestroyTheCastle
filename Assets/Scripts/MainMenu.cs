using System;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject optionsMenu;
    public GameObject mainMenu;
    public GameObject levelsMenu;

    public void DeactivateAllMenus() {
        levelsMenu.SetActive(false);
        mainMenu.SetActive(false);
        optionsMenu.SetActive(false);
    }
    
    // Method to be called when the play button is pressed
    public void ToggleLevelsMenu() {
        if (levelsMenu != null) {
            levelsMenu.SetActive(!levelsMenu.activeSelf);
        }
    }

    // Method to be called when the main menu button is pressed
    public void ToggleMainMenu() {
        if (mainMenu != null) {
            mainMenu.SetActive(!mainMenu.activeSelf);
        }
    }

    // Method to be called when the options button is pressed
    public void ToggleOptionsMenu() {
        if (optionsMenu != null) {
            optionsMenu.SetActive(!optionsMenu.activeSelf); // Toggle the active state of the options menu
        }
    }
}