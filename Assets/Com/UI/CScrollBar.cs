using System;
using UnityEngine;

namespace Assets.Scripts.Com.MingUI {
    public class CScrollBar : UIWidget {
        public UISprite Track;
        public UISprite Thumb;
        public CButton UpBtn;
        public CButton DownBtn;
        public float minRoll;
        public float maxRoll;
        public UIEventListener.FloatDelegate OnChangeFun;
        public float step = 0.08f;

        private bool isDownPress;
        private bool isProceed;
        private bool isUpPress;
        private float pressTime;
        private float _value;
        private bool hasStart=false;
        private bool autoScroll = false;

        public void Init() {
            OnStart();
        }
        protected override void OnStart() {
            UIEventListener.Get(Track.gameObject).onPress = OnPressTrack;
            UIEventListener.Get(Thumb.gameObject).onDrag = OnDragThumb;
            UIEventListener.Get(Track.gameObject).onScroll = OnMouseScroll;
            UIEventListener.Get(Thumb.gameObject).onScroll = OnMouseScroll;
            if (hasStart == false) {
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
                hasStart = true;
            }
            //Thumb.transform.localPosition = Track.transform.localPosition + Vector3.right;
            if (value == 0) {
                Thumb.transform.localPosition = Track.transform.localPosition;//+ Vector3.right;
            }
        }

        private void OnMouseScroll(GameObject go, float delta) {
            float v = value;
            float roll = delta;
            if (maxRoll != 0) {
                roll = Math.Min(maxRoll, Math.Abs(roll));
                roll *= delta < 0 ? -1 : 1;
            }
            if (minRoll != 0) {
                roll = Math.Max(minRoll, Math.Abs(roll));
                roll *= delta < 0 ? -1 : 1;
            }
            v += roll * -1;
            if (v < 0) {
                v = 0;
            } else if (v > 1) {
                v = 1;
            }
            value = v;

        }

        private void OnPressTrack(GameObject go, bool press) {
            autoScroll = false;
            if (BarSize >= 1) {
                return;
            }
            autoScroll = false;
            if (press == true) {
                Transform trans = Track.transform;
                Plane plane = new Plane(trans.rotation * Vector3.back, trans.position);

                float dist;
                Ray ray = UICamera.currentCamera.ScreenPointToRay(UICamera.lastTouchPosition);
                if (plane.Raycast(ray, out dist) == true) {
                    Vector2 p = trans.InverseTransformPoint(ray.GetPoint(dist));
                    value = -p.y / Track.height;
                }
            }
        }

        private void OnDragThumb(GameObject go, Vector2 d) {
            Profiler.BeginSample("OnDragThumb");
            Vector3 pos = Thumb.transform.localPosition;
            pos.y = pos.y + d.y;
            if (pos.y > Track.transform.localPosition.y) {
                pos.y = Track.transform.localPosition.y;
            } else if (pos.y < Thumb.height - Track.height + Track.transform.localPosition.y) {
                pos.y = Thumb.height - Track.height + Track.transform.localPosition.y;
            }
            Thumb.transform.localPosition = pos;
            value = -(pos.y - Track.transform.localPosition.y) / (Track.height - Thumb.height);
            Profiler.EndSample();
        }

        public float BarSize {
            set {
                if (value >= 1) {
                    Thumb.gameObject.SetActive(false);
                    Thumb.height = Track.height;
                } else {
                    Thumb.gameObject.SetActive(true);
                    Thumb.height = (Int32)(Track.height * value);
                    BoxCollider box = Thumb.gameObject.GetComponent<BoxCollider>();
                    if (box != null) NGUITools.UpdateWidgetCollider(box, true);
                    if (_value == 1 || (Thumb.height - Thumb.transform.localPosition.y) > Track.height) {
                        this.value = _value;
                    }
                }
            }
            get { return Thumb.height * 1.0f / Track.height; }
        }

        public void AutoScroll() {
            autoScroll = BarSize < 1;
        }

        public float value {
            set {
                if (value >= 0 && value <= 1) {
                    _value = value;
                    Vector3 pos = Thumb.transform.localPosition;
                    pos.y = Track.transform.localPosition.y - (Track.height - Thumb.height) * value;
                    Thumb.transform.localPosition = pos;
                    OnValueChange();
                }
            }
            get { return _value; }
        }

        private void OnUpBtnDown(GameObject go) {
            isUpPress = true;
            pressTime = Time.time;
        }

        private void OnDownBtnDown(GameObject go) {
            isDownPress = true;
            pressTime = Time.time;
        }

        private void OnBtnMouseUp(GameObject go) {
            isUpPress = false;
            isDownPress = false;
            isProceed = false;
            pressTime = 0;
        }

        private void OnClickUpBtn(GameObject go) {
            _value -= step;
            if (_value < 0) {
                _value = 0;
            }
            value = _value;
        }

        private void OnClickDownBtn(GameObject go) {
            _value += step;
            if (_value > 1) {
                _value = 1;
            }
            value = _value;
        }


        private void OnValueChange() {
            if (OnChangeFun != null) {
                if (value > .99f) {
                    iTween.Stop(Thumb.gameObject);
                }
                OnChangeFun(gameObject, value);
            }
        }

        protected override void OnUpdate() {
            base.OnUpdate();
            if (isUpPress || isDownPress) {
                if (Time.time - pressTime > 0.1f) {
                    isProceed = true;
                }
            }
            if (isProceed) {
                if (isUpPress) {
                    _value -= step;
                    if (_value < 0) {
                        _value = 0;
                    }
                } else if (isDownPress) {
                    _value += step;
                    if (_value > 1) {
                        _value = 1;
                    }
                }
                value = _value;
            }
            if (autoScroll) {
                if (value > .98f) {
                    value = 1;
                    autoScroll = false;
                }
                value += .03f;
            }
        }
    }
}