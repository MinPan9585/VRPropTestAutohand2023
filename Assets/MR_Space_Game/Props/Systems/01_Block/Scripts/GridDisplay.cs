using MRSpace.FrameWork.SystemProps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GridListName
{
    public static string upPoints = "upPoints";
    public static string rightPoints = "rightPoints";
    public static string downPoints = "downPoints";
    public static string leftPoints = "leftPoints";
    public static string frontPoints = "frontPoints";
    public static string backPoints = "backPoints";
}

public class GridDisplay : MonoBehaviour
{
    [Tooltip("网格尺寸大小")]
    public Vector3 gridSize;
    [Tooltip("网格细分段数")]
    public Vector3Int gridSegments;
    public Material touchPointMat;

    private float scaleX, scaleY, scaleZ;
    private List<Vector3> upPoints, rightPoints, downPoints, leftPoints, frontPoints, backPoints;
    private Dictionary<string, GameObject> touchPointsDir = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject>.KeyCollection keyTouchPoints;

    private void Awake()
    {
        scaleX = transform.localScale.x * (gridSize.x * 0.5f/gridSegments.x);
        scaleY = transform.localScale.y * (gridSize.y * 0.5f/gridSegments.y);
        scaleZ = transform.localScale.z * (gridSize.z * 0.5f/gridSegments.z);     
    }


    private void Start()
    {
        keyTouchPoints = touchPointsDir.Keys;
    }

    private void FixedUpdate()
    {
        if (touchPointsDir.Count > 0)
        {
            foreach (string name in keyTouchPoints)
            {
                foreach (Transform point in transform.Find(name))
                {
                    float dis = Vector3.Distance(point.transform.position, touchPointsDir[name].transform.position);
                    if (dis <= 0.2f)
                    {
                        point.transform.localScale = Vector3.one * (0.2f - dis) * 0.05f;
                    }
                    else
                    {
                        point.transform.localScale = Vector3.zero;
                    }

                    if (dis < 0.03f)
                    {
                        point.GetComponent<MeshRenderer>().material.color = Color.green;
                        touchPointsDir[name].transform.GetComponentInParent<GridAlign>().TargetCenter = point.gameObject;
                    }
                    else
                    {
                        point.GetComponent<MeshRenderer>().material.color = touchPointMat.color;
                        if (touchPointsDir[name].transform.GetComponentInParent<GridAlign>().TargetCenter == point.gameObject)
                        {
                            touchPointsDir[name].transform.GetComponentInParent<GridAlign>().TargetCenter = null;
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// 显示网格点
    /// </summary>
    public string CreatAndShowSidePoints(Vector3 point,GameObject _target)
    {
        Vector3 localPoint = transform.InverseTransformPoint(point);
        float x = Mathf.Abs(localPoint.x);
        float y = Mathf.Abs(localPoint.y);
        float z = Mathf.Abs(localPoint.z);
        if (x >= y && x >= z)//Right && Left
        {
            if (localPoint.x > 0)//Right
            {
                //Debug.Log("The point is on Right.");
                CreatTouchPointsObj(GridListName.rightPoints, ref rightPoints, Vector3.right, _target);
                return GridListName.rightPoints;
            }
            else//Left
            {
                //Debug.Log("The point is on Left");
                CreatTouchPointsObj(GridListName.leftPoints, ref leftPoints, Vector3.left, _target);
                return GridListName.leftPoints;
            }
        }
        else if (y >= x && y >= z)//Up && Down
        {
            if (localPoint.y > 0)
            {
                //Debug.Log("The point is on Up.");
                CreatTouchPointsObj(GridListName.upPoints, ref upPoints, Vector3.up, _target);
                return GridListName.upPoints;

            }
            else
            {
                //Debug.Log("The point is on Down");
                CreatTouchPointsObj(GridListName.downPoints, ref downPoints, Vector3.down, _target);
                return GridListName.downPoints;
            }
        }
        else//Front && Back
        {
            if (localPoint.z > 0)
            {
                //Debug.Log("The point is on Front.");
                CreatTouchPointsObj(GridListName.frontPoints, ref frontPoints, Vector3.forward, _target);
                return GridListName.frontPoints;
            }
            else
            {
                //Debug.Log("The point is on Back");
                CreatTouchPointsObj(GridListName.backPoints, ref backPoints, Vector3.back, _target);
                return GridListName.backPoints;
            }
        }     
    }

    public void HideSidePoints(string listName)
    {
        if(transform.Find(listName))
        {
            transform.Find(listName).gameObject.SetActive(false);
            touchPointsDir.Remove(listName);
        }
        else
        {
            Debug.LogError("没有找到该方向的空间点,方向为" + listName);
        }
       
    }

    public Vector3 GetTargetNormal(string listName) 
    {
        if (listName == GridListName.upPoints)
        {
            return transform.up;
        }
        else if(listName == GridListName.downPoints)
        {
            return -transform.up;
        }
        else if (listName == GridListName.leftPoints)
        {
            return -transform.right;
        }
        else if (listName == GridListName.rightPoints)
        {
            return transform.right;
        }
        else if (listName == GridListName.frontPoints)
        {
            return transform.forward;
        }
        else if (listName == GridListName.backPoints)
        {
            return -transform.forward;
        }
        return Vector3.zero;
    }

    private void CreatTouchPointsObj(string _name, ref List<Vector3> _sideList, Vector3 _dir, GameObject _target)
    {
        if (_sideList == null)
        {
            _sideList = new List<Vector3>();
            _sideList = CreatTouchPointsVector3List(_dir);
        }

        if (!transform.Find(_name))
        {
            GameObject pointList = new GameObject(_name);
            pointList.transform.SetParent(transform);
            pointList.transform.localPosition = Vector3.zero;
            pointList.transform.localRotation = Quaternion.identity;
            for (int i = 0; i < _sideList.Count; i++)
            {
                GameObject _sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                _sphere.transform.SetParent(pointList.transform);
                _sphere.GetComponent<MeshRenderer>().material = touchPointMat;
                Destroy(_sphere.GetComponent<Collider>());
                _sphere.transform.localPosition = _sideList[i];
                _sphere.transform.localScale = Vector3.zero;
            }
            pointList.SetActive(false);
        }

        if (transform.Find(_name).gameObject.activeSelf == false)
        {
            transform.Find(_name).gameObject.SetActive(true);
            touchPointsDir.Add(_name, _target);
        }
    }

    private List<Vector3> CreatTouchPointsVector3List(Vector3 side)
    {
        List<Vector3> touchPoints = new List<Vector3>();

        if (side.x != 0)//right && left
        {           
            for (int i = 0; i < gridSegments.y + 1; i++)
            {
                for (int j = 0; j < gridSegments.z + 1; j++)
                {
                    Vector3 _sidePoint = new Vector3(gridSegments.x * scaleX * side.x, scaleY * (gridSegments.y - 2 * i), scaleZ * (gridSegments.z - 2 * j));
                    touchPoints.Add(_sidePoint);
                }
            }
            for (int i = 0; i < gridSegments.y; i++)
            {
                for (int j = 0; j < gridSegments.z; j++)
                {
                    Vector3 _centerPoint = new Vector3(gridSegments.x * scaleX * side.x, scaleY * (gridSegments.y - 1 - 2 * i), scaleZ * (gridSegments.z - 1 - 2 * j));
                    touchPoints.Add(_centerPoint);
                }
            }
        }
        else if (side.y != 0)//up && down
        {
            for (int i = 0; i < gridSegments.x + 1; i++)
            {
                for (int j = 0; j < gridSegments.z + 1; j++)
                {
                    Vector3 _sidePoint = new Vector3(scaleX * (gridSegments.x - 2 * i), gridSegments.y * scaleY * side.y, scaleZ * (gridSegments.z - 2 * j));
                    touchPoints.Add(_sidePoint);
                }
            }
            for (int i = 0; i < gridSegments.x; i++)
            {
                for (int j = 0; j < gridSegments.z; j++)
                {
                    Vector3 _centerPoint = new Vector3(scaleX * (gridSegments.x - 1 - 2 * i), gridSegments.y * scaleY * side.y, scaleZ * (gridSegments.z - 1 - 2 * j));
                    touchPoints.Add(_centerPoint);
                }
            }          
        }
        else if (side.z != 0)//front && back
        {
            for (int i = 0; i < gridSegments.x + 1; i++)
            {
                for (int j = 0; j < gridSegments.y + 1; j++)
                {
                    Vector3 _sidePoint = new Vector3(scaleX * (gridSegments.x - 2 * i), scaleY * (gridSegments.y - 2 * j), gridSegments.z * scaleZ * side.z);
                    touchPoints.Add(_sidePoint);
                }
            }
            for (int i = 0; i < gridSegments.x; i++)
            {
                for (int j = 0; j < gridSegments.y; j++)
                {
                    Vector3 _centerPoint = new Vector3(scaleX * (gridSegments.x - 1 - 2 * i), scaleY * (gridSegments.y - 1 - 2 * j), gridSegments.z * scaleZ * side.z);
                    touchPoints.Add(_centerPoint);
                }
            }
        }
        return touchPoints;
    }

   
}
