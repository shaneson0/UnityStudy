using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using MingUI.Com.Utils;

namespace Assets.Scripts.Com.MingUI {
    public class CPageBar : UIWidget {
        public UILabel lbl;
        public CButton UpBtn;
        public CButton DownBtn;
        public CButton btnMax;
        public CButton btnMin;
        public float _Min = 1;
        public float _Max = 99;
        public float Step = 1;
        public float stepTime = 1;
        public float DefaultValue = 1;
        private float _value = -1;


        private bool _isDownPress;
        private bool _isUpPress;
        private bool _isProceed;
        private float _pressTime;
        private Action onChangeFun;

        protected override void OnStart() {
            base.OnStart();
            if (Value == -1)
                Value = DefaultValue;
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
        }

        private void OnClickMaxBtn(GameObject go) {
            if ((int)Value == (int)_Max) {
                FuncUtil.AddTip("当前已经是最后一页");
                return;
            }
            Value = _Max;
        }

        private void OnClickMinBtn(GameObject go) {
            if ((int)Value == (int)_Min) {
                FuncUtil.AddTip("当前已经是第一页");
                return;
            }
            Value = _Min;
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
            if ((int)Value == (int)_Max) {
                FuncUtil.AddTip("当前已经是最后一页");
                return;
            }
            Value += Step;
            if (Value > _Max) {
                Value = _Max;
            }
        }

        private void OnClickDownBtn(GameObject go) {
            if ((int)Value == (int)_Min) {
                FuncUtil.AddTip("当前已经是第一页");
                return;
            }
            Value -= Step;
            if (Value < _Min) {
                Value = _Min;
            }
        }
         private float nextStepTime=0;
        protected override void OnUpdate() {
            base.OnUpdate();
            if (_isUpPress || _isDownPress) {
                if (Time.time - _pressTime > 0.1f && _isProceed != true) {
                    _isProceed = true;
                    nextStepTime =Time.time + stepTime;
                }
            }
            if (_isProceed) {
                if (Time.time <= nextStepTime) {

                } else {
                    nextStepTime += stepTime;
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
                lbl.text = _value.ToString() + "/" + _Max;
                if (isChange && onChangeFun != null) {
                    onChangeFun.DynamicInvoke();
                }
            }
            get { return _value; }
        }

        public float Max {
            set {
                _Max = value;
                Value = _value;
            }
            get {
                return _Max;
            }
        }

        public float Min {
            set {
                _Min = value;
                Value = _value;
            }
            get {
                return _Min;
            }
        }

    }
}
