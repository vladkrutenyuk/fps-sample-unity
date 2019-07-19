using UnityEngine;

public class Util
{
	public static void SetLayerRecursevly(GameObject _obj, int _newLayer)
	{
		if (_obj == null)
			return;

		_obj.layer = _newLayer;

		foreach (Transform _child in _obj.transform)
		{
			if (_child == null)
				continue;

			SetLayerRecursevly(_child.gameObject, _newLayer);
		}
	}
}
