using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GunColor : MonoBehaviour
{
    [SerializeField] private InputActionReference changeColorAction;
    public GameObject[] colorObjects;
    private Vector3 originalScale = new Vector3(0.01f, 0.01f, 0.01f);
    private Vector3 selectedScale = new Vector3(0.02f, 0.02f, 0.02f);
    private int index;

    void OnEnable()
    {
        changeColorAction.action.performed += Changing;
    }

    void Changing(InputAction.CallbackContext obj)
    {
        Vector2 joystickValue = obj.ReadValue<Vector2>();
        if (joystickValue.y / joystickValue.x >=1 && joystickValue.x>= 0 && joystickValue.y>= 0) 
        {
            colorObjects[7].transform.localScale = selectedScale;
            colorObjects[6].transform.localScale = originalScale;
            colorObjects[5].transform.localScale = originalScale;
            colorObjects[4].transform.localScale = originalScale;
            colorObjects[3].transform.localScale = originalScale;
            colorObjects[2].transform.localScale = originalScale;
            colorObjects[1].transform.localScale = originalScale;
            colorObjects[0].transform.localScale = originalScale;
        }
        else if(joystickValue.y / joystickValue.x < 1 && joystickValue.x >= 0 && joystickValue.y >= 0)
        {
            colorObjects[7].transform.localScale = originalScale;
            colorObjects[6].transform.localScale = selectedScale;
            colorObjects[5].transform.localScale = originalScale;
            colorObjects[4].transform.localScale = originalScale;
            colorObjects[3].transform.localScale = originalScale;
            colorObjects[2].transform.localScale = originalScale;
            colorObjects[1].transform.localScale = originalScale;
            colorObjects[0].transform.localScale = originalScale;
        }
        else if (joystickValue.y / joystickValue.x > -1 && joystickValue.x < 0 && joystickValue.y >= 0)
        {
            colorObjects[7].transform.localScale = originalScale;
            colorObjects[6].transform.localScale = originalScale;
            colorObjects[5].transform.localScale = originalScale;
            colorObjects[4].transform.localScale = originalScale;
            colorObjects[3].transform.localScale = originalScale;
            colorObjects[2].transform.localScale = originalScale;
            colorObjects[1].transform.localScale = selectedScale;
            colorObjects[0].transform.localScale = originalScale;
        }
        else if (joystickValue.y / joystickValue.x <= -1 && joystickValue.x < 0 && joystickValue.y >= 0)
        {
            colorObjects[7].transform.localScale = originalScale;
            colorObjects[6].transform.localScale = originalScale;
            colorObjects[5].transform.localScale = originalScale;
            colorObjects[4].transform.localScale = originalScale;
            colorObjects[3].transform.localScale = originalScale;
            colorObjects[2].transform.localScale = originalScale;
            colorObjects[1].transform.localScale = originalScale;
            colorObjects[0].transform.localScale = selectedScale;
        }
        else if (joystickValue.y / joystickValue.x >= 1 && joystickValue.x < 0 && joystickValue.y < 0)
        {
            colorObjects[7].transform.localScale = originalScale;
            colorObjects[6].transform.localScale = originalScale;
            colorObjects[5].transform.localScale = originalScale;
            colorObjects[4].transform.localScale = originalScale;
            colorObjects[3].transform.localScale = selectedScale;
            colorObjects[2].transform.localScale = originalScale;
            colorObjects[1].transform.localScale = originalScale;
            colorObjects[0].transform.localScale = originalScale;
        }
        else if (joystickValue.y / joystickValue.x < 1 && joystickValue.x < 0 && joystickValue.y < 0)
        {
            colorObjects[7].transform.localScale = originalScale;
            colorObjects[6].transform.localScale = originalScale;
            colorObjects[5].transform.localScale = originalScale;
            colorObjects[4].transform.localScale = originalScale;
            colorObjects[3].transform.localScale = originalScale;
            colorObjects[2].transform.localScale = selectedScale;
            colorObjects[1].transform.localScale = originalScale;
            colorObjects[0].transform.localScale = originalScale;
        }
        else if (joystickValue.y / joystickValue.x <= -1 && joystickValue.x >= 0 && joystickValue.y < 0)
        {
            colorObjects[7].transform.localScale = originalScale;
            colorObjects[6].transform.localScale = originalScale;
            colorObjects[5].transform.localScale = originalScale;
            colorObjects[4].transform.localScale = selectedScale;
            colorObjects[3].transform.localScale = originalScale;
            colorObjects[2].transform.localScale = originalScale;
            colorObjects[1].transform.localScale = originalScale;
            colorObjects[0].transform.localScale = originalScale;
        }
        else if (joystickValue.y / joystickValue.x > -1 && joystickValue.x >= 0 && joystickValue.y < 0)
        {
            colorObjects[7].transform.localScale = originalScale;
            colorObjects[6].transform.localScale = originalScale;
            colorObjects[5].transform.localScale = selectedScale;
            colorObjects[4].transform.localScale = originalScale;
            colorObjects[3].transform.localScale = originalScale;
            colorObjects[2].transform.localScale = originalScale;
            colorObjects[1].transform.localScale = originalScale;
            colorObjects[0].transform.localScale = originalScale;
        }
    }
}
