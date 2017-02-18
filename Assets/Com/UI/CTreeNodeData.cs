using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Com.MingUI {
    public class CTreeNodeData {
        public CTreeNodeData parent;
        public List<CTreeNodeData> child;
        public bool isOpen;
    }
}
