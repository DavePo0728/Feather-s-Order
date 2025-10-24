using UnityEngine;
using System.Collections.Generic;

[ExecuteAlways]
public class DirectionToSkybox : MonoBehaviour
{
	public GameObject sun;   // 模拟太阳的空物体
	public GameObject moon;  // 模拟月亮的空物体
	public List<MeshRenderer> targetMaterialCloud;

	[Header("Shader Property Names")]
	public string sunDirectionPropertyName = "_SunDirection";
	public string moonDirectionPropertyName = "_MoonDirection";

	[Header("Material Mode")]
	public bool useSharedMaterial = true; // ✅ 切換共用或實例化

	void Update()
	{
		if (sun != null)
		{
			Vector3 sunDirection = -sun.transform.forward.normalized;
			ApplyToAllMaterials(sunDirectionPropertyName, sunDirection);
		}

		if (moon != null)
		{
			Vector3 moonDirection = -moon.transform.forward.normalized;
			ApplyToAllMaterials(moonDirectionPropertyName, moonDirection);
		}
	}

	private void ApplyToAllMaterials(string propertyName, Vector3 dir)
	{
		foreach (var item in targetMaterialCloud)
		{
			if (item == null) continue;

			if (useSharedMaterial)
				item.sharedMaterial.SetVector(propertyName, dir);  // ✅ 共用材質，不會Instance化
			else
				item.material.SetVector(propertyName, dir);        // ❌ Instance化，但能獨立修改
		}
	}
}
