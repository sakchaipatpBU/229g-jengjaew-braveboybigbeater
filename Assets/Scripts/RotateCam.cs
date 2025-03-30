using UnityEngine;
using UnityEngine.InputSystem;

public class RotateCam : MonoBehaviour
{
    public float rotateSpeed;

    private InputAction lookL_Action;
    private InputAction lookR_Action;

    void Start()
    {
        lookL_Action = InputSystem.actions.FindAction("LookL");
        lookR_Action = InputSystem.actions.FindAction("LookR");
    }

    void Update()
    {
        // q = rotate cam left
        if (lookL_Action.IsPressed())
        {
            transform.Rotate(Vector3.up, -rotateSpeed * Time.deltaTime);
        }
        // e = rotate cam right
        if (lookR_Action.IsPressed())
        {
            transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
        }
    }
}
