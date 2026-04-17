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
        public LayerMask hitLayers;

        [Header("Fire Rate (Per second)")]
        public float firerate = 1000f;
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
        public TMP_Text ammoText;


        void Start()
        {
        currentAmmo = maxAmmo;
        UpdateAmmoUI();
        }



        void Update()
        {
            if (canShoot && Input.GetMouseButton(0) && Time.time >= nextTimeToFire)
            {
                if (currentAmmo > 0)
                {
                    Shoot();
                    currentAmmo--;
                    UpdateAmmoUI();
                    nextTimeToFire = Time.time + 1f / firerate; // Set next allowed shot time
                }
                else
                {
                    StartReloading();
                }
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                StartReloading();
            }
        }

         if (Input.GetKeyDown(KeyCode.R))
        {
            StartReloading();
        }
       }
        IEnumerator Delay()
        {
            yield return new WaitForSeconds(1f / firerate);
            canShoot = true;
        }


            void Shoot()
        {
            if (muzzleFlash != null)
                muzzleFlash.Play();

            RaycastHit hit;

            Vector3 origin = playerCamera.transform.position;
            Vector3 direction = playerCamera.transform.forward;

            if (Physics.Raycast(origin, direction, out hit, range, hitLayers))
            {
                Debug.Log("Hit: " + hit.transform.name);

                Health targetHealth = hit.transform.GetComponent<Health>();
                if (targetHealth != null)
                {
                    targetHealth.TakeDamage(damage);
                }

                if (impactEffect != null)
                {
                    GameObject impact = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                    Destroy(impact, 2f);
                }
            }
        }
        void StartReloading()
        {
        if (!isReloading && currentAmmo < maxAmmo)
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
