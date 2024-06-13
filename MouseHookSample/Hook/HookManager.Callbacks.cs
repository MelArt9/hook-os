using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

// Отслеживает активность мыши на уровне операционной системы Windows, используя глобальные хуки.
// Метод MouseHookProc обрабатывает события, связанные с мышью, и генерирует различные события, такие как нажатие/отпускание кнопок мыши, двойные клики, перемещения колеса мыши
// и перемещения самой мыши.
// Он вызывает соответствующие делегаты, например, s_MouseDown, s_MouseUp, s_MouseClick, s_MouseMove и т.д., если они были установлены для обработки этих событий.
// Используются API Windows для установки глобальных хуков (SetWindowsHookEx), которые перехватывают сообщения о событиях мыши до того, как они достигнут целевого окна приложения.
// Это позволяет отслеживать и обрабатывать эти события, даже если приложение не активно или не в фокусе.

namespace MouseHookSample
{
    public static partial class HookManager
    {
        // Делегат для обработчика хука мыши
        private delegate int HookProc(int nCode, int wParam, IntPtr lParam);
        private static HookProc s_MouseDelegate; // Делегат для хука мыши
        private static int s_MouseHookHandle; // Идентификатор хука мыши

        private static int m_OldX; // Предыдущая координата X
        private static int m_OldY; // Предыдущая координата Y

        public delegate void MouseEventHandlerEx(object sender, MouseEventExtArgs e); // Делегат для событий мыши

        // Метод обработки событий хука мыши
        private static int MouseHookProc(int nCode, int wParam, IntPtr lParam)
        {
            if (nCode >= 0) // Проверка успешности хука
            {
                // Получение структуры хука мыши
                MouseLLHookStruct mouseHookStruct = (MouseLLHookStruct)Marshal.PtrToStructure(lParam, typeof(MouseLLHookStruct));

                MouseButtons button = MouseButtons.None; // Инициализация кнопки мыши
                short mouseDelta = 0; // Инициализация перемещения колеса мыши
                int clickCount = 0; // Инициализация количества кликов
                bool mouseDown = false; // Проверка нажатия кнопки мыши
                bool mouseUp = false; // Проверка отпускания кнопки мыши

                // Определение типа события мыши
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
                                                   mouseHookStruct.Point.X, // Получение координаты X
                                                   mouseHookStruct.Point.Y, // Получение координаты Y
                                                   mouseDelta); // Получение перемещения колеса мыши

                // Вызов делегатов для соответствующих событий мыши, если они установлены
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

                // Проверка для обработки перемещения мыши
                if ((s_MouseMove != null || s_MouseMoveExt != null) && (m_OldX != mouseHookStruct.Point.X || m_OldY != mouseHookStruct.Point.Y))
                {
                    m_OldX = mouseHookStruct.Point.X; // Обновление координаты X
                    m_OldY = mouseHookStruct.Point.Y; // Обновление координаты Y

                    if (s_MouseMove != null)
                    {
                        s_MouseMove.Invoke(null, e);
                    }

                    if (s_MouseMoveExt != null)
                    {
                        s_MouseMoveExt.Invoke(null, e);
                    }
                }

                if (e.Handled) // Проверка на обработанность события
                {
                    return -1; // Отметить событие как обработанное
                }
            }

            return CallNextHookEx(s_MouseHookHandle, nCode, wParam, lParam); // Вызов следующего хука
        }

        // Подписка на глобальные события мыши
        private static void EnsureSubscribedToGlobalMouseEvents()
        {
            if (s_MouseHookHandle == 0) // Если хук не активен
            {
                s_MouseDelegate = MouseHookProc; // Установка обработчика хука мыши

                s_MouseHookHandle = SetWindowsHookEx(
                    WH_MOUSE_LL, // Использование Low-Level Mouse Hook
                    s_MouseDelegate, // Обработчик хука мыши
                    Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]), // Получение инстанса текущего сборки
                    0); // Нулевой флаг

                if (s_MouseHookHandle == 0) // Если хук не установлен
                {
                    int errorCode = Marshal.GetLastWin32Error(); // Получение кода ошибки
                    throw new Win32Exception(errorCode); // Вызов исключения с кодом ошибки
                }
            }
        }

        // Попытка отписаться от глобальных событий мыши
        private static void TryUnsubscribeFromGlobalMouseEvents()
        {
            // Если делегаты для событий мыши не установлены
            if (s_MouseClick == null &&
                s_MouseDown == null &&
                s_MouseMove == null &&
                s_MouseUp == null &&
                s_MouseClickExt == null &&
                s_MouseMoveExt == null &&
                s_MouseWheel == null)
            {
                ForceUnsunscribeFromGlobalMouseEvents(); // Отписка от глобальных событий мыши
            }
        }

        // Принудительная отписка от глобальных событий мыши
        private static void ForceUnsunscribeFromGlobalMouseEvents()
        {
            if (s_MouseHookHandle != 0) // Если хук мыши активен
            {
                int result = UnhookWindowsHookEx(s_MouseHookHandle); // Отписка от хука мыши
                s_MouseHookHandle = 0; // Обнуление идентификатора хука мыши
                s_MouseDelegate = null; // Обнуление делегата хука мыши

                if (result == 0) // Если отписка не удалась
                {
                    int errorCode = Marshal.GetLastWin32Error(); // Получение кода ошибки
                    throw new Win32Exception(errorCode); // Вызов исключения с кодом ошибки
                }
            }
        }
    }
}