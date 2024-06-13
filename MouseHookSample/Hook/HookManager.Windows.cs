using System;
using System.Runtime.InteropServices;

/* C������� ����������� �������� Windows, ������� ������������ ��� ������ � ������ (hooks) ��� ��������� ������� ����. 
 * �� ����� �������� ������� ������� Windows API, ����������� ��� ���������, ������ � �������� �����.
 * 
 * ��� ������� �������� �������� ��������� � ����:
 * 
 * ��������� Windows: ��� ��������� �������� �������� ��������, ��������� � ���������� ������ ��������� Windows, ������ ��� ������� ���� (WM_MOUSEMOVE, WM_LBUTTONDOWN, � �.�.). 
 * ������ ��������� ������������ ������������ �������.
 * 
 * ������� Windows API:
 * CallNextHookEx: �������� ���������� ���� ��������� ��������� ���� � �������.
 * SetWindowsHookEx: ������������� ��������� ���� � ������� �����.
 * UnhookWindowsHookEx: ������� ��������� ���� �� �������.
 * GetDoubleClickTime: �������� ������� ����� ��� �������� ������ ����.
 * 
 * ��� ������� ������������ ��� ��������� �����, ��������� ��������� � �������� ���� � Windows. 
 * ��� ������������ ����������� ��������� � ��������� ���� ������� ������ ����������. */

namespace MouseHookSample
{
    public static partial class HookManager
    {
        #region Windows constants

        // ���������, �������������� ��������� ������� ����
        private const int WH_MOUSE_LL = 14;
        private const int WM_LBUTTONDOWN = 0x201;
        private const int WM_RBUTTONDOWN = 0x204;
        private const int WM_LBUTTONUP = 0x202;
        private const int WM_RBUTTONUP = 0x205;
        private const int WM_LBUTTONDBLCLK = 0x203;
        private const int WM_RBUTTONDBLCLK = 0x206;
        private const int WM_MOUSEWHEEL = 0x020A;

        #endregion

        #region Windows function imports

        // ������ ������� CallNextHookEx �� ���������� user32.dll
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern int CallNextHookEx(int idHook, int nCode, int wParam, IntPtr lParam);

        // ������ ������� SetWindowsHookEx �� ���������� user32.dll
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        private static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hMod, int dwThreadId);

        // ������ ������� UnhookWindowsHookEx �� ���������� user32.dll
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        private static extern int UnhookWindowsHookEx(int idHook);

        // ������ ������� GetDoubleClickTime �� ���������� user32.dll
        [DllImport("user32")]
        public static extern int GetDoubleClickTime();

        #endregion
    }
}