using UnityEngine;
using TMPro;
using System.Collections;

public class Gun : MonoBehaviour
{
    public enum FireType { Auto, Semi, Shotgun }

    [Header("Weapon Type")]
    public FireType fireType;

    [Header("Stats")]
    public float damage = 25f;
    public float range = 100f;
    public float fireRate = 10f;

    [Header("Shotgun Settings")]
    public PelletShape pelletShape = PelletShape.Cluster;
    public int pelletCount = 8;
    public float spreadSize = 0.06f;
    public float innerSpread = 0.02f;
    public float pelletRandomness = 0.01f;

    public enum PelletShape
    {
        Cluster,
        Cross,
        Ring,
        Horizontal,
        Vertical,
        DoubleCluster
    }

    [Header("Damage Falloff")]
    public float falloffStart = 15f;
    public float falloffRate = 1.2f;
    public float minDamage = 5f;

    [Header("Ammo")]
    public int maxAmmo = 30;
    public int currentAmmo;
    public float reloadTime = 1.5f;

    [Header("References")]
    public Camera cam;
    public TMP_Text ammoText;
    public GameObject impactEffect;

    private float nextFireTime;
    private bool reloading;

    void Start()
    {
        currentAmmo = maxAmmo;
        UpdateUI();
    }

    void Update()
    {
        if (reloading) return;

        HandleInput();
    }

    void HandleInput()
    {
        switch (fireType)
        {
            case FireType.Auto:
                if (Input.GetMouseButton(0))
                    TryShoot();
                break;

            case FireType.Semi:
            case FireType.Shotgun:
                if (Input.GetMouseButtonDown(0))
                    TryShoot();
                break;
        }

        if (Input.GetKeyDown(KeyCode.R))
            StartCoroutine(Reload());
    }

    void TryShoot()
    {
        if (Time.time < nextFireTime) return;

        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        currentAmmo--;
        nextFireTime = Time.time + 1f / fireRate;

        switch (fireType)
        {
            case FireType.Shotgun:
                FireShotgun();
                break;

            default:
                FireSingle();
                break;
        }

        UpdateUI();
    }

    void FireSingle()
    {
        ShootRay(cam.transform.forward);
    }

    void FireShotgun()
    {
        Vector2[] pattern = GeneratePattern();

        foreach (Vector2 p in pattern)
        {
            Vector2 offset = p + Random.insideUnitCircle * pelletRandomness;

            Vector3 dir =
                cam.transform.forward +
                cam.transform.right * offset.x +
                cam.transform.up * offset.y;

            ShootRay(dir.normalized);
        }
    }

    Vector2[] GeneratePattern()
    {
        switch (pelletShape)
        {
            case PelletShape.Cross:
                return new Vector2[]
                {
                    Vector2.zero,
                    Vector2.right * spreadSize,
                    Vector2.left * spreadSize,
                    Vector2.up * spreadSize,
                    Vector2.down * spreadSize
                };

            case PelletShape.Ring:
            {
                Vector2[] arr = new Vector2[pelletCount];

                for (int i = 0; i < pelletCount; i++)
                {
                    float a = i * Mathf.PI * 2f / pelletCount;
                    arr[i] = new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * spreadSize;
                }

                return arr;
            }

            case PelletShape.Horizontal:
            {
                Vector2[] arr = new Vector2[pelletCount];

                for (int i = 0; i < pelletCount; i++)
                {
                    float t = (i / (float)(pelletCount - 1)) * 2f - 1f;
                    arr[i] = new Vector2(t * spreadSize, 0);
                }

                return arr;
            }

            case PelletShape.Vertical:
            {
                Vector2[] arr = new Vector2[pelletCount];

                for (int i = 0; i < pelletCount; i++)
                {
                    float t = (i / (float)(pelletCount - 1)) * 2f - 1f;
                    arr[i] = new Vector2(0, t * spreadSize);
                }

                return arr;
            }

            case PelletShape.DoubleCluster:
            {
                Vector2[] arr = new Vector2[pelletCount];

                for (int i = 0; i < pelletCount; i++)
                {
                    Vector2 center =
                        (i % 2 == 0)
                        ? Vector2.left * spreadSize
                        : Vector2.right * spreadSize;

                    arr[i] = center + Random.insideUnitCircle * innerSpread;
                }

                return arr;
            }

            default:
            {
                Vector2[] arr = new Vector2[pelletCount];

                for (int i = 0; i < pelletCount; i++)
                    arr[i] = Random.insideUnitCircle * spreadSize;

                return arr;
            }
        }
    }

    void ShootRay(Vector3 dir)
    {
        if (Physics.Raycast(cam.transform.position, dir, out RaycastHit hit, range))
        {
            float dmg = CalculateDamage(hit.distance);

            Health hp = hit.transform.GetComponent<Health>();
            if (hp != null)
                hp.TakeDamage(dmg);

            if (impactEffect != null)
            {
                Instantiate(
                    impactEffect,
                    hit.point,
                    Quaternion.LookRotation(hit.normal)
                );
            }
        }
    }

    float CalculateDamage(float distance)
    {
        if (distance <= falloffStart)
            return damage;

        float drop = (distance - falloffStart) * falloffRate;
        return Mathf.Max(minDamage, damage - drop);
    }

    IEnumerator Reload()
    {
        reloading = true;

        if (ammoText)
            ammoText.text = "Reloading...";

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = maxAmmo;
        reloading = false;

        UpdateUI();
    }

    void UpdateUI()
    {
        if (ammoText)
            ammoText.text = $"{currentAmmo} / {maxAmmo}";
    }
}
