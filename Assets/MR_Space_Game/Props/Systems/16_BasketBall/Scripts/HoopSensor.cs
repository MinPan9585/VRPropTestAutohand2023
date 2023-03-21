using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MRSpace.FrameWork.SystemProps
{
    public class HoopSensor : MonoBehaviour
    {
        // Start is called before the first frame update
        public ParticleSystem vfx;

        void Start()
        {

        }

        private void OnTriggerEnter(Collider other)
        {

        }
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.layer != LayerMask.NameToLayer("Hand")&&other.GetComponent<Rigidbody>().velocity.y < 0 )
            {
                vfx.Play();
            }

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
