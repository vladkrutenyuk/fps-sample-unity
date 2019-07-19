using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Damagable : MonoBehaviour
{
	public float health, healthMax = 100;
	public Image healthBar;

	public void TakeDamage(float amount)
	{
		Debug.Log(gameObject.name + " Damaged " + amount);
		health -= amount;
		healthBar.fillAmount = health / healthMax;
		if (health <= 0)
		{
			Debug.Log("Death...");
			Die();
		}
	}

	void Die()
	{
		Destroy(gameObject);
	}
	
}
