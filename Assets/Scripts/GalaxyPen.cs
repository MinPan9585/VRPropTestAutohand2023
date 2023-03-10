using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GalaxyPen : MonoBehaviour
{
    [SerializeField] private InputActionReference changePrefabAction;
    [SerializeField] private InputActionReference drawPrefabAction;
    public GameObject cloudPrefab;
    public Transform tip;
    //float penCooldown = 1f;
    //float penNextCooldown = 0;
    public GameObject[] brushesPrefab;
    int index = 0;
    bool pressed = false;

    void OnEnable()
    {
        changePrefabAction.action.performed += ChangePrefab;
        drawPrefabAction.action.performed += setTrue;
    }

    void setTrue(InputAction.CallbackContext ctx)
    {
        pressed = true;    }

    void ChangePrefab(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            Vector2 joystickValue = ctx.ReadValue<Vector2>();
            if (joystickValue.x >= 0.2)
            {
                if (index < 2)
                {
                    index++;
                }
                else
                {
                    index = 0;
                }
            }
            if (joystickValue.x <= -0.2)
            {
                if (index > 0)
                {
                    index--;
                }
                else
                {
                    index = 2;
                }
            }
        }
    }

    void Update()
    {
        if (pressed)
        {
            DrawPrefab(InputAction.CallbackContext ctx);
            Vector3 rotation = new Vector3(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
            Instantiate(brushesPrefab[index], tip.position, Quaternion.Euler(rotation));
        }

    }

    void DrawPrefab(InputAction.CallbackContext ctx)
    {
        if (Time.time > penNextCooldown)
        {
            Instantiate(cloudPrefab, transform.position, Quaternion.identity);
            penNextCooldown = Time.time + penCooldown;
        }


        if (ctx.performed)
        {
            pressed = true;
            
        }
        if (ctx.canceled)
        {
            pressed = false;
            
        }
    }

    //IEnumerator SpawnPrefab()
    //{
    //    if(pressed)
    //    {
    //        Vector3 rotation = new Vector3(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
    //        Instantiate(brushesPrefab[index], tip.position, Quaternion.Euler(rotation));
    //        yield return new WaitForSeconds(0.4f);
    //    }

    //}
}
