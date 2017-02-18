using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Assets.Scripts.Com.Utils;

namespace Assets.Scripts.Com.MingUI {
	public class CTree : CCanvas {
		public delegate void VoidFun(CTreeNode selectItem);
		public UISprite Overlay;
		public UISprite spSelected;
		public Transform Recycle;
        protected System.Type _branchRender;
        protected System.Type _leafRender;

        protected int _branchWidth;
        protected int _branchHeight;
        protected int _leafWidth;
        protected int _leafHeight;
		public int TotalHeight;
		public int _branchGapV=0;//父节点的每个ITEM的距离
		public int _leafGapV=0;  //子节点的每个ITEM的距离
		public VoidFun OnItemSelect;

		public List<CTreeNodeData> _dataProvider;

        protected List<CTreeNode> _branchPool;//没有用到的分节点
        protected List<CTreeNode> _leafPool;//没有用到的子节点
        protected List<CTreeNode> _branchUsedList;//在用的
        protected List<CTreeNode> _leafUsedList;//在用的

		private int _openBranchIndex = -1;
        protected int _selectedLeafIndex = -1;
        protected CTreeNode openBranch;
        protected CTreeNode openLeaf;

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
				if (_selectedLeafIndex == -1) {
					Overlay.transform.localPosition = new Vector3(PaddingLeft, branchHeight, 0);
				}
				DestroyImmediate(item.go);
			}
		}

		public System.Type leafRender {
			set {
				_leafRender = value;
				CTreeNode item = (CTreeNode)Activator.CreateInstance(_leafRender);
				UIWidget uiw = item.GetComponent<UIWidget>();
				leafHeight = uiw.height;
				leafWidth = uiw.width;
				spSelected.SetDimensions(leafWidth, leafHeight);
				spSelected.transform.parent = Content.transform;
				spSelected.pivot = UIWidget.Pivot.TopLeft;
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

		public int leafHeight {
			get {
				return _leafHeight;
			}
			set {
				_leafHeight = value;
			}
		}

		public int leafWidth {
			get {
				return _leafWidth;
			}
			set {
				_leafWidth = value;
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
		public int leafGapU {
			get {
				return _leafGapV;
			}
			set {
				_leafGapV = value;
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
			CTreeNode item;
			while (n > 0) {
				if (isEnough == true) {
					item = _branchUsedList[_branchUsedList.Count - 1];
					_branchUsedList.RemoveAt(_branchUsedList.Count - 1);
					item.SetParent(Recycle);
					item.tran.localPosition = Vector3.zero;
					item.SetActive(false);
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
			if (OpenBranchIndex == -1 && _dataProvider.Count > 0 && autoSelectFirstBranch) {
				OpenBranchIndex = 0;
			} else {
				ResetItems();
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

        protected virtual void ResetItems() {
            CTreeNode branchItem;
            TotalHeight = 0;
            bool clearAllLeaf = true;
            int chlidLen = 0;
            if (_branchUsedList==null) {
                return;
            }
            for (int i = 0; i < _branchUsedList.Count; i++) {
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
                TotalHeight += branchHeight+branchGapU;
                if (branchItem.isOpen) {
                    int startIndex = chlidLen;
                    if (branchItem.data.child!=null) chlidLen += branchItem.data.child.Count;
                    if (chlidLen > 0) {
                        clearAllLeaf = false;
                    }
                    for (int n = 0; (n + startIndex) < chlidLen; n++) {
                        CTreeNode child;
                        if ((n + startIndex) < _leafUsedList.Count) {
                            child = _leafUsedList[n + startIndex];

                        } else {
                            child = GetLeaf();
                        }
                        child.name = "Item" + i;
                        child.SetParent(Content.transform);
                        child.SetActive(true);
                        child.data = branchItem.data.child[n];

                        float childY;
                        childY = -TotalHeight;
                        child.tran.localPosition = new Vector3(0, childY, 0);

                        UIEventListener childListener = UIEventListener.Get(child.go);
                        childListener.onHover = OnItemHover;
                        childListener.onPress = OnItemPress;
                        TotalHeight += _leafHeight+leafGapU;
                    }
                }
            }

            if (chlidLen < _leafUsedList.Count) {
                for (int j = _leafUsedList.Count - 1; j >= chlidLen; j--) {
                    CTreeNode temp = _leafUsedList[j];
                    temp.SetParent(Recycle);
                    _leafPool.Add(temp);
                    _leafUsedList.Remove(temp);
                }
            }
            if (clearAllLeaf) {
                for (int j = _leafUsedList.Count - 1; j >= 0; j--) {
                    CTreeNode temp = _leafUsedList[j];
                    temp.SetParent(Recycle);
                    _leafPool.Add(temp);
                    _leafUsedList.Remove(temp);
                }
            }
            spSelected.gameObject.SetActive(!clearAllLeaf);
            //if (_dataProvider == null) {
            //    TotalHeight = 0;
            //} else {
            //    float itemH = _branchHeight * _dataProvider.Count / float.Parse(ColNum.ToString());
            //        TotalHeight = PaddingTop + Mathf.CeilToInt(itemH) + PaddingBottom;
            //}

            //Bar.gameObject.SetActive(TotalHeight > height);
            //if (Bar.gameObject.activeSelf == true) {
            //    Bar.BarSize = (float)this.height / TotalHeight;
            //}
            CalculateHeight(TotalHeight);
            int barWidth = 0;
            if (Bar.gameObject.activeSelf == true) {
                barWidth = Bar.width;
            }
            Overlay.width = this.width - barWidth;
            spSelected.width = this.width - barWidth;
            Content.baseClipRegion = new Vector4((this.width - barWidth) / 2, -this.height / 2, this.width - barWidth, this.height);
        }

        public void CloseNowOpen() {
            if (openBranch!=null) {
                openBranch.isOpen = false;
                ResetItems();
            }
        }

        protected CTreeNode GetLeaf() {
			CTreeNode leaf;
			if (_leafPool.Count > 0) {
				leaf = _leafPool[0];
				_leafPool.RemoveAt(0);
			} else {
				leaf = (CTreeNode)Activator.CreateInstance(_leafRender);
			}
			leaf.type = CTreeNode.NodeType.Leaf;
			_leafUsedList.Add(leaf);
			return leaf;
		}
		private void InitPool() {
			if (_branchPool == null) {
				_branchPool = new List<CTreeNode>();
			}
			if (_leafPool == null) {
				_leafPool = new List<CTreeNode>();
			}

			if (_branchUsedList == null) {
				_branchUsedList = new List<CTreeNode>();
			}
			if (_leafUsedList == null) {
				_leafUsedList = new List<CTreeNode>();
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
				if (node.type == CTreeNode.NodeType.Branch) {
					Overlay.SetDimensions(branchWidth, branchHeight);
				} else {
					Overlay.SetDimensions(leafWidth, leafHeight);
				}
				Overlay.transform.localPosition = ((GameObject)go).transform.localPosition;
			} else {
				Overlay.transform.localPosition = new Vector3(PaddingLeft, Overlay.height + 100, 0);
			}
		}
        protected void OnItemPress(object go, bool isPressed) {
			if (isPressed) {
				CTreeNode c = ((GameObject)go).GetDisplayObj() as CTreeNode;
				SetSelectedItem(c);
                if(c.type == CTreeNode.NodeType.Branch){
                    RollTo(c);
                }
				if (OnItemSelect != null) {
					if (_leafUsedList.Count == 0) {
						return;
					}
					CTreeNode l = autoSelectFirstLeaf ? _leafUsedList[_selectedLeafIndex] : c;
					if (openLeaf != null) {
						openLeaf.isOpen = false;
					}
					if (autoSelectFirstLeaf || c.type == CTreeNode.NodeType.Leaf) {
						openLeaf = l;
						openLeaf.isOpen = true;
					}
					OnItemSelect(l);
					SetSelSp(l);
				}
			}
		}

        protected void OnItemPress(GameObject go, bool isPressed, bool needEvent) {
			if (isPressed) {
				CTreeNode c = go.GetDisplayObj() as CTreeNode;
				SelectedItem = c;
				if (OnItemSelect != null && needEvent) {
					if (_leafUsedList.Count == 0) {
						return;
					}
					CTreeNode l = autoSelectFirstLeaf ? _leafUsedList[_selectedLeafIndex] : c;
					if (openLeaf != null) {
						openLeaf.isOpen = false;
					}
					if (autoSelectFirstLeaf || c.type == CTreeNode.NodeType.Leaf) {
						openLeaf = l;
						openLeaf.isOpen = true;
					}
					OnItemSelect(l);
					SetSelSp(l);
				}
			}
		}

		public int OpenBranchIndex {
			get {
				return _openBranchIndex;
			}
			set {
				int i = value;
                if (i==-1 ||  _dataProvider == null || _dataProvider.Count <= i) {
                    _openBranchIndex = value;
                    return;
                }
				if (_openBranchIndex != i) {
					_openBranchIndex = i;
                    if (openBranch != null && onlyOneOpen) {
                        openBranch.isOpen = false;
                    }
                    openBranch = _branchUsedList[i];
                    openBranch.isOpen = true;
					ResetItems();
					SelectedLeafIndex = autoSelectFirstLeaf ? 0 : -1;
				} else {
					_openBranchIndex = value;
				}
			}
		}

		public int SelectedLeafIndex {
			get {
				return _selectedLeafIndex;
			}
			set {
				_selectedLeafIndex = value;
                if (_leafUsedList.Count > _selectedLeafIndex && _selectedLeafIndex>=0) {
                   OnItemPress(_leafUsedList[_selectedLeafIndex].go, true, true);
                }else{
				   OnItemPress(_branchUsedList[OpenBranchIndex].go, true, true);
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
					_selectedLeafIndex = nodeData.parent.child.IndexOf(nodeData);
				} else {
					int tempIndex = _dataProvider.IndexOf(nodeData);
					if (tempIndex != _openBranchIndex) {
						_selectedLeafIndex = autoSelectFirstLeaf ? 0 : -1;
						value.isOpen = true;
						if (openBranch != null && onlyOneOpen) {
							openBranch.isOpen = false;
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
			CTreeNodeData nodeData = node.data;
			if (node.type == CTreeNode.NodeType.Leaf) {
				_openBranchIndex = _dataProvider.IndexOf(nodeData.parent);
				_selectedLeafIndex = nodeData.parent.child.IndexOf(nodeData);
			} else {
				int tempIndex = _dataProvider.IndexOf(nodeData);
				if (tempIndex != _openBranchIndex) {
					_selectedLeafIndex = autoSelectFirstLeaf ? 0 : -1;
					node.isOpen = true;
					if (openBranch != null && onlyOneOpen) {
						openBranch.isOpen = false;
					}
					openBranch = node;
				} else if (openBranch != null) {
					openBranch.isOpen = !openBranch.isOpen;
				}
				_openBranchIndex = tempIndex;
				ResetItems();
			}
		}

		public CTreeNodeData SelectedData {
			set {
				CTreeNodeData nodeData = value;
				int index = _dataProvider.IndexOf(nodeData);
				if (index != -1) {
					OpenBranchIndex = index;
				} else {
					for (int i = 0, len = _dataProvider.Count; i < len; i++) {
						CTreeNodeData BranchData = _dataProvider[i];
						index = BranchData.child.IndexOf(nodeData);
						if (index != -1) {
							if (openBranch != null && onlyOneOpen) {
								openBranch.isOpen = false;
							}
							_branchUsedList[i].isOpen = true;
							openBranch = _branchUsedList[i];
							_openBranchIndex = i;
							_selectedLeafIndex = index;
							ResetItems();
							OnItemPress(_branchUsedList[OpenBranchIndex].go, true, true);
						}
					}
				}
               foreach( CTreeNode node in _branchUsedList){
                   if (node.data ==value) {
                       RollTo(node);
                       return;
                   }
               }

               foreach (CTreeNode node in _leafUsedList) {
                   if (node.data == value) {
                       RollTo(node);
                       return;
                   }
               }
			}
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

        public List<CTreeNode> GetUsedLeaf() {
            return _leafUsedList;
        }
	}
}
