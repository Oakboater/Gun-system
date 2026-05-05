using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Gun : MonoBehaviour
{
    public WeaponData data;

    [Header("References")]
    public Camera cam;
    public Transform recoilPivot;
    public ParticleSystem tracer;

    int ammo;
    float nextFire;
    bool reloading;

    Vector2 recoil;
    Vector2 recoilVel;

    Dictionary<Transform, float> shotDamage = new();

    void OnEnable()
    {
        ammo = data.magSize;
    }

    void Update()
    {
        if (reloading) return;

        HandleInput();
        UpdateRecoil();
    }

    void HandleInput()
    {
        bool fire =
            data.fireType == WeaponData.FireType.Auto
            ? Input.GetMouseButton(0)
            : Input.GetMouseButtonDown(0);

        if (fire) TryShoot();

        if (Input.GetKeyDown(KeyCode.R))
            StartCoroutine(Reload());
    }

    public void TryShoot()
    {
        if (Time.time < nextFire) return;
        if (ammo <= 0) { StartCoroutine(Reload()); return; }

        ammo--;
        nextFire = Time.time + 1f / data.fireRate;

        recoil += new Vector2(
            Random.Range(-data.recoilX, data.recoilX),
            data.recoilY
        );

        if (data.fireType == WeaponData.FireType.Shotgun)
            FireShotgun();
        else
            FireSingle();
    }

    void FireSingle()
    {
        Shoot(cam.transform.forward);
    }

    void FireShotgun()
    {
        shotDamage.Clear();

        for (int i = 0; i < data.pellets; i++)
        {
            Vector2 offset = Random.insideUnitCircle * data.spread;

            Vector3 dir =
                cam.transform.forward +
                cam.transform.right * offset.x +
                cam.transform.up * offset.y;

            Shoot(dir.normalized);
        }
    }

    void Shoot(Vector3 dir)
    {
        Vector3 origin = cam.transform.position;

        if (Physics.Raycast(origin, dir, out RaycastHit hit, data.range))
        {
            SpawnTracer(origin, hit.point);

            float dmg = CalculateDamage(hit.distance);
            ApplyDamage(hit.transform, dmg);
        }
        else
        {
            SpawnTracer(origin, origin + dir * data.range);
        }
    }

    float CalculateDamage(float dist)
    {
        if (dist <= data.falloffStart)
            return data.damage;

        float drop = (dist - data.falloffStart) * data.falloffRate;
        return Mathf.Max(data.minDamage, data.damage - drop);
    }

    void ApplyDamage(Transform t, float dmg)
    {
        if (t.TryGetComponent(out Health hp))
            hp.TakeDamage(dmg);
    }

    void SpawnTracer(Vector3 start, Vector3 end)
    {
        if (!tracer) return;

        ParticleSystem.EmitParams ep = new()
        {
            position = start,
            velocity = (end - start).normalized * data.tracerSpeed
        };

        tracer.Emit(ep, 1);
    }

    void UpdateRecoil()
    {
        recoil = Vector2.SmoothDamp(recoil, Vector2.zero, ref recoilVel, 0.08f);

        recoilPivot.localRotation = Quaternion.Euler(-recoil.y, recoil.x, 0);
    }

    IEnumerator Reload()
    {
        reloading = true;
        yield return new WaitForSeconds(data.reloadTime);
        ammo = data.magSize;
        reloading = false;
    }

    public int GetAmmo()
    {
    return ammo;
    }

}
