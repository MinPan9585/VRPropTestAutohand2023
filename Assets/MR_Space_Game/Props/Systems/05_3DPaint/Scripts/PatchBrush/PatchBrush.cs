using Autohand;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MRSpace.FrameWork.SystemProps
{
    public class PatchBrush : BrushBase
    {
        private Transform NibPos_1;
        private Transform NibPos_2;
        private PatchMesh currentBrushMesh;

        private const float brush_MinDistance = 0.003f;

        protected override void Init()
        {
            base.Init();
            NibPos_1 = nib.GetChild(0);
            NibPos_2 = nib.GetChild(1);
            nibMinSize = 0.4f;
            nibMaxSize = 1.6f;
        }
 

        protected override void CreatBrushLogic()
        {
            brushMat.color = currentColor;
            GameObject newBrushObj = GetNewBrush();
            newBrushObj.AddComponent<MeshFilter>();
            newBrushObj.AddComponent<MeshRenderer>();
            currentBrushMesh = newBrushObj.AddComponent<PatchMesh>();
            currentBrushMesh.CreateBrushMesh(brushMat);
        }

        protected override void FinishBruhsLogic()
        {
            MeshCollider meshCollider = currentBrushMesh.gameObject.AddComponent<MeshCollider>();
            meshCollider.convex = true;
            meshCollider.isTrigger = true;
            currentBrushMesh = null;
        }

        protected override void DrawBrushLogic()
        {
            if (Vector3.Distance(nibPrePos, nib.position) < brush_MinDistance) return;

            currentBrushMesh.AddMesh(NibPos_1.position, NibPos_2.position);
            nibPrePos = nib.position;
        }


        protected override void MatchNibSizeTex()
        {
            nibSize.localScale = new Vector3(currentSize, 1, 1);
            nib.localScale = Vector3.one * currentSize;
        }


        private void OnTriggerStay(Collider other)
        {
            if (functionType==BrushFunctionType.Eraser && other.transform.parent.name == brushName)
            {
                Destroy(other.gameObject);
            }
        }

    }
}