using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Earth : MonoBehaviour
{
	public float speed = 1;
	//ת����λ��
	public float progress = 0;
	//���ĵ�
	public Transform center;
	//�뾶
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
