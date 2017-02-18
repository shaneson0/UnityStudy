using Assets.Scripts.Com.MingUI;
using Assets.Scripts.Com.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MingUI.Com.UI {
    public class CMultiTree : CCanvas {
        public delegate void VoidFun(CTreeNode selectItem);
        public UISprite Overlay;
        public UISprite spSelected;
        public Transform Recycle;
        protected System.Type _branchRender;

        protected int _branchWidth;
        protected int _branchHeight;
        public int TotalHeight;
        public int _branchGapV = 0;//父节点的每个ITEM的距离
        public VoidFun OnItemSelect;

        public List<CTreeNodeData> _dataProvider;

        protected List<CTreeNode> _branchPool;//没有用到的分节点
        protected List<CTreeNode> _branchUsedList;//在用的
        protected Dictionary<CTreeNode, List<CTreeNode>> _branchUsedChildeList;//在用的

        private int _openBranchIndex = -1;
        //protected int _selectedLeafIndex = -1;
        protected CTreeNode openBranch;
        //protected CTreeNode openLeaf;

        public bool autoSelectFirstBranch = true;
        public bool autoSelectFirstLeaf = true;
        public bool onlyOneOpen = true;

        protected override void OnStart() {
            base.OnStart();
            int barWidth = 0;
            if (Bar.gameObject.activeSelf == true) {
                barWidth = Bar.width;
            }
            Overlay.width = this.width - barWidth;
            Overlay.transform.localPosition = new Vector3(PaddingLeft, Overlay.height + 100, 0);
            spSelected.width = this.width - barWidth;
            Content.baseClipRegion = new Vector4((this.width - barWidth) / 2, -this.height / 2, this.width - barWidth, this.height);
        }

        public System.Type branchRender {
            set {
                _branchRender = value;
                CTreeNode item = (CTreeNode)Activator.CreateInstance(_branchRender);
                UIWidget uiw = item.GetComponent<UIWidget>();
                branchHeight = uiw.height;
                branchWidth = uiw.width;
                Overlay.SetDimensions(branchWidth, branchHeight);
                Overlay.transform.parent = Content.transform;
                Overlay.pivot = UIWidget.Pivot.TopLeft;
                if (_openBranchIndex == -1) {
                    Overlay.transform.localPosition = new Vector3(PaddingLeft, branchHeight, 0);
                }
                DestroyImmediate(item.go);
            }
        }

        public int branchHeight {
            get {
                return _branchHeight;
            }
            set {
                _branchHeight = value;
            }
        }

        public int branchWidth {
            get {
                return _branchWidth;
            }
            set {
                _branchWidth = value;
            }
        }

        public int branchGapU {
            get {
                return _branchGapV;
            }
            set {
                _branchGapV = value;
            }
        }

        public virtual void SetDataProvider<T>(IEnumerable<T> value) {
            if (_branchRender == null) {
                Debug.LogError("set itemRender first!");
                return;
            }
            InitPool();

            //_dataProvider = value
            SetData<T>(value);
            //_dataProvider = (value ?? new List<T>()) as List<object>;

            bool isEnough = _branchUsedList.Count >= _dataProvider.Count;
            int n = Mathf.Abs(_branchUsedList.Count - _dataProvider.Count);
            _branchUsedChildeList.Clear();
            CTreeNode item;
            while (n > 0) {
                if (isEnough == true) {
                    item = _branchUsedList[_branchUsedList.Count - 1];
                    if (_branchUsedList.Contains(item)) {
                        _branchUsedList.Remove(item);
                    }
                    if (_branchPool.Contains(item) == false) {
                        _branchPool.Add(item);
                    }
                    item.SetParent(Recycle);
                    item.tran.localPosition = Vector3.zero;
                    item.SetActive(false);
                    //RecycleBranchItem(item);
                } else {
                    if (_branchPool.Count > 0) {
                        item = _branchPool[0];
                        _branchPool.RemoveAt(0);
                        item.SetActive(true);
                    } else {
                        item = (CTreeNode)Activator.CreateInstance(_branchRender);
                    }
                    item.SetParent(Content.transform);
                    _branchUsedList.Add(item);
                }
                n--;
            }
            ResetItems();
            if (OpenBranchIndex == -1 && _dataProvider.Count > 0 && autoSelectFirstBranch) {
                OpenBranchIndex = 0;
            }
        }

        private void SetData<T>(IEnumerable<T> value) {
            _dataProvider = _dataProvider ?? new List<CTreeNodeData>();
            while (_dataProvider.Count > 0) {
                _dataProvider.RemoveAt(0);
            }
            if (value == null) {
                return;
            }
            foreach (var t in value) {
                _dataProvider.Add(t as CTreeNodeData);
            }
        }

        public virtual void ResetItems() {
            CTreeNode branchItem;
            TotalHeight = 0;
            if (_branchUsedList == null) {
                return;
            }
            for (int i = 0; i < _dataProvider.Count; i++) {
                branchItem = _branchUsedList[i];
                branchItem.name = "Item" + i;
                branchItem.data = _dataProvider[i];

                float itemY;
                itemY = -TotalHeight;
                branchItem.tran.localPosition = new Vector3(0, itemY, 0);
                EventUtil.AddHover(branchItem.go, OnItemHover);
                EventUtil.AddPress(branchItem.go, OnItemHover);
                UIEventListener listener = UIEventListener.Get(branchItem.go);
                listener.onHover = OnItemHover;
                listener.onPress = OnItemPress;
                TotalHeight += branchHeight + branchGapU;
                CheckOpenAndResetChilde(branchItem);
            }

            //spSelected.gameObject.SetActive(!clearAllLeaf);

            CalculateHeight(TotalHeight);
            int barWidth = 0;
            if (Bar.gameObject.activeSelf == true) {
                barWidth = Bar.width;
            }
            Overlay.width = this.width - barWidth;
            spSelected.width = this.width - barWidth;
            Content.baseClipRegion = new Vector4((this.width - barWidth) / 2, -this.height / 2, this.width - barWidth, this.height);
        }

        private void CheckOpenAndResetChilde(CTreeNode branchItem) {
            int offX = 10;
            CTreeNodeData parentData = branchItem.data.parent; ;
            while (parentData != null) {
                offX += 10;
                parentData = parentData.parent;
            }
            int chlidLen = 0;
            if (branchItem.data.isOpen) {
                int startIndex = chlidLen;
                if (branchItem.data.child != null) chlidLen += branchItem.data.child.Count;
                for (int n = 0; (n + startIndex) < chlidLen; n++) {
                    CTreeNode child;
                    var childeList = GetUsedChildeList(branchItem);
                    if ((n + startIndex) < childeList.Count) {
                        child = childeList[n + startIndex];

                    } else {
                        child = GetBranchItem();
                        childeList.Add(child);
                    }
                    if (_branchUsedList.Contains(child) == false) _branchUsedList.Add(child);
                    child.name = "Item" + n;
                    child.SetParent(Content.transform);
                    child.SetActive(true);
                    child.data = branchItem.data.child[n];
                    float childY;
                    childY = -TotalHeight;
                    child.tran.localPosition = new Vector3(offX, childY, 0);

                    UIEventListener childListener = UIEventListener.Get(child.go);
                    childListener.onHover = OnItemHover;
                    childListener.onPress = OnItemPress;
                    TotalHeight += _branchHeight + branchGapU;
                    CheckOpenAndResetChilde(child);
                }
            } else {
                var childeList = GetUsedChildeList(branchItem);
                for (int n = 0, len = childeList.Count; n < len; n++) {
                    RecycleBranchItem(childeList[n]);
                }
                childeList.Clear();
            }

        }

        public void CloseNowOpen() {
            if (openBranch != null) {
                openBranch.data.isOpen = false;
                ResetItems();
            }
        }

        public List<CTreeNode> GetUsedChildeList(CTreeNode node) {
            if (_branchUsedChildeList.ContainsKey(node) == false) {
                _branchUsedChildeList[node] = new List<CTreeNode>();
            }
            return _branchUsedChildeList[node];
        }



        private void InitPool() {
            if (_branchPool == null) {
                _branchPool = new List<CTreeNode>();
            }

            if (_branchUsedList == null) {
                _branchUsedList = new List<CTreeNode>();
            }

            if (_branchUsedChildeList == null) {
                _branchUsedChildeList = new Dictionary<CTreeNode, List<CTreeNode>>();
            }
        }

        private void SetSelSp(CTreeNode node) {
            if (node.type == CTreeNode.NodeType.Branch) {
                spSelected.transform.localPosition = new Vector3(PaddingLeft, spSelected.height + 100, 0);
            } else {
                spSelected.transform.localPosition = node.tran.localPosition;
            }
        }
        protected void OnItemHover(object go, bool isOver) {
            if (isOver) {
                CTreeNode node = ((GameObject)go).GetDisplayObj() as CTreeNode;
                //if (node.type == CTreeNode.NodeType.Branch) {
                Overlay.SetDimensions(branchWidth, branchHeight);
                //} else {
                //    Overlay.SetDimensions(leafWidth, leafHeight);
                //}
                Overlay.transform.localPosition = ((GameObject)go).transform.localPosition;
            } else {
                Overlay.transform.localPosition = new Vector3(PaddingLeft, Overlay.height + 100, 0);
            }
        }
        protected void OnItemPress(object go, bool isPressed) {
            if (isPressed) {
                CTreeNode c = ((GameObject)go).GetDisplayObj() as CTreeNode;
                SetSelectedItem(c);
                if (c.type == CTreeNode.NodeType.Branch) {
                    RollTo(c);
                }
                if (OnItemSelect != null) {
                    OnItemSelect(c);
                    SetSelSp(c);
                }
            }
        }

        protected void OnItemPress(GameObject go, bool isPressed, bool needEvent) {
            if (isPressed) {
                CTreeNode c = go.GetDisplayObj() as CTreeNode;
                SelectedItem = c;
                if (OnItemSelect != null && needEvent) {
                    OnItemSelect(c);
                    SetSelSp(c);
                }
            }
        }

        public int OpenBranchIndex {
            get {
                return _openBranchIndex;
            }
            set {
                int i = value;
                if (i == -1 || _dataProvider == null || _dataProvider.Count <= i) {
                    _openBranchIndex = value;
                    return;
                }
                if (_openBranchIndex != i) {
                    _openBranchIndex = i;
                    if (openBranch != null && onlyOneOpen) {
                        openBranch.data.isOpen = false;
                    }
                    openBranch = _branchUsedList[i];
                    openBranch.data.isOpen = true;
                    ResetItems();
                } else {
                    _openBranchIndex = value;
                }
            }
        }

        private CTreeNode _SelectedItem;
        public CTreeNode SelectedItem {
            set {
                _SelectedItem = value;
                CTreeNodeData nodeData = value.data;
                if (value.type == CTreeNode.NodeType.Leaf) {
                    _openBranchIndex = _dataProvider.IndexOf(nodeData.parent);
                } else {
                    int tempIndex = _dataProvider.IndexOf(nodeData);
                    if (tempIndex != _openBranchIndex) {
                        value.data.isOpen = true;
                        if (openBranch != null && onlyOneOpen) {
                            openBranch.data.isOpen = false;
                        }
                        openBranch = value;
                    }
                    _openBranchIndex = tempIndex;
                    ResetItems();
                }
            }
            get {
                return _SelectedItem;
            }
        }

        public void SetSelectedItem(CTreeNode node) {
            node.data.isOpen = !node.data.isOpen;
            ResetItems();
        }

        public CTreeNodeData SelectedData {
            set {
                CTreeNodeData nodeData = value;
                nodeData.isOpen = true;
                while (nodeData.parent != null) {
                    nodeData = nodeData.parent;
                    nodeData.isOpen = true;
                }
                ResetItems();
                foreach (CTreeNode node in _branchUsedList) {
                    if (node.data == value) {
                        RollTo(node);
                        OnItemPress(node.go, true, true);
                        return;
                    }
                }
            }
            //    CTreeNodeData nodeData = value;
            //    int index = _dataProvider.IndexOf(nodeData);
            //    if (index != -1) {
            //        OpenBranchIndex = index;
            //    } else {
            //        for (int i = 0, len = _dataProvider.Count; i < len; i++) {
            //            CTreeNodeData BranchData = _dataProvider[i];
            //            index = BranchData.child.IndexOf(nodeData);
            //            if (index != -1) {
            //                if (openBranch != null && onlyOneOpen) {
            //                    openBranch.data.isOpen = false;
            //                }
            //                _branchUsedList[i].data.isOpen = true;
            //                openBranch = _branchUsedList[i];
            //                _openBranchIndex = i;
            //                ResetItems();
            //                OnItemPress(_branchUsedList[OpenBranchIndex].go, true, true);
            //            }
            //        }
            //    }
            //    foreach (CTreeNode node in _branchUsedList) {
            //        if (node.data == value) {
            //            RollTo(node);
            //            return;
            //        }
            //    }
            //}
        }


        public void RollTo(CTreeNode node) {
            float y = -node.y;
            if (TotalHeight > this.height && y > TotalHeight - this.height) {
                y = TotalHeight - this.height;
            }
            //contentPos.y = y;
            //maskOffset.y = -y;
            Bar.value = y / (TotalHeight - this.height);
        }
        public List<CTreeNode> GetUsedBranch() {
            return _branchUsedList;
        }


        private CTreeNode GetBranchItem() {
            CTreeNode leaf;
            if (_branchPool.Count > 0) {
                leaf = _branchPool[0];
                _branchPool.RemoveAt(0);
            } else {
                leaf = (CTreeNode)Activator.CreateInstance(_branchRender);
            }
            leaf.type = CTreeNode.NodeType.Leaf;
            _branchUsedList.Add(leaf);
            return leaf;
        }

        private void RecycleBranchItem(CTreeNode node) {
            if (_branchUsedList.Contains(node)) {
                _branchUsedList.Remove(node);
            }
            if (_branchPool.Contains(node) == false) {
                _branchPool.Add(node);
            }
            node.data.isOpen = false;
            node.SetParent(Recycle);
            node.tran.localPosition = Vector3.zero;
            node.SetActive(false);
            CheckOpenAndResetChilde(node);
        }
    }
}
