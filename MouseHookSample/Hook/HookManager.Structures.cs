// ������ ������������ ���� ��� ������ � �������������� ����������� (API) Windows
using System.Runtime.InteropServices;

/* C������� ����������� �������� Point, MouseLLHookStruct, ������� ������������� ���������� � �������������� �������� ���� ��� ������������� � 
 * ����������� ������ � Windows.
 * 
 * Point: ���������� ���������� X � Y ����� �� ������.
 * MouseLLHookStruct: �������� ���������� � �������� ����. ��� ��������� �������� ���������� �������, ������ � ������ ��������� ����, ����� �������, ������� ������� � �������������� ����������.
 * 
 * ��� ��������� ������������ � ���� ��� ������������� � ��������� ���������� � �������� ����, ������������� ����������� ������ � Windows. */

namespace MouseHookSample
{

    public static partial class HookManager
    {
        // ���������, ������������ ���������� X � Y ����� �� ������
        [StructLayout(LayoutKind.Sequential)]
        private struct Point
        {
            public int X;
            public int Y;
        }

        // ���������, ���������� ���������� � �������� ����
        [StructLayout(LayoutKind.Sequential)]
        private struct MouseLLHookStruct
        {
            public Point Point;      // ���������� �������
            public int MouseData;    // ������ � ������ ��������� ����
            public int Flags;        // ����� �������
            public int Time;         // ������� �������
            public int ExtraInfo;    // �������������� ����������
        }
    }
}