using Assets.Scripts.Com.MingUI;
using MingUI.Com.Manager;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Com.Managers {
    public class ToolTipManager : MonoBehaviour {
        static ToolTipManager() {

        }
        //声明和注册一种Tip
        private static Dictionary<int, Type> tipLib;
        public static Type GetToolTipType(int tipType) {
            return tipLib[tipType];
        }

        public static void RegisterToolTip(int tipType, Type type) {
            if (tipLib == null) {
                tipLib = new Dictionary<int, Type>();
            }
            tipLib.Add(tipType, type);
        }


        private const string loopKey = "ToolTipManager";
        private static Transform _container;
        private static Transform _recycle;
        private static Vector2 mouse;
        private static Vector2 pos;
        private static Vector2 offset;
        private static bool isShow;
        private static bool isInited;
        private static BaseToolTip tip;
        public static int tipType;
        public delegate void ConfigDelegate(ref BaseToolTip tip, int type, string tag);
        public static ConfigDelegate getConfigTipFun;

        public static void Init(Transform root) {
            if (isInited == false) {
                isInited = true;
                tipType = -1;
                _container = root.Find("Container");
                _recycle = root.Find("Recycle");
                //alertObj.localPosition = Vector3.zero;
                offset = new Vector2(25, -25);
                UILoopManager.AddToFrame(loopKey, OnEnterFrame);
            }
        }

        public static void Show(string text, float delay = 0.2f, int textWidth = 170) {
            Show(0, text, delay, textWidth);
        }

        public static void Show(int type, object data, float delay = 0.2f, object spData = null) {
            isShow = true;
            RecycleAll();

            if (tip == null || (tip != null && type != tipType)) {
                if (tip != null) {
                    tip.Remove();
                    tip = null;
                }
                Type tipClass = GetToolTipType(type);
                tip = (BaseToolTip)Activator.CreateInstance(tipClass);
            }
            if (tip.IsCreate()) {
                if (tip.IsActive() == false) {
                    tip.SetActive(true);
                }
                tip.spData = spData;
                tip.data = data;
                tip.SetParent(_container);
                tip.x = 0;
                tip.y = 0;
                OnEnterFrame();
                tipType = type;
            }
        }

        public static void Show_Template(int type, object data, string tipTag, float delay = 0.2f) {
            if (getConfigTipFun == null) {
                return;
            }
            isShow = true;
            RecycleAll();
            if (type == 0) {
                Show(data.ToString(), delay);
            }
            getConfigTipFun(ref tip, type, tipTag);
            tip.data = data;
            tip.SetParent(_container);
            tip.x = 0;
            tip.y = 0;
            OnEnterFrame();
            tipType = type;
        }


        public static void Hide() {
            isShow = false;
            RecycleAll();
            if (tip != null) {
                tip.Remove();
                tip = null;
            }
        }

        private static void RecycleAll(Transform tran = null) {
            if (_container != null) {
                for (int i = 0; i < _container.childCount; i++) {
                    Transform t = _container.GetChild(0);
                    if (tran == null || t != tran) {
                        t.parent = _recycle;
                        t.gameObject.SetActive(false);
                    }
                }
            }
        }

        private static int width {
            get {
                if (tip != null && isShow) {
                    return (int)tip.width;
                }
                return 0;
            }
        }
        private static int height {
            get {
                if (tip != null && isShow) {
                    return (int)tip.height;
                }
                return 0;
            }
        }

        private static void OnEnterFrame() {
            if (isShow == false) {
                return;
            }
            mouse = UICamera.lastTouchPosition;
            pos = mouse + offset;

            if (pos.y < height) {
                pos.y = height;
            }
            if (pos.x + width > Screen.width) {
                pos.x = mouse.x;
                pos.x = pos.x - width;
            }
            if (pos.x < 0) {
                pos.x = 0;
            }
            if (pos.y > Screen.height) {
                pos.y = Screen.height;
            }
            _container.transform.localPosition = UIUtil.BottomLeftToCenter(pos);
        }
    }
}