using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.XR.PXR;

public class SeeThrough : MonoBehaviour
{
    void Start()
    {
        PXR_Boundary.EnableSeeThroughManual(true);
        PXR_Plugin.Boundary.UPxr_ShutdownSdkGuardianSystem();
    }

    private void OnApplicationPause(bool pause)
    {
        if (!pause)
        {
            PXR_Boundary.EnableSeeThroughManual(true);
            PXR_Plugin.Boundary.UPxr_ShutdownSdkGuardianSystem();
        }

    }
}
