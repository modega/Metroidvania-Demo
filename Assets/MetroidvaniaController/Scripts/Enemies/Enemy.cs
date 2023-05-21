using UnityEngine;
using System.Collections;
using System;

public class Enemy : MonoBehaviour {

	public float life = 10;
	private bool isPlat;
	private bool isObstacle;
	private Transform fallCheck;
	private Transform wallCheck;
	private Transform playerCheck;
    public GameObject throwableObject;
    public LayerMask turnLayerMask;
	private Rigidbody2D rb;
	private Transform playerLocation; 

	private bool facingRight = true;
	
	public float speed = 5f;

	public bool isInvincible = false;
	private bool isHitted = false;
	private bool canShoot = false;
	private bool canAttack = true;

	void Awake () {
		fallCheck = transform.Find("FallCheck");
		wallCheck = transform.Find("WallCheck");
        playerCheck = transform.Find("PlayerCheck");
        rb = GetComponent<Rigidbody2D>();
	}

    private void Update()
    {
        if (canShoot)
        {
			rb.velocity = Vector2.zero;
			Vector2 dir = (facingRight) ? Vector2.left : Vector2.right;
            RaycastHit2D hit = Physics2D.Raycast(wallCheck.position, dir);
			//Debug.DrawRay(wallCheck.position, dir, Color.red);
			//Debug.Log(hit.collider.gameObject.name);
            if (hit.collider != null) {
				Debug.Log(hit.collider.gameObject.tag.ToString());
				//if (hit.collider.gameObject.tag.ToString() != "Player") Flip();
			}
            if (canAttack)
            {
                GameObject throwableProj = Instantiate(throwableObject, transform.position + new Vector3(transform.localScale.x * 0.5f, -0.2f), Quaternion.identity) as GameObject;
                throwableProj.GetComponent<ThrowableProjectile>().owner = gameObject;
                Vector2 direction = new Vector2(transform.localScale.x, 0f);
                throwableProj.GetComponent<ThrowableProjectile>().direction = direction;
                canAttack = false;
                StartCoroutine(AttackCooldown());
            }

        }
    }

    // Update is called once per frame
    void FixedUpdate () {
        if (life <= 0) {
			transform.GetComponent<Animator>().SetBool("IsDead", true);
			StartCoroutine(DestroyEnemy());
        }

		isPlat = Physics2D.OverlapCircle(fallCheck.position, .2f, 1 << LayerMask.NameToLayer("Default"));
		isObstacle = Physics2D.OverlapCircle(wallCheck.position, .2f, turnLayerMask);
		

        if (!isHitted && life > 0 && !canShoot && Mathf.Abs(rb.velocity.y) < 0.5f)
		{
            if (isPlat && !isObstacle && !isHitted)
            {
                if (facingRight)
                {
                    rb.velocity = new Vector2(-speed, rb.velocity.y);
                }
                else
                {
                    rb.velocity = new Vector2(speed, rb.velocity.y);
                }
            }
            else
            {
                Flip();
            }
		}
	}

	void Flip (){
		// Switch the way the player is labelled as facing.
		facingRight = !facingRight;
		
		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	public void ApplyDamage(float damage) {
		if (!isInvincible) 
		{
			float direction = damage / Mathf.Abs(damage);
			damage = Mathf.Abs(damage);
			transform.GetComponent<Animator>().SetBool("Hit", true);
			life -= damage;
			rb.velocity = Vector2.zero;
			rb.AddForce(new Vector2(direction * 500f, 100f));
			StartCoroutine(HitTime());
		}
	}

	void OnCollisionStay2D(Collision2D collision)
	{
		if (collision.gameObject.tag == "Player" && life > 0)
		{
			collision.gameObject.GetComponent<CharacterController2D>().ApplyDamage(2f, transform.position);
		}
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
		{
			playerLocation = collision.gameObject.transform;
            Debug.Log("Player Detected.");
			canShoot = true;
		}
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
		if (collision.gameObject.tag == "Player")
		{
			playerLocation = null;
            canShoot = false;
			Debug.Log("Player Not Detected.");
		}
    }

    IEnumerator HitTime()
	{
		isHitted = true;
		isInvincible = true;
		yield return new WaitForSeconds(0.1f);
		isHitted = false;
		isInvincible = false;
	}

	IEnumerator DestroyEnemy()
	{
		CapsuleCollider2D capsule = GetComponent<CapsuleCollider2D>();
		capsule.size = new Vector2(1f, 0.25f);
		capsule.offset = new Vector2(0f, -0.8f);
		capsule.direction = CapsuleDirection2D.Horizontal;
		yield return new WaitForSeconds(0.25f);
		rb.velocity = new Vector2(0, rb.velocity.y);
		yield return new WaitForSeconds(3f);
		Destroy(gameObject);
	}
    IEnumerator AttackCooldown()
    {
        canAttack = true;
        //Debug.Log("Can shoot now.");
        yield return new WaitForSeconds(15f);
    }
}
