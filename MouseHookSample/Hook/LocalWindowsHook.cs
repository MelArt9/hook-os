using System;
using System.Runtime.InteropServices;

/* ������������ ����� LocalWindowsHook, ������� ������������ ����� �������������� ��� ������������� Win32 ����� � ����������� �� .NET. ���� (hooks) ��������� 
 * ������������� � ������������ ��������� ������� Windows, ����� ��� ������� ����.
 * 
 * ��� ������� �������� �������� ��������� ����:
 * 
 * HookEventArgs: �����, �������������� ��������� ��� ������� ����. �� �������� ���� ��� ���� ���� (HookCode), wParam � lParam.
 * HookType: ������������, �������������� ��������� ���� �����, ����� ��� WH_MOUSE, WH_MOUSE_LL � ������.
 * LocalWindowsHook: �����, ���������� ������ ��������� � �������� �����, � ����� ������ �� ���������. �� �������� � ���� ������ ��� ��������� (Install()) � �������� (Uninstall()) �����, 
 * � ����� ������ ��������� ������� � ������ CoreHookProc.
 * 
 * ������� ������� Win32 API (SetWindowsHookEx, UnhookWindowsHookEx, CallNextHookEx), ������� ������������ ��� ���������, �������� � ������ ����� �� ���� �� C#. 
 * ��� ������������� ��������� ��� ������ � ��������������� ���������� ��������� Windows ��� ���������� ������.
 * 
 * ���� ����� ������������� ������� ���������� ��� ������ � Win32 ������ � .NET-�����������, �������� ������������� � ������������ ������� Windows. */

namespace Microsoft.Win32
{
    #region Class HookEventArgs
    // �����, ���������� ��������� ��� ������� ����
    public class HookEventArgs : EventArgs
    {
        public int HookCode;    // ��� ����
        public IntPtr wParam;   // wParam
        public IntPtr lParam;   // lParam
    }
    #endregion

    #region Enum HookType
    // ������������, �������������� ��������� ���� �����
    public enum HookType : int
    {
        WH_JOURNALRECORD = 0,
        WH_JOURNALPLAYBACK = 1,
        WH_GETMESSAGE = 3,
        WH_CALLWNDPROC = 4,
        WH_CBT = 5,
        WH_SYSMSGFILTER = 6,
        WH_MOUSE = 7,
        WH_HARDWARE = 8,
        WH_DEBUG = 9,
        WH_SHELL = 10,
        WH_FOREGROUNDIDLE = 11,
        WH_CALLWNDPROCRET = 12,
        WH_MOUSE_LL = 14        // �������������� ��� ��� ������� ����
    }
    #endregion

    #region Class LocalWindowsHook
    // ����� ��� ���������, �������� � ��������� �����
    public class LocalWindowsHook
    {
        // ������� ��� ��������� ����
        public delegate int HookProc(int code, IntPtr wParam, IntPtr lParam);
        protected IntPtr m_hhook = IntPtr.Zero;
        protected HookProc m_filterFunc = null;
        protected HookType m_hookType;
        public delegate void HookEventHandler(object sender, HookEventArgs e);
        public event HookEventHandler HookInvoked;

        // ���������� ������� ����
        protected void OnHookInvoked(HookEventArgs e)
        {
            if (HookInvoked != null)
                HookInvoked(this, e);
        }

        // ����������� ������ LocalWindowsHook
        public LocalWindowsHook(HookType hook)
        {
            m_hookType = hook;
            m_filterFunc = new HookProc(this.CoreHookProc);
        }

        // ����� ��� ��������� ����
        protected int CoreHookProc(int code, IntPtr wParam, IntPtr lParam)
        {
            if (code < 0)
                return CallNextHookEx(m_hhook, code, wParam, lParam);

            HookEventArgs e = new HookEventArgs();
            e.HookCode = code;
            e.wParam = wParam;
            e.lParam = lParam;
            OnHookInvoked(e);

            return CallNextHookEx(m_hhook, code, wParam, lParam);
        }

        // ����� ��� �������� ����
        public void Uninstall()
        {
            UnhookWindowsHookEx(m_hhook);
            m_hhook = IntPtr.Zero;
        }

        // �������� ��� �������� ��������������� ����
        public bool IsInstalled
        {
            get { return m_hhook != IntPtr.Zero; }
        }

        #region Win32 Imports

        // ������ ������� UnhookWindowsHookEx �� ���������� user32.dll
        [DllImport("user32.dll")]
        protected static extern int UnhookWindowsHookEx(IntPtr hhook);

        // ������ ������� CallNextHookEx �� ���������� user32.dll
        [DllImport("user32.dll")]
        protected static extern int CallNextHookEx(IntPtr hhook, int code, IntPtr wParam, IntPtr lParam);

        #endregion
    }
    #endregion
}