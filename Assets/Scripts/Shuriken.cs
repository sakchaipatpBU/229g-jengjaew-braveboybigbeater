using UnityEngine;
using System.Collections;
using System;

public class Shuriken : Item
{
    public Vector3 initialVelocity;
    public Vector3 spinVelocity;
    public float magnusForceMultiplier = 3f;
    private GameObject enemy;
    private bool isFlying = false;

    public ParticleSystem shurikenHit;
    public float shurikenDamage;

    Vector3 directionToEnemy;
    float distanceToEnemy;

    protected override void Start()
    {
        base.Start();
        enemy = GameObject.Find("Incenoth");
    }

    protected void Update()
    {
        if (isUse && !isFlying)
        {
            directionToEnemy = GetEnemyDirection();
            ThrowShuriken();
        }

        if (isFlying)
        {
            ApplyMagnusEffect();
        }
    }

    private void ThrowShuriken()
    {
        /*transform.parent = null; // ปลดออกจาก Player
        rb.isKinematic = false;*/
        rb.linearVelocity = initialVelocity + directionToEnemy;
        rb.angularVelocity = spinVelocity;
        isFlying = true;
    }

    private void ApplyMagnusEffect()
    {
        // ถ้าไม่มีเป้าหมาย ให้ดาวกระจายวิ่งตรงไป
        if (enemy == null) return; 

        directionToEnemy = GetEnemyDirection();
        distanceToEnemy = GetEnemyDistance();

        // คำนวณ Magnus Effect
        Vector3 magnusForce = Vector3.Cross(spinVelocity, rb.linearVelocity).normalized * magnusForceMultiplier;
        // เพิ่มแรง Magnus Effect ให้ตีโค้ง
        rb.AddForce(magnusForce * distanceToEnemy / 5);

        // ปรับทิศให้ค่อยๆ เข้าเป้าหมาย
        //rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, directionToEnemy * rb.linearVelocity.magnitude, Time.deltaTime * 0.5f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Shuriken hit the enemy!");
            isFlying = false;
            shurikenHit.Play();
            Destroy(gameObject); // ทำลายดาวกระจายหลังจากชนเป้าหมาย
        }
    }
    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        if (other.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Shuriken hit the enemy!");
            isFlying = false;
            Instantiate(shurikenHit, transform.position, Quaternion.identity);
            enemy.GetComponent<Enemy>().TakeDamage(shurikenDamage);
            Destroy(gameObject); // ทำลายดาวกระจายหลังจากชนเป้าหมาย
        }
    }

    Vector3 GetEnemyDirection()
    {
        return (enemy.transform.position - transform.position).normalized;
    }

    float GetEnemyDistance()
    {
        return Vector3.Distance(transform.position, enemy.transform.position);
    }
}
