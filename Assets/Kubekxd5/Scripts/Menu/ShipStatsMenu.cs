using TMPro;
using UnityEngine;

public class ShipStatsMenu : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI hullLevelText;
    public TextMeshProUGUI maneuverabilityText;
    public TextMeshProUGUI boostChargeText;
    public TextMeshProUGUI speedText;

    private ShipController shipController;

    private void Start()
    {
        GameObject playerShip = GameObject.FindWithTag("PlayerShip");
        if (playerShip != null)
        {
            shipController = playerShip.GetComponent<ShipController>();
            UpdateUI();
        }
        else
        {
            //Debug.LogError("Player ship not found!");
        }
    }

    private void Update()
    {
        if (shipController == null)
        {
            GameObject playerShip = GameObject.FindWithTag("PlayerShip");
            if (playerShip != null)
            {
                shipController = playerShip.GetComponent<ShipController>();
            }
        }
        if (shipController != null)
        {
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        if (healthText != null)
        {
            healthText.text = $"Health: {Mathf.RoundToInt(shipController.health)}";
        }

        if (hullLevelText != null)
        {
            hullLevelText.text = $"Hull: {Mathf.RoundToInt(shipController.hullLevel)} / 5";
        }

        if (speedText != null)
        {
            speedText.text = $"Speed: {Mathf.RoundToInt(shipController.speed)} / 40";
        }

        if (maneuverabilityText != null)
        {
            maneuverabilityText.text = $"Maneuverability: {Mathf.RoundToInt(shipController.maneuverability)} / 40";
        }

        if (boostChargeText != null)
        {
            boostChargeText.text = $"Boost Charge: {Mathf.RoundToInt(shipController.boostCharge)}";
        }
    }
}