using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableWeaponEnemy : MonoBehaviour
{
	public Vector2 direction;
	public bool hasHit = false;
	public float speed = 10f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
		if ( !hasHit)
		GetComponent<Rigidbody2D>().velocity = direction * speed;
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
        if (collision.gameObject.tag == "Player")
        {
            object[] values = new object[2];
            values[0] = Mathf.Sign(direction.x) * 2f;
            values[1] = transform.position;
            collision.gameObject.SendMessage("ApplyDamage", values);
            Destroy(gameObject);
        }
        else if (collision.gameObject.tag != "Enemy")
		{
			Destroy(gameObject);
		}
	}
}
