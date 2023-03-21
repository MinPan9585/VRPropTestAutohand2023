using Autohand;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MRSpace.FrameWork.SystemProps
{
    public class GalaxyBrush : SystemPropsBase
    {
        public string brushName = "";
        public List<GalaxyObjects> brushObjects = new List<GalaxyObjects>();
        private GalaxyObjects currentBrush;
        public Transform tip;

        float penCooldown = 0.2f;
        float penNextCooldown = 0f;
        int brushIndex = 0;
        bool isDraw = false;

        protected GameObject brush_galaxyParent = null;
        protected BrushFunctionType functionType;

        private float delayTime;
        private int functionTypeIndex = 0;

        private Transform brushUIImage;
        public List<Sprite> brushUISprites = new List<Sprite>();

        public enum BrushFunctionType
        {
            Size,
            Eraser,
        }

        private void OnTriggerStay(Collider other)
        {
            if (functionType == BrushFunctionType.Eraser && other.CompareTag("GalaxyBrush"))
            {
                Destroy(other.gameObject);
            }
        }

        protected override void Init()
        {
            base.Init();
            InitBrushData();
            InitGalaxyBrush();
        }

        private void InitBrushData()
        {
            //if (!GameObject.Find(brushName))
            //{
            //    brush_galaxyParent = new GameObject(brushName);
            //    brush_galaxyParent.transform.parent = null;
            //}
            //else
            //{
            //    brush_galaxyParent = GameObject.Find(brushName);
            //}

            functionType = BrushFunctionType.Size;
            currentBrush = brushObjects[0];
        }

        void Update()
        {
            if (isDraw)
            {
                if (Time.time > penNextCooldown)
                {
                    Vector3 rotation = new Vector3(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
                    int randomIndex = Random.Range(0, 3);
                    //GameObject brushGO = Instantiate(brushObjects[brushIndex].galaxyObjects[randomIndex], tip.position, Quaternion.Euler(rotation));
                    Instantiate(brushObjects[brushIndex].galaxyObjects[randomIndex], tip.position, Quaternion.Euler(rotation));
                    //brushGO.transform.parent = brush_galaxyParent.transform;
                    penNextCooldown = Time.time + penCooldown;
                }
            }
        }

        #region interaction
        public override void OnGrab(Hand hand, Grabbable grabbable)
        {
            SwitchFunctionType(0);
        }

        public override void OnRelease(Hand hand, Grabbable grabbable)
        {
            CloseUI();
        }

        public override void OnSqueeze(Hand hand, Grabbable grabbable)
        {
            if (functionType == BrushFunctionType.Eraser) return;
            if (!isDraw)
            {
                isDraw = true;
            }
        }

        public override void OnUnsqueeze(Hand hand, Grabbable grabbable)
        {
            if (functionType == BrushFunctionType.Eraser) return;
            if (isDraw)
            {
                isDraw = false;
            }
        }

        public override void OnJoyStickTurnRight(Hand hand, Grabbable grabbable)
        {
            switch (functionType)
            {
                case BrushFunctionType.Size:
                    UIColorTurnRight();
                    break;
                case BrushFunctionType.Eraser:
                    break;
            }
        }

        public override void OnJoyStickTurnLeft(Hand hand, Grabbable grabbable)
        {
            switch (functionType)
            {
                case BrushFunctionType.Size:
                    UIColorTurnLeft();
                    break;
                case BrushFunctionType.Eraser:
                    break;
            }
        }
        public override void OnJoyStickTurnUp(Hand hand, Grabbable grabbable)
        {
            SwitchFunctionType(-1);
        }

        public override void OnJoyStickTurnDown(Hand hand, Grabbable grabbable)
        {
            SwitchFunctionType(1);
        }
        #endregion

        private void SwitchFunctionType(int index)
        {
            if (delayTime > Time.time) return;
            delayTime = Time.time + 0.5f;

            functionTypeIndex += index;
            if (functionTypeIndex < 0)
            {
                functionTypeIndex = 1;
            }
            else
            {
                functionTypeIndex = functionTypeIndex % 2;
            }
            functionType = (BrushFunctionType)functionTypeIndex;

            CloseUI();
            transform.Find("UI/" + functionType.ToString()).gameObject.SetActive(true);
            switch (functionType)
            {
                case BrushFunctionType.Size:
                    break;
                case BrushFunctionType.Eraser:
                    break;
            }
        }

        private void CloseUI()
        {
            foreach (Transform item in transform.Find("UI"))
            {
                item.gameObject.SetActive(false);
            }
        }

        #region UI switch brush
        private void InitGalaxyBrush()
        {
            brushUIImage = transform.Find("UI/Size/GalaxyImage");
        }

        private void UIColorTurnLeft()
        {
            if (delayTime > Time.time) return;
            delayTime = Time.time + 0.5f;

            brushIndex++;
            if (brushIndex >= brushObjects.Count)
            {
                brushIndex = 0;
            }
            currentBrush = brushObjects[brushIndex];

            brushUIImage.GetComponent<Image>().sprite = brushUISprites[brushIndex];
        }

        private void UIColorTurnRight()
        {
            if (delayTime > Time.time) return;
            delayTime = Time.time + 0.5f;

            brushIndex--;
            if (brushIndex < 0)
            {
                brushIndex = brushObjects.Count - 1;
            }
            currentBrush = brushObjects[brushIndex];

            brushUIImage.GetComponent<Image>().sprite = brushUISprites[brushIndex];
        }
        #endregion
    }
}