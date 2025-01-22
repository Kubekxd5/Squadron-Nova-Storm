using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public ListManager shipsList, weaponList;
    public GameObject currentEquippedShip, playerHud, renderPreview;
    public TMP_InputField inputField;
    public ShipConfigObject selectedShipConfig;
    public WFC_Script wfcScript;

    [Header("Player Info:")]
    public string playerName;
    public int score;
    public int scoreMultiplier = 1;
    public TextMeshProUGUI scoreView;

    [Header("Combo System:")]
    public float comboDuration = 5f;
    private int currentKillCount = 0;
    private float comboTimer = 0f;
    private bool isComboActive = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        SceneManager.sceneLoaded += OnSceneLoaded;

        if (inputField != null)
        {
            inputField.onEndEdit.AddListener(SetPlayerName);
        }

        if (IsGameScene()) SetupGameScene();
    }

    private void Update()
    {
        if (isComboActive)
        {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0f)
            {
                ResetCombo();
            }
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        if (inputField != null)
        {
            inputField.onEndEdit.RemoveListener(SetPlayerName);
        }
    }

    public event Action OnPlayerShipSpawned;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (IsGameScene())
        {
            SetupGameScene();
            UpdateScore();
        }
    }

    private void SetupGameScene()
    {
        renderPreview.SetActive(true);
        playerHud.SetActive(false);
        Debug.Log("GameManager: playerHud set to inactive.");

        var wfcObject = GameObject.FindGameObjectWithTag("WFC");
        if (wfcObject != null)
            wfcScript = wfcObject.GetComponent<WFC_Script>();
        else
            Debug.LogWarning("WFC object with tag 'WFC' not found!");

        StartCoroutine(WaitForWorldGeneration());
    }

    private IEnumerator WaitForWorldGeneration()
    {
        Debug.Log("Waiting for world generation...");
        yield return new WaitUntil(() => wfcScript != null && wfcScript.IsGenerationComplete());

        Debug.Log("World generation complete. Activating HUD and spawning ship.");
        renderPreview.SetActive(false);
        SpawnPlayerShip();
        playerHud.SetActive(true);
    }
    
    private void SetPlayerName(string name)
    {
        if (!string.IsNullOrWhiteSpace(name))
        {
            playerName = name.Trim();
            Debug.Log($"Player name set to: {playerName}");
            UpdateScore(); // Update score display to include the new name
        }
        else
        {
            Debug.LogWarning("Player name cannot be empty!");
        }
    }
    
    public void UpdateScore()
    {
        string multiplierText = scoreMultiplier > 1 ? $" x{scoreMultiplier}" : string.Empty;

        // Update text color based on multiplier
        Color multiplierColor = GetMultiplierColor(scoreMultiplier);
        scoreView.color = multiplierColor;

        scoreView.text = $"{(string.IsNullOrEmpty(playerName) ? "Player" : playerName)} - score: {score}{multiplierText}";
    }

    private Color GetMultiplierColor(int multiplier)
    {
        switch (multiplier)
        {
            case 2: return Color.cyan;
            case 3: return Color.yellow;
            case 4: return new Color(1f, 0.5f, 0f);
            case 5: return Color.red;
            case 6: return Color.magenta;
            default: return Color.white;
        }
    }

    public void IncreaseScore(int amount)
    {
        score += amount * scoreMultiplier;
        UpdateScore();
        HandleCombo();
    }

    private void HandleCombo()
    {
        currentKillCount++;
        comboTimer = comboDuration;

        if (currentKillCount >= GetKillsForNextMultiplier())
        {
            scoreMultiplier = Mathf.Min(scoreMultiplier + 1, 6); // Cap multiplier at 6
            currentKillCount = 0; // Reset kill count for the next multiplier
        }

        isComboActive = true;
        UpdateScore();
    }

    private int GetKillsForNextMultiplier()
    {
        return scoreMultiplier < 3 ? 4 : 8; // First 2 multipliers require 4 kills, then 8 kills for subsequent multipliers
    }

    private void ResetCombo()
    {
        isComboActive = false;
        scoreMultiplier = 1;
        currentKillCount = 0;
        UpdateScore();
    }

    public void ResetScore()
    {
        score = 0;
        ResetCombo();
    }

    public void SpawnPlayerShip()
    {
        if (selectedShipConfig == null || shipsList.listOfObjects.Length <= selectedShipConfig.shipIndex)
        {
            Debug.LogWarning("Invalid ship configuration or ship index!");
            return;
        }

        currentEquippedShip = Instantiate(shipsList.listOfObjects[selectedShipConfig.shipIndex]);
        currentEquippedShip.transform.localPosition = new Vector3(0, 5, 0);
        currentEquippedShip.transform.localRotation = Quaternion.identity;
        currentEquippedShip.transform.localScale = new Vector3(5, 5, 5);
        currentEquippedShip.tag = "PlayerShip";

        AttachWeaponsToShip();

        OnPlayerShipSpawned?.Invoke();
    }

    private void AttachWeaponsToShip()
    {
        if (selectedShipConfig == null || currentEquippedShip == null)
        {
            Debug.LogWarning("Cannot attach weapons: Missing ship configuration or ship instance.");
            return;
        }

        foreach (var slotConfig in selectedShipConfig.weaponSlots)
        {
            var slotTransform = FindSlotOnShip(slotConfig.slotName);
            if (slotTransform == null)
            {
                Debug.LogWarning($"Slot '{slotConfig.slotName}' not found on the ship!");
                continue;
            }

            var weaponPrefab = weaponList.GetObjectByName(slotConfig.weaponName);
            if (weaponPrefab == null)
            {
                Debug.LogWarning($"Weapon '{slotConfig.weaponName}' not found in weapon list!");
                continue;
            }

            var weaponInstance = Instantiate(weaponPrefab, slotTransform);
            weaponInstance.transform.localPosition = Vector3.zero;
            weaponInstance.transform.localRotation = Quaternion.identity;
            weaponInstance.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        }
    }

    private Transform FindSlotOnShip(string slotName)
    {
        foreach (var child in currentEquippedShip.GetComponentsInChildren<Transform>())
            if (child.name == slotName)
                return child;
        return null;
    }

    private bool IsGameScene()
    {
        return SceneManager.GetActiveScene().buildIndex == 1;
    }

    public void ClearSelectedShip()
    {
        selectedShipConfig = null;
    }
}
