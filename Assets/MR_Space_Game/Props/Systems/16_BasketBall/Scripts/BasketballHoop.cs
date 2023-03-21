using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Autohand;

namespace MRSpace.FrameWork.SystemProps 
{ 
public class BasketballHoop : SystemPropsBase
{
    public GameObject virtualHoop;
    private RaycastHit hitWall;
    private float maxDistance = 2.5f;
    private bool isStick = false;
     
     // Start is called before the first frame update
    void Start()
    {
        
    }

     private void ColliderWall(bool isCollider)
     {
            if (isCollider)
            {
                print("Grabbed");
                if (Physics.Raycast(transform.position, -transform.forward,out  hitWall, maxDistance))
                {
                    print("Rayhit");
                    if (hitWall.transform.gameObject.layer == LayerMask.NameToLayer("Wall"))
                    {
                        print("Hitwall");
                        try
                        {
                            virtualHoop.SetActive(true);
                            virtualHoop.transform.position = hitWall.point + (hitWall.collider.transform.up * 0.02f);
                            virtualHoop.transform.forward = hitWall.collider.transform.up;
                            isStick = true;
                            return;
                        }
                        catch { }
                    }
                }
            }
            isStick = false;
            virtualHoop.SetActive(false);

        }
      
      
     public override void OnRelease(Hand hand, Grabbable grabbable)
     {
            if (isStick)
            {
                transform.position = virtualHoop.transform.position;
                transform.rotation = virtualHoop.transform.rotation;
                virtualHoop.SetActive(false);
                
                isStick = false;
            }
            
            foreach (var item in GetComponentsInChildren<Collider>())
            {
                item.enabled = true;
            }
             

        }

        public override void OnGrab(Hand hand, Grabbable grabbable)
        {

            GetComponentInChildren<Collider>().enabled = false;
            //foreach (var item in GetComponentsInChildren<Collider>())
            //{
            //    item.enabled = false;
            //}


        }
        void Update()
     {
            ColliderWall(isGrabed);
            
     }
  }
}