using Autohand;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MRSpace.FrameWork.SystemProps
{
    public enum BrushFunctionType
    {
        Color,
        Size,
        Eraser,
    }

    public abstract class BrushBase : SystemPropsBase
    {
        [Tooltip("笔刷名称")]
        public string brushName = "";
        [Tooltip("绘制线条的材质")]
        public Material brushMat;
        [Tooltip("画笔材质")]
        public SkinnedMeshRenderer vibMeshRenderer;
        [Tooltip("画笔颜色")]
        public List<Color> Colors = new List<Color>();

        //Static
        private const string parent_name = "BrushObject";

        //Protect
        protected Transform nib;
        protected Transform nibSize;
        protected Vector3 nibPrePos;
        protected Color currentColor;
        protected float currentSize;
        protected BrushFunctionType functionType;
        protected GameObject brush_totalParent = null;
        protected GameObject brush_singleParent = null;
        protected float nibMinSize = 0.5f;
        protected float nibMaxSize = 2f;

        //UI
        private List<Transform> colorItems = new List<Transform>();
        private TextMeshProUGUI sizeText;

        //private
        private int colorIndex = 0;
        private int functionTypeIndex = 0;
        private float nibSizeIndex = 0.1f;
        private float delayTime;
        private Vector3 preScale;
        private Vector3 prePosition;
        private Transform currentItem;
        private bool isDraw = false;

        protected override void Init()
        {
            base.Init();
            InitBrushData();
            InitColor();
            InitSize();
        }

        private void InitBrushData()
        {
            if (!GameObject.Find(parent_name))
            {
                brush_totalParent = new GameObject(parent_name);
            }
            else
            {
                brush_totalParent = GameObject.Find(parent_name);
            }

            functionType = BrushFunctionType.Color;
            currentColor = Colors[0];
            currentSize = 1;
            vibMeshRenderer.material.color = currentColor;         
        }

        protected virtual void FixedUpdate()
        {
            if (isDraw)
                DrawBrushLogic();
        }

        #region Interation 交互方法

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
                CreatBrushLogic();
            }
        }

        public override void OnUnsqueeze(Hand hand, Grabbable grabbable)
        {
            if (functionType == BrushFunctionType.Eraser) return;
            if (isDraw)
            {
                isDraw = false;
                FinishBruhsLogic();
            }
        }


        public override void OnJoyStickTurnRight(Hand hand, Grabbable grabbable)
        {
            switch (functionType)
            {
                case BrushFunctionType.Color:
                    UIColorTurnRight();
                    break;
                case BrushFunctionType.Size:
                    IncreaseSize();
                    break;
                case BrushFunctionType.Eraser:
                    break;
            }          
        }

        public override void OnJoyStickTurnLeft(Hand hand, Grabbable grabbable)
        {
            switch (functionType)
            {
                case BrushFunctionType.Color:
                    UIColorTurnLeft();
                    break;
                case BrushFunctionType.Size:
                    ReduceSize();
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

        #region Logic 逻辑方法
        /// <summary>
        /// 切换画笔状态
        /// </summary>
        private void SwitchFunctionType(int index)
        {
            if (delayTime > Time.time) return;
            delayTime = Time.time + 0.5f;

            functionTypeIndex += index;
            if(functionTypeIndex<0)
            {
                functionTypeIndex = 2;
            }
            else
            {
                functionTypeIndex = functionTypeIndex % 3;
            }       
            functionType = (BrushFunctionType)functionTypeIndex;

            CloseUI();
            transform.Find("UI/" + functionType.ToString()).gameObject.SetActive(true);
            switch (functionType)
            {
                case BrushFunctionType.Color:
                    vibMeshRenderer.material.color = currentColor;
                    break;
                case BrushFunctionType.Size:
                    vibMeshRenderer.material.color = currentColor;
                    break;
                case BrushFunctionType.Eraser:
                    vibMeshRenderer.material.color = Color.white;
                    break;
            }
        }
        /// <summary>
        /// 关闭UI面板
        /// </summary>
        private void CloseUI()
        {
            foreach (Transform item in transform.Find("UI"))
            {
                item.gameObject.SetActive(false);
            }
        }

        protected GameObject GetNewBrush()
        {
            if (brush_singleParent == null)
            {
                brush_singleParent = new GameObject(brushName);
                brush_singleParent.transform.SetParent(brush_totalParent.transform);
            }

            GameObject newBrushObj = new GameObject(brushName + brush_singleParent.transform.childCount);
            newBrushObj.transform.SetParent(brush_singleParent.transform);
            return newBrushObj;
        }

        /// <summary>
        /// 创建笔刷逻辑
        /// </summary>
        protected abstract void CreatBrushLogic();
        /// <summary>
        /// 绘制完毕逻辑
        /// </summary>
        protected abstract void FinishBruhsLogic();
        /// <summary>
        /// 绘制中的逻辑
        /// </summary>
        protected abstract void DrawBrushLogic();

        #endregion

        #region 颜色切换
        private void InitColor()
        {
            foreach (Transform item in transform.Find("UI/Color/Items"))
            {
                colorItems.Add(item);
            }
            MatchColor();
        }
        private void UIColorTurnLeft()
        {
            if (delayTime > Time.time) return;
            delayTime = Time.time + 0.5f;

            colorIndex++;
            if (colorIndex >= Colors.Count)
            {
                colorIndex = 0;
            }
            currentColor = Colors[colorIndex];

            for (int i = colorItems.Count - 1; i >= 0; i--)
            {
                if (i == 0)
                {
                    prePosition = new Vector3((colorItems.Count - 1) * 25, 0, 0);
                    preScale = Vector3.zero;
                }
                else
                {
                    prePosition = new Vector3((i - 1) * 25, 0, 0);
                    preScale = colorItems[i - 1].localScale;
                }
                colorItems[i].DOLocalMove(prePosition, 0.5f);
                colorItems[i].DOScale(preScale, 0.5f);
            }
            currentItem = colorItems[0];
            colorItems.RemoveAt(0);
            colorItems.Add(currentItem);
            MatchColor();
        }

        private void UIColorTurnRight()
        {
            if (delayTime > Time.time) return;
            delayTime = Time.time + 0.5f;

            colorIndex --;
            if (colorIndex < 0)
            {
                colorIndex = Colors.Count - 1;
            }
            currentColor = Colors[colorIndex];

            for (int i = 0; i < colorItems.Count; i++)
            {
                if (i == colorItems.Count - 1)
                {
                    prePosition = Vector3.zero;
                    preScale = Vector3.zero;
                }
                else
                {
                    prePosition = new Vector3((i + 1) * 25, 0, 0);
                    preScale = colorItems[i + 1].localScale;
                }
                colorItems[i].DOKill();
                colorItems[i].DOLocalMove(prePosition, 0.5f);
                colorItems[i].DOScale(preScale, 0.5f);
            }
            currentItem = colorItems[colorItems.Count - 1];
            colorItems.RemoveAt(colorItems.Count - 1);
            colorItems.Insert(0, currentItem);
            MatchColor();
        }
        private void MatchColor()
        {
            vibMeshRenderer.material.color = currentColor;
            for (int i = 0; i < colorItems.Count; i++)
            {
                if (colorIndex + i - 2 < 0)
                {
                    colorItems[i].GetComponent<Image>().color = Colors[Colors.Count + (colorIndex + i - 2)];
                }
                else if (colorIndex + i - 2 > Colors.Count - 1)
                {
                    colorItems[i].GetComponent<Image>().color = Colors[colorIndex + i - 2 - Colors.Count];
                }
                else
                {
                    colorItems[i].GetComponent<Image>().color = Colors[colorIndex + i - 2];
                }
            }
        }
        #endregion

        #region 笔刷大小切换
        private void InitSize()
        {
            nib = transform.Find("NibPos");
            nibPrePos = nib.position;
            nibSize = transform.Find("UI/Size/NibSize");
            sizeText = transform.Find("UI/Size/Text").GetComponent<TextMeshProUGUI>();
            sizeText.text = currentSize.ToString("f1");
            MatchNibSizeTex();
        }

        //增加笔刷尺寸
        private void IncreaseSize()
        {
            if (currentSize - nibSizeIndex < nibMinSize) return;
            currentSize -= nibSizeIndex;
            sizeText.text = currentSize.ToString("f1");
            MatchNibSizeTex();
        }

        //减小笔刷尺寸
        private void ReduceSize()
        {
            if (currentSize + nibSizeIndex > nibMaxSize) return;
            currentSize += nibSizeIndex;
            sizeText.text = currentSize.ToString("f1");
            MatchNibSizeTex();
        }

        protected virtual void MatchNibSizeTex()
        {

        }

        #endregion



    }
}