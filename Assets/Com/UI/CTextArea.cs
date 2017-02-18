using System;
namespace Assets.Scripts.Com.MingUI {
    public class CTextArea : UISprite {
        public CTextInput input;
        private UIWidget txtW;
        public CCanvas canvas;
        public Action textChangeFun;
        public bool isScrollToBottom = true;
        private int labelHeight = 0;
        protected override void OnStart() {
            input.onChange.Add(new EventDelegate(OnTextChange));
            txtW = input.gameObject.GetComponent<UIWidget>();
            if (txtW != null) txtW.height = height;
        }

        private void OnTextChange() {
            if (textChangeFun != null) {
                textChangeFun.DynamicInvoke();
            }
            if (labelHeight != input.label.lblHeight) {
                CheckHeight();
                labelHeight = (int)input.label.lblHeight;
                if (txtW != null) txtW.height = Math.Max(width, labelHeight - (int)input.label.transform.localPosition.y + 5);
            }
        }

        private void CheckHeight() {
            canvas.CalculateHeight((int)(input.label.lblHeight));
            if (canvas.Bar.gameObject.activeSelf && isScrollToBottom) {
                canvas.Bar.value = 1;
            }
        }

        public string value {
            get { return input.value; }
            set { input.value = value; }
        }

        public int MaxChar {
            get { return input.MaxChar; }
            set { input.MaxChar = value; }
        }

        public bool isSelected {
            get { return input.isSelected; }
            set { input.isSelected = value; }
        }
    }
}
