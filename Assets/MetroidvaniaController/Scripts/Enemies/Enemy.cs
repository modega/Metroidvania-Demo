using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{

    public float life = 10;
    private bool isPlat;
    private bool isObstacle;
    private Transform fallCheck;
    private Transform wallCheck;
    public LayerMask turnLayerMask;
    private Rigidbody2D rb;

    private bool facingRight = true;

    public float speed = 5f;

    public bool isInvincible = false;
    private bool isHitted = false;

    [field: SerializeField]
    public bool PlayerDetected { get; private set; }
    public Vector2 DirectionToTarget => target.transform.position - detectorOrigin.position;

    private GameObject target;

    public GameObject Target
    {
        get => target;
        private set
        {
            target = value;
            PlayerDetected = target != null;
        }
    }

    public float detectionDelay = 0.3f;
    public LayerMask detectorLayerMask;

    [SerializeField]
    private Transform detectorOrigin;
    public Vector2 detectorSize = Vector2.zero;
    public Vector2 detectorOriginOffset = Vector2.zero;

    public Color gizmoIdleColor = Color.green;
    public Color gizmoDetectedColor = Color.red;
    public bool showGizmos = true;

    public bool canShoot = true;

    private float cooldown = 2f;
    private float cooldownTimer;
    void Awake()
    {
        fallCheck = transform.Find("FallCheck");
        wallCheck = transform.Find("WallCheck");
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        StartCoroutine(DetectionCoroutine());
    }

    private void Update()
    {
        if (PlayerDetected)
        {
            shootProjectile();
        }
    }

    private void shootProjectile()
    {
        cooldownTimer -= Time.deltaTime;

        if (cooldownTimer > 0) return;

        cooldownTimer = cooldown;
        this.gameObject.GetComponent<EnemyAttack>().shoot();
    }
    // Update is called once per frame
    void FixedUpdate()
    {

        if (life <= 0)
        {
            transform.GetComponent<Animator>().SetBool("IsDead", true);
            StartCoroutine(DestroyEnemy());
        }

        isPlat = Physics2D.OverlapCircle(fallCheck.position, .2f, 1 << LayerMask.NameToLayer("Default"));
        isObstacle = Physics2D.OverlapCircle(wallCheck.position, .2f, turnLayerMask);

        if (!PlayerDetected)
        {
            if (!isHitted && life > 0 && Mathf.Abs(rb.velocity.y) < 0.5f)
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
                    detectorOriginOffset = new Vector2(-1 * detectorOriginOffset.x, detectorOriginOffset.y);
                }
            }

        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    void Flip()
    {
        // Switch the way the player is labelled as facing.
        facingRight = !facingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    public void ApplyDamage(float damage)
    {
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

    IEnumerator DetectionCoroutine()
    {
        yield return new WaitForSeconds(detectionDelay);
        detectPlayer();     
        StartCoroutine(DetectionCoroutine());
    }

    void detectPlayer()
    {
        Collider2D collider = Physics2D.OverlapBox((Vector2)detectorOrigin.position + detectorOriginOffset, detectorSize, 0, detectorLayerMask);
        if(collider != null)
        {
            Target = collider.gameObject;
        }
        else
        {
            Target = null;
        }
    }

    IEnumerator ShootDelay()
    {
        yield return new WaitForSeconds(3.5f);
        canShoot = true;
    }

    private void OnDrawGizmos()
    {
        if(showGizmos && detectorOrigin != null)
        {
            Gizmos.color = gizmoIdleColor;
            if (PlayerDetected) Gizmos.color = gizmoDetectedColor;
            Gizmos.DrawCube((Vector2)detectorOrigin.position + detectorOriginOffset, detectorSize);
        }
    }
}