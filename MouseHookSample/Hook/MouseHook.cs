using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;


/* ������������ ����� MouseHook, ������� �������� �������� ��� API Mouse Hook � Windows. �� ��������� ������������� ������� ���� � ���������� � ������������ ��. ��� �������� ������� ����� ������:
 * 
 * �����������:
 * MouseHook() ������� ������ ������ MouseHook ��� ��������� ������� ���� ���� WH_MOUSE.
 * 
 * �������:
 * MouseDown, MouseUp, MouseMove, MouseDoubleClick - �������, ���������� ��� ��������������� ��������� ����. ������ �� ���� ������� �������� ��������������� �����, ������� ������ ���� 
 * ��������� �� ������� ����.
 * 
 * ������:
 * MouseProc(int code, IntPtr wParam, IntPtr lParam): ��� �������� �����, ������������ �������� SetWindowsHookEx. �� ������������ ������� ����, ��������������� ����� ����.
 * RaiseMouseHookEvent(IntPtr wParam, MouseHookEventArgs args): ����������, ����� ������� ���� ��������� � �������� ��������������� ������� ������ ������ MouseHook.
 * CrackHookMsg(IntPtr wParam, Win32.MOUSEHOOKSTRUCT hookStruct): �������� ������� ���������, ���������� � MouseProc, � ������������� �� � ������� .NET.
 * 
 * Win32 Imports:
 * 
 * ���� ���� �������� ��������� � ������ �� Win32 API, ������������ ��� ��������� ��������� ���� � ������ � ���������.
 * ���� ����� MouseHook ������� ��� ��������� � ��������� ������� ���� � .NET-�����������, �������� ������������ ��������� ����������� �������� � ����� �� �������� ������������ � �����. */

namespace Microsoft.Win32
{
    // ������������, ������������ ����, ������������ ��� ������������ ��������� (hit test) ��������� �� ������
    public enum HitTestCode : int
    {
        HTERROR = -2,            // ������
        HTTRANSPARENT = -1,      // ���������� �������
        HTNOWHERE = 0,           // ��� ���� ��������
        HTCLIENT = 1,            // ���������� �������
        HTCAPTION = 2,           // ��������� ����
        HTSYSMENU = 3,           // ��������� ���� ����
        HTGROWBOX = 4,           // ��������� �����
        HTSIZE = HTGROWBOX,      // ������� ��������� �������
        HTMENU = 5,              // ����
        HTHSCROLL = 6,           // �������������� ������ ���������
        HTVSCROLL = 7,           // ������������ ������ ���������
        HTMINBUTTON = 8,         // ������ "��������"
        HTMAXBUTTON = 9,         // ������ "����������"
        HTLEFT = 10,             // ����� ������� ����
        HTRIGHT = 11,            // ������ ������� ����
        HTTOP = 12,              // ������� ������� ����
        HTTOPLEFT = 13,          // ������� ����� ���� ����
        HTTOPRIGHT = 14,         // ������� ������ ���� ����
        HTBOTTOM = 15,           // ������ ������� ����
        HTBOTTOMLEFT = 16,       // ������ ����� ���� ����
        HTBOTTOMRIGHT = 17,      // ������ ������ ���� ����
        HTBORDER = 18,           // ������� ����
        HTREDUCE = HTMINBUTTON,  // ��� ���������� ����
        HTZOOM = HTMAXBUTTON,    // ��� ���������� ����
        HTSIZEFIRST = HTLEFT,    // ������ ��� ��������� �������
        HTSIZELAST = HTBOTTOMRIGHT,  // ��������� ��� ��������� �������
        HTOBJECT = 19,           // ������
        HTCLOSE = 20,            // ������ "�������"
        HTHELP = 21              // ������ "������"
    }

    // ���������� ������� ��� ������ MouseHook, ������� ������������� ������� ����
    public delegate void MouseHookEventHandler(object sender, MouseHookEventArgs e);

    // ����� MouseHook, ����������� LocalWindowsHook � ����������� ��������� IDisposable
    public class MouseHook : LocalWindowsHook, IDisposable
    {
        #region Construction
        // ����������� ������, ��������������� ��� ����
        public MouseHook() : base(HookType.WH_MOUSE)
        {
            // ������������� ����������� ������� ��������� ������
            m_filterFunc = new HookProc(this.MouseProc);
        }
        #endregion

        #region Disposal
        // ���������� ������ MouseHook
        ~MouseHook()
        {
            Dispose(false);
        }

        // ����� Dispose ��� ������������ ��������
        protected void Dispose(bool disposing)
        {
            if (IsInstalled)
                Uninstall();

            if (disposing)
                GC.SuppressFinalize(this);
        }

        // ���������� ���������� IDisposable ��� ������������ ��������
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion

        #region Events
        // �������, ����������� ��� ������� ������ ����
        public event MouseHookEventHandler MouseDown;
        protected void OnMouseDown(MouseHookEventArgs e)
        {
            if (MouseDown != null)
                MouseDown(this, e);
        }

        // �������, ����������� ��� ���������� ������ ����
        public event MouseHookEventHandler MouseUp;
        // �����, ���������� ������� MouseUp � ����������� MouseHookEventArgs
        protected void OnMouseUp(MouseHookEventArgs e)
        {
            if (MouseUp != null)
                MouseUp(this, e);
        }

        // �������, ����������� ��� �������� ����
        public event MouseHookEventHandler MouseMove;
        // �����, ���������� ������� MouseMove � ����������� MouseHookEventArgs
        protected void OnMouseMove(MouseHookEventArgs e)
        {
            if (MouseMove != null)
                MouseMove(this, e);
        }

        // �������, ����������� ��� ������� ������ ����
        public event MouseHookEventHandler MouseDoubleClick;
        // �����, ���������� ������� MouseDoubleClick � ����������� MouseHookEventArgs
        protected void OnMouseDoubleClick(MouseHookEventArgs e)
        {
            if (MouseDoubleClick != null)
                MouseDoubleClick(this, e);
        }
        #endregion

        #region Mouse Hook specific code

        // �����-���������� ������� ����, ���������� ��� ������ ������� ����
        protected int MouseProc(int code, IntPtr wParam, IntPtr lParam)
        {
            if (code < 0)
                return CallNextHookEx(m_hhook, code, wParam, lParam);

            if (code == Win32.HC_ACTION)
                RaiseMouseHookEvent(wParam, CrackHookMsg(wParam, (Win32.MOUSEHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(Win32.MOUSEHOOKSTRUCT))));

            return CallNextHookEx(m_hhook, code, wParam, lParam);
        }

        // ��������� �����, ���������� ��������������� ������� ���� � ����������� �� ����������� �������� wParam
        private void RaiseMouseHookEvent(IntPtr wParam, MouseHookEventArgs args)
        {
            // �������� �������� wParam ��� ����������� ���� ������� ����
            switch (wParam.ToInt32())
            {
                // �������� ���� � ����
                case Win32.WM_MOUSEMOVE:
                    OnMouseMove(args);
                    break;
                // �������� ���� �� ��������� ���������� ������� ����
                case Win32.WM_NCMOUSEMOVE:
                    // ��������� ����� ������������ �������
                    args.SetNonClient();
                    // ����� ������� �������� ���� �� ��������� ���������� ������� ����
                    OnMouseMove(args);
                    break;

                // ������� ������ ���� (������� ������)
                case Win32.WM_LBUTTONDOWN:
                case Win32.WM_RBUTTONDOWN:
                case Win32.WM_MBUTTONDOWN:
                case Win32.WM_XBUTTONDOWN:
                    // ����� ������� ������� ������ ����
                    OnMouseDown(args);
                    break;

                // ������� ������ ���� (������������ ������)
                case Win32.WM_NCLBUTTONDOWN:
                case Win32.WM_NCRBUTTONDOWN:
                case Win32.WM_NCMBUTTONDOWN:
                case Win32.WM_NCXBUTTONDOWN:
                    // ��������� ����� ������������ �������
                    args.SetNonClient();
                    // ����� ������� ������� ������ ���� �� ��������� ���������� ������� ����
                    OnMouseDown(args);
                    break;

                // ���������� ������ ���� (������� ������)
                case Win32.WM_LBUTTONUP:
                case Win32.WM_RBUTTONUP:
                case Win32.WM_MBUTTONUP:
                case Win32.WM_XBUTTONUP:
                    // ����� ������� ���������� ������ ����
                    OnMouseUp(args);
                    break;

                // ���������� ������ ���� (������������ ������)
                case Win32.WM_NCLBUTTONUP:
                case Win32.WM_NCRBUTTONUP:
                case Win32.WM_NCMBUTTONUP:
                case Win32.WM_NCXBUTTONUP:
                    // ��������� ����� ������������ �������
                    args.SetNonClient();
                    // ����� ������� ���������� ������ ���� �� ��������� ���������� ������� ����
                    OnMouseUp(args);
                    break;

                // ������� ������ ���� (������� ������)
                case Win32.WM_LBUTTONDBLCLK:
                case Win32.WM_RBUTTONDBLCLK:
                case Win32.WM_MBUTTONDBLCLK:
                case Win32.WM_XBUTTONDBLCLK:
                    // ����� ������� �������� ������ ������ ����
                    OnMouseDoubleClick(args);
                    break;

                // ������� ������ ���� (������������ ������)
                case Win32.WM_NCLBUTTONDBLCLK:
                case Win32.WM_NCRBUTTONDBLCLK:
                case Win32.WM_NCMBUTTONDBLCLK:
                case Win32.WM_NCXBUTTONDBLCLK:
                    // ��������� ����� ������������ �������
                    args.SetNonClient();
                    // ����� ������� �������� ������ ������ ���� �� ��������� ���������� ������� ����
                    OnMouseDoubleClick(args);
                    break;
            }
        }

        // ��������� �����, ������� ���������� ������ ����, ��������� �������, � ������� ������ MouseHookEventArgs �� ������ ���������� ����������
        private MouseHookEventArgs CrackHookMsg(IntPtr wParam, Win32.MOUSEHOOKSTRUCT hookStruct)
        {
            // ������������� ���������� ��� ����������� ������ ����
            MouseButtons button = MouseButtons.None;

            // �������� �������� wParam ��� ����������� ���� ������� ����
            switch (wParam.ToInt32())
            {
                // ������� ��� ����� ������ ����
                case Win32.WM_LBUTTONDBLCLK:
                case Win32.WM_LBUTTONDOWN:
                case Win32.WM_LBUTTONUP:
                case Win32.WM_NCLBUTTONDBLCLK:
                case Win32.WM_NCLBUTTONDOWN:
                case Win32.WM_NCLBUTTONUP:
                    button = MouseButtons.Left;
                    break;

                // ������� ��� ������ ������ ����
                case Win32.WM_RBUTTONDBLCLK:
                case Win32.WM_RBUTTONDOWN:
                case Win32.WM_RBUTTONUP:
                case Win32.WM_NCRBUTTONDBLCLK:
                case Win32.WM_NCRBUTTONDOWN:
                case Win32.WM_NCRBUTTONUP:
                    button = MouseButtons.Right;
                    break;

                // ������� ��� ������� ������ ����
                case Win32.WM_MBUTTONDBLCLK:
                case Win32.WM_MBUTTONDOWN:
                case Win32.WM_MBUTTONUP:
                case Win32.WM_NCMBUTTONDBLCLK:
                case Win32.WM_NCMBUTTONDOWN:
                case Win32.WM_NCMBUTTONUP:
                    button = MouseButtons.Middle;
                    break;

                // ������� ��� �������������� ������ ����
                case Win32.WM_XBUTTONDBLCLK:
                case Win32.WM_XBUTTONDOWN:
                case Win32.WM_XBUTTONUP:
                case Win32.WM_NCXBUTTONDBLCLK:
                case Win32.WM_NCXBUTTONDOWN:
                case Win32.WM_NCXBUTTONUP:
                    // ����������� �������������� ������ ����
                    button = GetXButton();
                    break;
            }

            // ��������� �������� ���������� (Control) �� ����������� ����
            Control control = Control.FromChildHandle(hookStruct.hwnd);

            // �������� � ����������� ������� MouseHookEventArgs � ������������� �����������
            return new MouseHookEventArgs(button, hookStruct.pt.x, hookStruct.pt.y, control, (HitTestCode)hookStruct.wHitTestCode);
        }

        // ��������� �����, ������� ���������� ��������� �������������� ������ ����
        private MouseButtons GetXButton()
        {
            // ���� ������ ������ XButton1, ���������� MouseButtons.XButton1
            if (Win32.IsKeyDown(Win32.VK_XBUTTON1))
                return MouseButtons.XButton1;

            // ���� ������ ������ XButton2, ���������� MouseButtons.XButton2
            if (Win32.IsKeyDown(Win32.VK_XBUTTON2))
                return MouseButtons.XButton2;

            // ���� �� ���� �� �������������� ������ ���� �� ������, ���������� MouseButtons.None
            return MouseButtons.None;
        }
        #endregion

        // ������, ���������� ��������� � ������ �� Win32 API ��� ��������� ������� ���� � ������ � ���������
        #region Win32 Imports
        private struct Win32
        {
            // ��������� MOUSEHOOKSTRUCT ��� �������� ���������� � �������� ����
            public struct MOUSEHOOKSTRUCT
            {
                public POINT pt;             // ���������� ����
                public IntPtr hwnd;          // ���������� ����, ��� ������� ��������� �������
                public uint wHitTestCode;    // ��� ��� ����������� �����, �� ������� �������� ����
                public IntPtr dwExtraInfo;   // �������������� ����������
            }

            // ��������� POINT ��� �������� ���������
            public struct POINT
            {
                public int x;   // ���������� X
                public int y;   // ���������� Y
            }

            // �������� ������� �������
            public static bool IsKeyDown(short KeyCode)
            {
                // ��������� ��������� �������
                short state = GetKeyState(KeyCode);
                return ((state & 0x10000) == 0x10000);  // ���������� true, ���� ������� ������
            }

            // ������ ������� GetAsyncKeyState �� user32.dll ��� �������� ��������� ����������� ������
            [DllImport("user32.dll")]
            public static extern short GetAsyncKeyState(int nVirtKey);

            // ������ ������� GetKeyState �� user32.dll ��� �������� ��������� �������
            [DllImport("user32.dll")]
            public static extern short GetKeyState(int nVirtKey);

            // ��������� �������� ����� �� ��������
            public static ushort LOWORD(int l) { return (ushort)(l); }

            // ��������� �������� ����� �� ��������
            public static ushort HIWORD(int l) { return (ushort)(((int)(l) >> 16) & 0xFFFF); }

            // ���������, �������������� ��������� ���� ������� ����
            public const int WM_MOUSEMOVE = 0x0200;

            public const int WM_LBUTTONDOWN		= 0x0201;
			public const int WM_LBUTTONUP		= 0x0202;
			public const int WM_LBUTTONDBLCLK	= 0x0203;

			public const int WM_RBUTTONDOWN		= 0x0204;
			public const int WM_RBUTTONUP		= 0x0205;
			public const int WM_RBUTTONDBLCLK	= 0x0206;

			public const int WM_MBUTTONDOWN		= 0x0207;
			public const int WM_MBUTTONUP		= 0x0208;
			public const int WM_MBUTTONDBLCLK	= 0x0209;

			public const int WM_XBUTTONDOWN		= 0x020B;
			public const int WM_XBUTTONUP		= 0x020C;
			public const int WM_XBUTTONDBLCLK	= 0x020D;

			public const int WM_NCMOUSEMOVE     = 0x00A0;
			public const int WM_NCLBUTTONDOWN   = 0x00A1;
			public const int WM_NCLBUTTONUP     = 0x00A2;
			public const int WM_NCLBUTTONDBLCLK = 0x00A3;
			public const int WM_NCRBUTTONDOWN   = 0x00A4;
			public const int WM_NCRBUTTONUP     = 0x00A5;
			public const int WM_NCRBUTTONDBLCLK = 0x00A6;
			public const int WM_NCMBUTTONDOWN   = 0x00A7;
			public const int WM_NCMBUTTONUP     = 0x00A8;
			public const int WM_NCMBUTTONDBLCLK = 0x00A9;
			public const int WM_NCXBUTTONDOWN   = 0x00AB;
			public const int WM_NCXBUTTONUP     = 0x00AC;
            public const int WM_NCXBUTTONDBLCLK = 0x00AD;

            // ��������� ��� �������������� ������ ���� (XButton1 � XButton2)
            public const int VK_XBUTTON1 = 0x05;
            public const int VK_XBUTTON2 = 0x06;

            // ��������� ��� ��������� ����
            public const int HC_ACTION = 0;
            public const int HC_NOREMOVE = 3;
        }
        #endregion
    }
}
