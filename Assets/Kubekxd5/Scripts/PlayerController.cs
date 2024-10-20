using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player info:")] public string playerName;
    public int score, scoreMultiplier;
    public enum ShipState
    {
        Normal,
        Damaged,
        Invincible,
        Stealth
    };
    [Header("Ship State:")] public ShipState shipState;
    
    [Header("Equipment:")] public GameObject shipSlot;
    public GameObject hangarSlot, specialAbilityChipSlot, powerUpSlot;
    public List<WeaponController> primaryWeapons = new List<WeaponController>();
    public List<WeaponController> secondaryWeapons = new List<WeaponController>();
    
    [Header("Defense & Durability:")] public float health;
    public float hullIntegrity,energyShield, damageReduction;

    [Header("Regeneration & Recovery:")] public float shieldRegenRate;
    public float healthRegenRate;

    [Header("Mobility & Performance:")] public float speed;
    public float maxSpeed, maneuverability, boostCharge; 
    [Range(0.0f, 1.0f)] public float boostStrength;

    [Header("Damage Resistance:")] 
    [Range(0.0f, 2.0f)] public float plasmaRes;
    [Range(0.0f, 2.0f)] public float energyRes;
    [Range(0.0f, 2.0f)] public float explosionRes;
    [Range(0.0f, 2.0f)] public float kineticRes;
    [Range(0.0f, 2.0f)] public float collisionRes;
    
    private Rigidbody _rb;
    private void Start()
    {
        _rb = gameObject.GetComponent<Rigidbody>();
        ShipSlots[] shipSlots = GetComponentsInChildren<ShipSlots>();
        foreach (ShipSlots slot in shipSlots)
        {
            if (slot.weaponController != null)
            {
                switch (slot.weaponMount)
                {
                    case ShipSlots.WeaponMount.Primary:
                        primaryWeapons.Add(slot.weaponController);
                        Debug.Log("Primary weapon added: " + slot.weaponController.weaponName);
                        break;
                    case ShipSlots.WeaponMount.Secondary:
                        secondaryWeapons.Add(slot.weaponController);
                        Debug.Log("Secondary weapon added: " + slot.weaponController.weaponName);
                        break;
                    // Add more cases if needed for other weapon mounts
                }
            }
        }
    }

    private void Update()
    {
        RotationHandler();
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot(primaryWeapons);
        }
        if (Input.GetButtonDown("Fire2"))
        {
            Shoot(secondaryWeapons);
        }
    }

    private void FixedUpdate()
    {
        //ForwardMovement();
    }

    private void RotationHandler()
    {
        float forwardInput = Input.GetAxis("Vertical");
        float turnInput = Input.GetAxis("Horizontal");
        
        transform.Translate(Vector3.forward * (forwardInput * speed * Time.deltaTime));
        
        if (Math.Abs(forwardInput) < 0.01f)
        {
            _rb.velocity *= 0.99f;
        }
        
        transform.Rotate(Vector3.up, turnInput * (maneuverability*10) * Time.deltaTime);
    }

    /*private void ForwardMovement()
    {
        float forwardInput = Input.GetAxis("Vertical");
        Vector3 forwardVelocity = Vector3.Project(_rb.velocity, transform.forward);
        float currentSpeed = forwardVelocity.magnitude;
        
        if (forwardInput > 0)
        {
            if (currentSpeed < maxSpeed)
            {
                _rb.AddForce(transform.forward * ((speed*10) * Time.fixedDeltaTime * forwardInput), ForceMode.Acceleration);
            }
        }
        else if (forwardInput < 0)
        {
            _rb.AddForce(transform.forward * ((speed*10) * Time.fixedDeltaTime * forwardInput), ForceMode.Acceleration);
        }

        //Spowalnia gracza z czasem
        
        
        //Debug.Log("Current speed:" + _rb.velocity.magnitude);
    }*/
    
    private void Shoot(List<WeaponController> weaponControllers)
    {
        foreach (var weapon in weaponControllers)
        {
            if (weapon != null)
            {
                weapon.Shoot();
            }
        }
    }

}
