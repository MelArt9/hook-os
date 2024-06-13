// Импорт пространства имен для работы с межпрограммным интерфейсом (API) Windows
using System.Runtime.InteropServices;

/* Cодержит определения структур Point, MouseLLHookStruct, которые предоставляют информацию о низкоуровневых событиях мыши для использования с 
 * глобальными хуками в Windows.
 * 
 * Point: Определяет координаты X и Y точки на экране.
 * MouseLLHookStruct: Содержит информацию о событиях мыши. Эта структура включает координаты курсора, данные о колесе прокрутки мыши, флаги события, отметку времени и дополнительную информацию.
 * 
 * Эти структуры используются в коде для интерпретации и обработки информации о событиях мыши, перехваченных глобальными хуками в Windows. */

namespace MouseHookSample
{

    public static partial class HookManager
    {
        // Структура, определяющая координаты X и Y точки на экране
        [StructLayout(LayoutKind.Sequential)]
        private struct Point
        {
            public int X;
            public int Y;
        }

        // Структура, содержащая информацию о событиях мыши
        [StructLayout(LayoutKind.Sequential)]
        private struct MouseLLHookStruct
        {
            public Point Point;      // Координаты курсора
            public int MouseData;    // Данные о колесе прокрутки мыши
            public int Flags;        // Флаги события
            public int Time;         // Отметка времени
            public int ExtraInfo;    // Дополнительная информация
        }
    }
}