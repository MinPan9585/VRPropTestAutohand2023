using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GalaxyPen : MonoBehaviour
{
    [SerializeField] private InputActionReference changePrefabAction;
    [SerializeField] private InputActionReference drawPrefabAction;
    public GameObject cloudPrefab;
    public GameObject stonePrefab;

    void OnEnable()
    {
        changePrefabAction.action.performed += ChangePrefab;
        drawPrefabAction.action.performed += DrawPrefab;
    }

    void ChangePrefab(InputAction.CallbackContext obj)
    {
        
    }

    void DrawPrefab(InputAction.CallbackContext obj)
    {
        Instantiate(cloudPrefab, transform.position, Quaternion.identity);
    }
}
