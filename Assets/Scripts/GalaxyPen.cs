using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Autohand;

public class GalaxyPen : MonoBehaviour
{
    //[SerializeField] private InputActionReference changePrefabAction;
    //[SerializeField] private InputActionReference drawPrefabAction;
    public GameObject cloudPrefab;
    public Transform tip;
    float penCooldown = 0.2f;
    float penNextCooldown = 0f;
    public GameObject[] brushesPrefab;
    int index = 0;
    bool pressed = false;
    //float scaleMultiplier;

    //void OnEnable()
    //{
    //    changePrefabAction.action.performed += ChangePrefab;
    //}
    void Update()
    {
        if (pressed)
        {
            if (Time.time > penNextCooldown)
            {
                Vector3 rotation = new Vector3(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
                Instantiate(brushesPrefab[index], tip.position, Quaternion.Euler(rotation));
                penNextCooldown = Time.time + penCooldown;
            }
        }
        //GetComponent<Grabbable>().OnSqueeze+= 

    }
    public void SetPressedTrue()
    {
        pressed = true;
    }

    public void SetPressedFalse()
    {
        pressed = false;
    }

    //void ChangePrefab(InputAction.CallbackContext ctx)
    //{
    //    if (ctx.performed)
    //    {
    //        Vector2 joystickValue = ctx.ReadValue<Vector2>();
    //        if (joystickValue.x >= 0.2)
    //        {
    //            if (index < 2)
    //            {
    //                index++;
    //            }
    //            else
    //            {
    //                index = 0;
    //            }
    //        }
    //        if (joystickValue.x <= -0.2)
    //        {
    //            if (index > 0)
    //            {
    //                index--;
    //            }
    //            else
    //            {
    //                index = 2;
    //            }
    //        }
    //    }
    //}
}
