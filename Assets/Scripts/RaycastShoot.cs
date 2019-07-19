using System.Collections;
using UnityEngine;

public class RaycastShoot : MonoBehaviour
{
	public float weaponDamage = 10; // Урон
	public float weaponFireRate = 15f; // Скорострельность (выстрелы/мин)
	public float weaponRange = 100f; // Макимальное расстояние выстрела
	public float weaponHitForce = 100f;  // С какой силой действует выстрел при попадании в объект с физикой
	public float wpnDvtnVer = .035f; // Коэфф. вертикального разброса
	public float wpnDvtnHor = .035f; // Коэфф. горизонтального разброса

	public int i = 0; // Перемення контроля заброса
	private float nextTimeToZeroDvnt; // Время до сброса разброса донуля после стрельбы
	public float timeToZeroDvnt = .05f; // Промежуток этого времени
	public int mltUpDvnt = 6; // Ограничение для разброса (i)

	public int maxAmmo = 10;
	public int currentAmmo;
	public float weaponReloadTime = 2.5f;
	private bool isReloading;

	public Camera fpsCam;
	
	public ParticleSystem shellSpawn;
	public ParticleSystem muzzleFlash1;
	public ParticleSystem muzzleFlash2;
	public ParticleSystem muzzleFlash3;
	public GameObject hitEffectPrefab;

	public GameObject bulletPrefab;
	public Transform bulletSpawn;


	public GameObject shotFXprefab;
	public AudioSource soundReload;

	private Animator animator;

	float shootFloat = 0f;

	void Start()
	{
		animator = GetComponent<Animator>();
		fpsCam = GetComponentInParent<Camera>();

		currentAmmo = maxAmmo;
	}

	void Update()
	{
		#region Reloading...
		if (isReloading)
			return;
		
		if (currentAmmo <= 0)
		{
			StartCoroutine(Reload());
			return;
		}

		if (Input.GetKeyDown(KeyCode.R) && currentAmmo < maxAmmo)
		{
			StartCoroutine(Reload());
			return;
		}
		#endregion Reloading...

		#region Shooting...
		if (weaponFireRate <= 0)
		{
			if (Input.GetButtonDown("Fire1"))
			{
				Shoot();
			}
		} else
		{
			if (Input.GetButtonDown("Fire1"))
			{
				InvokeRepeating("Shoot", 0f, 60f / weaponFireRate);
			} else if (Input.GetButtonUp("Fire1"))
			{
				CancelInvoke("Shoot");
			}
		}
		#endregion Shooting...

		if (Time.time >= nextTimeToZeroDvnt)
		{
			i = 0;
		}

		animator.SetFloat("ShootFloat", shootFloat);
		shootFloat = Mathf.Lerp(shootFloat, 0, Time.deltaTime * 0.6f);
	}

	void Shoot()
	{
		currentAmmo--;

		#region ShootEffects
		shootFloat = 1.1f;
		muzzleFlash1.Play();
		muzzleFlash2.Play();
		muzzleFlash3.Play();
		shellSpawn.Play();

		GameObject shotFX = Instantiate<GameObject>(shotFXprefab, gameObject.transform.position, Quaternion.identity);
		Destroy(shotFX, 0.5f);
		#endregion

		#region Deviation of shooting
		nextTimeToZeroDvnt = Time.time + timeToZeroDvnt;
		Vector3 rayDeviatonHor = fpsCam.transform.right * Random.Range(-wpnDvtnHor * i / mltUpDvnt, wpnDvtnHor * i / mltUpDvnt);
		Vector3 rayDeviatonVer = fpsCam.transform.up * Random.Range(-wpnDvtnVer * i / mltUpDvnt, wpnDvtnVer * i / mltUpDvnt);
		if (i < mltUpDvnt)
		{
			i++;
		}
		#endregion

		#region Raycast Shooting
		Vector3 rayDirection = fpsCam.transform.forward + rayDeviatonHor + rayDeviatonVer;
		RaycastHit hit;
		if (Physics.Raycast(fpsCam.transform.position, rayDirection, out hit, weaponRange))
		{
			GameObject impact = (GameObject)Instantiate(hitEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));
			Destroy(impact, .4f);
		}
		#endregion

		var bullet = (GameObject)Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
		bullet.GetComponent<Rigidbody>().velocity = rayDirection * 100;
		Destroy(bullet, 0.2f);
	}

	IEnumerator Reload()
	{
		CancelInvoke("Shoot");
		animator.SetBool("isShooting", false);

		i = 0;

		isReloading = true;
			yield return new WaitForSeconds(.05f);
			animator.SetBool("isReloading", true);
		soundReload.Play();
			yield return new WaitForSeconds(weaponReloadTime - .1f);
			animator.SetBool("isReloading", false);
			yield return new WaitForSeconds(.05f);
		isReloading = false;

		currentAmmo = maxAmmo;
	}
}
