using Assets.Scripts.Com.MingUI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MingUI.Com.Utils {
    public class FuncUtil {
        //获取资源
        public delegate UnityEngine.Object URLDelegate(string url, Type type);
        public delegate Font FontDelegate();

        //资源加载
        public delegate void LoaderDelegate(string url, Delegate onComplete, Action<bool, string, float> onProgress, Type resType, bool changeDispose, int level, params object[] args);

        private static URLDelegate GetAss;
        private static LoaderDelegate loaderHandler;

        private static Action<string> CanDisposeFun;//可以释放的资源
        private static Action<string> NoDisposeFun;//不让释放的资源
        private static FontDelegate getMFontFun;
        private static Action<string,string> showErrorFun;//错误弹窗
        private static Action<string> logFun;//
        private static Action<UnityEngine.Object, GameObject> uidepFun;//ui对象的依赖方法
        private static Action<UISprite> initUISprite;//ui
        private static Action<Image> initImage;//ui
        private static Action<BMFont,string> initUIFont;//初始化UIFont

        public static void SetGetUIAssFun(URLDelegate fun) {
            GetAss = fun;
        }

        public static UnityEngine.Object GetUIAssetByPath(string url, Type type = null) {
            return GetAss(url, type);
        }

        public static void SetLoadFun(LoaderDelegate fun) {
            loaderHandler = fun;
        }

        public static void Load(string url, Delegate OnCompCallBack, params object[] args) {
            loaderHandler(url, OnCompCallBack, null, null, false, 1, args);
        }

        //淡出提示
        private static Action<string> topTipFun;

        public static void SetAddTipFun(Action<string> fun) {
            topTipFun = fun;
        }

        public static void AddTip(string msg) {
            topTipFun(msg);
        }

        //获取shader
        public delegate Shader ShaderDelegate(string name);

        private static ShaderDelegate getShader;
        private static bool shaderInit;

        public static void SetShaderFunc(ShaderDelegate func) {
            getShader = func;
            shaderInit = true;
        }

        public static Shader FindShader(string name) {
            if (shaderInit) {
                return getShader(name);
            } else {
                return Shader.Find(name);
            }
        }

        public delegate List<UISpriteData> SpriteDataDelegate(string name);

        public static SpriteDataDelegate spriteDataDel;

        public static void SetSpriteData(SpriteDataDelegate action) {
            spriteDataDel = action;
        }

        public static List<UISpriteData> GetSpriteData(string name) {
            if (spriteDataDel != null) {
                return spriteDataDel(name);
            } else {
                return null;
            }
        }

        public static Action<string> setCursorFunc;

        public static void SetCursor(string name) {
            if (setCursorFunc != null) {
                setCursorFunc(name);
            }
        }

        public delegate bool IsUICursorFuncDelegate();

        public static IsUICursorFuncDelegate isUICursorFunc;

        public static bool IsUICursor() {
            return isUICursorFunc.Invoke();
        }

        //资源释放
        public static void SetCanDispose(Action<string> fun) {
            CanDisposeFun = fun;
        }

        public static void CanDispose(string path) {
            if (CanDisposeFun != null && string.IsNullOrEmpty(path) == false) {
                CanDisposeFun.Invoke(path);
            }
        }

        public static void SetNoDispose(Action<string> fun) {
            NoDisposeFun = fun;
        }

        public static void NoDispose(string path) {
            if (NoDisposeFun != null && string.IsNullOrEmpty(path) == false) {
                NoDisposeFun.Invoke(path);
            }
        }

        public static void SetMFontFun(FontDelegate fun) {
            getMFontFun = fun;
        }

        public static Font GetMFont() {
            if (getMFontFun != null) {
                return getMFontFun.Invoke();
            }
            return null;
        }

        public static void SetShowErrorFun(Action<string,string> fun) {
            showErrorFun = fun;
        }

        public static void ShowError(string msg,string detail = null) {
            if (showErrorFun != null) {
                showErrorFun.Invoke(msg, detail);
            }
        }

        public static void SetLogFun(Action<string> fun) {
            logFun = fun;
        }

        public static void WriteLog(string msg) {
            if (logFun != null) {
                logFun.Invoke(msg);
            }
        }

        public static void SetUIDepFun(Action<UnityEngine.Object, GameObject> fun) {
            uidepFun = fun;
        }

        public static void AddUIDep(UnityEngine.Object srcObj, GameObject insObj) {
            if (uidepFun != null) {
                uidepFun.Invoke(srcObj, insObj);
            }
        }

        public static void SetInitUISprite(Action<UISprite> fun) {
            initUISprite = fun;
        }

        public static void InitUISprite(UISprite sp) {
            if (initUISprite != null) {
                initUISprite.Invoke(sp);
            }
        }

        public static void SetInitImage(Action<Image> fun) {
            initImage = fun;
        }

        public static void InitImage(Image img) {
            if (initImage != null) {
                initImage.Invoke(img);
            }
        }

        public static void SetInitUIFont(Action<BMFont,string> fun) {
            initUIFont = fun;
        }

        public static void InitUIFont(BMFont font,string name) {
            if (initUIFont != null) {
                initUIFont.Invoke(font,name);
            }
        }
        
    }
}