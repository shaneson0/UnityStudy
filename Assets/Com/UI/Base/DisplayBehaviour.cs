using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Com.UI.Base{
    public class DisplayBehaviour : MonoBehaviour{
        public DisplayObj disObj { get; set; }
        public object data { get; set; } //存取普通数据，放在这不放DisplayObj是为了避免有的go不说DisplayObj

        private void OnBecameVisible(){
            if (disObj != null){
                disObj.OnBecameVisible();
            }
        }

        private void OnBecameInvisible(){
            if (disObj != null){
                disObj.OnBecameInvisible();
            }
        }

        private void OnEnable(){
            if (disObj != null){
                disObj.OnEnable();
            }
        }

        private void OnDisable(){
            if (disObj != null){
                disObj.OnDisable();
            }
        }
      
    }
}