using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MRSpace.FrameWork.SystemProps
{
	public class Earth : SystemPropsBase
	{
		public float speed = 1;
		public float startProgress = 0;
		public float progress = 0;
		public Transform center;
		public float radius;
		bool circling = true;
		bool returning = false;
		float returningSpeed;
		Vector3 closestPos;
		Vector3 returningDir;

		private void Start()
		{
			returningSpeed = radius * 1.35f;
		}
		void Update()
		{
			if (circling)
			{
				progress += Time.deltaTime * speed;
				if (progress >= 360)
				{
					progress -= 360;
				}

				float x2 = radius * Mathf.Cos(progress);
				float y2 = radius * Mathf.Sin(progress);
				this.transform.localPosition = new Vector3(x2, 0, y2);
			}
			if (returning)
			{
				Vector3 outDir = transform.localPosition;
				Vector3 planeDir = Vector3.ProjectOnPlane(outDir, Vector3.up);
				closestPos = planeDir.normalized * radius;
				returningDir = (closestPos - transform.localPosition).normalized;
				float distanceThisFrame = returningSpeed * Time.deltaTime;
				transform.localPosition += returningDir * distanceThisFrame;
				if (distanceThisFrame >= Vector3.Distance(closestPos, transform.localPosition))
				{
					progress = Mathf.Atan2(closestPos.z, closestPos.x);

					returning = false;
					circling = true;
				}
			}
		}

		public void ReleaseEarth()
		{
			returning = true;
		}

		public void SetCirclingFalse()
		{
			circling = false;
		}
	}
}

