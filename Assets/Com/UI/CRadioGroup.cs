using Assets.Scripts.Com.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Com.MingUI {
	public class CRadioGroup : MonoBehaviour {
		protected List<CButtonToggle> _nowUseList;
		protected List<CButtonToggle> _unUseList;
		protected Dictionary<int, CButtonToggle> _dic;
		protected int _index;
		protected Action<int> changeFun;
        protected Action<int> clickFun;//前端埋点需要
		public bool changeOver;
		private bool isInit;
		protected List<string> labelStrs;
		protected List<int> datas;
		public int defaultIndex = 0;
		protected int dist;
		public bool isHorizon = true;
		private string normalSp;
		protected CButtonToggle selBtn = null;
		public bool enable = true;
		public void Start() {
			if (isInit == false) {
				isInit = true;
				Reset();
				if (labelStrs != null) {
					UpdateLabels(labelStrs, datas, isHorizon);
				}
				index = defaultIndex;
			}

		}

		public virtual void Reset() {
			//            int group = gameObject.GetInstanceID();
			_nowUseList = new List<CButtonToggle>();
			_unUseList = new List<CButtonToggle>();
			_dic = new Dictionary<int, CButtonToggle>();
			for (int i = 0; i < transform.childCount; i++) {
				CButtonToggle toggle = transform.GetChild(i).GetComponent<CButtonToggle>();
				if (toggle != null) {
					//保证回调的index都是初始的时候的index 省的模块那边蛋疼要去动态改变index的判断
					toggle.gameObject.SetData(i);
					EventUtil.AddClick(toggle.gameObject, OnClick, i);
					_nowUseList.Add(toggle);
					_dic[i] = toggle;
				}
			}
			normalSp = _nowUseList.Count != 0 ? _nowUseList[0].normalSprite : "";
		}

		public int GetListIndex() {
			if (_nowUseList == null) return 0;
			for (var i = 0; i < _nowUseList.Count; i++) {
				if (_nowUseList[i].seleted) return i;
			}
			return 0;
		}

		public virtual Action<int> OnChange {
			set {
				changeFun = value;
			}
		}

        public virtual Action<int> OnTabClick {
            set {
                clickFun = value;
            }
        }

		protected void OnClick(object arg) {
			if (enable) {
				index = (int)arg;
                if (clickFun != null)
                    clickFun.DynamicInvoke((int)arg);
			}
		}

		protected virtual void ChangeIndex(bool needFun = true) {
			if (_nowUseList == null) {
				return;
			}
			selBtn = null;
			for (int i = 0, len = _nowUseList.Count; i < len; i++) {
				CButtonToggle btn = _nowUseList[i];
				btn.seleted = (Convert.ToInt32(btn.gameObject.GetData()) == index);
				if (btn.seleted) {
					selBtn = btn;
				}
			}
			if (selBtn != null && needFun && changeFun != null && index >= 0) {
				var data = selBtn.gameObject.GetData();
				changeFun.DynamicInvoke(data != null ? Convert.ToInt32(data) : index);
			}
		}

		public int index {
			get {
				return _index;
			}
			set {
				if (value < 0) {
					if (selBtn != null) { selBtn.seleted = false; }
					selBtn = null;
					return;
				}

				if (isInit == false) {
					Start();
				}
				_index = value;
				ChangeIndex();
			}
		}

		public void CleanSel() {
			_index = -1;
			ChangeIndex();
		}

		public int setIndexWithOutFun {
			set {
				if (value < 0) {
					return;
				}

				if (isInit == false) {
					Start();
				}
				_index = value;
				ChangeIndex(false);
			}
		}

		public List<CButtonToggle> allBox {
			get {
				return _nowUseList;
			}
		}

		/// <summary>
		/// 需要改变选项卡的值及其响应时使用
		/// </summary>
		/// <param name="labelStrs">选项卡的字符串列表</param>
		/// <param name="datas">选项卡携带的值，changeIndex时，优先响应携带值，否则响应index</param>
		/// <param name="isHorizon">排布水平，否则垂直排列</param>
		public void UpdateLabels(List<string> labelStrs, List<int> datas = null, bool isHorizon = true) {
			this.isHorizon = isHorizon;
			this.datas = datas;
			this.labelStrs = labelStrs;
			if (!isInit) {
				return;
			}
			var needLen = labelStrs.Count;
			var len = _nowUseList.Count;
			var count = Math.Min(needLen, len);
			var needData = datas != null && datas.Count == labelStrs.Count;
			CButtonToggle box = null;
			if (len > 1) {
				if (isHorizon) {
					dist = (int)(_nowUseList[1].transform.localPosition.x - _nowUseList[0].transform.localPosition.x);
				} else {
					dist = (int)(_nowUseList[1].transform.localPosition.y - _nowUseList[0].transform.localPosition.y);
				}
			} else if (dist == 0) {
				dist = isHorizon ? _nowUseList[0].width : -_nowUseList[0].height;
			}
			for (var i = 0; i < count; i++) {
				_nowUseList[i].Text = labelStrs[i];
				if (needData) {
					_nowUseList[i].gameObject.SetData(datas[i]);
					EventUtil.RemoveClick(_nowUseList[i].gameObject, OnClick);
					EventUtil.AddClick(_nowUseList[i].gameObject, OnClick, (datas[i]));
				}
			}
			if (len < needLen) {
				for (var i = len; i < needLen; i++) {
					if (_unUseList.Count > 0) {
						box = _unUseList[0];
						_unUseList.RemoveAt(0);
						box.Text = labelStrs[i];
					} else {
						var boxObj = Instantiate(_nowUseList[0].gameObject) as GameObject;
						if (boxObj != null) {
							boxObj.name = "Tab" + i;
							box = boxObj.GetComponent<CButtonToggle>();
							box.normalSprite = normalSp;
							box.Text = labelStrs[i];
						}
					}
					if (box != null) {
						if (needData) box.gameObject.SetData(datas[i]);
						box.transform.parent = transform;
                        if (_nowUseList.Count > 0) {
                            if (isHorizon) {
                                box.transform.localPosition = new Vector2(_nowUseList[0].transform.localPosition.x + dist * i,
                                    _nowUseList[0].transform.localPosition.y);
                            } else {
                                box.transform.localPosition = new Vector2(_nowUseList[0].transform.localPosition.x,
                                    _nowUseList[0].transform.localPosition.y + dist * i);
                            }
                        } else {
                            box.transform.localPosition = Vector2.zero;
                        }
						box.transform.localScale = Vector2.one;
						box.gameObject.SetActive(true);
						if (needData) {
							EventUtil.RemoveClick(box.gameObject, OnClick);
							EventUtil.AddClick(box.gameObject, OnClick, (datas[i]));
						}
						_nowUseList.Add(box);
					}
				}
			} else {
				for (var i = 0; i < len; i++) {
					box = _nowUseList[i];
					box.gameObject.SetActive(true);
				}
				for (var i = len - 1; i >= needLen; i--) {
					box = _nowUseList[i];
					box.gameObject.SetActive(false);
					_nowUseList.RemoveAt(i);
					//_unUseList.Add(box);
					_unUseList.Insert(0, box);
				}
			}
		}
	}
}
