using UnityEngine;

public abstract class Character : MonoBehaviour
{
    public float charHp;
    public float charMaxHp;
    public float charAtk;
    public float weakness;
    public float moveSpeed;

    protected bool isDead = false;
    protected Rigidbody rb;

    public Animator charAnim;

    


    void Start()
    {
    }

    void Update()
    {
        
    }

    public abstract void TakeDamage(float damage);

    public abstract void PerformAtk();
    public abstract void SetAnimatorController();

}
