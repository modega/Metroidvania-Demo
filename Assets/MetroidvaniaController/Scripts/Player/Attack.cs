﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
	public float dmgValue = 4;
	public GameObject throwableObject;
	public Transform attackCheck;
	private Rigidbody2D m_Rigidbody2D;
	public Animator animator;
	public bool canAttack = true;
	public bool isTimeToCheck = false;

	public GameObject cam;

	private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();
	}

	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		if ( (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.Joystick1Button2)) && canAttack)
		{
			canAttack = false;
			animator.SetBool("IsAttacking", true);
			StartCoroutine(AttackCooldown());
		}

		if (Input.GetKeyDown(KeyCode.V) || Input.GetKeyDown(KeyCode.Joystick1Button3))
		{
			GameObject throwableWeapon = Instantiate(throwableObject, transform.position + new Vector3(transform.localScale.x * 0.5f,-0.2f), Quaternion.identity) as GameObject; 
			Vector2 direction = new Vector2(transform.localScale.x, 0);
			throwableWeapon.GetComponent<ThrowableWeapon>().direction = direction; 
			throwableWeapon.name = "ThrowableWeapon";
		}
		if(Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.Joystick1Button5))
		{
			GetComponent<CharacterController2D>().isEmpowered = true;
		}
	}

	IEnumerator AttackCooldown()
	{
		yield return new WaitForSeconds(0.25f);
		canAttack = true;
	}

	public void DoDashDamage()
	{
		CharacterController2D charControl = gameObject.GetComponent<CharacterController2D>();
        dmgValue = (charControl.isParry && charControl.empoweredAttackCount < 1f) ? Mathf.Abs(dmgValue) * 2 : Mathf.Abs(dmgValue);
		Collider2D[] collidersEnemies = Physics2D.OverlapCircleAll(attackCheck.position, 0.9f);
		for (int i = 0; i < collidersEnemies.Length; i++)
		{
			if (collidersEnemies[i].gameObject.tag == "Enemy")
			{
				if (charControl.isParry)
				{
					charControl.empoweredAttackCount++;
                    charControl.isParry = false;
                }
				if (collidersEnemies[i].transform.position.x - transform.position.x < 0)
				{
					dmgValue = -dmgValue;
                    charControl.empoweredAttackCount = 0;

                }
				collidersEnemies[i].gameObject.SendMessage("ApplyDamage", dmgValue);
				cam.GetComponent<CameraFollow>().ShakeCamera();
			}
		}
	}
}
