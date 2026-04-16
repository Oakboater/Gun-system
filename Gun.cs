using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
public class Gun : MonoBehaviour
{

    [Header("Shooting")]
    public float damage = 25f;
    public float range = 100f;
    public Transform barrelEnd;
    public Camera playerCamera;
    public bool canShoot = true;


    [Header("Fire Rate (RPM)")]
    public float firerate = 15f;
    private float nextTimeToFire = 60f; // This is so we can make an rpm

    [Header("Ammo")]
    public int maxAmmo = 30;
    public int currentAmmo;
    public float reloadTime = 1.5f;
    private bool isReloading = false;


    [Header("Effects")]
    public ParticleSystem muzzleFlash; // Optional
    public GameObject impactEffect;    // Optional hit effect

    [Header("UI")]
    public TMP_Text ammoText;


    void Start()
    {
    currentAmmo = maxAmmo;
    UpdateAmmoUI();
    }

    void Update()
    {
        if (Input.GetButton("Fire1") && canShoot == true)
        {
            if (currentAmmo > 0)
                Shoot();
                canShoot = false;
                currentAmmo = currentAmmo - 1;
                UpdateAmmoUI();
                StartCoroutine(Delay());

         if (currentAmmo <= 0)
         canShoot = false;
         StartReloading();
        }
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(nextTimeToFire/firerate);
        Debug.Log("off cd");
        if (currentAmmo > 0)
        canShoot = true;
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
    public void StartReloading()
    {
    if (!isReloading && currentAmmo <= 0)
    {
        StartCoroutine(Reloading());
    }
}
    IEnumerator Reloading()
    {
        isReloading = true;
        canShoot = false;

        if (ammoText != null)
        {
            ammoText.text = $"Reloading {reloadTime}";
        }

        yield return new WaitForSeconds(reloadTime);
    
        currentAmmo = maxAmmo;
        canShoot = true;
        isReloading = false;

        UpdateAmmoUI();
}
    void UpdateAmmoUI()
    {
        if (ammoText != null)
        {
            ammoText.text = $"{currentAmmo} / {maxAmmo}";
        }
        
    }
}
