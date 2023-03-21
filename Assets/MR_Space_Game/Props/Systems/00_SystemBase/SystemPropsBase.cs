using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MRSpace.FrameWork.SystemProps
{
    public class SystemPropsBase : SystemXRInput
    {
        public bool isReleaseKinematic;

        /// <summary>
        /// 初始化 替代Awake
        /// </summary>
        protected override void Init()
        {

        }

        protected override void SetRigidbodyKinematic()
        {
            if (GetComponent<Rigidbody>())
                GetComponent<Rigidbody>().isKinematic = isReleaseKinematic;
        }



    }


}