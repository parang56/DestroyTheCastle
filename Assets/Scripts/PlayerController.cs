using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PlayerController : MonoBehaviour {
    public GameObject ballPrefab; // Assign the ball prefab in the Inspector
    public float shootForce = 700f; // Adjust the shooting force as needed
    public LevelInfoManager levelInfoManager;
    public int currentLevel;
    public int starsEarned; // Calculate this based on player's performance
    public TextMeshProUGUI warningText; // Reference to the warning text UI
    public GameObject forceField; // Reference to the ForceField object in the scene

    private bool touchEnabled = false; // Flag to enable or disable touch/mouse input
    private List<GameObject> shotBalls = new List<GameObject>(); // Track the shot balls
    public GameManager gameManager; // Reference to the GameManager

    public Button hasConfirmedBtn;
    private bool hasConfirmed = false;
    private bool isTouching = false;

    private void Update() {
        if (Input.GetKey("escape")) {
            Exit();
        }

        if (touchEnabled) {
            hasConfirmed = true;
        }

        if (!touchEnabled) return; // Early exit if touch is not enabled

#if UNITY_EDITOR || UNITY_STANDALONE
        // Check for mouse input
        if (Input.GetMouseButtonDown(0)) {
            HandleTouch(Input.mousePosition);
        }
#elif UNITY_IOS || UNITY_ANDROID
        // Check for touch input
        if (Input.touchCount > 0) {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began && !isTouching) {
                isTouching = true;
                HandleTouch(touch.position);
            } else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) {
                isTouching = false;
            }
        }
#endif
    }

    private void HandleTouch(Vector3 touchPosition) {
        ShootBall();
    }

    // Function for shooting ball
    public void ShootBall() {
        // Instantiate the ball at the camera's position and orientation
        GameObject ball = Instantiate(ballPrefab, transform.position, transform.rotation);
        shotBalls.Add(ball); // Track the shot ball

        // Get the Rigidbody component of the instantiated ball
        Rigidbody rb = ball.GetComponent<Rigidbody>();

        if (rb != null) {
            // Apply force to the ball to shoot it forward
            rb.AddForce(transform.forward * shootForce);
        }

        gameManager.DecrementBallCount();
    }

    // Return to GameManager the amount of balls shot
    public int ShotBallsCount() {
        return shotBalls.Count;
    }

    // Method for exiting app
    public void Exit() {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // Method to enable or disable touch/mouse input
    public void EnableTouch(bool enable) {
        touchEnabled = enable;
    }

    // Method to reset the shot balls list
    public void ResetShotBalls() {
        shotBalls.Clear();
    }

    // Method to handle trigger events for proximity checking
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject == forceField) {
            EnableTouch(false);
            warningText.gameObject.SetActive(true);
            hasConfirmedBtn.gameObject.SetActive(false);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject == forceField) {
            if (hasConfirmed) {
                EnableTouch(true);
            }
            else {
                hasConfirmedBtn.gameObject.SetActive(true);
            }
            warningText.gameObject.SetActive(false);
        }
    }
}
