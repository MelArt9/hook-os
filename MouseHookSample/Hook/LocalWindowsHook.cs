using System;
using System.Runtime.InteropServices;

/* Представляет класс LocalWindowsHook, который обеспечивает общую инфраструктуру для использования Win32 хуков в приложениях на .NET. Хуки (hooks) позволяют 
 * перехватывать и обрабатывать различные события Windows, такие как события мыши.
 * 
 * Вот краткое описание ключевых элементов кода:
 * 
 * HookEventArgs: Класс, представляющий аргументы для события хука. Он содержит поля для кода хука (HookCode), wParam и lParam.
 * HookType: Перечисление, представляющее различные типы хуков, такие как WH_MOUSE, WH_MOUSE_LL и другие.
 * LocalWindowsHook: Класс, содержащий логику установки и удаления хуков, а также логику их обработки. Он включает в себя методы для установки (Install()) и удаления (Uninstall()) хуков, 
 * а также логику обработки событий в методе CoreHookProc.
 * 
 * Внешние функции Win32 API (SetWindowsHookEx, UnhookWindowsHookEx, CallNextHookEx), которые используются для установки, удаления и вызова хуков из кода на C#. 
 * Они предоставляют интерфейс для работы с низкоуровневыми системными функциями Windows для управления хуками.
 * 
 * Этот класс предоставляет удобную абстракцию для работы с Win32 хуками в .NET-приложениях, позволяя перехватывать и обрабатывать события Windows. */

namespace Microsoft.Win32
{
    #region Class HookEventArgs
    // Класс, содержащий аргументы для события хука
    public class HookEventArgs : EventArgs
    {
        public int HookCode;    // Код хука
        public IntPtr wParam;   // wParam
        public IntPtr lParam;   // lParam
    }
    #endregion

    #region Enum HookType
    // Перечисление, представляющее различные типы хуков
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
        WH_MOUSE_LL = 14        // Низкоуровневый хук для событий мыши
    }
    #endregion

    #region Class LocalWindowsHook
    // Класс для установки, удаления и обработки хуков
    public class LocalWindowsHook
    {
        // Делегат для процедуры хука
        public delegate int HookProc(int code, IntPtr wParam, IntPtr lParam);
        protected IntPtr m_hhook = IntPtr.Zero;
        protected HookProc m_filterFunc = null;
        protected HookType m_hookType;
        public delegate void HookEventHandler(object sender, HookEventArgs e);
        public event HookEventHandler HookInvoked;

        // Обработчик события хука
        protected void OnHookInvoked(HookEventArgs e)
        {
            if (HookInvoked != null)
                HookInvoked(this, e);
        }

        // Конструктор класса LocalWindowsHook
        public LocalWindowsHook(HookType hook)
        {
            m_hookType = hook;
            m_filterFunc = new HookProc(this.CoreHookProc);
        }

        // Метод для обработки хука
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

        // Метод для удаления хука
        public void Uninstall()
        {
            UnhookWindowsHookEx(m_hhook);
            m_hhook = IntPtr.Zero;
        }

        // Свойство для проверки установленности хука
        public bool IsInstalled
        {
            get { return m_hhook != IntPtr.Zero; }
        }

        #region Win32 Imports

        // Импорт функции UnhookWindowsHookEx из библиотеки user32.dll
        [DllImport("user32.dll")]
        protected static extern int UnhookWindowsHookEx(IntPtr hhook);

        // Импорт функции CallNextHookEx из библиотеки user32.dll
        [DllImport("user32.dll")]
        protected static extern int CallNextHookEx(IntPtr hhook, int code, IntPtr wParam, IntPtr lParam);

        #endregion
    }
    #endregion
}