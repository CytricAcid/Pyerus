using UnityEngine;
using UnityEditor;

public class TransformMacros : Editor
{
	[MenuItem("Macros/Snap Selection To Ground #END")]
	public static void SnapToGround()
	{
		foreach (Transform t in Selection.transforms)
		{
			RaycastHit rayhit;
			if (Physics.Raycast(t.position, Vector3.down, out rayhit))
			{
				Vector3 offset = Vector3.zero;
				MeshRenderer renderer = t.GetComponentInChildren<MeshRenderer>();
				if (renderer != null)
				{
					if (renderer.bounds.center.y - renderer.bounds.extents.y < t.position.y)
					{
						offset = new Vector3(0, renderer.bounds.extents.y, 0);
					}
				}
				t.position = rayhit.point + offset;
			}
		}
	}
}
