using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{

    public GameObject throwableObject;

    public void shoot()
    {
        GameObject throwableProj = Instantiate(throwableObject, transform.position + new Vector3(transform.localScale.x * 1f, -0.2f), Quaternion.identity) as GameObject;
        Vector2 direction = new Vector2(transform.localScale.x, 0f);
        throwableProj.GetComponent<ThrowableWeaponEnemy>().direction = direction;
    }
}
