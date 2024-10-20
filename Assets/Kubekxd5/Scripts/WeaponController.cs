using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("Weapon Info:")] 
    public string weaponName;

    public enum WeaponDmgType
    {
        Plasma,
        Energy,
        Explosive,
        Kinetic
    };
    [Header("Weapon Damage Type:")] 
    public WeaponDmgType weaponDmgType;

    public enum WeaponMount
    {
        Primary,
        Secondary,
        Hangar,
        Special
    };
    [Header("Weapon Mount:")] 
    public WeaponMount weaponMount;

    [Header("Weapon Stats:")] 
    public float damageValue;
    [Range(0.0f, 2.0f)] public float damageMultiplier;
    public float piercing;
    [Range(0.0f, 10.0f)] public float explosionRadius;

    [Header("Fire Control:")]
    public float fireRate;
    public float range;
    public float weaponCooldown;

    [Header("Projectile Properties:")]
    public float projectileSpeed;
    public int projectileAmount;
    public float projectileInterval;

    [Header("Overheat & Ammo:")]
    public float overheatThreshold;
    public int ammoMax, ammoCurrent;

    [Header("Upgrade Module:")] 
    public GameObject weaponUpgradeModule;

    [Header("Visual & Sound Effects:")] 
    public ParticleSystem weaponVfx;
    public AudioSource weaponSfx;
    public ParticleSystem overheatEffect;

    private float _nextFireTime;

    private void Start()
    {
        _nextFireTime = 0f;
    }

    public void Shoot()
    {
        if (Time.time >= _nextFireTime && ammoCurrent > 0 && ammoMax!=0)
        {
            _nextFireTime = Time.time + fireRate;
            ammoCurrent--;
            weaponVfx.Play();
            weaponSfx.Play();
        }
        else if (ammoCurrent <= 0 && ammoMax!=0)
        {
            //Debug.Log("Out of ammo!");
        }
        else if (ammoMax == 0)
        {
            _nextFireTime = Time.time + fireRate;
            ammoCurrent--;
            weaponVfx.Play();
            weaponSfx.Play();
        }
    }
}
