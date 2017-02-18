using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Assets.Scripts.Com.Utils;
using System;
using MingUI.Com.Manager;

namespace Assets.Scripts.Com.MingUI {
    public class CTileList : UISprite {
        public delegate void VoidFun(CItemRender selectItem);

        public VoidFun OnItemSelect;
        public UIPanel Content;
        public UISprite Overlay;
        public UISprite spSelected;
        //public CItemRender ItemRender;
        private System.Type _itemRender = typeof(CDefaultItemRender);
        public CScrollBar Bar;
        public Transform Recycle;
        public int PaddingLeft = 4; //item与content的距离
        public int PaddingTop = 4;
        private List<CItemRender> _itemPool;
        private List<CItemRender> _allItem;
        private List<int> _allItemIndex;
        private List<object> _dataProvider;
        private Vector2 maskOffset;
        private Vector3 contentPos;
        private int _itemWidth;
        private int _itemHeight = 30;
        public int TotalHeight;
        public int ColNum = 1; //列数
        private bool _showBar = true;
        private bool _isAutoSelect = true;

        private int firstPoint;
        private int lastPoint;

        private bool isDefaultItemRender = true;

        public float maxRoll;   //滚动的最大值
        public float minRoll;   //滚动的最小值
        public float maxRollPixel = 70;
        protected override void OnStart() {
            base.OnStart();
            Overlay.width = this.width - Bar.width;
            Overlay.transform.localPosition = new Vector3(PaddingLeft, Overlay.height, 0);
            if (spSelected != null) {
                spSelected.width = this.width - Bar.width;
            }
            Content.baseClipRegion = new Vector4((this.width - Bar.width) / 2, -this.height / 2, this.width - Bar.width, this.height);
            UIEventListener.Get(this.gameObject).onScroll = OnMouseScroll;
            if (isDefaultItemRender) {
                itemRender = typeof(CDefaultItemRender);
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
                //更换ItemRender需清空_allItem和_itemPool
                lastDataProvider = null;
                if (dataProvider != null) {
                    dataProvider.Clear();
                }
                RestScrollBar();
                if (value != _itemRender) {
                    if (_allItem != null) {
                        for (int i = 0; i < _allItem.Count; i++) {
                            DestroyImmediate(_allItem[i].go);
                        }
                        _allItem.Clear();
                    }
                    if (_itemPool != null) {
                        for (int i = 0; i < _itemPool.Count; i++) {
                            DestroyImmediate(_itemPool[i].go);
                        }
                        _itemPool.Clear();
                    }
                }
                _itemRender = value;
                CItemRender item = (CItemRender)Activator.CreateInstance(_itemRender);
                UIWidget uiw = item.GetComponent<UIWidget>();
                itemWidth = uiw.width;
                itemHeight = uiw.height;
                Overlay.SetDimensions(itemWidth, _itemHeight);
                Overlay.transform.parent = Content.transform;
                Overlay.pivot = UIWidget.Pivot.TopLeft;
                if (_selectedIndex == -1) {
                    Overlay.transform.localPosition = new Vector3(PaddingLeft, Overlay.height + 100, 0);
                    if (spSelected != null) spSelected.transform.localPosition = new Vector3(PaddingLeft, spSelected.height + 100, 0);
                }
                if (spSelected != null) {
                    spSelected.SetDimensions(itemWidth, _itemHeight);
                }
                DestroyImmediate(item.go);

            }
        }
        public void Clear() {
            lastDataProvider = null;
            if (dataProvider != null) {
                dataProvider.Clear();
            }
            RestScrollBar();
            if (_allItem != null) {
                for (int i = 0; i < _allItem.Count; i++) {
                    DestroyImmediate(_allItem[i].go);
                }
                _allItem.Clear();
            }
            if (_itemPool != null) {
                for (int i = 0; i < _itemPool.Count; i++) {
                    DestroyImmediate(_itemPool[i].go);
                }
                _itemPool.Clear();
            }

        }

        public int itemHeight {
            get {
                return _itemHeight;
            }
            set {
                _itemHeight = value;
                Overlay.SetDimensions(itemWidth, _itemHeight);
                Overlay.transform.parent = Content.transform;
                Overlay.pivot = UIWidget.Pivot.TopLeft;
                if (_selectedIndex == -1) {
                    Overlay.transform.localPosition = new Vector3(PaddingLeft, Overlay.height + 100, 0);
                }
                if (spSelected != null) {
                    spSelected.SetDimensions(itemWidth, _itemHeight);
                }
            }
        }

        public int itemWidth {
            get {
                return _itemWidth;
            }
            set {
                _itemWidth = value;
                Overlay.SetDimensions(itemWidth, _itemHeight);
                Overlay.transform.parent = Content.transform;
                Overlay.pivot = UIWidget.Pivot.TopLeft;
                if (_selectedIndex == -1) {
                    Overlay.transform.localPosition = new Vector3(PaddingLeft, Overlay.height + 100, 0);
                }
                if (spSelected != null) {
                    spSelected.SetDimensions(itemWidth, _itemHeight);
                }
            }
        }

        private object lastDataProvider;
        public void SetDataProvider<T>(IEnumerable<T> value) {
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
            if (_allItemIndex == null) {
                _allItemIndex = new List<int>();
            }
            SetData<T>(value);
            if (lastDataProvider != null && lastDataProvider == value) {
                InitCellRender(false);
            } else {
                InitCellRender();
            }

            if (_dataProvider == null) {
                TotalHeight = 0;
            } else {
                int itemH = _itemHeight * Mathf.CeilToInt(_dataProvider.Count / float.Parse(ColNum.ToString()));
                TotalHeight = 2 * PaddingTop + itemH;
            }

            if (ShowBar) {
                Bar.gameObject.SetActive(TotalHeight > this.height);
                if (Bar.gameObject.activeSelf == true) {
                    Bar.BarSize = (float)this.height / TotalHeight;
                    if (maxRollPixel != 0) Bar.maxRoll = maxRollPixel / TotalHeight;
                }
            } else {
                Bar.gameObject.SetActive(false);
            }
            if (_selectedIndex == -1 && _dataProvider.Count > 0 && _isAutoSelect) {
                SelectedIndex = 0;
            } else {
                if (lastDataProvider != null && lastDataProvider == value) {
                    CheckShowItem(true);
                } else {
                    CheckShowItem();
                }
                if (_dataProvider.Count == 0 && spSelected != null) {
                    spSelected.transform.localPosition = new Vector3(PaddingLeft, spSelected.height, 0);
                }
            }
            lastDataProvider = value;
        }

        private void InitCellRender(bool SetData = true) {
            int rowNum = Convert.ToInt32(Mathf.Ceil(this.height / (float)itemHeight));
            int renderNum = Math.Min(_dataProvider.Count, rowNum * ColNum);
            bool isEnough = _allItem.Count >= renderNum;
            int n = Mathf.Abs(_allItem.Count - renderNum);
            CItemRender item;
            while (n > 0) {
                if (isEnough == true) {
                    item = _allItem[_allItem.Count - 1];
                    RetrieveRender(item);
                } else {
                    item = GetOneRender();
                    _allItem.Add(item);
                    item.SetParent(Content.transform);
                }
                n--;
            }

            if (SetData == true || isEnough == false) {
                for (int i = 0; i < _allItem.Count; i++) {
                    item = _allItem[i];
                    item.name = "Item" + i;
                    CItemRender w = item;
                    w.Data = _dataProvider[i];
                    int offY = i / ColNum;
                    float itemY = (offY * _itemHeight * -1);
                    float itemX = (itemWidth) * (i % ColNum);
                    item.tran.localPosition = new Vector3(PaddingLeft + itemX, -PaddingTop + itemY, 0);
                }
                firstPoint = 0;
                lastPoint = renderNum - 1;
            }
        }

        private CItemRender GetOneRender() {
            CItemRender item;
            if (_itemPool.Count > 0) {
                item = _itemPool[_itemPool.Count - 1];
                _itemPool.RemoveAt(_itemPool.Count - 1);
            } else {
                item = (CItemRender)Activator.CreateInstance(_itemRender);
                UIEventListener listener = UIEventListener.Get(item.go);
                listener.onHover = OnItemHover;
                listener.onPress = OnItemPress;
            }
            item.SetParent(Content.transform);
            if (delayKey2 == -1) {
                delayKey2 = UILoopManager.SetTimeout(DelayRetrieveRender2, 0.01f);
            }
            return item;
        }

        private int delayKey2 = -1;
        private int delayKey = -1;
        private void RetrieveRender(CItemRender item) {
            //item = _allItem[_allItem.Count - 1];
            _allItem.Remove(item);
            _itemPool.Add(item);
            //item.SetParent(Recycle);
            //item.tran.localPosition = Vector3.zero;
            //item.SetActive(false);
            if (delayKey == -1) {
                delayKey = UILoopManager.SetTimeout(DelayRetrieveRender, 0.01f);
            }
        }

        private void DelayRetrieveRender() {
            foreach (CItemRender item in _itemPool) {
                item.SetParent(Recycle);
                item.tran.localPosition = Vector3.zero;
                item.SetActive(false);
            }
            delayKey = -1;
        }
        private void DelayRetrieveRender2() {
            foreach (CItemRender item in _allItem) {
                item.SetActive(true);
            }
            delayKey2 = -1;
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

        public List<object> dataProvider {
            get {
                return _dataProvider;
            }
        }
        public void RestScrollBar() {
            if (Bar != null) {
                Bar.value = 0;
            }
        }

        private void OnMouseScroll(GameObject go, float delta) {
            if ((go == gameObject || UICamera.hoveredObject.transform.IsChildOf(Content.transform) == true) && Bar.gameObject.activeSelf) {
                float v = Bar.value;
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

                v += roll * -1f;
                if (v < 0) {
                    v = 0;
                } else if (v > 1) {
                    v = 1;
                }
                Bar.value = v;
            }
        }

        public void OnBarChange(GameObject go, float v) {
            float cy = Mathf.Lerp(0, TotalHeight - this.height, v);
            contentPos.y = cy;
            Content.transform.localPosition = contentPos;
            maskOffset.y = -cy;
            Content.clipOffset = maskOffset;
            CheckShowItem();
        }

        //把index显示在第一行
        public void ScrollToIndex(int index) {
            if (TotalHeight < this.height) {
                return;
            }
            float y = _itemHeight * index;
            if (y > TotalHeight - this.height) {
                y = TotalHeight - this.height;
            }
            contentPos.y = y;
            Content.transform.localPosition = contentPos;
            maskOffset.y = -y;
            Content.clipOffset = maskOffset;
        }

        public void ScrollToNext() {
            if (TotalHeight < this.height) {
                return;
            }
            int now = int.Parse(Mathf.Ceil((contentPos.y) / _itemHeight).ToString());
            float y = _itemHeight * (now + 1);
            float itemH = _itemHeight * _dataProvider.Count / float.Parse(ColNum.ToString());
            if (y > itemH - this.height) {
                y = itemH - this.height;
            }
            contentPos.y = y;
            Content.transform.localPosition = contentPos;
            maskOffset.y = -y;
            Content.clipOffset = maskOffset;
        }

        public void ScrollToBefore() {
            if (TotalHeight < this.height) {
                return;
            }
            int now = int.Parse(Mathf.Ceil((contentPos.y) / _itemHeight).ToString());
            float y = _itemHeight * (now - 1);
            if (y < 0) {
                y = 0;
            }
            contentPos.y = y;
            Content.transform.localPosition = contentPos;
            maskOffset.y = -y;
            Content.clipOffset = maskOffset;
        }

        private void CheckShowItem(bool resetData = false) {
            float offY = Content.transform.localPosition.y;

            for (int i = _allItem.Count - 1; i >= 0; i--) {
                CItemRender item = _allItem[i];
                if ((-item.y + itemHeight) <= offY || (offY + this.height) <= (-item.y)) {
                    RetrieveRender(item);
                }
            }
            int n = int.Parse(Mathf.Floor(offY / itemHeight).ToString());
            int nowFirstPoint = n * ColNum;
            n = int.Parse(Mathf.Ceil((this.height + offY) / (float)itemHeight).ToString());
            int nowLastPoint = ColNum * n - 1;
            nowLastPoint = Mathf.Max(0, nowLastPoint);
            nowLastPoint = Math.Min(nowLastPoint, _dataProvider.Count - 1);



            if (nowLastPoint > lastPoint) {
                int needItemNum = nowLastPoint - lastPoint;
                for (int i = 1; i <= needItemNum; i++) {
                    int index = lastPoint + i;
                    if (_dataProvider.Count <= index) {
                        continue;
                    }
                    CItemRender item = GetOneRender();
                    item.Data = _dataProvider[index];
                    int offYIndex = index / ColNum;
                    float itemY = (offYIndex * _itemHeight * -1);
                    float itemX = (itemWidth) * ((i - 1) % ColNum);
                    item.tran.localPosition = new Vector3(PaddingLeft + itemX, -PaddingTop + itemY, 0);
                    _allItem.Add(item);
                }
            } if (nowLastPoint == lastPoint && nowFirstPoint == firstPoint && (nowLastPoint - nowFirstPoint + 1) > _allItem.Count) {
                int needItemNum = (nowLastPoint - nowFirstPoint + 1) - _allItem.Count;
                for (int i = 1; i <= needItemNum; i++) {
                    int index = lastPoint - needItemNum + i;
                    if (_dataProvider.Count <= index) {
                        continue;
                    }
                    CItemRender item = GetOneRender();
                    item.Data = _dataProvider[index];
                    int offYIndex = index / ColNum;
                    float itemY = (offYIndex * _itemHeight * -1);
                    float itemX = (itemWidth) * ((i - 1) % ColNum);
                    item.tran.localPosition = new Vector3(PaddingLeft + itemX, -PaddingTop + itemY, 0);
                    _allItem.Add(item);
                }
            }
            if (nowFirstPoint < firstPoint) {
                int needItemNum = firstPoint - nowFirstPoint;
                for (int i = 1; i <= needItemNum; i++) {
                    int index = firstPoint - i;
                    if (_dataProvider.Count <= index) {
                        continue;
                    }
                    CItemRender item = GetOneRender();
                    item.Data = _dataProvider[index];
                    int offYIndex = index / ColNum;
                    float itemY = (offYIndex * _itemHeight * -1);
                    float itemX = (itemWidth) * (ColNum - ((i - 1) % ColNum) - 1);
                    item.tran.localPosition = new Vector3(PaddingLeft + itemX, -PaddingTop + itemY, 0);
                    _allItem.Insert(0, item);
                }
            }
            firstPoint = nowFirstPoint;
            lastPoint = nowLastPoint;

            if (resetData) {
                for (int i = 0; i <= lastPoint - firstPoint; i++) {
                    CItemRender item = _allItem[i];
                    item.Data = _dataProvider[firstPoint + i];
                }
            }
        }

        private void OnItemHover(GameObject go, bool isOver) {
            if (isOver) {
                Overlay.transform.localPosition = go.transform.localPosition;
            } else {
                Overlay.transform.localPosition = new Vector3(PaddingLeft, Overlay.height + 100, 0);
            }
        }
        private void OnItemPress(GameObject go, bool isPressed) {
            OnItemPress(go, isPressed, true);
        }
        private void OnItemPress(GameObject go, bool isPressed, bool needEvent) {
            if (isPressed) {
                CItemRender c = go.GetDisplayObj() as CItemRender;
                SelectedItem = c.Data;
                if (spSelected != null) {
                    spSelected.transform.localPosition = c.tran.localPosition;
                }
                if (OnItemSelect != null && needEvent) {
                    OnItemSelect(c);
                }
            }
        }

        private int GetItemIndex(CItemRender item) {
            return _dataProvider.IndexOf(item.Data);
        }

        private object _selectedItem;
        //设置SelectedItem不导致引发SelectedIndex事件，避免死循环
        public object SelectedItem {
            set {
                _selectedIndex = _dataProvider.IndexOf(value);
                if (_selectedIndex == -1)
                    return;
                _selectedItem = _dataProvider[_selectedIndex];
                SetSpSelectedPos(_allItem[_selectedIndex - firstPoint].localPosition);
            }
            get { return _selectedItem; }
        }

        private int _selectedIndex = -1;

        public int SelectedIndex {
            set {
                _selectedIndex = value;
                if (_selectedIndex == -1) {
                    if (spSelected != null) {
                        spSelected.width = this.width - Bar.width;
                        spSelected.transform.localPosition = new Vector3(PaddingLeft, spSelected.height + 100, 0);
                    }
                    return;
                }
                _selectedItem = _dataProvider[_selectedIndex];
                SetSpSelectedPos(_allItem[_selectedIndex - firstPoint].localPosition);
                ScrollToIndex(_selectedIndex);
                OnItemPress(_allItem[_selectedIndex - firstPoint].go, true);
            }
            get { return _selectedIndex; }
        }

        public int SelectIndexWithOutEvent {
            set {
                _selectedIndex = value;
                if (_selectedIndex == -1) {
                    if (spSelected != null) {
                        spSelected.width = this.width - Bar.width;
                        spSelected.transform.localPosition = new Vector3(PaddingLeft, spSelected.height + 100, 0);
                    }
                    return;
                }
                _selectedItem = _dataProvider[_selectedIndex];
                SetSpSelectedPos(_allItem[_selectedIndex - firstPoint].localPosition);
                ScrollToIndex(_selectedIndex);
                OnItemPress(_allItem[_selectedIndex - firstPoint].go, true, false);
            }
        }

        public int SelectedIndexWithOutScroll {
            set {
                _selectedIndex = value;
                if (_selectedIndex == -1) {
                    if (spSelected != null) {
                        spSelected.width = this.width - Bar.width;
                        spSelected.transform.localPosition = new Vector3(PaddingLeft, spSelected.height + 100, 0);
                    }
                    return;
                }
                _selectedItem = _dataProvider[_selectedIndex];
                SetSpSelectedPos(_allItem[_selectedIndex - firstPoint].localPosition);
                OnItemPress(_allItem[_selectedIndex - firstPoint].go, true);
            }
            get { return _selectedIndex; }
        }

        private void SetSpSelectedPos(Vector3 v) {
            if (spSelected != null) {
                spSelected.transform.localPosition = v;
            }
        }

        public bool ShowBar {
            set {
                _showBar = value;
            }
            get {
                return _showBar;
            }
        }

        public bool IsAutoSelect {
            set {
                _isAutoSelect = value;
            }
            get {
                return _isAutoSelect;
            }
        }

        public int ListHeight {
            set {
                this.height = value;
                Content.baseClipRegion = new Vector4((this.width - Bar.width) / 2, -this.height / 2, this.width - Bar.width, this.height);
                Bar.Track.height = value;
            }
            get { return this.height; }
        }

        public List<CItemRender> allItem {
            get {
                return _allItem;
            }
        }
    }
}
