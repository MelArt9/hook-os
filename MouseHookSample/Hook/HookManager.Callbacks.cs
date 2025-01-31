using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

// ����������� ���������� ���� �� ������ ������������ ������� Windows, ��������� ���������� ����.
// ����� MouseHookProc ������������ �������, ��������� � �����, � ���������� ��������� �������, ����� ��� �������/���������� ������ ����, ������� �����, ����������� ������ ����
// � ����������� ����� ����.
// �� �������� ��������������� ��������, ��������, s_MouseDown, s_MouseUp, s_MouseClick, s_MouseMove � �.�., ���� ��� ���� ����������� ��� ��������� ���� �������.
// ������������ API Windows ��� ��������� ���������� ����� (SetWindowsHookEx), ������� ������������� ��������� � �������� ���� �� ����, ��� ��� ��������� �������� ���� ����������.
// ��� ��������� ����������� � ������������ ��� �������, ���� ���� ���������� �� ������� ��� �� � ������.

namespace MouseHookSample
{
    public static partial class HookManager
    {
        // ������� ��� ����������� ���� ����
        private delegate int HookProc(int nCode, int wParam, IntPtr lParam);
        private static HookProc s_MouseDelegate; // ������� ��� ���� ����
        private static int s_MouseHookHandle; // ������������� ���� ����

        private static int m_OldX; // ���������� ���������� X
        private static int m_OldY; // ���������� ���������� Y

        public delegate void MouseEventHandlerEx(object sender, MouseEventExtArgs e); // ������� ��� ������� ����

        // ����� ��������� ������� ���� ����
        private static int MouseHookProc(int nCode, int wParam, IntPtr lParam)
        {
            if (nCode >= 0) // �������� ���������� ����
            {
                // ��������� ��������� ���� ����
                MouseLLHookStruct mouseHookStruct = (MouseLLHookStruct)Marshal.PtrToStructure(lParam, typeof(MouseLLHookStruct));

                MouseButtons button = MouseButtons.None; // ������������� ������ ����
                short mouseDelta = 0; // ������������� ����������� ������ ����
                int clickCount = 0; // ������������� ���������� ������
                bool mouseDown = false; // �������� ������� ������ ����
                bool mouseUp = false; // �������� ���������� ������ ����

                // ����������� ���� ������� ����
                switch (wParam)
                {
                    case WM_LBUTTONDOWN:
                        mouseDown = true;
                        button = MouseButtons.Left;
                        clickCount = 1;
                        break;
                    case WM_LBUTTONUP:
                        mouseUp = true;
                        button = MouseButtons.Left;
                        clickCount = 1;
                        break;
                    case WM_LBUTTONDBLCLK:
                        button = MouseButtons.Left;
                        clickCount = 2;
                        break;
                    case WM_RBUTTONDOWN:
                        mouseDown = true;
                        button = MouseButtons.Right;
                        clickCount = 1;
                        break;
                    case WM_RBUTTONUP:
                        mouseUp = true;
                        button = MouseButtons.Right;
                        clickCount = 1;
                        break;
                    case WM_RBUTTONDBLCLK:
                        button = MouseButtons.Right;
                        clickCount = 2;
                        break;
                    case WM_MOUSEWHEEL:
                        mouseDelta = (short)((mouseHookStruct.MouseData >> 16) & 0xffff);
                        break;
                }

                MouseEventExtArgs e = new MouseEventExtArgs(
                                                   button,
                                                   clickCount,
                                                   mouseHookStruct.Point.X, // ��������� ���������� X
                                                   mouseHookStruct.Point.Y, // ��������� ���������� Y
                                                   mouseDelta); // ��������� ����������� ������ ����

                // ����� ��������� ��� ��������������� ������� ����, ���� ��� �����������
                if (s_MouseUp != null && mouseUp)
                {
                    s_MouseUp.Invoke(null, e);
                }

                if (s_MouseDown != null && mouseDown)
                {
                    s_MouseDown.Invoke(null, e);
                }

                if (s_MouseClick != null && clickCount > 0)
                {
                    s_MouseClick.Invoke(null, e);
                }

                if (s_MouseClickExt != null && clickCount > 0)
                {
                    s_MouseClickExt.Invoke(null, e);
                }

                if (s_MouseDoubleClick != null && clickCount == 2)
                {
                    s_MouseDoubleClick.Invoke(null, e);
                }

                if (s_MouseWheel != null && mouseDelta != 0)
                {
                    s_MouseWheel.Invoke(null, e);
                }

                // �������� ��� ��������� ����������� ����
                if ((s_MouseMove != null || s_MouseMoveExt != null) && (m_OldX != mouseHookStruct.Point.X || m_OldY != mouseHookStruct.Point.Y))
                {
                    m_OldX = mouseHookStruct.Point.X; // ���������� ���������� X
                    m_OldY = mouseHookStruct.Point.Y; // ���������� ���������� Y

                    if (s_MouseMove != null)
                    {
                        s_MouseMove.Invoke(null, e);
                    }

                    if (s_MouseMoveExt != null)
                    {
                        s_MouseMoveExt.Invoke(null, e);
                    }
                }

                if (e.Handled) // �������� �� �������������� �������
                {
                    return -1; // �������� ������� ��� ������������
                }
            }

            return CallNextHookEx(s_MouseHookHandle, nCode, wParam, lParam); // ����� ���������� ����
        }

        // �������� �� ���������� ������� ����
        private static void EnsureSubscribedToGlobalMouseEvents()
        {
            if (s_MouseHookHandle == 0) // ���� ��� �� �������
            {
                s_MouseDelegate = MouseHookProc; // ��������� ����������� ���� ����

                s_MouseHookHandle = SetWindowsHookEx(
                    WH_MOUSE_LL, // ������������� Low-Level Mouse Hook
                    s_MouseDelegate, // ���������� ���� ����
                    Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]), // ��������� �������� �������� ������
                    0); // ������� ����

                if (s_MouseHookHandle == 0) // ���� ��� �� ����������
                {
                    int errorCode = Marshal.GetLastWin32Error(); // ��������� ���� ������
                    throw new Win32Exception(errorCode); // ����� ���������� � ����� ������
                }
            }
        }

        // ������� ���������� �� ���������� ������� ����
        private static void TryUnsubscribeFromGlobalMouseEvents()
        {
            // ���� �������� ��� ������� ���� �� �����������
            if (s_MouseClick == null &&
                s_MouseDown == null &&
                s_MouseMove == null &&
                s_MouseUp == null &&
                s_MouseClickExt == null &&
                s_MouseMoveExt == null &&
                s_MouseWheel == null)
            {
                ForceUnsunscribeFromGlobalMouseEvents(); // ������� �� ���������� ������� ����
            }
        }

        // �������������� ������� �� ���������� ������� ����
        private static void ForceUnsunscribeFromGlobalMouseEvents()
        {
            if (s_MouseHookHandle != 0) // ���� ��� ���� �������
            {
                int result = UnhookWindowsHookEx(s_MouseHookHandle); // ������� �� ���� ����
                s_MouseHookHandle = 0; // ��������� �������������� ���� ����
                s_MouseDelegate = null; // ��������� �������� ���� ����

                if (result == 0) // ���� ������� �� �������
                {
                    int errorCode = Marshal.GetLastWin32Error(); // ��������� ���� ������
                    throw new Win32Exception(errorCode); // ����� ���������� � ����� ������
                }
            }
        }
    }
}