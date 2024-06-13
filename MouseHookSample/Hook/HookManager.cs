using System;
using System.Windows.Forms;

/* ������������ ����������� ����� HookManager, ������� ������������� ������� ��� ������������ ���������� ���� �� ���������� ������ � ���������� Windows Forms.
 * ��� ���� (Mouse) ����������� ������� ��� ������������ �����������, ������, ������� � ���������� ������, �������� ������ � ������� ������.
 * ������ ������� �������� ������ add � remove, ����� ����������� � ���������� ����������� �������. ��������, MouseMove, MouseClick, MouseDown, MouseUp, MouseWheel, MouseDoubleClick.
 * ���� ��� ���������� ���������� ���� Windows ��� ��������� ������� ���� ��� �������� �������� ����������. 
 * �� ����� ��������� ������ ��� ��������� ������� ������ �����, ��������� ������ ��� ����������� ��������� ����� ����� ����������������� �������. */

namespace MouseHookSample
{
    public static partial class HookManager
    {
        private static event MouseEventHandler s_MouseMove; // ������� ��� ����������� ����

        public static event MouseEventHandler MouseMove // ������� ��� ����������� ����
        {
            add
            {
                EnsureSubscribedToGlobalMouseEvents(); // ��������� � �������� �� ���������� ������� ����
                s_MouseMove += value; // �������� ���������� �������
            }

            remove
            {
                s_MouseMove -= value; // ������� ���������� �������
                TryUnsubscribeFromGlobalMouseEvents(); // ���������� ���������� �� ���������� ������� ����
            }
        }

        private static event EventHandler<MouseEventExtArgs> s_MouseMoveExt; // ������� ��� ������������ ����������� ����

        public static event EventHandler<MouseEventExtArgs> MouseMoveExt // ������� ��� ������������ ����������� ����
        {
            add
            {
                EnsureSubscribedToGlobalMouseEvents(); // ��������� � �������� �� ���������� ������� ����
                s_MouseMoveExt += value; // �������� ���������� �������
            }

            remove
            {
                s_MouseMoveExt -= value; // ������� ���������� �������
                TryUnsubscribeFromGlobalMouseEvents(); // ���������� ���������� �� ���������� ������� ����
            }
        }

        private static event MouseEventHandler s_MouseClick; // ������� ��� ����� ����

        public static event MouseEventHandler MouseClick // ������� ��� ����� ����
        {
            add
            {
                EnsureSubscribedToGlobalMouseEvents(); // ��������� � �������� �� ���������� ������� ����
                s_MouseClick += value; // �������� ���������� �������
            }
            remove
            {
                s_MouseClick -= value; // ������� ���������� �������
                TryUnsubscribeFromGlobalMouseEvents(); // ���������� ���������� �� ���������� ������� ����
            }
        }

        // ���������� ������� ��� ��������������� ����� ����
        private static event EventHandler<MouseEventExtArgs> s_MouseClickExt;

        // ������� ��� ��������������� ����� ����
        public static event EventHandler<MouseEventExtArgs> MouseClickExt
        {
            // ���������� ����������� �������
            add
            {
                EnsureSubscribedToGlobalMouseEvents(); // ����������� �������� �� ���������� ������� ����
                s_MouseClickExt += value;
            }
            // �������� ����������� �������
            remove
            {
                s_MouseClickExt -= value;
                TryUnsubscribeFromGlobalMouseEvents(); // �������� ���������� �� ���������� ������� ����
            }
        }

        // ���������� ������� ��� ������� ������ ����
        private static event MouseEventHandlerEx s_MouseDown;

        // ������� ��� ������� ������ ����
        public static event MouseEventHandlerEx MouseDown
        {
            // ���������� ����������� �������
            add
            {
                EnsureSubscribedToGlobalMouseEvents(); // ����������� �������� �� ���������� ������� ����
                s_MouseDown += value;
            }
            // �������� ����������� �������
            remove
            {
                s_MouseDown -= value;
                TryUnsubscribeFromGlobalMouseEvents(); // �������� ���������� �� ���������� ������� ����
            }
        }

        // ���������� ������� ��� ���������� ������ ����
        private static event MouseEventHandler s_MouseUp;

        // ������� ��� ���������� ������ ����
        public static event MouseEventHandler MouseUp
        {
            // ���������� ����������� �������
            add
            {
                EnsureSubscribedToGlobalMouseEvents(); // ����������� �������� �� ���������� ������� ����
                s_MouseUp += value;
            }
            // �������� ����������� �������
            remove
            {
                s_MouseUp -= value;
                TryUnsubscribeFromGlobalMouseEvents(); // �������� ���������� �� ���������� ������� ����
            }
        }

        // ���������� ������� ��� ��������� ��������� ������ ����
        private static event MouseEventHandlerEx s_MouseWheel;

        // ������� ��� ��������� ��������� ������ ����
        public static event MouseEventHandlerEx MouseWheel
        {
            // ���������� ����������� �������
            add
            {
                EnsureSubscribedToGlobalMouseEvents(); // ����������� �������� �� ���������� ������� ����
                s_MouseWheel += value;
            }
            // �������� ����������� �������
            remove
            {
                s_MouseWheel -= value;
                TryUnsubscribeFromGlobalMouseEvents(); // �������� ���������� �� ���������� ������� ����
            }
        }

        // ���������� ������� ��� �������� ����� ����
        private static event MouseEventHandler s_MouseDoubleClick;

        // ������� ��� �������� ����� ����
        public static event MouseEventHandler MouseDoubleClick
        {
            // ���������� ����������� �������
            add
            {
                EnsureSubscribedToGlobalMouseEvents(); // ����������� �������� �� ���������� ������� ����
                if (s_MouseDoubleClick == null)
                {
                    // ������������� ������� ��� ������� ������
                    s_DoubleClickTimer = new Timer
                    {
                        Interval = GetDoubleClickTime(), // ������������� �������� ������� ��� �������� �����
                        Enabled = false // ��������� ������
                    };
                    s_DoubleClickTimer.Tick += DoubleClickTimeElapsed; // ��������� ���������� ������� ��������� ������� �������
                    MouseUp += OnMouseUp; // ��������� ���������� ������� ���������� ������ ����
                }
                s_MouseDoubleClick += value; // ��������� ���������� ��� �������� ����� ����
            }
            // �������� ����������� �������
            remove
            {
                if (s_MouseDoubleClick != null)
                {
                    s_MouseDoubleClick -= value; // ������� ���������� ��� �������� ����� ����
                    if (s_MouseDoubleClick == null)
                    {
                        MouseUp -= OnMouseUp; // ������� ���������� ������� ���������� ������ ����
                        s_DoubleClickTimer.Tick -= DoubleClickTimeElapsed; // ������� ���������� ������� ��������� ������� �������
                        s_DoubleClickTimer = null; // �������� ������ ��� ������� ������
                    }
                }
                TryUnsubscribeFromGlobalMouseEvents(); // �������� ���������� �� ���������� ������� ����
            }
        }

        // ���������� ��� �������� ���������� ������� ������ ����
        private static MouseButtons s_PrevClickedButton;

        // ������ ��� ������������ ������� ������ ����
        private static Timer s_DoubleClickTimer;

        // �����, ���������� ��� ��������� ������� ������� ������� ������
        private static void DoubleClickTimeElapsed(object sender, EventArgs e)
        {
            s_DoubleClickTimer.Enabled = false; // ��������� ������ ������� ������
            s_PrevClickedButton = MouseButtons.None; // ���������� ���������� ������� ������ ����
        }

        // �����, ���������� ��� ���������� ������ ����
        private static void OnMouseUp(object sender, MouseEventArgs e)
        {
            // ���� ���������� ������ ������ 1, ������ ����� �� ������
            if (e.Clicks < 1) { return; }

            // ���� ������� ������ ��������� � ����������
            if (e.Button.Equals(s_PrevClickedButton))
            {
                // ���� ���� ���������� �� ������� MouseDoubleClick
                if (s_MouseDoubleClick != null)
                {
                    s_MouseDoubleClick.Invoke(null, e); // ������� ������� MouseDoubleClick
                }
                s_DoubleClickTimer.Enabled = false; // ��������� ������ ������� ������
                s_PrevClickedButton = MouseButtons.None; // ���������� ���������� ������� ������ ����
            }
            else
            {
                s_DoubleClickTimer.Enabled = true; // �������� ������ ������� ������
                s_PrevClickedButton = e.Button; // ��������� ������� ������ ����
            }
        }
    }
}
