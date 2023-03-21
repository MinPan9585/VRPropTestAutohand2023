using Autohand;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MRSpace.FrameWork.SystemProps
{
    public class TubeBrush : BrushBase
    {
        private float m_BrushRadius;      
        private TubeMesh currentBrushMesh = null;

        private const float brush_MinDistance = 0.006f;


        protected override void Init()
        {
            base.Init();
            nibMinSize = 1f;
            nibMaxSize = 3f;
        }
        protected override void CreatBrushLogic()
        {
            brushMat.color = currentColor;
            GameObject newBrushObj = GetNewBrush();
            newBrushObj.AddComponent<MeshFilter>();
            newBrushObj.AddComponent<MeshRenderer>();
            currentBrushMesh = newBrushObj.AddComponent<TubeMesh>();
            currentBrushMesh.CreatBrushMesh(brushMat, nib.position, m_BrushRadius * 0.003f);
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
            if (Vector3.Distance(nib.position, nibPrePos) < brush_MinDistance) return;
            currentBrushMesh.AddMesh(nib.position);
            nibPrePos = nib.position;
        }

        protected override void MatchNibSizeTex()
        {
            nibSize.localScale = Vector3.one * currentSize;
            m_BrushRadius = currentSize;
        }

        private void OnTriggerStay(Collider other)
        {
            if (functionType == BrushFunctionType.Eraser && other.transform.parent.name == brushName)
            {
                Destroy(other.gameObject);
            }
        }


    }
}