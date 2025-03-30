using UnityEngine.InputSystem;
using UnityEngine;

public class Item : MonoBehaviour
{
    protected Rigidbody rb;
    protected bool isUse = false;
    protected bool isPlayerCanUse = false;
    protected InputAction interactAction;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
        interactAction = InputSystem.actions.FindAction("Interact");
    }

    void FixedUpdate()
    {
        if (!isUse)
        {
            transform.Rotate(0, 5, 0);
        }
        if (interactAction.IsPressed() && isPlayerCanUse)
        {
            isUse = true;
            Debug.Log("Use Item");
        }
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isPlayerCanUse = true;
            Debug.Log("Can Use Item");
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            isPlayerCanUse = false;
            Debug.Log("Can Not Use Item");
        }
    }
}