//使用自适应manual_height方案
#define ADJUST_BY_MANUAL_HEIGHT
using UnityEngine;
using System.Collections;

/// <summary>
/// 根据设备的宽高比，调整camera.orthographicSize. 以保证UI在不同分辨率(宽高比)下的自适应
/// 须与UIAnchor配合使用
/// 将该脚本添加到UICamera同一节点上
/// </summary>

[RequireComponent(typeof(UICamera))]
public class UICameraAdjustor : MonoBehaviour
{
		public float standard_width = 1024f;//标准UI宽度
		public float standard_height = 768;//标准UI高度

		public float device_width = 0f;
		public float device_height = 0f;

		void Awake ()
		{
				device_width = Screen.width;
				device_height = Screen.height;
				SetCameraSize ();
		}

		private void SetCameraSize ()
		{
				float standard_aspect = standard_width / standard_height;
				float device_aspect = device_width / device_height;
#if ADJUST_BY_MANUAL_HEIGHT
				UIRoot root = GameObject.Find ("UI Root").GetComponent<UIRoot> ();
				if (device_aspect < standard_aspect) {//如果目标设备的宽高比小于标准的宽高则自动调整manualHeight
						float curScreenH = (float)Screen.width / standard_aspect;
						float Hrate = curScreenH / Screen.height;
						root.manualHeight = (int)(standard_height / Hrate);
				} else {
						root.manualHeight = (int)standard_height;
				}
#else
				float adjustor = 0f;
				if (device_aspect < standard_aspect) {
						adjustor = standard_aspect / device_aspect;
						camera.orthographicSize = adjustor;
				} else {
						camera.orthographicSize = 1;
				}   
#endif
		}
#if UNITY_EDITOR &&!DLL_TYPE &&!DLL_TYPE
	void Update()
	{
		//使得动态改变分辨率时可以自适应调整
		if(device_width != Screen.width && device_height != Screen.height)
		{
			print("...");
			device_width = Screen.width;
			device_height = Screen.height;
			SetCameraSize();
		}
	}
#endif
}