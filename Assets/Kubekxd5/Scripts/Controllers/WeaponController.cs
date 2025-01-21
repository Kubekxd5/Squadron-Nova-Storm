using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("Weapon Info:")] 
    public string weaponName;

    public enum WeaponDmgType { Plasma, Energy, Explosive, Kinetic };
    [Header("Weapon Damage Type:")] 
    public WeaponDmgType weaponDmgType;

    public enum WeaponMount { Primary, Secondary, Hangar, Special };
    [Header("Weapon Mount:")] 
    public WeaponMount weaponMount;

    [Header("Weapon Stats:")] 
    public float damageValue = 10f;
    [Range(0.0f, 2.0f)] public float damageMultiplier = 1f;
    public float piercing = 0f;
    [Range(0.0f, 10.0f)] public float explosionRadius = 0f;

    [Header("Fire Control:")]
    public float fireRate = 0.5f;
    public float range = 20f;

    [Header("Projectile Properties:")]
    public float projectileSpeed = 10f;
    public int projectileAmount = 1;
    public float projectileInterval = 0.1f;

    [Header("Overheat & Ammo:")]
    public bool usesHeat = true;  // New boolean to check if the weapon uses heat for overheat
    public float overheatThreshold = 100f;
    public int ammoMax = 100;
    public int ammoCurrent = 100;
    public float weaponCooldown = 0f;
    public float coolingRate = 10f;

    [Header("Upgrade Module:")] 
    public GameObject weaponUpgradeModule;

    [Header("Visual & Sound Effects:")] 
    public ParticleSystem[] weaponVfx;
    public AudioSource weaponSfx;
    public ParticleSystem overheatEffect;

    public bool isEquippedByPlayer;

    private float _nextFireTime;
    private float _currentHeat;
    private bool _isOverheating;
    private ShipSlot _parentSlot;

    private void Start()
    {
        _parentSlot = gameObject.GetComponentInParent<ShipSlot>();
        _nextFireTime = 0f;

        if (_parentSlot != null)
        {
            _parentSlot.weaponController = this;
            _parentSlot.AddNewWeapon();
            isEquippedByPlayer = true;
            ChangeProjectileLayerMask();
            Debug.Log($"{weaponName} equipped in {weaponMount} slot.");
        }
        else
        {
            Debug.LogWarning("WeaponController: Parent slot not found. Weapon may not function correctly.");
        }

        if (weaponVfx == null || weaponVfx.Length == 0)
        {
            Debug.LogWarning("WeaponController: No weapon visual effects assigned.");
        }

        if (weaponSfx == null)
        {
            Debug.LogWarning("WeaponController: No weapon sound effects assigned.");
        }
    }

    public void Shoot()
    {
        if (_isOverheating)
        {
            //Debug.Log("WeaponController: Weapon is overheating!");
            return;
        }

        if (ammoMax == 0 || ammoCurrent > 0)
        {
            if (Time.time >= _nextFireTime)
            {
                _nextFireTime = Time.time + fireRate;

                if (ammoMax > 0) ammoCurrent--;

                if (usesHeat)  // Only accumulate heat if the weapon uses heat
                {
                    _currentHeat += damageValue;
                    if (_currentHeat >= overheatThreshold)
                    {
                        _isOverheating = true;
                        _nextFireTime = Time.time + weaponCooldown;
                        overheatEffect?.Play();
                        Debug.LogWarning("WeaponController: Weapon overheated!");
                    }
                }

                if (weaponVfx != null)
                {
                    foreach (var weaponvfx in weaponVfx)
                    {
                        weaponvfx?.Play();
                    }
                }
                weaponSfx?.Play();
            }
        }
        else
        {
            Debug.LogWarning("WeaponController: Out of ammo.");
        }
    }

    private void Update()
    {
        if (_isOverheating && usesHeat)
        {
            _currentHeat -= Time.deltaTime * coolingRate;
            if (_currentHeat <= 0)
            {
                _currentHeat = 0;
                _isOverheating = false;
                Debug.Log("WeaponController: Weapon cooled down.");
            }
        }
    }

    private void ChangeProjectileLayerMask()
    {
        if (weaponVfx != null)
        {
            foreach (var particle in weaponVfx)
            {
                if (particle == null) continue;
                var collisionModule = particle.collision;
                collisionModule.collidesWith = isEquippedByPlayer ? LayerMask.GetMask("Enemy") : LayerMask.GetMask("Player");
            }
        }
    }
}
