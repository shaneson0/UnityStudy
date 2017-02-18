using Assets.Scripts.Com.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Assets.Scripts.Com.MingUI {
    public class C3DTexture : UITexture {
        private Action<object> _clickFun;
        private Camera _useCamera;
        public void AddClick(Action<object> value) {
            if (_clickFun != null) {
                Debug.LogError("C3DTexture only support one function");
                return;
            }
            EventUtil.AddClick(gameObject, OnClickTexture, "");
            _clickFun = value;
        }


        private void OnClickTexture(object arg) {
            if (_useCamera == null) {
                return;
            }
            Vector3 mouse = Input.mousePosition;
            mouse.x = mouse.x / Screen.width;
            mouse.y = mouse.y / Screen.height;
            mouse.z = 0;
            mouse = UICamera.currentCamera.ViewportToWorldPoint(mouse);
            Vector3 pos = transform.position;
            pos.x = (mouse.x - pos.x) / UIUtil.UIRootScale;
            pos.y = (pos.y - mouse.y) / UIUtil.UIRootScale;
            pos.y = height - pos.y;
            pos.z = 0;

            Vector3 pos2 = pos;
            pos2.y = (pos.y / height) * 768 + (Screen.height - 768) / 2;
            pos2.x = (pos.x / width) * 1024 + (Screen.width - 1024) / 2;


            Ray ray = _useCamera.ScreenPointToRay(pos2);
            RaycastHit hit;
            LayerMask layerMask = 1 << _useCamera.gameObject.layer;
            Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow);
            if (Physics.Raycast(ray, out hit, 1000, layerMask)) {
                if (hit.collider != null) {
                    if (hit.collider.transform != null && hit.collider.transform.parent != null) {
                        object data = hit.collider.transform.parent.gameObject.GetData();
                        if (_clickFun != null) {
                            _clickFun(data);
                        }
                    }
                }
            }
        }
        public Camera useCamera {
            set {
                _useCamera = value;
                mainTexture = _useCamera.targetTexture;
            }
            get {
                return _useCamera;
            }
        }

        public float fieldOfView {
            set {
                if (_useCamera) {
                    _useCamera.fieldOfView = value;
                }
            }
            get {
                if (_useCamera) {
                    return _useCamera.fieldOfView;
                }
                return 0;
            }
        }

        public Camera SetCamrea {
            set {
                _useCamera = value;
            }
        }


        public GameObject GetTranUnderMouse() {
            Vector3 mouse = Input.mousePosition;
            mouse.x = mouse.x / Screen.width;
            mouse.y = mouse.y / Screen.height;
            mouse.z = 0;
            mouse = UICamera.currentCamera.ViewportToWorldPoint(mouse);
            Vector3 nowScale = UIUtil.ScaleInUIRoot(transform);
            Vector3 pos = transform.position;
            pos.x = ((mouse.x - pos.x) / UIUtil.UIRootScale) / nowScale.x;
            pos.y = ((mouse.y - pos.y) / UIUtil.UIRootScale) / nowScale.y;
            pos.z = 0;


            Vector3 pos2 = pos;
            pos2.y = ((height + pos.y) / height) * _useCamera.targetTexture.height;
            pos2.x = (pos.x / width) * _useCamera.targetTexture.width;

            Ray ray = _useCamera.ScreenPointToRay(pos2);
            RaycastHit hit;
            LayerMask layerMask = 1 << _useCamera.gameObject.layer;
            Debug.DrawRay(ray.origin, ray.direction * 10000, Color.yellow);
            if (Physics.Raycast(ray, out hit, 1000, layerMask)) {
                if (hit.collider != null) {
                    if (hit.collider.transform != null && hit.collider.transform.parent != null) {
                        return hit.collider.transform.parent.gameObject;
                    }
                }
            }
            return null;
        }
        private List<GameObject> rttObjectList = new List<GameObject>();
        //设置RTT的GameObject
        public void SetGameObj(GameObject rttObj, Vector3 posOffset, Quaternion rot) {
            rttObj.layer = useCamera.gameObject.layer;
            Transform[] tranList = DisplayUtil.getChildList(rttObj.transform);
            for (int i = 0; i < tranList.Length; i++)
                tranList[i].gameObject.layer = rttObj.layer;
            rttObj.transform.parent = useCamera.transform;
            rttObj.transform.localPosition = posOffset;
            rttObj.transform.localRotation = rot;
            this.mainTexture = useCamera.targetTexture;
            if (rttObjectList.Contains(rttObj) == false) {
                rttObjectList.Add(rttObj);
            }
        }

        public void RemoveAllObject() {
            for (int i = 0, len = rttObjectList.Count; i < len; i++) {
                if (rttObjectList[i] == null) continue;
                object obj = rttObjectList[i].GetData();
                if (obj != null) {
                    MethodBase m = obj.GetType().GetMethod("Remove");
                    if (m != null) {
                        m.Invoke(obj, null);
                    }
                }
            }
        }
    }
}
