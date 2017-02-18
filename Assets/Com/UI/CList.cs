using Assets.Scripts.Com.Utils;
using MingUI.Com.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Com.MingUI {
    public class CList : UISprite {
        public delegate void VoidFun(CItemRender selectItem);

        protected bool _horizon = false;
        public VoidFun OnItemSelect;
        public VoidFun OnItemDoubleClick;
        //public VoidFun OnItemOver;
        public Action<CItemRender, bool> OnItemOver;
        public UIPanel Content;
        public UISprite Overlay;
        public UISprite spSelected;
        //public CItemRender ItemRender;
        protected System.Type _itemRender = typeof(CDefaultItemRender);
        public CScrollBar Bar;
        public Transform Recycle;
        public int PaddingLeft = 4; //item与content的距离
        public int PaddingTop = 4;
        public int PaddingBottom = 4;
        protected List<CItemRender> _itemPool;
        protected List<CItemRender> _allItem;
        protected object _dataCondition; //额外条件
        protected List<object> _dataProvider;
        protected Vector2 maskOffset;
        protected Vector3 contentPos;
        protected int _itemWidth = 158;
        protected int _itemHeight = 30;
        public int TotalHeight;
        public int TotalWidth;
        public int ColNum = 1; //列数
        public int RowNum = 1; //行数
        protected bool _showBar = true;
        public bool barInContent = false;
        public int gapU;//每个Item之间的水平间距
        public int gapV;//每个Item之间的纵向间距
        public bool auotSel = true;
        public bool IsAddItemEvent = true;//item的事件是否添加
        protected bool isDefaultItemRender = true;
        public int preIndex;    //用于分页自动分配index
        public float stepOffset = 1f;//滚动条的偏移

        protected CItemRender selectedItemRender;

        public int defaultBarValue;

        public float maxRoll;   //滚动的最大值
        public float minRoll;   //滚动的最小值
        public float maxRollPixel = 70;

        protected override void OnStart() {
            base.OnStart();
            try {
                Overlay.width = _itemWidth;
                Overlay.transform.localPosition = new Vector3(PaddingLeft, Overlay.height, 0);
                resetSpSel();
                if (barInContent == false) {
                    Content.baseClipRegion = new Vector4((this.width - Bar.width) / 2, -this.height / 2, this.width - Bar.width, this.height);
                } else {
                    Content.baseClipRegion = new Vector4((this.width) / 2, -this.height / 2, this.width, this.height);
                }
                UIEventListener.Get(this.gameObject).onScroll = OnMouseScroll;
                if (isDefaultItemRender) {
                    itemRender = typeof(CDefaultItemRender);
                }
                Recycle.position = new Vector3(9999, 9999, 9999);
            } catch (Exception e) {
                FuncUtil.ShowError(e.Message);
            }
        }

        protected override void OnEnable() {
            base.OnEnable();
            UICameraUtil.AddGenericScroll(OnMouseScroll);
        }

        protected override void OnDisable() {
            base.OnDisable();
            UICameraUtil.RemoveGenericScroll(OnMouseScroll);
        }

        public System.Type itemRender {
            set {
                isDefaultItemRender = false;
                _itemRender = value;
                CItemRender item = (CItemRender)Activator.CreateInstance(_itemRender);
                UIWidget uiw = item.GetComponent<UIWidget>();
                if (uiw != null) {
                    if (_itemHeight == 30) {
                        itemHeight = uiw.height;
                    }
                    if (itemWidth == 158) {
                        itemWidth = uiw.width;
                    }

                    Overlay.SetDimensions(itemWidth, _itemHeight);
                    Overlay.transform.parent = Content.transform;
                    Overlay.pivot = UIWidget.Pivot.TopLeft;
                    if (_selectedIndex == -1) {
                        Overlay.transform.localPosition = new Vector3(PaddingLeft, _itemHeight, 0);
                    }
                    if (spSelected != null) {
                        spSelected.SetDimensions(itemWidth, _itemHeight);
                    }
                }
                DestroyImmediate(item.go);
            }
            get {
                return _itemRender;
            }
        }

        public int itemHeight {
            get { return _itemHeight; }
            set {
                _itemHeight = value;
                Overlay.SetDimensions(itemWidth, _itemHeight);
                Overlay.transform.parent = Content.transform;
                Overlay.pivot = UIWidget.Pivot.TopLeft;
                if (_selectedIndex == -1) {
                    Overlay.transform.localPosition = new Vector3(PaddingLeft, _itemHeight, 0);
                }
                if (spSelected != null) {
                    spSelected.SetDimensions(itemWidth, _itemHeight);
                }
            }
        }

        public int itemWidth {
            get { return _itemWidth; }
            set {
                _itemWidth = value;
                Overlay.SetDimensions(itemWidth, _itemHeight);
                Overlay.transform.parent = Content.transform;
                Overlay.pivot = UIWidget.Pivot.TopLeft;
                if (_selectedIndex == -1) {
                    Overlay.transform.localPosition = new Vector3(PaddingLeft, _itemHeight, 0);
                }
                if (spSelected != null) {
                    spSelected.SetDimensions(itemWidth, _itemHeight);
                }
            }
        }

        public void SetDataCondition<T>(T value) {
            _dataCondition = value;
        }

        protected object lastDataProvider;

        public virtual void SetDataProvider<T>(IEnumerable<T> value) {
            if (_itemRender == null) {
                Debug.LogError("set itemRender first!");
                return;
            }
            if (_itemPool == null) {
                _itemPool = new List<CItemRender>();
            }
            maskOffset = Content.clipOffset;
            contentPos = Content.transform.localPosition;
            Bar.OnChangeFun = OnBarChange;
            if (_allItem == null) {
                _allItem = new List<CItemRender>();
            }
            SetData<T>(value);
            //_dataProvider = (value ?? new List<T>()) as List<object>;

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
                    //item.SetActive(false);
                    item.alpha = 1;
                } else {
                    if (_itemPool.Count > 0) {
                        item = _itemPool[0];
                        _itemPool.RemoveAt(0);
                        if (item.IsActive() == false) item.SetActive(true);
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
            ResetItems();
            if (_selectedIndex == -1 && _dataProvider.Count > 0 && auotSel) {
                if (spSelected != null) {
                    spSelected.transform.localPosition = new Vector3(PaddingLeft, spSelected.height, 0);
                }
                lastDataProvider = value;
                SelectedIndex = 0;
            } else if (_dataProvider.Count == 0 || _selectedIndex == -1) {
                if (spSelected != null) {
                    spSelected.transform.localPosition = new Vector3(PaddingLeft, spSelected.height, 0);
                }
            }
            lastDataProvider = value;

            //Bar.value = Bar.value; //修复SetDatePro之后Item被设置为Null后出现的显示异常问题

        }

        protected virtual void ResetItems() {
            CItemRender item;
            for (int i = 0; i < _allItem.Count; i++) {
                item = _allItem[i];
                item.name = "Item" + i;
                CItemRender w = item;
                if (item.IsActive() == false) item.SetActive(true);
                w.SetAlpha(1);
                if (_dataCondition != null) {
                    w.SetCondition(_dataCondition);
                }
                w.index = i + preIndex;
                w.Data = _dataProvider[i];
                int offX = i / RowNum;
                int offY = i / ColNum;
                float itemX, itemY;
                if (_horizon) {
                    itemY = -(itemHeight + gapV) * (i % RowNum); //一列一列的顺序？
                    itemX = offX * (itemWidth + gapU);
                } else {
                    itemY = (offY * (_itemHeight * item.localScale.y + gapV) * -1);
                    itemX = (itemWidth + gapU) * (i % ColNum);
                }
                item.tran.localPosition = new Vector3(PaddingLeft + itemX, -PaddingTop + itemY, 0);

                //                UIEventListener listener = UIEventListener.Get(item.go);
                UIEventListener.Get(item.go);
                //listener.onHover = OnItemHover;
                if (IsAddItemEvent == true) {
                    EventUtil.AddHover(item.go, OnItemHover, item.go);
                    EventUtil.AddDoubleClick(item.go, OnDoubleClickItem, item);
                    EventUtil.AddClick(item.go, OnItemPress, item.go);
                } else {
                    EventUtil.RemoveHover(item.go, OnItemHover);
                    EventUtil.RemoveDoubleClick(item.go, OnDoubleClickItem);
                    EventUtil.RemoveClick(item.go, OnItemPress);
                }
            }
            if (_dataProvider == null) {
                TotalWidth = 0;
                TotalHeight = 0;
            } else {
                if (_horizon) {
                    var itemW = (_itemWidth + gapU) * Mathf.CeilToInt(_dataProvider.Count / (float)RowNum) - gapU;
                    TotalWidth = PaddingLeft * 2 + itemW;
                } else {
                    var itemH = (_itemHeight + gapV) * Mathf.CeilToInt(_dataProvider.Count / (float)ColNum) - gapV;
                    TotalHeight = PaddingTop + itemH + PaddingBottom;
                }
            }
            UpdateScrollBar();
        }

        public void AutoScroll() {
            Bar.AutoScroll();
        }

        public virtual void UpdateScrollBar() {
            if (ShowBar && TotalHeight > 0) {
                Bar.gameObject.SetActive(TotalHeight > height || TotalWidth > width);
                if (Bar.gameObject.activeSelf == true) {
                    if (_horizon) {
                        Bar.BarSize = (float)width / TotalWidth;
                        if (maxRollPixel != 0) Bar.maxRoll = maxRollPixel / TotalWidth;
                    } else {
                        Bar.BarSize = (float)this.height / TotalHeight;
                        if (maxRollPixel != 0) Bar.maxRoll = maxRollPixel / TotalHeight;
                    }
                    Overlay.width = _itemWidth;
                    if (spSelected != null) {
                        spSelected.width = this.width - Bar.width;
                    }
                } else {
                    Overlay.width = _itemWidth;
                    if (spSelected != null) {
                        spSelected.width = _itemWidth;
                    }
                    Bar.value = defaultBarValue;
                }
            } else {
                Bar.gameObject.SetActive(false);
            }
        }

        protected void SetData<T>(IEnumerable<T> value) {
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

        public List<object> dataProvider {
            get { return _dataProvider; }
        }

        public void RestScrollBar() {
            if (Bar != null) {
                Bar.value = 0;
            }
        }

        protected void OnMouseScroll(GameObject go, float delta) {
            if ((go == gameObject || UICamera.hoveredObject.transform.IsChildOf(Content.transform) == true) && Bar.gameObject.activeSelf) {
                float v = Bar.value;
                //这个偏移不能为0，为0，滚动条就挂了
                if (stepOffset <= 0) {
                    stepOffset = 1;
                }
                float roll = delta;
                if (maxRoll != 0) {
                    roll = Math.Min(maxRoll, Math.Abs(roll));
                    roll *= delta < 0 ? -1 : 1;
                } else if (maxRollPixel != 0) {
                    roll = Math.Min(maxRollPixel / TotalHeight, Math.Abs(roll));
                    roll *= delta < 0 ? -1 : 1;
                }
                if (minRoll != 0) {
                    roll = Math.Max(minRoll, Math.Abs(roll));
                    roll *= delta < 0 ? -1 : 1;
                }

                v += roll * -1f * stepOffset;
                if (v < 0) {
                    v = 0;
                } else if (v > 1) {
                    v = 1;
                }
                Bar.value = v;
            }
        }

        public void OnBarChange(GameObject go, float v) {
            if (_horizon) {
                float cx = Mathf.Lerp(0, TotalWidth - this.width, v);
                contentPos.x = -cx;
                maskOffset.x = cx;
            } else {
                if (TotalHeight < this.height) v = 0;
                float cy = Mathf.Lerp(0, TotalHeight - this.height, v);
                contentPos.y = cy;
                maskOffset.y = -cy;
            }
            Content.transform.localPosition = contentPos;
            Content.clipOffset = maskOffset;
        }

        //把index显示在第一行
        public virtual void ScrollToIndex(int index) {
            if (TotalHeight < height && TotalWidth < width) {
                return;
            }
            if (_horizon) {//这里如果是多行的就不对了,先不改
                float x = (_itemWidth + gapU) * index;
                if (x > TotalWidth - width) {
                    x = TotalWidth - width;
                }
                contentPos.x = -x;
                maskOffset.x = x;
                Content.transform.localPosition = contentPos;
                Content.clipOffset = maskOffset;
            } else {
                float y = (_itemHeight + gapV) * (index / ColNum);
                if (y > TotalHeight - this.height) {
                    y = TotalHeight - this.height;
                }
                //contentPos.y = y;
                //maskOffset.y = -y;
                Bar.value = y / (TotalHeight - this.height);
            }
            //Content.transform.localPosition = contentPos;
            //Content.clipOffset = maskOffset;
        }

        public int ScrollToNext() {
            if (contentPos.x == (this.width - TotalWidth) && contentPos.y == this.height - TotalHeight) return -1;
            if (TotalHeight < this.height && TotalWidth < this.width) {
                return -1;
            }
            if (_horizon) {
                int now = (int)Mathf.Ceil(Mathf.Abs((contentPos.x) / (_itemWidth + gapU)));
                float x = (_itemWidth + gapU) * (now + 1);
                if (x > TotalWidth - this.width) {
                    x = TotalWidth - this.width;
                }
                Bar.value = x / (TotalWidth - this.width);
            } else {
                int now = int.Parse(Mathf.Ceil((contentPos.y) / (_itemHeight + gapV)).ToString());
                float y = (_itemHeight + gapV) * ((now + 1) / ColNum);
                if (y > TotalHeight - this.height) {
                    y = TotalHeight - this.height;
                }
                Bar.value = y / (TotalHeight - this.height);
            }
            return 0;
        }

        public int ScrollToBefore() {
            if (contentPos.x == 0 && contentPos.y == 0) return -1;
            if (TotalHeight < this.height && TotalWidth < this.width) {
                return -1;
            }
            if (_horizon) {
                int now = (int)Mathf.Ceil(Mathf.Abs((contentPos.x) / (_itemWidth + gapU)));
                float x = (_itemWidth + gapU) * (now - 1);
                if (x < 0) {
                    x = 0;
                }
                Bar.value = x / (TotalWidth - this.width);
            } else {
                int now = int.Parse(Mathf.Ceil((contentPos.y) / (_itemHeight + gapV)).ToString());
                float y = (_itemHeight + gapV) * ((now - 1) / ColNum);
                if (y < 0) {
                    y = 0;
                }
                Bar.value = y / (TotalHeight - this.height);
            }
            return 0;
        }

        public void ScrollToLast() {
            if (Bar.gameObject.activeSelf == true) {
                Bar.value = 1;
            } else {
                RestScrollBar();
            }
        }

        protected void OnItemHover(object go, bool isOver) {
            if (isOver) {
                Overlay.transform.localPosition = (go as GameObject).transform.localPosition;
            } else {
                Overlay.transform.localPosition = new Vector3(PaddingLeft, Overlay.height + 100, 0);
            }
            if (OnItemOver != null) {
                OnItemOver((go as GameObject).GetDisplayObj() as CItemRender, isOver);
            }
        }

        protected virtual void OnItemPress(object go) {
            OnItemPress((GameObject)go, true);
        }

        protected virtual void OnItemPress(GameObject go, bool needEvent) {
            CItemRender c = go.GetDisplayObj() as CItemRender;
            if (selectedItemRender != null)
                selectedItemRender.SetSelected(false);
            selectedItemRender = c;
            c.SetSelected(true);
            SelectedItem = c.Data;
            if (spSelected != null) {
                spSelected.transform.localPosition = c.tran.localPosition;
            }
            if (OnItemSelect != null && needEvent) {
                OnItemSelect(c);
            }
        }

        private void OnDoubleClickItem(object arg) {
            CItemRender c = arg as CItemRender;
            if (OnItemDoubleClick != null) {
                OnItemDoubleClick(c);
            }
        }

        protected object _selectedItem;
        //设置SelectedItem不导致引发SelectedIndex事件，避免死循环
        public object SelectedItem {
            set {
                _selectedIndex = _dataProvider.IndexOf(value);
                if (_selectedIndex == -1)
                    return;
                _selectedItem = _dataProvider[_selectedIndex];
                SetSpSelectedPos(_allItem[_selectedIndex].localPosition);
            }
            get { return _selectedItem; }
        }

        protected int _selectedIndex = -1;

        public virtual int SelectedIndex {
            set {
                if (_dataProvider != null && _dataProvider.Count > value) {
                    _selectedIndex = value;
                    _selectedItem = _dataProvider[_selectedIndex];
                    SetSpSelectedPos(_allItem[_selectedIndex].localPosition);
                    ScrollToIndex(_selectedIndex);
                    OnItemPress(_allItem[_selectedIndex].go);
                }
            }
            get { return _selectedIndex; }
        }

        public virtual int SelectIndexWithOutEvent {
            set {
                _selectedIndex = value;
                if (_dataProvider == null || _dataProvider.Count <= _selectedIndex || _selectedIndex == -1)
                    return;
                _selectedItem = _dataProvider[_selectedIndex];
                SetSpSelectedPos(_allItem[_selectedIndex].localPosition);
                ScrollToIndex(_selectedIndex);
                OnItemPress(_allItem[_selectedIndex].go, false);
            }
        }

        public virtual int SelectedIndexWithOutScroll {
            set {
                _selectedIndex = value;
                _selectedItem = _dataProvider[_selectedIndex];
                SetSpSelectedPos(_allItem[_selectedIndex].localPosition);
                OnItemPress(_allItem[_selectedIndex].go);
            }
            get { return _selectedIndex; }
        }

        private void SetSpSelectedPos(Vector3 v) {
            if (spSelected != null) {
                spSelected.transform.localPosition = v;
            }
        }

        public bool ShowBar {
            set { _showBar = value; }
            get { return _showBar; }
        }

        public int ListHeight {
            set {
                this.height = value;
                if (barInContent == false) {
                    Content.baseClipRegion = new Vector4((this.width - Bar.width) / 2, -this.height / 2, this.width - Bar.width, this.height);
                } else {
                    Content.baseClipRegion = new Vector4((this.width) / 2, -this.height / 2, this.width, this.height);
                }
                UpdateScrollBar();
                Bar.Track.ResetAnchors();
                (Bar.Track as UIRect).Update();
            }
            get { return this.height; }
        }

        public List<CItemRender> allItem {
            get { return _allItem; }
        }

        public bool horizon {
            set {
                _horizon = value;
                resetSpSel();
            }
            get { return _horizon; }
        }

        private void resetSpSel() {
            if (spSelected != null) {
                if (_horizon) {
                    spSelected.width = _itemWidth;
                } else {
                    if (Bar.gameObject.activeInHierarchy)
                        spSelected.width = this.width - Bar.width;
                    else
                        spSelected.width = _itemWidth;
                }
            }
        }

        public void ResetHeight() {
            ListHeight = TotalHeight;
        }
    }
}