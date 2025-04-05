using UnityEngine;
using UnityEngine.InputSystem;

public class Paddle : MonoBehaviour
{
    [SerializeField] private float speed = 10f; // Speed of the paddle movement

    private void Update()
    {
        Vector2 moveDirection = Vector2.zero;

#if UNITY_ANDROID || UNITY_EDITOR                              
        // Android Controls (Touch)
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            float touchX = Touchscreen.current.primaryTouch.position.x.ReadValue();
            moveDirection = (touchX < Screen.width / 2f) ? Vector2.left : Vector2.right;
        }
#endif

#if UNITY_STANDALONE || UNITY_EDITOR
        // PC Controls
        if (Keyboard.current.leftArrowKey.isPressed)
        {
            moveDirection = Vector2.left;
        }
        else if (Keyboard.current.rightArrowKey.isPressed)
        {
            moveDirection = Vector2.right;
        }
#endif


        transform.Translate(moveDirection * speed * Time.deltaTime);
    }
}
