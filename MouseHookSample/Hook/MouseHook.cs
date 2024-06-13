using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;


/* Представляет класс MouseHook, который является оберткой над API Mouse Hook в Windows. Он позволяет перехватывать события мыши в приложении и обрабатывать их. Вот основные моменты этого класса:
 * 
 * Конструктор:
 * MouseHook() создает объект класса MouseHook для перехвата событий мыши типа WH_MOUSE.
 * 
 * События:
 * MouseDown, MouseUp, MouseMove, MouseDoubleClick - события, вызываемые при соответствующих действиях мыши. Каждое из этих событий вызывает соответствующий метод, который должен быть 
 * определен во внешнем коде.
 * 
 * Методы:
 * MouseProc(int code, IntPtr wParam, IntPtr lParam): Это обратный вызов, используемый функцией SetWindowsHookEx. Он обрабатывает события мыши, перехватываемые хуком мыши.
 * RaiseMouseHookEvent(IntPtr wParam, MouseHookEventArgs args): Определяет, какое событие мыши произошло и вызывает соответствующее событие внутри класса MouseHook.
 * CrackHookMsg(IntPtr wParam, Win32.MOUSEHOOKSTRUCT hookStruct): Помогает извлечь аргументы, переданные в MouseProc, и переупаковать их в объекты .NET.
 * 
 * Win32 Imports:
 * 
 * Этот блок содержит структуры и методы из Win32 API, используемые для обработки сообщений мыши и работы с клавишами.
 * Этот класс MouseHook полезен для перехвата и обработки событий мыши в .NET-приложениях, позволяя разработчику выполнить специальные действия в ответ на действия пользователя с мышью. */

namespace Microsoft.Win32
{
    // Перечисление, определяющее коды, используемые для тестирования попадания (hit test) элементов на экране
    public enum HitTestCode : int
    {
        HTERROR = -2,            // Ошибка
        HTTRANSPARENT = -1,      // Прозрачная область
        HTNOWHERE = 0,           // Вне всех областей
        HTCLIENT = 1,            // Клиентская область
        HTCAPTION = 2,           // Заголовок окна
        HTSYSMENU = 3,           // Системное меню окна
        HTGROWBOX = 4,           // Размерная рамка
        HTSIZE = HTGROWBOX,      // Область изменения размера
        HTMENU = 5,              // Меню
        HTHSCROLL = 6,           // Горизонтальная полоса прокрутки
        HTVSCROLL = 7,           // Вертикальная полоса прокрутки
        HTMINBUTTON = 8,         // Кнопка "Свернуть"
        HTMAXBUTTON = 9,         // Кнопка "Развернуть"
        HTLEFT = 10,             // Левая граница окна
        HTRIGHT = 11,            // Правая граница окна
        HTTOP = 12,              // Верхняя граница окна
        HTTOPLEFT = 13,          // Верхний левый угол окна
        HTTOPRIGHT = 14,         // Верхний правый угол окна
        HTBOTTOM = 15,           // Нижняя граница окна
        HTBOTTOMLEFT = 16,       // Нижний левый угол окна
        HTBOTTOMRIGHT = 17,      // Нижний правый угол окна
        HTBORDER = 18,           // Граница окна
        HTREDUCE = HTMINBUTTON,  // Код уменьшения окна
        HTZOOM = HTMAXBUTTON,    // Код увеличения окна
        HTSIZEFIRST = HTLEFT,    // Первый код изменения размера
        HTSIZELAST = HTBOTTOMRIGHT,  // Последний код изменения размера
        HTOBJECT = 19,           // Объект
        HTCLOSE = 20,            // Кнопка "Закрыть"
        HTHELP = 21              // Кнопка "Помощь"
    }

    // Обработчик событий для класса MouseHook, который перехватывает события мыши
    public delegate void MouseHookEventHandler(object sender, MouseHookEventArgs e);

    // Класс MouseHook, расширяющий LocalWindowsHook и реализующий интерфейс IDisposable
    public class MouseHook : LocalWindowsHook, IDisposable
    {
        #region Construction
        // Конструктор класса, устанавливающий хук мыши
        public MouseHook() : base(HookType.WH_MOUSE)
        {
            // Устанавливаем собственную функцию обратного вызова
            m_filterFunc = new HookProc(this.MouseProc);
        }
        #endregion

        #region Disposal
        // Деструктор класса MouseHook
        ~MouseHook()
        {
            Dispose(false);
        }

        // Метод Dispose для освобождения ресурсов
        protected void Dispose(bool disposing)
        {
            if (IsInstalled)
                Uninstall();

            if (disposing)
                GC.SuppressFinalize(this);
        }

        // Реализация интерфейса IDisposable для освобождения ресурсов
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion

        #region Events
        // Событие, возникающее при нажатии кнопки мыши
        public event MouseHookEventHandler MouseDown;
        protected void OnMouseDown(MouseHookEventArgs e)
        {
            if (MouseDown != null)
                MouseDown(this, e);
        }

        // Событие, возникающее при отпускании кнопки мыши
        public event MouseHookEventHandler MouseUp;
        // Метод, вызывающий событие MouseUp с аргументами MouseHookEventArgs
        protected void OnMouseUp(MouseHookEventArgs e)
        {
            if (MouseUp != null)
                MouseUp(this, e);
        }

        // Событие, возникающее при движении мыши
        public event MouseHookEventHandler MouseMove;
        // Метод, вызывающий событие MouseMove с аргументами MouseHookEventArgs
        protected void OnMouseMove(MouseHookEventArgs e)
        {
            if (MouseMove != null)
                MouseMove(this, e);
        }

        // Событие, возникающее при двойном щелчке мыши
        public event MouseHookEventHandler MouseDoubleClick;
        // Метод, вызывающий событие MouseDoubleClick с аргументами MouseHookEventArgs
        protected void OnMouseDoubleClick(MouseHookEventArgs e)
        {
            if (MouseDoubleClick != null)
                MouseDoubleClick(this, e);
        }
        #endregion

        #region Mouse Hook specific code

        // Метод-обработчик событий мыши, вызываемый при каждом событии мыши
        protected int MouseProc(int code, IntPtr wParam, IntPtr lParam)
        {
            if (code < 0)
                return CallNextHookEx(m_hhook, code, wParam, lParam);

            if (code == Win32.HC_ACTION)
                RaiseMouseHookEvent(wParam, CrackHookMsg(wParam, (Win32.MOUSEHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(Win32.MOUSEHOOKSTRUCT))));

            return CallNextHookEx(m_hhook, code, wParam, lParam);
        }

        // Приватный метод, вызывающий соответствующее событие мыши в зависимости от полученного значения wParam
        private void RaiseMouseHookEvent(IntPtr wParam, MouseHookEventArgs args)
        {
            // Проверка значения wParam для определения типа события мыши
            switch (wParam.ToInt32())
            {
                // Движение мыши в окне
                case Win32.WM_MOUSEMOVE:
                    OnMouseMove(args);
                    break;
                // Движение мыши за пределами клиентской области окна
                case Win32.WM_NCMOUSEMOVE:
                    // Установка флага неклиентской области
                    args.SetNonClient();
                    // Вызов события движения мыши за пределами клиентской области окна
                    OnMouseMove(args);
                    break;

                // Нажатие кнопки мыши (обычные кнопки)
                case Win32.WM_LBUTTONDOWN:
                case Win32.WM_RBUTTONDOWN:
                case Win32.WM_MBUTTONDOWN:
                case Win32.WM_XBUTTONDOWN:
                    // Вызов события нажатия кнопки мыши
                    OnMouseDown(args);
                    break;

                // Нажатие кнопки мыши (неклиентские кнопки)
                case Win32.WM_NCLBUTTONDOWN:
                case Win32.WM_NCRBUTTONDOWN:
                case Win32.WM_NCMBUTTONDOWN:
                case Win32.WM_NCXBUTTONDOWN:
                    // Установка флага неклиентской области
                    args.SetNonClient();
                    // Вызов события нажатия кнопки мыши за пределами клиентской области окна
                    OnMouseDown(args);
                    break;

                // Отпускание кнопки мыши (обычные кнопки)
                case Win32.WM_LBUTTONUP:
                case Win32.WM_RBUTTONUP:
                case Win32.WM_MBUTTONUP:
                case Win32.WM_XBUTTONUP:
                    // Вызов события отпускания кнопки мыши
                    OnMouseUp(args);
                    break;

                // Отпускание кнопки мыши (неклиентские кнопки)
                case Win32.WM_NCLBUTTONUP:
                case Win32.WM_NCRBUTTONUP:
                case Win32.WM_NCMBUTTONUP:
                case Win32.WM_NCXBUTTONUP:
                    // Установка флага неклиентской области
                    args.SetNonClient();
                    // Вызов события отпускания кнопки мыши за пределами клиентской области окна
                    OnMouseUp(args);
                    break;

                // Двойной щелчок мыши (обычные кнопки)
                case Win32.WM_LBUTTONDBLCLK:
                case Win32.WM_RBUTTONDBLCLK:
                case Win32.WM_MBUTTONDBLCLK:
                case Win32.WM_XBUTTONDBLCLK:
                    // Вызов события двойного щелчка кнопки мыши
                    OnMouseDoubleClick(args);
                    break;

                // Двойной щелчок мыши (неклиентские кнопки)
                case Win32.WM_NCLBUTTONDBLCLK:
                case Win32.WM_NCRBUTTONDBLCLK:
                case Win32.WM_NCMBUTTONDBLCLK:
                case Win32.WM_NCXBUTTONDBLCLK:
                    // Установка флага неклиентской области
                    args.SetNonClient();
                    // Вызов события двойного щелчка кнопки мыши за пределами клиентской области окна
                    OnMouseDoubleClick(args);
                    break;
            }
        }

        // Приватный метод, который определяет кнопку мыши, вызвавшую событие, и создает объект MouseHookEventArgs на основе переданных параметров
        private MouseHookEventArgs CrackHookMsg(IntPtr wParam, Win32.MOUSEHOOKSTRUCT hookStruct)
        {
            // Инициализация переменной для определения кнопки мыши
            MouseButtons button = MouseButtons.None;

            // Проверка значения wParam для определения типа события мыши
            switch (wParam.ToInt32())
            {
                // События для левой кнопки мыши
                case Win32.WM_LBUTTONDBLCLK:
                case Win32.WM_LBUTTONDOWN:
                case Win32.WM_LBUTTONUP:
                case Win32.WM_NCLBUTTONDBLCLK:
                case Win32.WM_NCLBUTTONDOWN:
                case Win32.WM_NCLBUTTONUP:
                    button = MouseButtons.Left;
                    break;

                // События для правой кнопки мыши
                case Win32.WM_RBUTTONDBLCLK:
                case Win32.WM_RBUTTONDOWN:
                case Win32.WM_RBUTTONUP:
                case Win32.WM_NCRBUTTONDBLCLK:
                case Win32.WM_NCRBUTTONDOWN:
                case Win32.WM_NCRBUTTONUP:
                    button = MouseButtons.Right;
                    break;

                // События для средней кнопки мыши
                case Win32.WM_MBUTTONDBLCLK:
                case Win32.WM_MBUTTONDOWN:
                case Win32.WM_MBUTTONUP:
                case Win32.WM_NCMBUTTONDBLCLK:
                case Win32.WM_NCMBUTTONDOWN:
                case Win32.WM_NCMBUTTONUP:
                    button = MouseButtons.Middle;
                    break;

                // События для дополнительных кнопок мыши
                case Win32.WM_XBUTTONDBLCLK:
                case Win32.WM_XBUTTONDOWN:
                case Win32.WM_XBUTTONUP:
                case Win32.WM_NCXBUTTONDBLCLK:
                case Win32.WM_NCXBUTTONDOWN:
                case Win32.WM_NCXBUTTONUP:
                    // Определение дополнительных кнопок мыши
                    button = GetXButton();
                    break;
            }

            // Получение элемента управления (Control) из дескриптора окна
            Control control = Control.FromChildHandle(hookStruct.hwnd);

            // Создание и возвращение объекта MouseHookEventArgs с определенными параметрами
            return new MouseHookEventArgs(button, hookStruct.pt.x, hookStruct.pt.y, control, (HitTestCode)hookStruct.wHitTestCode);
        }

        // Приватный метод, который определяет состояние дополнительных кнопок мыши
        private MouseButtons GetXButton()
        {
            // Если нажата кнопка XButton1, возвращает MouseButtons.XButton1
            if (Win32.IsKeyDown(Win32.VK_XBUTTON1))
                return MouseButtons.XButton1;

            // Если нажата кнопка XButton2, возвращает MouseButtons.XButton2
            if (Win32.IsKeyDown(Win32.VK_XBUTTON2))
                return MouseButtons.XButton2;

            // Если ни одна из дополнительных кнопок мыши не нажата, возвращает MouseButtons.None
            return MouseButtons.None;
        }
        #endregion

        // Регион, содержащий структуры и методы из Win32 API для обработки событий мыши и работы с клавишами
        #region Win32 Imports
        private struct Win32
        {
            // Структура MOUSEHOOKSTRUCT для хранения информации о событиях мыши
            public struct MOUSEHOOKSTRUCT
            {
                public POINT pt;             // Координаты мыши
                public IntPtr hwnd;          // Дескриптор окна, над которым произошло событие
                public uint wHitTestCode;    // Код для определения места, на которое наведена мышь
                public IntPtr dwExtraInfo;   // Дополнительная информация
            }

            // Структура POINT для хранения координат
            public struct POINT
            {
                public int x;   // Координата X
                public int y;   // Координата Y
            }

            // Проверка нажатия клавиши
            public static bool IsKeyDown(short KeyCode)
            {
                // Получение состояния клавиши
                short state = GetKeyState(KeyCode);
                return ((state & 0x10000) == 0x10000);  // Возвращает true, если клавиша нажата
            }

            // Импорт функции GetAsyncKeyState из user32.dll для проверки состояния асинхронных клавиш
            [DllImport("user32.dll")]
            public static extern short GetAsyncKeyState(int nVirtKey);

            // Импорт функции GetKeyState из user32.dll для проверки состояния клавиши
            [DllImport("user32.dll")]
            public static extern short GetKeyState(int nVirtKey);

            // Получение младшего слова из значения
            public static ushort LOWORD(int l) { return (ushort)(l); }

            // Получение старшего слова из значения
            public static ushort HIWORD(int l) { return (ushort)(((int)(l) >> 16) & 0xFFFF); }

            // Константы, представляющие различные типы событий мыши
            public const int WM_MOUSEMOVE = 0x0200;

            public const int WM_LBUTTONDOWN		= 0x0201;
			public const int WM_LBUTTONUP		= 0x0202;
			public const int WM_LBUTTONDBLCLK	= 0x0203;

			public const int WM_RBUTTONDOWN		= 0x0204;
			public const int WM_RBUTTONUP		= 0x0205;
			public const int WM_RBUTTONDBLCLK	= 0x0206;

			public const int WM_MBUTTONDOWN		= 0x0207;
			public const int WM_MBUTTONUP		= 0x0208;
			public const int WM_MBUTTONDBLCLK	= 0x0209;

			public const int WM_XBUTTONDOWN		= 0x020B;
			public const int WM_XBUTTONUP		= 0x020C;
			public const int WM_XBUTTONDBLCLK	= 0x020D;

			public const int WM_NCMOUSEMOVE     = 0x00A0;
			public const int WM_NCLBUTTONDOWN   = 0x00A1;
			public const int WM_NCLBUTTONUP     = 0x00A2;
			public const int WM_NCLBUTTONDBLCLK = 0x00A3;
			public const int WM_NCRBUTTONDOWN   = 0x00A4;
			public const int WM_NCRBUTTONUP     = 0x00A5;
			public const int WM_NCRBUTTONDBLCLK = 0x00A6;
			public const int WM_NCMBUTTONDOWN   = 0x00A7;
			public const int WM_NCMBUTTONUP     = 0x00A8;
			public const int WM_NCMBUTTONDBLCLK = 0x00A9;
			public const int WM_NCXBUTTONDOWN   = 0x00AB;
			public const int WM_NCXBUTTONUP     = 0x00AC;
            public const int WM_NCXBUTTONDBLCLK = 0x00AD;

            // Константы для дополнительных кнопок мыши (XButton1 и XButton2)
            public const int VK_XBUTTON1 = 0x05;
            public const int VK_XBUTTON2 = 0x06;

            // Константы для обработки хука
            public const int HC_ACTION = 0;
            public const int HC_NOREMOVE = 3;
        }
        #endregion
    }
}
