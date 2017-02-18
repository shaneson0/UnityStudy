using System.Reflection.Emit;
using Assets.Scripts.Com.MingUI;
using UnityEngine;
using System.Collections.Generic;
using System.Text;

namespace Assets.Scripts.Com.MingUI{
    public class CVText : MonoBehaviour{
        public UILabel Label;
        public CScrollBar Bar;
        public int MaxParagraph = 50;

        protected class Paragraph{
            public string text; // Original text
            public string[] lines; // Split lines
        }

        protected char[] mSeparator = new char[]{'\n'};
        protected BetterList<Paragraph> mParagraphs = new BetterList<Paragraph>();
        protected float mScroll = 0f;
        protected int mTotalLines = 0;
        protected int mLastWidth = 0;
        protected int mLastHeight = 0;


        private void Start(){
            Label.overflowMethod = UILabel.Overflow.ClampContent;
            Label.pivot = UIWidget.Pivot.TopLeft;
            Bar.OnChangeFun = OnScrollValueChange;
            UIEventListener.Get(Label.gameObject).onScroll = OnMouseWheel;
            scrollValue = 0f;
        }

        private void OnMouseWheel(GameObject go, float delta){
            float v = Bar.value;
            v += delta*-0.8f;
            if (v < 0){
                v = 0;
            }
            else if (v > 1){
                v = 1;
            }
            Bar.value = v;
        }

        private void OnScrollValueChange(GameObject go, float v){
            mScroll = v;
            UpdateVisibleText();
        }

        private void Update(){
            if (isValid){
                if (Label.width != mLastWidth || Label.height != mLastHeight){
                    mLastWidth = Label.width;
                    mLastHeight = Label.height;
                    Rebuild();
                }
            }
        }

        public bool isValid{
            get { return Label != null && (Label.bitmapFont != null || Label.ambigiousFont != null); }
        }

        /// <summary>
        /// Relative (0-1 range) scroll value, with 0 being the oldest entry and 1 being the newest entry.
        /// </summary>
        public float scrollValue{
            get { return mScroll; }
            set{
                value = Mathf.Clamp01(value);

                if (isValid && mScroll != value){
                    if (Bar != null){
                        Bar.value = value;
                    }
                    else{
                        mScroll = value;
                        UpdateVisibleText();
                    }
                }
            }
        }

        /// <summary>
        /// Height of each line.
        /// </summary>
        protected float lineHeight{
            get { return (Label != null) ? Label.fontSize + Label.spacingY : 20f; }
        }

        /// <summary>
        /// Height of the scrollable area (outside of the visible area's bounds).
        /// </summary>
        protected int scrollHeight{
            get{
                if (!isValid) return 0;
                int maxLines = Mathf.FloorToInt((float) Label.height/lineHeight);
                return Mathf.Max(0, mTotalLines - maxLines);
            }
        }

        public void Clear(){
            mParagraphs.Clear();
            UpdateVisibleText();
        }


        public void OnScroll(float val){
            int sh = scrollHeight;

            if (sh != 0){
                val *= lineHeight;
                scrollValue = mScroll - val/sh;
            }
        }


        /// <summary>
        /// Delegate function called when the scroll bar's value changes.
        /// </summary>
        private void OnScrollBar(){
            mScroll = Bar.value;
            UpdateVisibleText();
        }

        /// <summary>
        /// Add a new paragraph.
        /// </summary>
        public void Add(string text){
            Add(text, true);
        }

        /// <summary>
        /// Add a new paragraph.
        /// </summary>
        protected void Add(string text, bool updateVisible){
            Paragraph ce = null;

            if (mParagraphs.size < MaxParagraph){
                ce = new Paragraph();
            }
            else{
                ce = mParagraphs[0];
                mParagraphs.RemoveAt(0);
            }

            ce.text = text;
            mParagraphs.Add(ce);
            Rebuild();
        }

        /// <summary>
        /// Rebuild the visible text.
        /// </summary>
        protected void Rebuild(){
            if (isValid){
                // Although we could simply use UILabel.Wrap, it would mean setting the same data
                // over and over every paragraph, which is not ideal. It's faster to only do it once
                // and then do wrapping ourselves in the 'for' loop below.
                Label.UpdateNGUIText();
                NGUIText.rectHeight = 1000000;
                mTotalLines = 0;
                bool success = true;

                for (int i = 0; i < mParagraphs.size; ++i){
                    string final;
                    Paragraph p = mParagraphs.buffer[i];

                    if (NGUIText.WrapText(p.text, out final)){
                        p.lines = final.Split('\n');
                        mTotalLines += p.lines.Length;
                    }
                    else{
                        success = false;
                        break;
                    }
                }

                // Recalculate the total number of lines
                mTotalLines = 0;

                if (success){
                    for (int i = 0, imax = mParagraphs.size; i < imax; ++i)
                        mTotalLines += mParagraphs.buffer[i].lines.Length;
                }

                // Update the bar's visibility and size
                if (Bar != null){
                    Bar.gameObject.SetActive(scrollHeight !=0);
                    if (Bar.gameObject.activeSelf == true) {
                        Bar.BarSize = (mTotalLines == 0) ? 1f : 1f - (float)scrollHeight / mTotalLines;
                    }
                }

                // Update the visible text
                UpdateVisibleText();
            }
        }

        protected void UpdateVisibleText(){
            if (isValid){
                if (mTotalLines == 0){
                    Label.text = "";
                    return;
                }

                int maxLines = Mathf.FloorToInt((float) Label.lblHeight/lineHeight);
                int sh = Mathf.Max(0, mTotalLines - maxLines);
                int offset = Mathf.RoundToInt(mScroll*sh);
                if (offset < 0) offset = 0;

                StringBuilder final = new StringBuilder();

                for (int i = 0, imax = mParagraphs.size; maxLines > 0 && i < imax; ++i){
                    Paragraph p = mParagraphs.buffer[i];

                    for (int b = 0, bmax = p.lines.Length; maxLines > 0 && b < bmax; ++b){
                        string s = p.lines[b];

                        if (offset > 0){
                            --offset;
                        }
                        else{
                            if (final.Length > 0) final.Append("\n");
                            final.Append(s);
                            --maxLines;
                        }
                    }
                }
                Label.text = final.ToString();
            }
        }
    }
}