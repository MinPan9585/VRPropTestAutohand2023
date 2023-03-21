using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;
using Autohand;
using Hand = Autohand.Hand;

namespace MRSpace.FrameWork.SystemProps
{
    public abstract class SystemXRInput : MonoBehaviour
    {

        //Protected
        protected Grabbable grab;
        protected InputDevice inputDevice;

        protected bool isGrabed;
        protected int grabedHandIndex;
        protected Vector3 handVelocity;

        //Private
        private float joysitckIntervalTime;
        private Dictionary<string, bool> stateDic = new Dictionary<string, bool>();

        public virtual void Awake()
        {
            if (GetComponent<Grabbable>())
            {
                grab = GetComponent<Grabbable>();
            }
            else
            {
                Debug.LogWarning("该道具缺少 Grabbable 组件");
            }
            Init();
        }


        /// <summary>
        /// 默认初始化道具
        /// </summary>
        protected abstract void Init();


        #region XR Input Action
        public virtual void OnEnable()
        {
            if (grab != null)
            {
                grab.OnBeforeGrabEvent += OnBeforeGrab;
                grab.OnGrabEvent += OnGrabAction;
                grab.OnReleaseEvent += OnReleaseAction;
                grab.OnSqueezeEvent += OnSqueeze;
                grab.OnUnsqueezeEvent += OnUnsqueeze;
                grab.OnHighlightEvent += OnHighlight;
                grab.OnUnhighlightEvent += OnUnhighlight;
            }

        }

        public virtual void OnDisable()
        {
            if (grab != null)
            {
                grab.OnBeforeGrabEvent -= OnBeforeGrab;
                grab.OnGrabEvent -= OnGrabAction;
                grab.OnReleaseEvent -= OnReleaseAction;
                grab.OnSqueezeEvent -= OnSqueeze;
                grab.OnUnsqueezeEvent -= OnUnsqueeze;
                grab.OnHighlightEvent -= OnHighlight;
                grab.OnUnhighlightEvent -= OnUnhighlight;
            }
        }

        private void OnGrabAction(Hand hand, Grabbable grabbable)
        {
            GetInputDeviceAndHandIndex(hand);
            GetComponent<Rigidbody>().isKinematic = false;
            isGrabed = true;
            StartCoroutine(InputAction(hand, grabbable));
            OnGrab(hand, grabbable);
        }
        private void OnReleaseAction(Hand hand, Grabbable grabbable)
        {
            isGrabed = false;
            SetRigidbodyKinematic();
            OnRelease(hand, grabbable);
        }

        protected abstract void SetRigidbodyKinematic();
 
        public virtual void OnBeforeGrab(Hand hand, Grabbable grabbable)
        {

        }

        public virtual void OnGrab(Hand hand, Grabbable grabbable)
        {

        }

        public virtual void OnRelease(Hand hand, Grabbable grabbable)
        {

        }

        public virtual void OnSqueeze(Hand hand, Grabbable grabbable)
        {

        }
        public virtual void OnUnsqueeze(Hand hand, Grabbable grabbable)
        {

        }
        public virtual void OnHighlight(Hand hand, Grabbable grabbable)
        {

        }
        public virtual void OnUnhighlight(Hand hand, Grabbable grabbable)
        {

        }

        public virtual void OnBtn1Down(Hand hand, Grabbable grabbable)
        {

        }
        public virtual void OnBtn1Up(Hand hand, Grabbable grabbable)
        {

        }
        public virtual void OnBtn2Down(Hand hand, Grabbable grabbable)
        {

        }
        public virtual void OnBtn2Up(Hand hand, Grabbable grabbable)
        {

        }
        public virtual void OnMenuDown(Hand hand, Grabbable grabbable)
        {

        }
        public virtual void OnMenuUp(Hand hand, Grabbable grabbable)
        {

        }
        public virtual void OnJoyStickTurnLeft(Hand hand, Grabbable grabbable)
        {

        }
        public virtual void OnJoyStickTurnRight(Hand hand, Grabbable grabbable)
        {

        }
        public virtual void OnJoyStickTurnUp(Hand hand, Grabbable grabbable)
        {

        }
        public virtual void OnJoyStickTurnDown(Hand hand, Grabbable grabbable)
        {

        }
        private void GetInputDeviceAndHandIndex(Hand hand)
        {
            string str = hand.name.Substring(0, 1);
            if (hand.left)
            {
                inputDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
                grabedHandIndex = 1;
            }
            else
            {
                inputDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
                grabedHandIndex = 2;
            }
        }
        IEnumerator InputAction(Hand hand, Grabbable grabbable)
        {
            while (isGrabed)
            {
                yield return 1;
                ButtonDispatchModel(CommonUsages.primaryButton, OnBtn1Down, OnBtn1Up, hand, grabbable);
                ButtonDispatchModel(CommonUsages.secondaryButton, OnBtn2Down, OnBtn2Up, hand, grabbable);
                ButtonDispatchModel(CommonUsages.menuButton, OnMenuDown, OnMenuUp, hand, grabbable);
                JoysitckMoveModel(OnJoyStickTurnLeft, OnJoyStickTurnRight, OnJoyStickTurnUp, OnJoyStickTurnDown, hand, grabbable);
                GetControllerVelocity(CommonUsages.deviceVelocity);
            }
        }
        private void ButtonDispatchModel(InputFeatureUsage<bool> usage, UnityAction<Hand, Grabbable> btnDown, UnityAction<Hand, Grabbable> btnUp, Hand hand, Grabbable grabbable)
        {
            if (!stateDic.ContainsKey(usage.name))
            {
                stateDic.Add(usage.name, false);
            }
            bool isDown;
            if (inputDevice.TryGetFeatureValue(usage, out isDown) && isDown)
            {
                if (!stateDic[usage.name])
                {
                    stateDic[usage.name] = true;
                    btnDown(hand, grabbable);
                }
            }
            else
            {
                if (stateDic[usage.name])
                {
                    btnUp(hand, grabbable);
                    stateDic[usage.name] = false;
                }
            }
        }

        private void JoysitckMoveModel(UnityAction<Hand, Grabbable> joystickRight, UnityAction<Hand, Grabbable> joystickLeft, UnityAction<Hand, Grabbable> joystickUp, UnityAction<Hand, Grabbable> joystickDown, Hand hand, Grabbable grabbable)
        {
            if (inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 axisValue) && Time.time > joysitckIntervalTime)
            {
                float horizontal = Vector2.Dot(axisValue, Vector2.right);
                float vertical = Vector2.Dot(axisValue, Vector2.up);

                if (horizontal > 0.8)         // right
                {
                    joysitckIntervalTime = Time.time + 0.25f;
                    joystickRight(hand, grabbable);
                }
                else if (horizontal < -0.8)   // left
                {
                    joysitckIntervalTime = Time.time + 0.25f;
                    joystickLeft(hand, grabbable);
                }

                if (vertical > 0.8)              //Up
                {
                    joysitckIntervalTime = Time.time + 0.25f;
                    joystickUp(hand, grabbable);
                }
                else if (vertical < -0.8)        //Down
                {
                    joysitckIntervalTime = Time.time + 0.25f;
                    joystickDown(hand, grabbable);
                }
            }
        }

        private void GetControllerVelocity(InputFeatureUsage<Vector3> usage)
        {
            Vector3 velocity;
            if (inputDevice.TryGetFeatureValue(usage, out velocity) && !velocity.Equals(Vector3.zero))
            {
                handVelocity = velocity;
            }
        }

        #endregion


    }
}