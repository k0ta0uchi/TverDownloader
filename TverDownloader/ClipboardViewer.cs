using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace TverDownloader
{
    public class ClipboardEventArgs : EventArgs
    {
        private string text;

        public string Text
        {
            get { return this.text; }
        }

        public ClipboardEventArgs(string str)
        {
            this.text = str;
        }
    }

    public delegate void cbEventHandler(object sender, ClipboardEventArgs ev);

    [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
    public class ClipboardViewer : NativeWindow
    {
        [DllImport("user32")]
        public static extern IntPtr SetClipboardViewer(IntPtr hWndNewViewer);

        [DllImport("user32")]
        public static extern bool ChangeClipboardChain(IntPtr hWndRemove, IntPtr hWndNewNext);

        [DllImport("user32")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        private const int WM_DRAWCLIPBOARD = 0x0308;
        private const int WM_CHANGECBCHAIN = 0x030D;
        private IntPtr nextHandle;

        private Form parent;
        public event cbEventHandler ClipboardHandler;

        public ClipboardViewer(Form f)
        {
            f.HandleCreated += new EventHandler(this.OnHandleCreated);
            f.HandleDestroyed += new EventHandler(this.OnHandleDestroyed);
            this.parent = f;
        }

        internal void OnHandleCreated(object sender, EventArgs e)
        {
            AssignHandle(((Form)sender).Handle);
            // ビューワを登録
            nextHandle = SetClipboardViewer(this.Handle);
        }

        internal void OnHandleDestroyed(object sender, EventArgs e)
        {
            // ビューワを解除
            bool sts = ChangeClipboardChain(this.Handle, nextHandle);
            ReleaseHandle();
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_DRAWCLIPBOARD:
                    if (Clipboard.ContainsText())
                    {
                        // クリップボードの内容がテキストの場合のみ
                        if (ClipboardHandler != null)
                        {
                            // クリップボードの内容を取得してハンドラを呼び出す
                            ClipboardHandler(this, new ClipboardEventArgs(Clipboard.GetText()));
                        }
                    }
                    if ((int)nextHandle != 0)
                        SendMessage(nextHandle, m.Msg, m.WParam, m.LParam);
                    break;

                // クリップボード・ビューワ・チェーンが更新された
                case WM_CHANGECBCHAIN:
                    if (m.WParam == nextHandle)
                    {
                        nextHandle = (IntPtr)m.LParam;
                    }
                    else if ((int)nextHandle != 0)
                        SendMessage(nextHandle, m.Msg, m.WParam, m.LParam);
                    break;
            }
            base.WndProc(ref m);
        }
    }
}
