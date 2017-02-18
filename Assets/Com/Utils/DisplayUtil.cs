using Assets.Scripts.Com.Managers;
using Assets.Scripts.Com.UI.Base;
using MingUI.Com.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Com.Utils {
    public static class DisplayUtil {
        public static Transform[] getChildList(Transform tar) {
            return tar.GetComponentsInChildren<Transform>(true);
        }

        public static Transform GetChildByName(Transform tar, string name, bool inActive = true) {
            if (tar != null) {
                Transform[] tarList = tar.GetComponentsInChildren<Transform>(inActive);
                if (tarList != null) {
                    foreach (Transform t in tarList) {
                        if (t.name == name) {
                            return t;
                        }
                    }
                }
            }
            return null;
        }

        public static GameObject getChildObjByName(Transform tar, string name) {
            Transform t = GetChildByName(tar, name);
            if (t != null) {
                return t.gameObject;
            }
            return null;
        }

        public static Component getComponentByName(GameObject tar, string name) {
            Transform[] tarList = tar.transform.GetComponentsInChildren<Transform>();
            foreach (Transform t in tarList) {
                Component comp = t.gameObject.GetComponent(name);
                if (comp != null) {
                    return comp;
                }
            }
            return null;
        }

        public static List<Component> GetComponentsByName(GameObject tar, string name) {
            Transform[] tarList = tar.transform.GetComponentsInChildren<Transform>();
            List<Component> list = null;
            foreach (Transform t in tarList) {
                Component comp = t.gameObject.GetComponent(name);
                if (comp != null) {
                    if (list == null) {
                        list = new List<Component>();
                    }
                    list.Add(comp);
                }
            }
            return list;
        }

        public static List<Component> getComponentByType(GameObject tar, Type type, bool inActive = true) {
            List<Component> list = null;
            Component comp = tar.GetComponent(type);
            Transform[] tarList = tar.GetComponentsInChildren<Transform>(inActive);
            for (int i = 0, len = tarList.Length; i < len; i++) {
                Transform t = tarList[i];
                comp = t.gameObject.GetComponent(type);
                if (comp != null) {
                    if (list == null) {
                        list = new List<Component>();
                    }
                    list.Add(comp);
                }
            }
            return list;
        }

        public static List<T> GetComponentByType<T>(GameObject tar, bool inActive = true) where T : Component {
            List<T> list = null;
            T comp = tar.GetComponent<T>();
            Transform[] tarList = tar.GetComponentsInChildren<Transform>(inActive);
            for (int i = 0, len = tarList.Length; i < len; i++) {
                Transform t = tarList[i];
                comp = t.gameObject.GetComponent<T>();
                if (comp != null) {
                    if (list == null) {
                        list = new List<T>();
                    }
                    list.Add(comp);
                }
            }
            return list;
        }

        public static int getTrisAllByObj(GameObject tar) {
            int tris = 0;
            Transform[] tarList = tar.GetComponentsInChildren<Transform>();
            foreach (Transform t in tarList) {
                MeshFilter mF = t.gameObject.GetComponent("MeshFilter") as MeshFilter;
                if (mF != null) {
                    tris += mF.mesh.vertexCount;
                }
            }
            return tris;
        }

        public static MeshFilter GetMeshFilterByObj(GameObject tar) {
            MeshFilter[] tarList = tar.GetComponentsInChildren<MeshFilter>();
            return tarList[0];
        }

        public static void SetLightMap(GameObject obj, int index, Vector4 offset) {
            List<Component> renderers = getComponentByType(obj, typeof(Renderer)) as List<Component>;
            if (renderers != null && (renderers.Count > 0)) {
                for (int i = 0; i < renderers.Count; i++) {
                    Renderer renderer = renderers[i] as Renderer;
                    if (renderer != null) {
                        renderer.lightmapIndex = index;
                        renderer.lightmapScaleOffset = offset;
                        for (var j = 0; j < renderer.materials.Length; j++) {
                            renderer.materials[j].shader = FuncUtil.FindShader(renderer.materials[j].shader.name);
                        }
                    }
                }
            } else {
                Terrain t = obj.GetComponent<Terrain>();
                if (t != null) {
                    t.lightmapIndex = index;
                    t.lightmapScaleOffset = offset;
                }
            }
        }

        public static void ResetShader(GameObject obj) {
            List<Component> renderers = getComponentByType(obj, typeof(Renderer));
            if (renderers != null && (renderers.Count > 0)) {
                for (int i = 0; i < renderers.Count; i++) {
                    Renderer renderer = renderers[i] as Renderer;
                    for (var j = 0; j < renderer.materials.Length; j++) {
                        renderer.materials[j].shader = FuncUtil.FindShader(renderer.materials[j].shader.name);
                    }
                }
            }
        }

        public static void SetShadows(GameObject obj, bool castShadows, bool receiveShadows) {
            List<Component> renderers = getComponentByType(obj, typeof(Renderer)) as List<Component>;
            if (renderers != null && (renderers.Count > 0)) {
                for (int i = 0; i < renderers.Count; i++) {
                    MeshRenderer renderer = renderers[i] as MeshRenderer;
                    if (renderer != null) {
                        renderer.castShadows = castShadows;
                        renderer.receiveShadows = receiveShadows;
                    }
                }
            }
        }

        //可以把gameobject上面的脚本暂停的方法
        public static void SetBehaviourEnable(GameObject tar, Boolean isEnable) {
            Component[] list = tar.GetComponents(typeof(MonoBehaviour));
            for (int i = 0; i < list.Length; i++) {
                MonoBehaviour behaviour = list[i] as MonoBehaviour;
                if (behaviour != null) behaviour.enabled = isEnable;
            }
        }

        public static void SetData(this GameObject go, object data){
            DisplayBehaviour vo = go.GetComponent<DisplayBehaviour>() ?? go.AddComponent<DisplayBehaviour>();
            vo.data = data;
        }

        public static object GetData(this GameObject go){
            DisplayBehaviour vo = go.GetComponent<DisplayBehaviour>();
            if (vo != null){
                return vo.data;
            }
            return null;
        }

        public static void SetDisplayObj(this GameObject go,ref DisplayObj disObj){
            DisplayBehaviour vo = go.GetComponent<DisplayBehaviour>() ?? go.AddComponent<DisplayBehaviour>();
            vo.disObj = disObj;
        }

        public static DisplayObj GetDisplayObj(this GameObject go){
            DisplayBehaviour vo = go.GetComponent<DisplayBehaviour>();
            if (vo != null){
                return vo.disObj;
            }
            return null;
        }

        public static DisplayObj GetDisPlayObj(Collider collider){
            if (collider != null){
                if (collider.transform != null && collider.transform.parent != null){
                    return collider.transform.parent.gameObject.GetDisplayObj();
                }
            }
            return null;
        }

        public static void AddParentDepth(int parentDepth, GameObject go){
            UIWidget[] subWidgets = go.GetComponentsInChildren<UIWidget>(true);
            for (int i = 0; i < subWidgets.Length; i++){
                subWidgets[i].depth += parentDepth;
            }
        }

        public static void AddTip(this GameObject gameObj, string tip) {
            EventUtil.AddHover(gameObj, (o, b) => {
                if (b) {
                    ToolTipManager.Show(tip);
                } else {
                    ToolTipManager.Hide();
                }
            });
        }
    }
}