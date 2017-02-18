using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Assets.Scripts.Com.Utils;
using System;

namespace Assets.Scripts.Com.MingUI {
    public class CMissionList : CList {
        public int maxNum;
        public bool keepCurPos;
        public float delItemHeight;
        protected override void ResetItems() {
            CItemRender item;
            bool isSameHeight = true;
            float realTotalHeight = 0;
            for (int i = 0; i < _allItem.Count; i++) {
                item = _allItem[i];
                item.name = "Item" + i;
                CItemRender w = item;
                if (_dataCondition != null) {
                    w.SetCondition(_dataCondition);
                }
                w.Data = _dataProvider[i];
                //默认为itemHeight（等高情况）
                //不等高情况，请自行设置CItemRender的height属性
                float realItemHeight = 0;
                if (w.height == 0) {
                    realItemHeight = _itemHeight;
                } else {
                    realItemHeight = w.height;
                    isSameHeight = false;
                }
                int offX = i / RowNum;
                int offY = i / ColNum;
                float itemX, itemY;
                if (horizon) {
                    itemY = -(itemHeight + 5) * (i % RowNum);  //一列一列的顺序？
                    itemX = offX * itemWidth;
                } else {
                    if (isSameHeight)
                        itemY = (offY * _itemHeight * -1);
                    else
                        itemY = (realTotalHeight * -1);
                    itemX = (itemWidth + 5) * (i % ColNum);
                }
                item.tran.localPosition = new Vector3(PaddingLeft + itemX, -PaddingTop + itemY, 0);

                realTotalHeight += realItemHeight;

                UIEventListener listener = UIEventListener.Get(item.go);
                //listener.onHover = OnItemHover;
                EventUtil.AddHover(item.go, OnItemHover, item.go);
                listener.onClick = OnItemPress;
            }
            if (_dataProvider == null) {
                TotalWidth = 0;
                TotalHeight = 0;
            } else {
                if (horizon) {
                    var itemW = _itemWidth * _dataProvider.Count / (float)RowNum;
                    TotalWidth = PaddingLeft * 2 + Mathf.CeilToInt(itemW);
                } else {
                    float itemH = 0;
                    if (isSameHeight)
                        itemH = _itemHeight * _dataProvider.Count / float.Parse(ColNum.ToString());
                    else
                        itemH = realTotalHeight;
                    TotalHeight = PaddingTop + Mathf.CeilToInt(itemH) + PaddingBottom;
                }
            }
            if (keepCurPos == false) {
                UpdateScrollBar(); 
            } else {
                if (ShowBar) {
                    bool needShowBar = TotalHeight > height || TotalWidth > width;
                    Bar.gameObject.SetActive(needShowBar);
                } else {
                    Bar.gameObject.SetActive(false);
                }
            }
        }

        public override void UpdateScrollBar() {
            if (ShowBar) {
                bool needShowBar = TotalHeight > height || TotalWidth > width;
                if (needShowBar) {
                    if (horizon) {
                        Bar.BarSize = (float)width / TotalWidth;
                    } else {
                        Bar.BarSize = (float)this.height / TotalHeight;
                    }
                    Overlay.width = _itemWidth;
                    if (spSelected != null) {
                        spSelected.width = this.width - Bar.width;
                    }
                } else {
                    Overlay.width = _itemWidth;
                    if (spSelected != null) {
                        spSelected.width = this.width;
                    }
                    Bar.value = defaultBarValue;
                }
                Bar.gameObject.SetActive(needShowBar);
            } else {
                Bar.gameObject.SetActive(false);
            }
        }

        public override void ScrollToIndex(int index) {
            if (TotalHeight < height && TotalWidth < width) {
                return;
            }
            float y = 0;
            for (int i = 0; i < index; i++) {
                y += (_allItem[i].height + gapV);
            }
            if (y > TotalHeight - this.height) {
                y = TotalHeight - this.height;
            }
            //contentPos.y = y;
            //maskOffset.y = -y;
            Bar.value = y / (TotalHeight - this.height);
            //Content.transform.localPosition = contentPos;
            //Content.clipOffset = maskOffset;
        }

        public void AddData<T>(T vo) {
            delItemHeight = 0;
            _dataProvider.Add(vo);
            int i = 0;
            if (maxNum != 0) {
                while (_dataProvider.Count > maxNum) {
                    _dataProvider.RemoveAt(0);
                    delItemHeight += allItem[i].height;
                    i++;
                }
            }

            bool isEnough = _allItem.Count >= _dataProvider.Count;
            int n = Mathf.Abs(_allItem.Count - _dataProvider.Count);
            CItemRender item;
            while (n > 0) {
                if (isEnough == true) {
                    item = _allItem[_allItem.Count - 1];
                    _itemPool.Add(item);
                    _allItem.RemoveAt(_allItem.Count - 1);
                    item.SetParent(Recycle);
                    item.tran.localPosition = Vector3.zero;
                    item.SetActive(false);
                } else {
                    if (_itemPool.Count > 0) {
                        item = _itemPool[0];
                        _itemPool.RemoveAt(0);
                        item.SetActive(true);
                    } else {
                        item = (CItemRender)Activator.CreateInstance(_itemRender);
                        if (_itemRender == typeof(CDefaultItemRender)) {
                            item.width = itemWidth;
                            item.height = itemHeight;
                        }
                        UIEventListener.Get(item.go).onScroll = OnMouseScroll;
                    }
                    item.SetParent(Content.transform);
                    _allItem.Add(item);
                }
                n--;
            }
            keepCurPos = true;
            ResetItems();
            keepCurPos = false;
            UpdatePos();
        }

        private void UpdatePos() {
            if (ShowBar) {
                bool needShowBar = TotalHeight > height || TotalWidth > width;
                if (needShowBar) {
                    float oldSize = Bar.BarSize;
                    if (horizon) {
                        Bar.BarSize = (float)width / TotalWidth;
                    } else {
                        Bar.BarSize = (float)this.height / TotalHeight;
                    }
                    if (Bar.value != 1) {
                        if (delItemHeight == 0) {
                            Bar.value = contentPos.y / (TotalHeight - this.height);
                        } else {
                            float pos = (contentPos.y - delItemHeight);
                            Bar.value = Math.Max(0, pos / (TotalHeight - this.height));
                        }
                    }
                    Overlay.width = _itemWidth;
                    if (spSelected != null) {
                        spSelected.width = this.width - Bar.width;
                    }
                } else {
                    Overlay.width = _itemWidth;
                    if (spSelected != null) {
                        spSelected.width = this.width;
                    }
                    Bar.value = defaultBarValue;
                }
                Bar.gameObject.SetActive(needShowBar);
            } else {
                Bar.gameObject.SetActive(false);
            }
        }
        #region UNUSED

        //public delegate void VoidFun(CItemRender selectItem);

        //public VoidFun OnItemSelect;
        //public UIPanel Content;
        //public UISprite Overlay;
        //public CItemRender ItemRender;
        //public CScrollBar Bar;
        //public Transform Recycle;
        //public int PaddingLeft = 4; //item与content的距离
        //public int PaddingTop = 4;
        //private List<GameObject> _itemPool;
        //private List<GameObject> _allItem;
        //private List<object> _dataProvider;
        //private Vector2 maskOffset;
        //private Vector3 contentPos;
        //private int _itemHeight;
        //public int TotalHeight;
        //private bool _showBar = true;

        //protected override void OnStart() {
        //    base.OnStart();
        //    Overlay.width = this.width - Bar.width;
        //    Overlay.transform.localPosition = new Vector3(PaddingLeft, Overlay.height, 0);
        //    Content.baseClipRegion = new Vector4((this.width - Bar.width) / 2, -this.height / 2, this.width - Bar.width, this.height);
        //    UIEventListener.Get(this.gameObject).onScroll = OnMouseScroll;
        //}

        //protected override void OnEnable() {
        //    base.OnEnable();
        //    UICameraUtil.AddGenericScroll(OnMouseScroll);
        //}

        //protected override void OnDisable() {
        //    base.OnDisable();
        //    UICameraUtil.RemoveGenericScroll(OnMouseScroll);
        //}
        //public void SetDataProvider<T>(List<T> value) {
        //    if (ItemRender == null) {
        //        GameLog.LogError("set itemRender first!");
        //        return;
        //    }
        //    if (_itemPool == null) {
        //        _itemPool = new List<GameObject>();
        //    }
        //    maskOffset = Content.clipOffset;
        //    contentPos = Content.transform.localPosition;
        //    Bar.OnChangeFun = OnBarChange;
        //    if (_allItem == null) {
        //        _allItem = new List<GameObject>();
        //    }
        //    SetData<T>(value);
        //    //_dataProvider = (value ?? new List<T>()) as List<object>;
        //    UIWidget uiw = ItemRender.GetComponent<UIWidget>();
        //    int itemWidth = uiw.width;
        //    _itemHeight = uiw.height;
        //    Overlay.SetDimensions(itemWidth, _itemHeight);
        //    Overlay.transform.parent = Content.transform;
        //    Overlay.pivot = UIWidget.Pivot.TopLeft;
        //    if (_selectedIndex == -1) {
        //        Overlay.transform.localPosition = new Vector3(PaddingLeft, _itemHeight, 0);
        //    }
        //    bool isEnough = _allItem.Count >= _dataProvider.Count;
        //    int n = Mathf.Abs(_allItem.Count - _dataProvider.Count);
        //    ItemRender.gameObject.SetActive(true);
        //    GameObject item;
        //    while (n > 0) {
        //        if (isEnough == true) {
        //            item = _allItem[_allItem.Count - 1];
        //            _itemPool.Add(item);
        //            _allItem.RemoveAt(_allItem.Count - 1);
        //            item.transform.parent = Recycle;
        //            item.transform.localScale = Vector3.one;
        //            item.transform.localPosition = Vector3.zero;
        //            item.SetActive(false);
        //        } else {
        //            if (_itemPool.Count > 0) {
        //                item = _itemPool[0];
        //                _itemPool.RemoveAt(0);
        //                item.SetActive(true);
        //            } else {
        //                item = Instantiate(ItemRender.gameObject) as GameObject;
        //            }
        //            item.transform.parent = Content.transform;
        //            item.transform.localScale = Vector3.one;
        //            _allItem.Add(item);
        //        }
        //        n--;
        //    }
        //    ItemRender.gameObject.SetActive(false);
        //    TotalHeight = PaddingTop;
        //    for (int i = 0; i < _allItem.Count; i++) {
        //        item = _allItem[i];
        //        item.name = "Item" + i;
        //        CItemRender w = item.GetComponent<CItemRender>();
        //        w.Data = _dataProvider[i];
        //        uiw = ItemRender.GetComponent<UIWidget>();
        //        item.transform.localPosition = new Vector3(PaddingLeft, -TotalHeight - _itemHeight, 0);
        //        UIEventListener listener = UIEventListener.Get(item);
        //        listener.onHover = OnItemHover;
        //        listener.onPress = OnItemPress;
        //        _itemHeight = uiw.height;
        //        TotalHeight += _itemHeight;
        //    }
        //    if (_dataProvider == null) {
        //        TotalHeight = 0;
        //    } else {
        //        TotalHeight += TotalHeight;
        //    }

        //    if (ShowBar) {
        //        Bar.gameObject.SetActive(TotalHeight > this.height);
        //        if (Bar.gameObject.activeSelf == true) {
        //            Bar.BarSize = (float)this.height / TotalHeight;
        //        }
        //    } else {
        //        Bar.gameObject.SetActive(false);
        //    }
        //}

        //private void SetData<T>(List<T> value) {
        //    _dataProvider = _dataProvider ?? new List<object>();
        //    while (_dataProvider.Count > 0) {
        //        _dataProvider.RemoveAt(0);
        //    }
        //    if (value == null) {
        //        return;
        //    }
        //    for (int i = 0, len = value.Count; i < len; i++) {
        //        _dataProvider.Add(value[i]);
        //    }
        //}

        //public List<object> dataProvider {
        //    get {
        //        return _dataProvider;
        //    }
        //}

        //private void OnMouseScroll(GameObject go, float delta) {
        //    if ((go == gameObject || UICamera.hoveredObject.transform.IsChildOf(Content.transform) == true) && Bar.gameObject.activeSelf) {
        //        float v = Bar.value;
        //        v += delta * -0.8f;
        //        if (v < 0) {
        //            v = 0;
        //        } else if (v > 1) {
        //            v = 1;
        //        }
        //        Bar.value = v;
        //    }
        //}

        //public void OnBarChange(GameObject go, float v) {
        //    float cy = Mathf.Lerp(0, TotalHeight - this.height, v);
        //    contentPos.y = cy;
        //    Content.transform.localPosition = contentPos;
        //    maskOffset.y = -cy;
        //    Content.clipOffset = maskOffset;
        //}

        //private void OnItemHover(GameObject go, bool isOver) {
        //    if (isOver) {
        //        Overlay.transform.localPosition = go.transform.localPosition;
        //    }
        //}

        //private void OnItemPress(GameObject go, bool isPressed) {
        //    if (isPressed) {
        //        CItemRender c = go.GetComponent<CItemRender>();
        //        SelectedItem = c.Data;
        //        if (OnItemSelect != null) {
        //            OnItemSelect(c);
        //        }
        //    }
        //}

        //private object _selectedItem;
        ////设置SelectedItem不导致引发SelectedIndex事件，避免死循环
        //public object SelectedItem {
        //    set {
        //        _selectedIndex = _dataProvider.IndexOf(value);
        //        _selectedItem = _dataProvider[_selectedIndex];
        //        Overlay.transform.localPosition = _allItem[_selectedIndex].transform.localPosition;
        //    }
        //    get { return _selectedItem; }
        //}

        //private int _selectedIndex = -1;

        //public int SelectedIndex {
        //    set {
        //        _selectedIndex = value;
        //        _selectedItem = _dataProvider[_selectedIndex];
        //        Overlay.transform.localPosition = _allItem[_selectedIndex].transform.localPosition;
        //        OnItemPress(_allItem[_selectedIndex], true);
        //    }
        //    get { return _selectedIndex; }
        //}

        //public int SelectedIndexWithOutScroll {
        //    set {
        //        _selectedIndex = value;
        //        _selectedItem = _dataProvider[_selectedIndex];
        //        Overlay.transform.localPosition = _allItem[_selectedIndex].transform.localPosition;
        //        OnItemPress(_allItem[_selectedIndex], true);
        //    }
        //    get { return _selectedIndex; }
        //}

        //public int ItemHeight {
        //    get {
        //        if (ItemRender != null && _itemHeight == 0) {
        //            UIWidget uiw = ItemRender.GetComponent<UIWidget>();
        //            _itemHeight = uiw.width;
        //        }
        //        return _itemHeight;
        //    }
        //}

        //public bool ShowBar {
        //    set {
        //        _showBar = value;
        //    }
        //    get {
        //        return _showBar;
        //    }
        //}

        //public int ListHeight {
        //    set {
        //        this.height = value;
        //        Content.baseClipRegion = new Vector4((this.width - Bar.width) / 2, -this.height / 2, this.width - Bar.width, this.height);
        //    }
        //    get { return this.height; }
        //}

        //public List<GameObject> allItem {
        //    get {
        //        return _allItem;
        //    }
        //}

        #endregion
    }
}
