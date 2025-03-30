using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.UI;

public class Enemy : Character
{
    private GameObject player;
    GameObject target;
    private Collider enemyCollider;

    private bool isEnemySeePlayer;
    Vector3 directionToPlayer;
    float distanceToPlayer;
    private Coroutine walkCoroutine;
    private Coroutine rotateCoroutine;

    bool isWalk;
    bool isRun;
    float runSpeed;
    public float walkCycleTime;
    private float lastWalk;
    bool isBasicAtk;
    public float basicAtkCycleTime;
    private float lastBasicAtk;
    bool isClawAtk;
    public float clawAtkCycleTime;
    private float lastClawAtk;
    bool isFlameAtk;
    public float flameAtkCycleTime;
    private float lastFlameAtk;
    bool isScream;
    public float screamCycleTime;
    private float lastScream;
    public float basicAtkAnimTime;
    public float clawAtkAnimTime;
    public float flameAtkAnimTime;
    private float atkAnimTime;
    public float screamAnimTime;
    private bool isAtking;
    private bool isTakeDamage;

    public ParticleSystem flameAtkParticleSystem;

    // ระยะที่ enemy มองเห็น player
    public float visionRange;
    public float visionAngle;
    public float rotateSpeed;

    public float atkRange;

    public RectTransform bossHp;


    void Start()
    {
        player = GameObject.Find("Player");
        runSpeed = moveSpeed * 1.5f;
        enemyCollider = gameObject.GetComponent<Collider>();
        charMaxHp = charHp;
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (isDead)
        {
            return;
        }
        if (charHp <= 0)
        {
            isDead = true;
            charAnim.SetTrigger("isDead");
            isTakeDamage = false;
            isWalk = false;
            SetAnimatorController();
            Destroy(enemyCollider);
        }

        // เลือดน้อย บัพวิ่งไว
        if (charHp <= charMaxHp * 0.3f)
        {
            isRun = true;
        }

        isEnemySeePlayer = CanSeePlayer();
        if (isEnemySeePlayer)
        {
            directionToPlayer = GetPlayerDirection();
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotateSpeed);

            if (Time.time - lastWalk > walkCycleTime && walkCoroutine == null)
            {
                walkCoroutine = StartCoroutine(WalkToPlayerRoutine());
            }

            distanceToPlayer = GetPlayerDistance();
            if (distanceToPlayer < atkRange)
            {
                PerformAtk();
                SetAnimatorController();
            }
        }
        else
        {
            // หยุดวิ่งถ้าไม่เห็น player
            if (walkCoroutine != null)
            {
                StopCoroutine(walkCoroutine);
                walkCoroutine = null;
            }

        }

        bossHp.sizeDelta = new Vector2(charHp / charMaxHp * 500, 70);


        // scream
        if (Time.time - lastScream > screamCycleTime)
        {
            Scream();
        }
        SetAnimatorController();
    }

    bool CanSeePlayer()
    {
        directionToPlayer = GetPlayerDirection();
        distanceToPlayer = GetPlayerDistance();

        // ตรวจสอบว่า player อยู่ในระยะสายตามั้ย
        if (distanceToPlayer > visionRange)
        {
            Debug.Log($"{player.name} is out off Range");
            isWalk = false;
            return false;
        }

        // ตรวจสอบว่า player อยู่ในมุมที่มังกรเห็นได้มั้ย
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        if (angleToPlayer > visionAngle / 2)
        {
            Debug.Log($"{player.name} is out off Angle");
            isWalk = false;
            return false;
        }

        // ตรวจสอบสิ่งกีดขวางระหว่าง player กับ enemy
        RaycastHit hit;
        Debug.DrawRay(transform.position, directionToPlayer);
        if (Physics.Raycast(transform.position, directionToPlayer, out hit, visionRange))
        {
            if (!hit.collider.gameObject.CompareTag("Player"))
            {
                Debug.Log("player hide behide obstacle");
                isWalk = false;
                return false;
            }
        }
        // enemy เห็น player
        return true;
    }

    Vector3 GetPlayerDirection()
    {
        return (player.transform.position - transform.position).normalized;
    }

    float GetPlayerDistance()
    {
        return Vector3.Distance(transform.position, player.transform.position);
    }

    
    public override void TakeDamage(float damage)
    {
        isTakeDamage = true;
        charHp -= damage;
        Debug.Log($"{gameObject.name} take {damage} damage, left {charHp} hp");
        GetHit();
    }
    public override void PerformAtk()
    {
        // หยุด ถ้ากำลังโจมตีอยู่แล้ว
        if (player == null || isAtking) return;

        Character playerCharacter = player.GetComponent<Character>();
        if (playerCharacter == null) return;

        // Random ท่าโจมตี %
        float randomAtk = Random.Range(0f, 1f);

        // เช็คการโจมตีแต่ละแบบตาม cooldown และล็อกไม่ให้ซ้อนกัน
        if (Time.time - lastBasicAtk > basicAtkCycleTime)
        {
            // โจมตีธรรมดา มี เดเมจ = charAtk
            StartCoroutine(AttackRoutine("Basic", charAtk, basicAtkAnimTime));
            lastBasicAtk = Time.time;
            return;
        }
        if (Time.time - lastClawAtk > clawAtkCycleTime)
        {
            // Claw ทำดาเมจแรงขึ้น 20%
            StartCoroutine(AttackRoutine("Claw", charAtk * 1.2f, clawAtkAnimTime));
            lastClawAtk = Time.time;
            return;
        }
        if (Time.time - lastFlameAtk > flameAtkCycleTime)
        {
            StartCoroutine(AttackRoutine("Flame", charAtk * 1.5f, flameAtkAnimTime));
            lastFlameAtk = Time.time;
            return;
        }

        // ถ้า cooldown ทุกท่าครบ หรือ ใช้ได้ทุกท่าโจมตี
        // 25% โอกาส FlameAtk
        // 35% โอกาส ClawAtk
        // 40% โอกาส BasicAtk
        if (randomAtk > 0.75f && Time.time - lastFlameAtk > flameAtkCycleTime)
        {
            // Flame ทำดาเมจแรงขึ้น 50 %
            StartCoroutine(AttackRoutine("Flame", charAtk * 1.5f, flameAtkAnimTime));
            lastFlameAtk = Time.time;
            return;
        }
        if (randomAtk > 0.4f && Time.time - lastClawAtk > clawAtkCycleTime)
        {
            StartCoroutine(AttackRoutine("Claw", charAtk * 1.2f, clawAtkAnimTime));
            lastClawAtk = Time.time;
            return;
        }
        if (Time.time - lastBasicAtk > basicAtkCycleTime)
        {
            StartCoroutine(AttackRoutine("Basic", charAtk, basicAtkAnimTime));
            lastBasicAtk = Time.time;
            return;
        }
    }
    private IEnumerator AttackRoutine(string atkType, float damage, float animTime)
    {
        // ล็อกให้โจมตีทีละท่า
        isAtking = true;

        if (atkType == "Basic") isBasicAtk = true;
        else if (atkType == "Claw") isClawAtk = true;
        else if (atkType == "Flame") isFlameAtk = true;

        // อัปเดตแอนิเมชัน
        SetAnimatorController();

        // ให้ Player รับดาเมจหลังเริ่มแอนิเมชัน 
        // ให้โดนดาเมจตอนกลางแอนิเมชัน
        yield return new WaitForSeconds(animTime * 0.5f); 
        if (target != null) player.GetComponent<Character>().TakeDamage(damage);
        Debug.Log($"Enemy used {atkType} attack: {damage} dmg");

        // รอให้แอนิเมชันจบก่อนคืนสถานะ
        yield return new WaitForSeconds(animTime * 0.5f);
        // reset ท่าการโจมตี
        isBasicAtk = isClawAtk = isFlameAtk = false;
        isAtking = false;

        // อัปเดตแอนิเมชันหลังโจมตีเสร็จ
        SetAnimatorController(); 
    }


    public override void SetAnimatorController()
    {
        if (isDead)
        {
            return;
        }
        charAnim.SetBool("isTakeDamage", isTakeDamage);
        isTakeDamage = false;
        charAnim.SetBool("isWalk", isWalk);
        charAnim.SetBool("isRun", isRun);
        charAnim.SetBool("isBasicAtk", isBasicAtk);
        charAnim.SetBool("isClawAtk", isClawAtk);
        charAnim.SetBool("isFlameAtk", isFlameAtk);
        charAnim.SetBool("isScream", isScream);
        if (isFlameAtk)
        {
            flameAtkParticleSystem.Play();
        }


    }


    IEnumerator WalkToPlayerRoutine()
    {
        while (CanSeePlayer()) 
        {
            directionToPlayer = GetPlayerDirection();
            distanceToPlayer = GetPlayerDistance();

            // ถ้าอยู่ใกล้ player ออกจาก WalkToPlayerRoutine()
            if (distanceToPlayer < atkRange)
            {
                charAnim.SetBool("isRun", false);
                charAnim.SetBool("isWalk", false);
                isWalk = false;
                walkCoroutine = null;
                yield break; 
            }

            if (isRun)
            {
                charAnim.SetBool("isRun", isRun);
                moveSpeed = runSpeed;
            }
            else
            {
                isWalk = true;
                charAnim.SetBool("isWalk", isWalk);
            }

            // ค่อยๆ เคลื่อนที่ไปหาผู้เล่น
            transform.Translate(directionToPlayer * moveSpeed * Time.deltaTime, Space.World);

            // รอ 1 เฟรมแล้วทำซ้ำ
            yield return null; 
        }

        walkCoroutine = null;
    }

    private void GetHit()
    {
        if (rotateCoroutine != null)
        {
            StopCoroutine(rotateCoroutine);
        }
        rotateCoroutine = StartCoroutine(RotateToPlayeroutine());
    }
    private IEnumerator RotateToPlayeroutine()
    {
        Quaternion targetRotation = Quaternion.LookRotation(GetPlayerDirection());
        Vector3 targetEulerAngles = targetRotation.eulerAngles;

        // ล็อคให้หมุนเฉพาะแกน Y
        Vector3 currentEulerAngles = rb.rotation.eulerAngles;
        targetEulerAngles.x = currentEulerAngles.x;
        targetEulerAngles.z = currentEulerAngles.z;

        float rotateDuration = 1f;
        float elapsedTime = 0;

        while (elapsedTime < rotateDuration)
        {
            Quaternion newRotation = Quaternion.Slerp(
                rb.rotation,
                Quaternion.Euler(targetEulerAngles),
                elapsedTime / rotateDuration
            );

            // ใช้ Rigidbody เพื่อหมุน
            rb.MoveRotation(newRotation); 

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // จัดตำแหน่งสุดท้ายให้แม่นยำ
        rb.MoveRotation(Quaternion.Euler(targetEulerAngles)); 
        rotateCoroutine = null;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // ถ้าโจมตีจะโดน player
        if (collision.gameObject.CompareTag("Player"))
        {
            target = collision.gameObject;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            target = null;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            target = other.gameObject;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            target = null;
        }
    }

    /*private IEnumerator ResetAtk(string atkType)
    {
        // เวลาของแอนิเมชันโจมตี
        yield return new WaitForSeconds(atkAnimTime); 
        if (atkType == "Basic") isBasicAtk = false;
        else if (atkType == "Claw") isClawAtk = false;
        else if (atkType == "Flame") isFlameAtk = false;
    }*/

    // Scream
    private void Scream()
    {
        isScream = true;
        lastScream = Time.time;

        Debug.Log("Enemy is Screaming");
        SetAnimatorController();
        StartCoroutine(ResetScream());
    }

    private IEnumerator ResetScream()
    {
        // ระบะเวลา scream
        yield return new WaitForSeconds(screamAnimTime); 
        isScream = false;
    }
}
