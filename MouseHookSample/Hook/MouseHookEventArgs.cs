// ������ ������������ ���� System.Windows.Forms ��� ������������� ���� MouseButtons � Control
using System.Windows.Forms;

/* ������������ ����� MouseHookEventArgs, ������� �������� ��������, ������������ ������� MouseHook � ����� MouseProc, ����������� ��� ���� .NET. ��� �������� ������� ����� ������:
 * 
 * ����:
 * 
 * Button: ������ ����, ��������� � ��������.
 * X � Y: ���������� ��������� ���� � ������ �������.
 * Control: ������� ����������, ������� � �������� ����� ������� ������������� ���������.
 * HitTestCode: ������� ����, ��� ������� ��������� ��������� ����.
 * 
 * ������:
 * 
 * MouseHookEventArgs(...): ����������� ������, ������� �������������� ���� ����������, ����������� �� MouseHook.
 * SetNonClient(): �������� ����������, ��������� �� ������� ��� �������� �� ���������� ����� ����.
 * ���� ����� MouseHookEventArgs ������������ ��� �������� ���������� � �������� ����, ������������ � ������ ������� MouseHook, � ������������� ������� ��� ����������, 
 * ������� ���������� ����������� ����������� ������� ����. �� �������� ���������� � ��������� ����, ������ ���� � �������� ����������, � ����� ������� ����, ��� ������� ���������� �������. */

// ���������� ������������ ���� Microsoft.Win32, � ������� ���������� ����� MouseHookEventArgs
namespace Microsoft.Win32
{
    // ���������� ���������� ������ MouseHookEventArgs
    public class MouseHookEventArgs
    {
        // ��������� ������ ��� ������ ���� Button ���� MouseButtons, �������������� ������ ����
        public readonly MouseButtons Button = MouseButtons.None;

        // ��������� ������ ��� ������ ���� X ���� int, �������������� ���������� X
        public readonly int X = 0;

        // ��������� ������ ��� ������ ���� Y ���� int, �������������� ���������� Y
        public readonly int Y = 0;

        // ��������� ������ ��� ������ ���� Control ���� Control, �������������� ������� ����������
        public readonly Control Control = null;

        // ��������� ������ ��� ������ ���� HitTestCode ���� HitTestCode, �������������� ��� ������������ ���������
        public readonly HitTestCode HitTestCode = HitTestCode.HTNOWHERE;

        // ��������� ���� isNonClientArea ���� bool, ������������ ��� ����������� �� ���������� ������� ����
        private bool isNonClientArea = false;

        // ��������� ����������� ������ MouseHookEventArgs, ���������������� ���� ����������, ����������� �� MouseHook
        public MouseHookEventArgs(MouseButtons button, int x, int y, Control control, HitTestCode hitTestCode)
        {
            Button = button;
            X = x;
            Y = y;
            Control = control;
            HitTestCode = hitTestCode;
        }

        // ���������� ����� SetNonClient(), ���������� ����������, ��������� �� ������� ��� ���������� ������� ����
        internal void SetNonClient() { isNonClientArea = true; }
    }
}