using System.Linq;
using UnityEngine;
using Assets.Scripts.Com.Utils;
using System;

namespace Assets.Scripts.Com.MingUI {
	/// <summary>
	///     这个类的子对象的位置是可以随意摆放的
	/// </summary>
	public class CCanvas : UISprite {
		public int PaddingLeft = 4; //Content与背景的距离
		public int PaddingTop = 4;
		public CScrollBar Bar;
		public CScrollBar hBar;
		public UIPanel Content;
		public bool alwaysShowBar;
		protected int contentHeight;
		protected int contentWidth;
		private Vector2 maskOffset;
		private UIEventListener.FloatDelegate barChangeFun;
		private bool hasStart;
		public GameObject childPanel;
		public float maxRoll;   //滚动的最大值
		public float minRoll;   //滚动的最小值
        public float maxRollPixel = 70;
		public void Init() {
			OnStart();
			Bar.Init();
		}
		protected override void OnStart() {
			base.OnStart();
			if (Bar != null) {
				Bar.Init();
				Bar.OnChangeFun = OnBarChange;
			}
			if (hBar != null) {
				hBar.OnChangeFun = OnVBarChange;
			}
			if (hasStart == false) {
				CalculateHeight();
				CalculateWidth();
				UICameraUtil.AddGenericScroll(OnCameraScroll);
				UIEventListener.Get(this.gameObject).onScroll = OnCameraScroll;
				maskOffset = Content.clipOffset;
				hasStart = true;
			}
		}

		protected override void OnEnable() {
			base.OnEnable();
			UICameraUtil.AddGenericScroll(OnCameraScroll);
		}

		protected override void OnDisable() {
			base.OnDisable();
			UICameraUtil.RemoveGenericScroll(OnCameraScroll);
		}

		private void OnCameraScroll(GameObject go, float delta) {
			if (childPanel != null && (go == childPanel || UICamera.hoveredObject.transform.IsChildOf(childPanel.transform) == true)) {
				return;
			}

			if (go == gameObject || UICamera.hoveredObject.transform.IsChildOf(transform) == true) {
				if (Bar != null && Bar.isVisible) {
					float v = Bar.value;
					float roll = delta;
					if (maxRoll != 0 && delta >= 0) {
						roll = Math.Min(maxRoll, roll);
                    } else if (maxRollPixel != 0) {
                        roll = Math.Min(maxRollPixel / contentHeight, Math.Abs(roll));
                        roll *= delta < 0 ? -1 : 1;
                    }
					if (minRoll != 0 && delta <= 0) {
						roll = Math.Max(minRoll, roll);
					}

					v += roll * -0.8f;
					if (v < 0) {
						v = 0;
					} else if (v > 1) {
						v = 1;
					}
					Bar.value = v;
				}
			}
		}

		private void OnBarChange(GameObject go, float v) {
			//Debug.Log("contentHeight: " + contentHeight + "  this.height: " + this.height + " v:" + v);
			//if (contentHeight > this.height) {
		    float dh = contentHeight - this.height;
            float cy = Mathf.Lerp(0, (dh <= 0 ? 0 : dh) + 2, v);
			Vector3 p = Content.transform.localPosition;
			if (rawPivot == UIWidget.Pivot.BottomLeft) {
				p.y = contentHeight + cy + PaddingTop;
				Content.transform.localPosition = p;//new Vector3(PaddingLeft, cy - PaddingTop, 0);
			} else {
				p.y = cy - PaddingTop;
				Content.transform.localPosition = p;//new Vector3(PaddingLeft, cy - PaddingTop, 0);
			}
			maskOffset.y = -(cy - PaddingTop);
			Content.GetComponent<UIPanel>().clipOffset = maskOffset;
			if (barChangeFun != null)
				barChangeFun(gameObject, v);

			//}
		}

		private void OnVBarChange(GameObject go, float v) {
			if (contentWidth > this.width) {
				float cx = Mathf.Lerp(0, contentWidth - this.width + 2, v);
				Vector3 p = Content.transform.localPosition;
				p.x = -(cx + PaddingLeft);
				Content.transform.localPosition = p;
				maskOffset.x = -p.x;
				Content.GetComponent<UIPanel>().clipOffset = maskOffset;
			}
		}

		public void AddBarChange(UIEventListener.FloatDelegate fun) {
			if (barChangeFun == null) {
				barChangeFun = fun;
			} else {
				barChangeFun += fun;
			}
		}

		///reCalHeight是否要计算高度，当同时添加很多个子对象时需要设置为false
		/// 避免不必要的计算
		/// 全部添加好后再计算
		public void AddChild(Transform t, bool reCalHeight = true) {
			t.parent = Content.transform;
			t.localScale = Vector3.one;
			if (reCalHeight == true) {
				CalculateHeight();
				CalculateWidth();
			}
		}

		public void CalculateHeight(int conHeight = 0) {
			int oldContentHeight = contentHeight;
			if (conHeight == 0) {
				contentHeight = Mathf.Abs(UIUtil.getBottomY(Content.transform));
			} else {
				contentHeight = conHeight;
			}
			if (Bar == null) {
				return;
			}
			Bar.gameObject.SetActive(alwaysShowBar || contentHeight > this.height);
			if (Bar.gameObject.activeSelf == true) {
				float temp = oldContentHeight * Bar.value;
				Bar.BarSize = (float)this.height / (float)contentHeight;
				float value = Mathf.Min(1f, temp / contentHeight);
				Bar.value = value;
			} else {
				Bar.value = 0;
			}


		}

		public void CalculateWidth() {
			contentWidth = Mathf.Abs(UIUtil.getRightX(Content.transform));
			if (hBar == null) {
				return;
			}
			hBar.gameObject.SetActive(contentWidth > this.width);
			if (hBar.gameObject.activeSelf == true) {
				hBar.BarSize = (float)this.width / (float)contentWidth;
			}
		}

		public void RemoveChild(Transform t) {
			Destroy(t.gameObject);
		}

		public void Clear() {
			Transform[] list = DisplayUtil.getChildList(Content.transform);
			foreach (Transform t in list) {
				RemoveChild(t);
			}
		}
	}
}