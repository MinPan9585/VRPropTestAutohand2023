using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GridDisplay))]
public class ExampleEditor : Editor
{
    void OnSceneGUI()
    {
        GridDisplay grid = target as GridDisplay;
        Handles.color = Color.yellow;
        Vector3 gridPos = grid.transform.position;

        float _scaleX = grid.gridSize.x * grid.transform.localScale.x * 0.5f;
        float _scaleY = grid.gridSize.y * grid.transform.localScale.y * 0.5f;
        float _scaleZ = grid.gridSize.z * grid.transform.localScale.z * 0.5f;

        Vector3 p1 = grid.transform.position + grid.transform.forward.normalized * _scaleZ + grid.transform.up.normalized * _scaleY - grid.transform.right.normalized * _scaleX;
        Vector3 p2 = grid.transform.position + grid.transform.forward.normalized * _scaleZ + grid.transform.up.normalized * _scaleY + grid.transform.right.normalized * _scaleX;
        Vector3 p3 = grid.transform.position + grid.transform.up.normalized * _scaleY + grid.transform.right.normalized * _scaleX - grid.transform.forward.normalized * _scaleZ;
        Vector3 p4 = grid.transform.position + grid.transform.up.normalized * _scaleY - grid.transform.right.normalized * _scaleX - grid.transform.forward.normalized * _scaleZ;
        Vector3 p5 = grid.transform.position + grid.transform.forward.normalized * _scaleZ - grid.transform.up.normalized * _scaleY - grid.transform.right.normalized * _scaleX;
        Vector3 p6 = grid.transform.position + grid.transform.forward.normalized * _scaleZ - grid.transform.up.normalized * _scaleY + grid.transform.right.normalized * _scaleX;
        Vector3 p7 = grid.transform.position - grid.transform.up.normalized * _scaleY + grid.transform.right.normalized * _scaleX - grid.transform.forward.normalized * _scaleZ;
        Vector3 p8 = grid.transform.position - grid.transform.up.normalized * _scaleY - grid.transform.right.normalized * _scaleX - grid.transform.forward.normalized * _scaleZ;

        Handles.DrawLine(p1, p2, 3f);
        Handles.DrawLine(p2, p3, 3f);
        Handles.DrawLine(p3, p4, 3f);
        Handles.DrawLine(p4, p1, 3f);
        Handles.DrawLine(p5, p6, 3f);
        Handles.DrawLine(p6, p7, 3f);
        Handles.DrawLine(p7, p8, 3f);
        Handles.DrawLine(p8, p5, 3f);
        Handles.DrawLine(p1, p5, 3f);
        Handles.DrawLine(p2, p6, 3f);
        Handles.DrawLine(p3, p7, 3f);
        Handles.DrawLine(p4, p8, 3f);
    }
}
