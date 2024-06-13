using System;
using System.Runtime.InteropServices;

/* Cодержит определения констант Windows, которые используются при работе с хуками (hooks) для перехвата событий мыши. 
 * Он также содержит импорты функций Windows API, необходимых для установки, вызова и удаления хуков.
 * 
 * Вот краткое описание основных элементов в коде:
 * 
 * Константы Windows: Эти константы содержат числовые значения, связанные с различными типами сообщений Windows, такими как события мыши (WM_MOUSEMOVE, WM_LBUTTONDOWN, и т.д.). 
 * Каждая константа представляет определенное событие.
 * 
 * Функции Windows API:
 * CallNextHookEx: Передает информацию хука следующей процедуре хука в цепочке.
 * SetWindowsHookEx: Устанавливает процедуру хука в цепочку хуков.
 * UnhookWindowsHookEx: Удаляет процедуру хука из цепочки.
 * GetDoubleClickTime: Получает текущее время для двойного щелчка мыши.
 * 
 * Эти функции используются для установки хуков, обработки сообщений о событиях мыши в Windows. 
 * Это обеспечивает возможность перехвата и обработки этих событий внутри приложения. */

namespace MouseHookSample
{
    public static partial class HookManager
    {
        #region Windows constants

        // Константы, представляющие различные события мыши
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

        // Импорт функции CallNextHookEx из библиотеки user32.dll
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern int CallNextHookEx(int idHook, int nCode, int wParam, IntPtr lParam);

        // Импорт функции SetWindowsHookEx из библиотеки user32.dll
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        private static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hMod, int dwThreadId);

        // Импорт функции UnhookWindowsHookEx из библиотеки user32.dll
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        private static extern int UnhookWindowsHookEx(int idHook);

        // Импорт функции GetDoubleClickTime из библиотеки user32.dll
        [DllImport("user32")]
        public static extern int GetDoubleClickTime();

        #endregion
    }
}