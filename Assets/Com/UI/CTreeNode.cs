using Assets.Scripts.Com.UI.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Com.MingUI {
    public class CTreeNode : DisplayObj {
        protected bool _isOpen = false;
        private CTreeNodeData _data;
        public enum NodeType {
            Branch,
            Leaf,
        }
        private NodeType _type = NodeType.Branch;
        public NodeType type {
            set {
                _type = value;
            }
            get {
                return _type;
            }
        }
        public virtual CTreeNodeData data {
            set { _data = value; }
            get { return _data; }
        }

        public virtual bool isOpen {
            set {
                _isOpen = value;
            }
            get {
                return _isOpen;
            }
        }
    }
}
