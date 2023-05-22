using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableWeaponEnemy : MonoBehaviour
{
	public Vector2 direction;
	public bool hasHit = false;
	public float speed = 15f;
	public GameObject owner;

    void FixedUpdate()
    {
		if (!hasHit) GetComponent<Rigidbody2D>().velocity = direction * speed;
	}
	void OnCollisionEnter2D(Collision2D collision)
	{

        if (owner != null && collision.gameObject != owner && collision.gameObject.tag == "Player" && collision.gameObject.GetComponent<CharacterController2D>().isEmpowered)
        {
            collision.gameObject.GetComponent<CharacterController2D>().increaseBlood(1);
            Destroy(gameObject);
        }
        else if (owner != null && collision.gameObject != owner && collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<CharacterController2D>().ApplyDamage(2f, transform.position);
            Destroy(gameObject);
        }
        else if (collision.gameObject.tag != "Enemy" && collision.gameObject.tag != "Player")
		{
			Destroy(gameObject);
		}
	}
}
