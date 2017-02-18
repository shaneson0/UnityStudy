using Assets.Scripts.Com.MingUI;
using Assets.Scripts.Com.UI.Base;
using MingUI.Com.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Com.Utils {
    public class EventUtil {
        private static Dictionary<GameObject, Dictionary<Action<object>, UIEventListener.VoidDelegate>> clickDic = new Dictionary<GameObject, Dictionary<Action<object>, UIEventListener.VoidDelegate>>();
        private static Dictionary<GameObject, Dictionary<Action<object, bool>, UIEventListener.BoolDelegate>> pressDic = new Dictionary<GameObject, Dictionary<Action<object, bool>, UIEventListener.BoolDelegate>>();
        private static Dictionary<GameObject, Dictionary<Action<object, bool>, UIEventListener.BoolDelegate>> hoverDic = new Dictionary<GameObject, Dictionary<Action<object, bool>, UIEventListener.BoolDelegate>>();
        /// <summary>
        /// Click
        /// </summary>
        /// <param name="go"></param>
        /// <param name="handler"></param>
        /// <param name="arg"></param>
        /// <returns></returns>

        public static UIEventListener.VoidDelegate AddClick(DisplayObj display, Action<object> handler, object arg = null) {
            return AddClick(display.go, handler, arg);
        }
        public static UIEventListener.VoidDelegate AddClick(GameObject go, Action<object> handler, object arg = null) {
            if (clickDic.ContainsKey(go)) {
                Dictionary<Action<object>, UIEventListener.VoidDelegate> dic = clickDic[go];
                if (dic.ContainsKey(handler)) {
                    return null;
                }
            } else {
                clickDic.Add(go, new Dictionary<Action<object>, UIEventListener.VoidDelegate>());
                if (go.name != "PanelBG" && go.GetComponent<UIWidget>() != null && go.GetComponent<CButton>() == null) {
                    var label = go.GetComponent<UILabel>();
                    if (label == null) {
                        AddPress(go, (o, b) => FuncUtil.SetCursor(b ? "CURSOR_CLICK_DOWN" : "CURSOR_NORMAL"));
                        AddHover(go, (o, b) => 
                            FuncUtil.SetCursor(b ? "CURSOR_CLICK_OVER" : "CURSOR_NORMAL"));
                    } else {
                        AddHover(go, (o, b) => { if (!b) FuncUtil.SetCursor("CURSOR_NORMAL"); });
                        AddPress(go, OnLabelPress, label);
                    }
                }
            }
            UIEventListener.VoidDelegate de = delegate(GameObject go1) {
                handler.DynamicInvoke(arg);
            };
            UIEventListener.Get(go).onClick += de;
            clickDic[go].Add(handler, de);
            return de;
        }

        public static void RemoveClick(GameObject go, Action<object> handler) {
            if (clickDic.ContainsKey(go)) {
                Dictionary<Action<object>, UIEventListener.VoidDelegate> dic = clickDic[go];
                if (dic.ContainsKey(handler) == false) { return; }
                UIEventListener.Get(go).onClick -= dic[handler];
                dic.Remove(handler);
            }
        }

        /// <summary>
        /// DoubleClick
        /// </summary>
        /// <param name="go"></param>
        /// <param name="handler"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static UIEventListener.VoidDelegate AddDoubleClick(DisplayObj display, Action<object> handler, object arg = null) {
            return AddDoubleClick(display.go, handler, arg);
        }

        public static UIEventListener.VoidDelegate AddDoubleClick(GameObject go, Action<object> handler, object arg = null) {
            UIEventListener.VoidDelegate de = delegate(GameObject go1) {
                handler.DynamicInvoke(arg);
            };
            UIEventListener.Get(go).onDoubleClick = de;
            return de;
        }

        public static void RemoveDoubleClick(GameObject go, UIEventListener.VoidDelegate handler) {
            UIEventListener.Get(go).onDoubleClick -= handler;
        }
        /// <summary>
        /// Press
        /// </summary>
        /// <param name="display"></param>
        /// <param name="handler"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static UIEventListener.BoolDelegate AddPress(DisplayObj display, Action<object, bool> handler, object arg = null) {
            return AddPress(display.go, handler, arg);
        }

        public static UIEventListener.BoolDelegate AddPress(GameObject go, Action<object, bool> handler, object arg = null) {
            if (pressDic.ContainsKey(go)) {
                Dictionary<Action<object, bool>, UIEventListener.BoolDelegate> dic = pressDic[go];
                if (dic.ContainsKey(handler)) {
                    return null;
                }
            } else {
                pressDic.Add(go, new Dictionary<Action<object, bool>, UIEventListener.BoolDelegate>());
            }
            if (arg == null) {
                arg = go;
            }
            UIEventListener.BoolDelegate de = delegate(GameObject go1, bool pressed) {
                handler.DynamicInvoke(arg, pressed);
            };
            UIEventListener.Get(go).onPress += de;
            pressDic[go].Add(handler, de);
            return de;
        }

        public static void RemovePress(GameObject go, Action<object, bool> handler) {
            //UIEventListener.Get(go).onPress -= handler;
            if (pressDic.ContainsKey(go)) {
                Dictionary<Action<object, bool>, UIEventListener.BoolDelegate> dic = pressDic[go];
                if (dic.ContainsKey(handler) == false) { return; }
                UIEventListener.Get(go).onPress -= dic[handler];
                dic.Remove(handler);
            }
        }


        //public static UIEventListener.VoidDelegate AddMouseDown(GameObject go, object obj, Action<object> handler, object arg) {
        //    UIEventListener.VoidDelegate de = delegate(GameObject go1) {
        //        handler.DynamicInvoke(arg);
        //    };
        //    UIEventListener.Get(go).onMouseDown = de;
        //    return de;
        //}

        //public static void RemoveMouseDown(GameObject go, UIEventListener.VoidDelegate handler) {
        //    UIEventListener.Get(go).onMouseDown -= handler;
        //}

        /// <summary>
        /// MouseUp
        /// </summary>
        /// <param name="display"></param>
        /// <param name="handler"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static UIEventListener.VoidDelegate AddMouseUp(DisplayObj display, Action<object> handler, object arg = null) {
            return AddMouseUp(display.go, handler, arg);
        }
        public static UIEventListener.VoidDelegate AddMouseUp(GameObject go, Action<object> handler, object arg = null) {
            if (arg == null) {
                arg = go;
            }
            UIEventListener.VoidDelegate de = delegate(GameObject go1) {
                handler.DynamicInvoke(arg);
            };
            UIEventListener.Get(go).onMouseUp = de;
            return de;
        }

        public static void RemoveMouseUp(GameObject go, UIEventListener.VoidDelegate handler) {
            UIEventListener.Get(go).onMouseUp -= handler;
        }


        /// <summary>
        /// Hover
        /// </summary>
        /// <param name="display"></param>
        /// <param name="handler"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static UIEventListener.BoolDelegate AddHover(DisplayObj display, Action<object, bool> handler, object arg = null) {
            return AddHover(display.go, handler, arg);
        }

        public static UIEventListener.BoolDelegate AddHover(GameObject go, Action<object, bool> handler, object arg = null) {
            if (hoverDic.ContainsKey(go)) {
                Dictionary<Action<object, bool>, UIEventListener.BoolDelegate> dic = hoverDic[go];
                if (dic.ContainsKey(handler)) {
                    return null;
                }
            } else {
                hoverDic.Add(go, new Dictionary<Action<object, bool>, UIEventListener.BoolDelegate>());
            }
            if (arg == null) {
                arg = go;
            }
            UIEventListener.BoolDelegate de = delegate(GameObject go1, bool isHover) {
                handler.DynamicInvoke(arg, isHover);
            };
            UIEventListener.Get(go).onHover += de;
            hoverDic[go].Add(handler, de);
            return de;
        }

        public static void RemoveHover(GameObject go, Action<object, bool> handler) {
            //UIEventListener.Get(go).onHover -= handler;
            if (hoverDic.ContainsKey(go)) {
                Dictionary<Action<object, bool>, UIEventListener.BoolDelegate> dic = hoverDic[go];
                if (dic.ContainsKey(handler) == false) { return; }
                UIEventListener.Get(go).onHover -= dic[handler];
                dic.Remove(handler);
            }
        }

        /// <summary>
        /// onDragStart
        /// </summary>
        /// <param name="go"></param>
        /// <param name="handler"></param>
        /// <param name="arg"></param>
        /// <returns></returns>

        public static UIEventListener.VoidDelegate AddDragStart(DisplayObj display, Action<object> handler, object arg = null) {
            return AddDragStart(display.go, handler, arg);
        }
        public static UIEventListener.VoidDelegate AddDragStart(GameObject go, Action<object> handler, object arg = null) {
            UIEventListener.VoidDelegate de = delegate(GameObject go1) {
                handler.DynamicInvoke(arg);
            };
            UIEventListener.Get(go).onDragStart = de;
            return de;
        }

        public static void RemoveDragStar(GameObject go, UIEventListener.VoidDelegate handler) {
            UIEventListener.Get(go).onDragStart -= handler;
        }

        public static UIEventListener.VoidDelegate AddLink(UILabel lbl, Action<object> handler) {
            if (clickDic.ContainsKey(lbl.gameObject)) {
                Dictionary<Action<object>, UIEventListener.VoidDelegate> dic = clickDic[lbl.gameObject];
                if (dic.ContainsKey(handler)) {
                    return null;
                }
            } else {
                clickDic.Add(lbl.gameObject, new Dictionary<Action<object>, UIEventListener.VoidDelegate>());
                AddPress(lbl.gameObject, OnLabelPress, lbl);
                AddHover(lbl.gameObject, (o, b) => { if (!b) FuncUtil.SetCursor("CURSOR_NORMAL"); });
            }
            UIEventListener.VoidDelegate de = delegate(GameObject go1) {
                string url = lbl.GetUrlAtPosition(UICamera.lastHit.point);
                if (url != null) {
                    handler.DynamicInvoke(url);
                }
            };
            UIEventListener.Get(lbl.gameObject).onClick += de;
            clickDic[lbl.gameObject].Add(handler, de);
            return de;
        }

        private static void OnLabelPress(object obj, bool isPress) {
            if (!isPress) { FuncUtil.SetCursor("CURSOR_NORMAL"); return; }
            var label = obj as UILabel;
            if (label == null) return;
            string url = label.GetUrlAtPosition(UICamera.lastHit.point);
            if (url != null) {
                FuncUtil.SetCursor("CURSOR_CLICK_DOWN");
            } else {
                FuncUtil.SetCursor("CURSOR_NORMAL");
            }
        }

        public static bool HasRegisterClickFunc(UILabel label) {
            return clickDic.ContainsKey(label.gameObject);
        }

        public static bool HasRegisterClickFunc(GameObject gameObj) {
            return clickDic.ContainsKey(gameObj);
        }
    }
}
