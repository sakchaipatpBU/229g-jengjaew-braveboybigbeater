using System;
using System.Collections;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using static UnityEngine.GraphicsBuffer;


public class Player : Character
{

    private InputAction shieldAction;
    private InputAction atkAction;
    private float atkStageCooldown = 0.55f;
    private float atkComboTime = 3f;
    private float lastAtkTime;
    private GameObject target;
    private PlayerController playerController;

    private bool isSheild;
    private bool isShieldHit;
    private bool isCombat;
    private int atkStage = 0;
    private bool isAtking;
    private bool isTakeDamage;
    private bool isDizzy;

    public float dizzyTime;

    public RectTransform playerHp;

    private void Awake()
    {

    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        shieldAction = InputSystem.actions.FindAction("Shield");
        atkAction = InputSystem.actions.FindAction("Attack");
        playerController = GetComponent<PlayerController>();
        charMaxHp = charHp;
    }

    void Update()
    {
        if (isDead) return;

        if (isTakeDamage)
        {
            isTakeDamage = false;
        }

        // player hold shield
        if (shieldAction.IsPressed())
        {
            isSheild = true;
            ResetCombo();
        }
        else
        {
            isSheild = false;
            SetAnimatorController();
        }
        if (atkAction.triggered && !isAtking)
        {
            PerformAtk();
        }

        if (Time.time - lastAtkTime > atkComboTime)
        {
            atkStage = 0;
            charAnim.SetInteger("atkStage", atkStage);
        }
        if (Time.time - lastAtkTime > atkStageCooldown)
        {
            isAtking = false;
            if (atkStage >= 4)
            {
                ResetCombo();
            }
        }

        playerHp.sizeDelta = new Vector2(charHp / charMaxHp * 500, 50);


        if (charHp <= 0)
        {
            isDead = true;
            charAnim.SetTrigger("isDead");
            isTakeDamage = false;
            playerController.PlayerDead();
            SetAnimatorController();
        }
    }



    public void ResetCombo()
    {
        atkStage = 0;
        isAtking = false;
        SetAnimatorController();
    }
    public override void PerformAtk()
    {
        // update last atk time
        lastAtkTime = Time.time;
        atkStage++;
        charAnim.SetInteger("atkStage", atkStage);
        charAnim.SetBool("isAtking", true);
        isAtking = true;
        if (target != null)
        {
            target.GetComponent<Enemy>().TakeDamage(charAtk);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // ถ้าโจมตีจะโนน enemy เพราะชน collisioon
        if (collision.gameObject.CompareTag("Enemy"))
        {
            target = collision.gameObject;
        }
        
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            target = null;
        }
        
    }

    public override void TakeDamage(float damage)
    {
        isTakeDamage = true;
        charHp -= damage;
        Debug.Log($"{gameObject.name} take {damage} damage");
        SetAnimatorController();
    }

    public override void SetAnimatorController()
    {
        if (isDead)
        {
            return;
        }
        charAnim.SetBool("isAtking", isAtking);
        charAnim.SetBool("isShield", isSheild);
        charAnim.SetInteger("atkStage", atkStage);
        charAnim.SetBool("isAtking", false);
        charAnim.SetBool("isTakeDamage", isTakeDamage);


    }


}
