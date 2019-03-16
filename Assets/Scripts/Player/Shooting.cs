using System.Collections;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    public MaxAmmoAvailable maxAmmo;
    public AvailableAmmo availableAmmo;
    public Transform barrelEnd;
    public ParticleSystem shooting;
    public GameObject barrel;

    public int maxDistanceFromTarget = 50;
    public float despawnTime = 3.0f;

    public bool shootAble = true;
    public float waitBeforeNextShot = 0.25f;

    public AudioClip shootSound;
    public float shootRadiusIntensity = 25f;
    public LayerMask enemiesLayer;

    private AudioSource audioSource;
    public AimBehaviourBasic aimBehaviour;

    private void Start()
    {
        maxAmmo.SetAmount(100);
        availableAmmo.SetAmount(100);

        audioSource = GetComponent<AudioSource>();

        barrel.SetActive(false);
    }

    private void Update()
    {
        if (aimBehaviour.aim && Input.GetKey(KeyCode.Mouse0) && shootAble && availableAmmo.IsAmmoAvailable())
        {
            shootAble = false;
            Shoot();
            StartCoroutine(ShootingYield());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Collectable Ammo")
        {
            var ammo = other.GetComponent<AmmoCollectable>();

            if (!ammo.picked)
            {
                var total = availableAmmo.Ammo + ammo.amount;
                int pickedAmount = ammo.amount;

                if (availableAmmo.Ammo == maxAmmo.Ammo) //full, dont need to pick
                {
                    //FloatingTextHandler.CreateFloatingText(transform.position, "Ammo Is Full", Color.red, 25);
                    return;
                }
                else if (total > maxAmmo.Ammo) //not full, but need a bit of
                {
                    pickedAmount = maxAmmo.Ammo - availableAmmo.Ammo;
                    availableAmmo.SetAmount(maxAmmo.Ammo);
                }
                else //need full amount
                {
                    availableAmmo.IncreaseAmmo(ammo.amount);
                }

                ammo.Pick(pickedAmount);
            }
        }
    }

    private IEnumerator ShootingYield()
    {
        yield return new WaitForSeconds(waitBeforeNextShot);
        shootAble = true;
    }

    private IEnumerator ShootingFlashYield()
    {
        yield return new WaitForSeconds(0.1f);
        barrel.SetActive(false);
    }

    private void Shoot()
    {
        barrel.SetActive(true);
        availableAmmo.DecreaseAmmo(1);
        StartCoroutine(ShootingFlashYield());

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0));
        if (Physics.Raycast(ray, out hit, maxDistanceFromTarget))
        {
            hit.transform.SendMessage("OnShootHit", hit.point, SendMessageOptions.DontRequireReceiver);
        }

        #region Bullet Sound

        audioSource.PlayOneShot(shootSound);

        Collider[] enemiesNearBy = Physics.OverlapSphere(transform.position, shootRadiusIntensity, enemiesLayer);
        foreach (var enemy in enemiesNearBy)
        {
            var enemyPatrol = enemy.GetComponent<EnemyPatrolMovement>();
            if (enemyPatrol != null)
                enemyPatrol.OnAware();
        }

        #endregion
    }
}