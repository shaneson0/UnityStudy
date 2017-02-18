using Assets.Scripts.Com.Utils;
using MingUI.Com.Utils;
using System;
using UnityEngine;

namespace Assets.Scripts.Com.MingUI {
	public class CNumStepper : UIWidget {
		public CTextInput Input;
		public CButton UpBtn;
		public CButton DownBtn;
		public CButton btnMax;
		public CButton btnMin;
        public UILabel lblMax;
		public float Min = 1;
		public float Max = 99;
		public float Step = 1;
		public float DefaultValue = 1;
		private float _value = -1;

		private bool _isDownPress;
		private bool _isUpPress;
		private bool _isProceed;
		private float _pressTime;
		private Action onChangeFun;

		protected override void OnStart() {
			base.OnStart();
            if (Value == -1) {
                Value = DefaultValue;
            }
			if (UpBtn != null) {
				UpBtn.AddClick(OnClickUpBtn);
				UpBtn.AddMouseDown(OnUpBtnDown);
				UpBtn.AddMouseUp(OnBtnMouseUp);
			}
			if (DownBtn != null) {
				DownBtn.AddClick(OnClickDownBtn);
				DownBtn.AddMouseDown(OnDownBtnDown);
				DownBtn.AddMouseUp(OnBtnMouseUp);
			}
			if (btnMax != null) {
				btnMax.AddClick(OnClickMaxBtn);
			}
			if (btnMin != null) {
				btnMin.AddClick(OnClickMinBtn);
			}
			if (Input != null) {
				Input.onChange.Add(new EventDelegate(OnTextChange));
				Input.numOnly = true;
			}
            if (lblMax != null) {
                EventUtil.AddClick(lblMax.gameObject, OnLabelMax);
            }
			UIEventListener.Get(Input.gameObject).onSubmit = OnTextSubmit;
		}

		private void OnClickMaxBtn(GameObject go) {
            SetMaxValue();
		}

		private void OnClickMinBtn(GameObject go) {
			if ((int)Value == (int)Min) {
				FuncUtil.AddTip("当前已经是最小值");
				return;
			}
			Value = Min;
		}

		private void OnUpBtnDown(GameObject go) {
			_isUpPress = true;
			_pressTime = Time.time;
		}

		private void OnDownBtnDown(GameObject go) {
			_isDownPress = true;
			_pressTime = Time.time;
		}

		private void OnBtnMouseUp(GameObject go) {
			_isUpPress = false;
			_isDownPress = false;
			_isProceed = false;
			_pressTime = 0;
		}

		private void OnClickUpBtn(GameObject go) {

			if (Max - Value < float.Epsilon) {
				FuncUtil.AddTip("当前已经是最大值");
				return;
			}
			Value += Step;
		}

		private void OnClickDownBtn(GameObject go) {

			if ((Value - Min < float.Epsilon)) {
				FuncUtil.AddTip("当前已经是最小值");
				Value = Min;
				return;
			}
			Value -= Step;

		}

		private void OnTextChange() {
			try {
				if (Input.Text == "") {
					Value = Min;
					return;
				}

				var f = float.Parse(Input.Text);
				float i = f > Max ? Max : f;
				i = i < Min ? Min : i;
				Value = i;

			} catch (Exception e) {
				FuncUtil.AddTip("请输入正确的数字格式 :" + e.Message);
				Value = Min;
				Input.Text = Min + "";
			}
		}

		private void OnTextSubmit(GameObject go) {
			Value = Int32.Parse(Input.Text);
		}

		protected override void OnUpdate() {
			base.OnUpdate();
			if (_isUpPress || _isDownPress) {
				if (Time.time - _pressTime > 0.1f) {
					_isProceed = true;
				}
			}
			if (_isProceed) {
				if (_isUpPress) {
					if (Value < Max) {
						Value = Math.Min(Max, Value + Step);
					}
				} else if (_isDownPress) {
					if (Value > Min) {
						Value = Math.Max(Min, Value - Step);
					}
				}
			}
		}

		public void OnChange(Action fun) {
			onChangeFun = fun;
		}

		public float Value {
			set {
				bool isChange = false;
				if (_value != value) {
					isChange = true;
				}
				_value = value;
				Input.Text = _value.ToString();
				if (isChange && onChangeFun != null) {
					onChangeFun.DynamicInvoke();
				}
			}
			get { return _value; }
		}

        public void SetMaxLabel(bool b = true) {
            if (lblMax != null) {
                lblMax.gameObject.SetActive(b);
            }
        }

        private void OnLabelMax(object obj) {
            SetMaxValue();
        }

        private void SetMaxValue() {
            if ((int)Value == (int)Max) {
                FuncUtil.AddTip("当前已经是最大值");
                return;
            }
            Value = Max;
        }
	}
}