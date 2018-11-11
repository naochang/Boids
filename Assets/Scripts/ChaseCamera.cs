using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 群衆を追うカメラ。
/// </summary>
public class ChaseCamera : MonoBehaviour
{
	private void Update()
	{
		this.transform.LookAt(Boid.GetCenterPosition());
	}
}
