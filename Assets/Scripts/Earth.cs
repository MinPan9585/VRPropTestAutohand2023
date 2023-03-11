using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Earth : MonoBehaviour
{
	public float speed = 1;
	//转动的位置
	public float progress = 0;
	//中心点
	public Transform center;
	//半径
	public float radius;
	bool circling = true;
	void Update()
	{
        if (circling)
        {
			progress += Time.deltaTime * speed;
			if (progress >= 360)
			{
				progress -= 360;
			}
			float x1 = center.position.x + radius * Mathf.Cos(progress);
			float y1 = center.position.y + radius * Mathf.Sin(progress);
			this.transform.localPosition = new Vector3(x1, 0, y1);
		}
	}

	public void SetCirclingTrue()
    {
		circling = true;
    }

	public void SetCirclingFalse()
	{
		circling = false;
	}
}
