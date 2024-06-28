using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Serializable]
public class LevelPrefab
{
    public int level;
    public GameObject prefab;
}

public class PlaceOnPlane : MonoBehaviour
{
    public Button confirmBtn;
    public PlayerController playerController; // Reference to the PlayerController
    public GameObject forceField; // Reference to the ForceField object in the scene
    public TextMeshProUGUI warningText; // Reference to the warning text UI

    // List to set up level-prefab mappings in the Inspector
    public List<LevelPrefab> levelPrefabsList;

    private Dictionary<int, GameObject> levelPrefabs = new Dictionary<int, GameObject>();

    // The instantiated object
    public GameObject _spawnedObject;

    private ARRaycastManager _arRaycastManager;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private int currentLevel = 1; // Default level
    private bool touchEnabled = false; // Flag to enable or disable touch/mouse input
    private bool isTouching = false;

    private void Awake()
    {
        _arRaycastManager = GetComponent<ARRaycastManager>();

        // Populate the dictionary from the list
        foreach (LevelPrefab levelPrefab in levelPrefabsList)
        {
            if (!levelPrefabs.ContainsKey(levelPrefab.level))
            {
                levelPrefabs.Add(levelPrefab.level, levelPrefab.prefab);
            }
        }

        warningText.gameObject.SetActive(false); // Hide warning text at start
    }

    private void Update()
    {
        if (!touchEnabled) return; // Stop if touch is not enabled

        if (IsPointerOverUIElement()) return; // Stop if pointer is over a UI element

#if UNITY_EDITOR || UNITY_STANDALONE
        // Check for mouse input
        if (Input.GetMouseButtonDown(0))
        {
            HandleTouch(Input.mousePosition);
        }
#elif UNITY_IOS || UNITY_ANDROID
        // Check for touch input
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began && !isTouching)
            {
                isTouching = true;
                HandleTouch(touch.position);
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isTouching = false;
            }
        }
#endif
    }

    private void HandleTouch(Vector2 touchPosition)
    {
        // Check if raycast hit any trackables
        if (_arRaycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            // Raycast hits are sorted by distance, so the first hit means the closest
            var hitPose = hits[0].pose;

            // Check if there is an already spawned object. If there is none, instantiate the prefab
            if (_spawnedObject == null)
            {
                hitPose.position.y += 0.25f;
                _spawnedObject = Instantiate(levelPrefabs[currentLevel], hitPose.position, hitPose.rotation);
                EnableKinematicOnSpawnedObject(); // Enable kinematic when the object is spawned

                // Set ForceField position to match the spawned object
                if (forceField != null)
                {
                    forceField.transform.position = _spawnedObject.transform.position;
                }

                confirmBtn.gameObject.SetActive(true);
            }
            else
            {
                // Change the spawned object position and rotation to touch or click position
                hitPose.position.y += 0.25f;
                _spawnedObject.transform.position = hitPose.position;
                _spawnedObject.transform.rotation = hitPose.rotation;

                // Move the ForceField to follow the spawned object
                if (forceField != null)
                {
                    forceField.transform.position = _spawnedObject.transform.position;
                }
            }
            forceField.gameObject.SetActive(true);
        }
    }

    public void SetCurrentLevel(TMP_Text levelText)
    {
        if (int.TryParse(levelText.text, out int level))
        {
            if (levelPrefabs.ContainsKey(level))
            {
                currentLevel = level;
            }
            else
            {
                Debug.LogWarning("Level not found in the dictionary.");
            }
        }
        else
        {
            Debug.LogError("Invalid level text.");
        }
    }

    // Method to enable or disable touch/mouse input
    public void EnableTouch(bool enable)
    {
        touchEnabled = enable;
    }

    // Method to enable kinematic on the spawned object's rigidbodies
    public void EnableKinematicOnSpawnedObject()
    {
        if (_spawnedObject != null)
        {
            SetKinematicForAllChildren(_spawnedObject, true);
        }
        else
        {
            Debug.LogWarning("No spawned object to enable kinematic on.");
        }
    }

    // Method to disable kinematic on the spawned object's rigidbodies
    public void DisableKinematicOnSpawnedObject()
    {
        if (_spawnedObject != null)
        {
            SetKinematicForAllChildren(_spawnedObject, false);
        }
        else
        {
            Debug.LogWarning("No spawned object to disable kinematic on.");
        }
    }

    // Helper method to set kinematic for all child rigidbodies
    private void SetKinematicForAllChildren(GameObject obj, bool isKinematic)
    {
        Rigidbody[] rigidbodies = obj.GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rigidbodies)
        {
            rb.isKinematic = isKinematic;
        }
    }

    // Helper method to check if the pointer is over a UI element
    private bool IsPointerOverUIElement()
    {
        // Check mouse input
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return true;
        }

        // Check touch input
        if (Input.touchCount > 0 && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
        {
            return true;
        }

        return false;
    }

    // Method to get the count of all block objects in the spawned prefab
    public int GetBlockCount()
    {
        if (_spawnedObject != null)
        {
            return _spawnedObject.GetComponentsInChildren<Durability>().Length;
        }
        return 0;
    }

    // Method to be called when a block is destroyed
    public void OnBlockDestroyed()
    {
        FindObjectOfType<GameManager>().IncrementDestroyedBlockCount();
    }
}
