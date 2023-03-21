using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MRSpace.FrameWork.SystemProps
{
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshFilter))]
    public class TubeMesh : MonoBehaviour
    {
        private Mesh mesh;
        private List<Vector3> vertices;
        private List<int> tri_indexes;
        private List<Vector2> uvs;

        private int angle_num = 16;
        private float radius = 0.2f;
        private const float min_dist = 0.003f;
        //private const int angle_min = 3, angle_max = 30;
        private const float radius_min = 0.001f, radius_max = 500.0f;
        private bool first_mesh = true;
        private Vector3 prev_pose0 = Vector3.zero;
        private Vector3 prev_pose1 = Vector3.zero;
        private Vector3 prev_normal = Vector3.zero;

        //生成mesh
        public void CreatBrushMesh(Material _matrial,Vector3 _pose, float _brushRadius)
        {
            vertices = new List<Vector3>();
            tri_indexes = new List<int>();
            uvs = new List<Vector2>();
            mesh = GetComponent<MeshFilter>().mesh;
            GetComponent<MeshRenderer>().sharedMaterial = new Material(_matrial);

            //半径
            radius = Mathf.Clamp(_brushRadius, radius_min, radius_max);

            //初始化点和三角形索引
            AddVertex();
            AddVertex();
            AddTriangleIndex();
            AddFace();

            //初始化值2
            prev_pose0 = _pose;
            prev_pose1 = _pose;
            ApplyMesh();
        }



        public void AddMesh(Vector3 _pose)
        {
            //如果与前一个值的距离小于一定距离，则运行
            float distance = Vector3.Distance(_pose, prev_pose0);
            if (distance < min_dist) return;

            //添加面
            AddFace();

            //法线、方向
            Vector3 normal = Vector3.zero;
            Vector3 direction = Vector3.zero;

            if (first_mesh)
            {
                //计算角度
                direction = _pose - prev_pose0;
                float angleUp = Vector3.Angle(direction, Vector3.up);


                if (angleUp < 10.0f || angleUp > 170.0f)
                    normal = Vector3.Cross(direction, Vector3.right);
                else
                    normal = Vector3.Cross(direction, Vector3.up);

                normal = normal.normalized;
                prev_normal = normal;
            }
            else
            {
                Vector3 prev_perp = Vector3.Cross(prev_pose0 - prev_pose1, prev_normal);
                normal = Vector3.Cross(prev_perp, _pose - prev_pose0).normalized;
            }

            //Vertex 更新
            if (first_mesh)
                UpdateVertex(4, prev_pose0);

            UpdateVertex(1, _pose);
            UpdateVertex(2, _pose, _pose - prev_pose0, normal);
            UpdateVertex(3, prev_pose0, _pose - prev_pose1, prev_normal);

            prev_pose1 = prev_pose0;
            prev_pose0 = _pose;
            prev_normal = normal;

            ApplyMesh();
            first_mesh = false;
        }

        #region 点和面的位置，index计算
        //添加面=添加点+三角形index
        private void AddFace()
        {
            AddVertex();
            AddTriangleIndex();
        }

        //添加点
        private void AddVertex()
        {
            for (int i = 0; i < angle_num; i++)
            {
                vertices.Add(Vector3.zero);
                uvs.Add(new Vector2(i / (angle_num - 1.0f), 0f));
            }
        }

        //添加三角形index
        private void AddTriangleIndex()
        {
            int last_index = vertices.Count - 1;
            for (int i = 0; i < angle_num; i++)
            {
                int index_0 = last_index - i;
                int index_1 = last_index - (i + 1) % angle_num;

                tri_indexes.Add(index_0);
                tri_indexes.Add(index_1 - angle_num);
                tri_indexes.Add(index_0 - angle_num);

                tri_indexes.Add(index_0);
                tri_indexes.Add(index_1);
                tri_indexes.Add(index_1 - angle_num);
            }
        }


        //末尾是第几索引/位置/方向/法线
        private void UpdateVertex(int _face_index, Vector3 _pose, Vector3 _dir, Vector3 _normal)
        {
            Vector3 dir = _dir.normalized;
            Vector3 normal = _normal.normalized;

            //查找末尾的索引
            int start_index = vertices.Count - angle_num * _face_index;
            for (int i = 0; i < angle_num; i++)
            {
                //计算点的位置
                float angle = 360.0f * (i / (float)angle_num);
                Quaternion rotator = Quaternion.AngleAxis(angle, dir);
                Vector3 result_anlge = rotator * normal * radius;
                //添加点
                vertices[start_index + i] = _pose + result_anlge;
            }
        }
        private void UpdateVertex(int _face_index, Vector3 _pose)
        {
            int start_index = vertices.Count - angle_num * _face_index;
            //添加点
            for (int i = 0; i < angle_num; i++)
                vertices[start_index + i] = _pose;

        }
        #endregion

        #region 网格相关
        private void ApplyMesh()
        {
            if (mesh != null)
            {
                mesh.SetVertices(vertices);
                mesh.SetUVs(0, uvs);
                mesh.SetIndices(tri_indexes.ToArray(), MeshTopology.Triangles, 0);
                mesh.RecalculateBounds();
                mesh.RecalculateNormals();
            }
        }

        public void UpdateMesh()
        {
            if (mesh != null)
            {
                mesh.RecalculateBounds();
                mesh.RecalculateNormals();
            }
        }
      
        #endregion
    }
}