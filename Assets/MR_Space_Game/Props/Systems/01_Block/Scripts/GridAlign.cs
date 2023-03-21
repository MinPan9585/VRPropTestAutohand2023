using Autohand;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MRSpace.FrameWork.SystemProps
{
    public class GridAlign : SystemPropsBase
    {
        public float rayMaxDis = 0.3f;
        public Material centerPointMat;


        private static string centerStr = "CenterPoint";
        private List<GameObject> centerList = new List<GameObject>();

        private string targetDir = "";
        private GameObject lastObject;
        private GameObject currentCenter;
        private GameObject targetCenter;
        public GameObject TargetCenter { get => targetCenter; set => targetCenter = value; }

        protected override void Init()
        {
            base.Init();
            InitCenterPoint(GetComponentInChildren<BoxCollider>().size * 0.5f);
        }


        public override void OnRelease(Hand hand, Grabbable grabbable)
        {
            SetTransformPosAndRot();
            ResetSelf();
        }


        private void FixedUpdate()
        {
            RayToSide();
        }
        private void RayToSide()
        {
            if (isGrabed)
            {
                for (int i = 0; i < centerList.Count; i++)
                {
                    if (currentCenter != null && currentCenter != centerList[i]) continue;

                    Ray ray = new Ray(transform.position, centerList[i].transform.position - transform.position);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, 0.2f))
                    {
                        if (hit.transform.GetComponent<GridDisplay>())
                        {
                            string hitStr = hit.transform.GetComponent<GridDisplay>().CreatAndShowSidePoints(hit.point, centerList[i]);

                            if (lastObject != null && targetDir != "")
                            {
                                if (lastObject != hit.transform.gameObject || targetDir != hitStr)
                                {
                                    lastObject.transform.GetComponent<GridDisplay>().HideSidePoints(targetDir);
                                }
                            }
                            targetDir = hitStr;
                            lastObject = hit.transform.gameObject;
                            centerList[i].GetComponent<MeshRenderer>().enabled = true;
                            currentCenter = centerList[i];
                        }
                        else
                        {
                            ResetSelf();
                        }
                    }
                    else
                    {
                        ResetSelf();
                    }
                    Debug.DrawLine(centerList[i].transform.position, centerList[i].transform.position + (centerList[i].transform.position - transform.position).normalized * rayMaxDis, Color.red);
                }
            }
        }

        private void ResetSelf()
        {
            if (lastObject != null && targetDir != "")
            {
                lastObject.GetComponent<GridDisplay>().HideSidePoints(targetDir);
                currentCenter.GetComponent<MeshRenderer>().enabled = false;
                targetDir = "";
                lastObject = null;
                currentCenter = null;
            }
        }

        private void SetTransformPosAndRot()
        {
            if (targetCenter == null) return;
            if (currentCenter == null) return;

            Vector3 centerNormal = transform.position - currentCenter.transform.position;
            Vector3 targetNormal = lastObject.GetComponent<GridDisplay>().GetTargetNormal(targetDir);

            //同步位置
            Vector3 dir = targetNormal.normalized * Vector3.Distance(currentCenter.transform.position, transform.position);
            transform.position = targetCenter.transform.position + dir;

            //调整物体旋转
            AngleRotate(centerNormal, targetNormal);
            foreach (Transform tran in transform.Find(centerStr))
            {
                if (Vector3.Dot((tran.position - transform.position).normalized, lastObject.transform.forward.normalized) > 0.8f)
                {
                    AngleRotate(tran.position - transform.position, lastObject.transform.forward);
                    return;
                }
            }
        }

        private void AngleRotate(Vector3 self, Vector3 target)
        {
            // 计算需要旋转的角度
            float _angle = Vector3.Angle(self, target);
            // 计算需要旋转的轴
            Vector3 _axis = Vector3.Cross(self, target);
            // 将第一个平面绕旋转轴旋转需要的角度
            transform.rotation = Quaternion.AngleAxis(_angle, _axis) * transform.rotation;
        }


        private void InitCenterPoint(Vector3 localScale)
        {
            GameObject centerPoint = new GameObject(centerStr);
            centerPoint.transform.SetParent(transform);
            centerPoint.transform.localPosition = Vector3.zero;
            CreatPoint(centerPoint, new Vector3(0, localScale.y, 0));
            CreatPoint(centerPoint, new Vector3(0, -localScale.y, 0));
            CreatPoint(centerPoint, new Vector3(localScale.x, 0, 0));
            CreatPoint(centerPoint, new Vector3(-localScale.x, 0, 0));
            CreatPoint(centerPoint, new Vector3(0, 0, localScale.z));
            CreatPoint(centerPoint, new Vector3(0, 0, -localScale.z));
        }


        private void CreatPoint(GameObject parent, Vector3 pos)
        {
            GameObject _sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            _sphere.transform.SetParent(parent.transform);
            _sphere.GetComponent<MeshRenderer>().material = centerPointMat;
            Destroy(_sphere.GetComponent<Collider>());
            _sphere.transform.localPosition = pos;
            _sphere.transform.localScale = Vector3.one * 0.01f;
            _sphere.GetComponent<MeshRenderer>().enabled = false;
            centerList.Add(_sphere);
        }



    }
}