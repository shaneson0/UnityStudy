using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Com.MingUI
{
    public class TestStart : MonoBehaviour{
        private void Awake(){
            Debug.Log(gameObject.name + "  Awake");
        }
        private void Start(){
            Debug.Log(gameObject.name + "  Start");
            UIEventListener.Get(gameObject).onPress = OnPressi;
            GameObject s = transform.GetChild(0).gameObject;
            GameObject.Destroy(s);
            s.transform.parent = gameObject.transform;
        }

        private void OnPressi(GameObject go,bool isPress){
            if (isPress){
                Debug.Log(go.name + isPress);
            }
        }

        private void OnDrag(Vector2 delta){
//            Debug.Log(delta);
            transform.localPosition += new Vector3(delta.x, delta.y, 0);
        }
    }
}