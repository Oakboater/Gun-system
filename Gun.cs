using UnityEngine;
using UnityEngine.UI;

public class Gun : MonoBehaviour
{

    [Header("Shooting")]
    public float damage = 25f;
    public float range = 100f;
    public Transform barrelEnd;
    public Camera playerCamera;


    [Header("Fire Rate")]
    public float firerate = 10f;
    private float nextTimeToFire = 0f;

    [Header("Ammo")]
    public int maxAmmo = 30;
    public int currentAmmo;
    public float reloadTime = 1.5f;
    private bool isReloading = false;

    [Header("Effects")]
    public ParticleSystem muzzleFlash; // Optional
    public GameObject impactEffect;    // Optional hit effect

    [Header("UI")]
    public Text ammoText;


    void Start()
    {
    currentAmmo = maxAmmo;
    UpdateAmmoUI();
    }
    
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        if (muzzleFlash != null)
            muzzleFlash.Play();

        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, range))
        {
            Health targetHealth = hit.transform.GetComponent<Health>();
            if (targetHealth != null)
            {
                targetHealth.TakeDamage(damage);
                Debug.Log("Damage Dealt");
            }

            // Optional: spawn impact effect
            if (impactEffect != null)
            {
                GameObject impact = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impact, 2f);
            }
        }
    }
}
