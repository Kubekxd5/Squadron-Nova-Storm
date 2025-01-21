using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public ListManager shipsList, weaponList;
    public GameObject currentEquippedShip, playerHud, renderPreview;
    public ShipConfigObject selectedShipConfig;
    public WFC_Script wfcScript;

    [Header("Player Info:")]
    public string playerName;
    public int score, scoreMultiplier = 1;
    public TextMeshProUGUI scoreView;
    public event Action OnPlayerShipSpawned;
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

        if (IsGameScene())
        {
            SetupGameScene();
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

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

        GameObject wfcObject = GameObject.FindGameObjectWithTag("WFC");
        if (wfcObject != null)
        {
            wfcScript = wfcObject.GetComponent<WFC_Script>();
        }
        else
        {
            Debug.LogWarning("WFC object with tag 'WFC' not found!");
        }

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

    public void UpdateScore()
    {
        scoreView.text = $"{playerName} - score: {score}";
    }

    public void IncreaseScore(int amount)
    {
        score += amount;
        UpdateScore();
    }

    public void ResetScore()
    {
        score = 0;
        UpdateScore();
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
            Transform slotTransform = FindSlotOnShip(slotConfig.slotName);
            if (slotTransform == null)
            {
                Debug.LogWarning($"Slot '{slotConfig.slotName}' not found on the ship!");
                continue;
            }

            GameObject weaponPrefab = weaponList.GetObjectByName(slotConfig.weaponName);
            if (weaponPrefab == null)
            {
                Debug.LogWarning($"Weapon '{slotConfig.weaponName}' not found in weapon list!");
                continue;
            }

            GameObject weaponInstance = Instantiate(weaponPrefab, slotTransform);
            weaponInstance.transform.localPosition = Vector3.zero;
            weaponInstance.transform.localRotation = Quaternion.identity;
            weaponInstance.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        }
    }

    private Transform FindSlotOnShip(string slotName)
    {
        foreach (Transform child in currentEquippedShip.GetComponentsInChildren<Transform>())
        {
            if (child.name == slotName)
            {
                return child;
            }
        }
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
