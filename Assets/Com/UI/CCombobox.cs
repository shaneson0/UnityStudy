using Assets.Scripts.Com.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Com.MingUI {
    public class CCombobox : UIWidget {
        public delegate void VoidFun(int index);
        public UISprite Bg;
        public CButton Btn;
        public CList List;
        public NGUIText.Alignment alignment = NGUIText.Alignment.Center;
        public int size = 12;
        public UILabel.Overflow overflow = UILabel.Overflow.ResizeHeight;
        public bool isUpDirection = false;
        [SerializeField]
        private int _MaxListHeight;
        public VoidFun OnChange;
        public bool needChange = true;
        private bool useDefaultItem=true;
        public static System.Type defaultItem;
        public Action clickAction;
        public bool autoSelect { set { List.auotSel = value; } }

        private string spNormal;
        private string spOver;
        private CButton spArrow;
        protected override void OnStart() {
            base.OnStart();
            if (Btn != null) {
                Btn.AddClick(OnClickBtn);
                spNormal = Btn.normalSprite;
                spOver = Btn.hoverSprite;
            }
            List.barInContent = true;
            UISprite btnSprite = Btn.GetComponent<UISprite>();
            if (isUpDirection) {
                List.transform.localPosition = new Vector3(List.transform.localPosition.x, List.ListHeight + 2, 0);
            }
            var objArrow = DisplayUtil.GetChildByName(Btn.transform, "spArrowDown");
            if (objArrow != null) {
                spArrow = objArrow.GetComponent<CButton>();
            }
            //else
            //{
            //    List.transform.localPosition = Btn.transform.localPosition + Vector3.down * btnSprite.height;
            //}
            //List.gameObject.SetActive(false);
            ActivieList(false);
        }

        public System.Type itemRender {
            set {
                List.itemRender = value;
                useDefaultItem = false;
            }
        }

        private List<object> _dataProvider;

        public List<object> DataProvider {
            set {
                _dataProvider = value;
                if (useDefaultItem && defaultItem != null) {
                    List.itemRender = defaultItem;
                    useDefaultItem = false;
                }
                if (_dataProvider == null) return;
                int count = _dataProvider.Count;
                if (ItemHeight != 0) {
                    int trueListHeight = count * ItemHeight + List.PaddingTop + List.PaddingBottom;
                    if (_MaxListHeight != 0) {
                        trueListHeight = Math.Min(trueListHeight, _MaxListHeight);
                    }
                    List.ListHeight = trueListHeight;
                    if (isUpDirection) {
                        List.transform.localPosition = new Vector3(List.transform.localPosition.x, trueListHeight + 2, 0);
                    }
                }
                List.SetDataCondition(new List<object> { overflow, alignment, size });
                List.OnItemSelect = OnItemSelect;
                List.SetDataProvider<object>(_dataProvider);
                //if (UICamera.genericEventHandler != null) {
                //    UIEventListener.Get(UICamera.genericEventHandler).onPress += OnRelease;
                //}
                UICameraUtil.AddGenericPress(OnRelease);
                if (Bg != null)
                    Bg.ResetAndUpdateAnchors();
            }
            get { return _dataProvider; }
        }

        public void SetDataProvider<T>(IEnumerable<T> list) {
            SetData<T>(list);
            if (useDefaultItem && defaultItem != null) {
                List.itemRender = defaultItem;
                useDefaultItem = false;
            }
            int count = _dataProvider.Count;
            if (ItemHeight != 0) {
                int trueListHeight = count * ItemHeight + List.PaddingTop + List.PaddingBottom;
                if (_MaxListHeight != 0) {
                    trueListHeight = Math.Min(trueListHeight, _MaxListHeight);
                }
                List.ListHeight = trueListHeight;
                if (isUpDirection) {
                    List.transform.localPosition = new Vector3(List.transform.localPosition.x, trueListHeight + 2, 0);
                }
            } else if (_MaxListHeight != 0) {
                List.ListHeight = _MaxListHeight;
            }
            List.OnItemSelect = OnItemSelect;
            List.SetDataProvider<T>(list);
            UICameraUtil.AddGenericPress(OnRelease);
            //if (UICamera.genericEventHandler != null) {
            //    UIEventListener.Get(UICamera.genericEventHandler).onPress += OnRelease;
            //}
            if (Bg != null)
                Bg.ResetAndUpdateAnchors();
        }

        private void SetData<T>(IEnumerable<T> value) {
            _dataProvider = _dataProvider ?? new List<object>();
            while (_dataProvider.Count > 0) {
                _dataProvider.RemoveAt(0);
            }
            if (value == null) {
                return;
            }
            foreach (var t in value) {
                _dataProvider.Add(t);
            }
        }

        public int SelectedIndex {
            set {
                if (needChange) {
                    List.SelectedIndex = value;
                }
                if (OnChange != null) {
                    OnChange(value);
                }
            }
            get { return List.SelectedIndex; }
        }

        public object SelectedItem {
            set {
                List.SelectedItem = value;
                Btn.Text = value + "";
            }
            get { return List.SelectedItem; }
        }

        public int ItemHeight {
            set {
                List.itemHeight = value;
            }
            get { return List.itemHeight; }
        }

        public int ItemWidh {
            set {
                List.itemWidth = value;
            }
            get { return List.itemWidth; }
        }
        [SerializeField]
        public int MaxListHeight {
            set {
                _MaxListHeight = value;
                DataProvider = _dataProvider;
            }
            get { return _MaxListHeight; }
        }

        private void OnItemSelect(object item) {
            if (OnChange != null) {
                OnChange(List.SelectedIndex);
            }
            if (needChange) {
                var render = item as CItemRender;
                Btn.Text = render != null ? render.Data.ToString() : "";
            }
            ActivieList(false);
        }

        private void OnRelease(GameObject go, bool isPress) {
            if (isPress == false) {
                UICamera.MouseOrTouch t = UICamera.currentTouch;
                if (t != null) {
                    bool isChild = CheckIsChild(List.gameObject.transform, t.pressed.transform);
                    if (isChild == false && t.pressed.transform != Btn.transform) {
                        //List.gameObject.SetActive(false);
                        ActivieList(false);
                    }
                }
            }
        }

        private void OnClickBtn(GameObject go) {
            if (List.gameObject.activeSelf == true) {
                //List.gameObject.SetActive(false);
                ActivieList(false);
                return;
            }
            ActivieList(true);
            if (clickAction != null) {
                clickAction.Invoke();
            }
            //List.gameObject.SetActive(true);
        }

        private void OnSelectBtn(GameObject go, bool isSelected) {
            if (isSelected == false) {
                //if (List.gameObject.activeSelf == true) {
                //    List.gameObject.SetActive(false);
                //}
                ActivieList(false);
            }
        }

        public void ActivieList(bool activie) {
            if (List.gameObject.activeSelf != activie) {
                List.gameObject.SetActive(activie);
                if (Bg != null)
                    Bg.gameObject.SetActive(activie);
            }
            if (activie) {
                Btn.normalSprite = spOver;
                if (Bg != null) {
                    Bg.SetAnchor(this.List.Content.gameObject, 2, 2, -2, -2);
                }
            } else {
                Btn.normalSprite = spNormal;
            }
        }

        public bool CheckIsChild(Transform big, Transform child) {
            if (child.parent == big) {
                return true;
            }
            if (child.parent != null) {
                return CheckIsChild(big, child.parent);
            }
            return false;
        }

        public bool IsEnabled {
            set {
                Btn.isEnabled = value;
                if (spArrow != null) {
                    spArrow.isEnabled = value;
                }
            }
        }
    }
}