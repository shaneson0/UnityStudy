using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Com.MingUI {
    public class UICameraUtil {
        static UICameraUtil() {
            UICamera.genericEventHandler = GameObject.Find("UI Root/Camera/genericEventHandler");
        }

        public static void AddGenericScroll(UIEventListener.FloatDelegate fun) {
            if (UICamera.genericEventHandler != null) {
                UIEventListener.Get(UICamera.genericEventHandler).onScroll += fun;
            }
        }

        public static void RemoveGenericScroll(UIEventListener.FloatDelegate fun) {
            if (UICamera.genericEventHandler != null) {
                UIEventListener e = UIEventListener.Get(UICamera.genericEventHandler);
                if (e != null) {
                    e.onScroll -= fun;
                }
            }
        }

        public static void AddGenericPress(UIEventListener.BoolDelegate fun) {
            if (UICamera.genericEventHandler != null) {
                UIEventListener.Get(UICamera.genericEventHandler).onPress += fun;
            }
        }

        public static void RemoveGenericPress(UIEventListener.BoolDelegate fun) {
            if (UICamera.genericEventHandler != null) {
                UIEventListener e = UIEventListener.Get(UICamera.genericEventHandler);
                if (e != null) {
                    e.onPress -= fun;
                }
            }
        }

        public static void AddGenericMouseUp(UIEventListener.VoidDelegate fun) {
            if (UICamera.genericEventHandler != null) {
                UIEventListener.Get(UICamera.genericEventHandler).onMouseUp += fun;
            }
        }

        public static void RemoveGenericMouseUp(UIEventListener.VoidDelegate fun) {
            if (UICamera.genericEventHandler != null) {
                UIEventListener e = UIEventListener.Get(UICamera.genericEventHandler);
                if (e != null) {
                    e.onMouseUp -= fun;
                }
            }
        }
    }
}