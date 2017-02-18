using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Com.MingUI {
    /// <summary>
    /// Overlay不能去掉，因为它的尺寸决定了Thumb的拖拉范围
    /// </summary>
    public class CSlider : UISprite {
        public delegate void FloatFun(float v);
		

        private FloatFun onValueChange;
        private Action<float,bool> OnPressChange;

        public UISprite Thumb;
        public int PaddingLeft;
        public int PaddingRight;
        private float _value;
        private Vector3 thumbPos;
        private bool _hasInited;

        protected override void OnStart() {
            base.OnStart();
            UIEventListener.Get(Thumb.gameObject).onDrag = OnDragThumb;
			UIEventListener.Get(Thumb.gameObject).onPress = OnPress;
            InitThumbPos();
        }

        private void OnPress(GameObject go, bool state){
            if (OnPressChange != null) {
                OnPressChange(_value,state);
            }
		}

        private void InitThumbPos() {
            if (_hasInited == false) {
                thumbPos = Thumb.transform.localPosition;
                _hasInited = true;
            }
        }

        public float Value {
            set {
                InitThumbPos();
                _value = value;
                float x = _value * (this.width - PaddingRight - Thumb.width) + PaddingLeft;
                thumbPos.x = x;
                if (thumbPos.x < PaddingLeft) {
                    thumbPos.x = PaddingLeft;
                }
                if (thumbPos.x > this.width - PaddingRight - Thumb.width) {
                    thumbPos.x = this.width - PaddingRight - Thumb.width;
                }
                Thumb.transform.localPosition = thumbPos;
             //   onValueChange(_value);
            }
            get { return _value; }
        }


        private void OnDragThumb(GameObject go, Vector2 d) {
            thumbPos = Thumb.transform.localPosition;
            thumbPos.x += d.x;
            if (thumbPos.x < PaddingLeft) {
                thumbPos.x = PaddingLeft;
            }
            if (thumbPos.x > this.width - PaddingRight - Thumb.width) {
                thumbPos.x = this.width - PaddingRight - Thumb.width;
            }
            Thumb.transform.localPosition = thumbPos;
            _value = (thumbPos.x - PaddingLeft) / (this.width - PaddingLeft - PaddingRight - Thumb.width);
            if (onValueChange != null) {
                onValueChange(_value);
            }
        }

        public void AddChangeFun(FloatFun f) {
            if (onValueChange == null) {
                onValueChange = f;
            } else {
                Debug.LogError("CSlider only support one function");
            }
        }

		public void AddPress(Action<float,bool> f) {
            if (OnPressChange == null) {
                OnPressChange = f;
			} else {
				Debug.LogError("CSlider only support one function");
			}
		}
    }
}