// Импорт пространства имен System.Windows.Forms для использования типа MouseButtons и Control
using System.Windows.Forms;

/* Представляет класс MouseHookEventArgs, который содержит значения, передаваемые объекту MouseHook в метод MouseProc, упакованные как типы .NET. Вот ключевые аспекты этого класса:
 * 
 * Поля:
 * 
 * Button: Кнопка мыши, связанная с событием.
 * X и Y: Координаты положения мыши в момент события.
 * Control: Элемент управления, который в конечном итоге получит перехваченное сообщение.
 * HitTestCode: Область окна, над которой находится указатель мыши.
 * 
 * Методы:
 * 
 * MouseHookEventArgs(...): Конструктор класса, который инициализирует поля значениями, переданными из MouseHook.
 * SetNonClient(): Помогает определить, произошло ли событие над областью не клиентской части окна.
 * Этот класс MouseHookEventArgs используется для передачи информации о событиях мыши, происходящих в рамках объекта MouseHook, и предоставляет обертку над значениями, 
 * которые передаются внутреннему обработчику событий мыши. Он содержит информацию о положении мыши, кнопке мыши и элементе управления, а также области окна, над которой происходит событие. */

// Объявление пространства имен Microsoft.Win32, в котором содержится класс MouseHookEventArgs
namespace Microsoft.Win32
{
    // Объявление публичного класса MouseHookEventArgs
    public class MouseHookEventArgs
    {
        // Публичное только для чтения поле Button типа MouseButtons, представляющее кнопку мыши
        public readonly MouseButtons Button = MouseButtons.None;

        // Публичное только для чтения поле X типа int, представляющее координату X
        public readonly int X = 0;

        // Публичное только для чтения поле Y типа int, представляющее координату Y
        public readonly int Y = 0;

        // Публичное только для чтения поле Control типа Control, представляющее элемент управления
        public readonly Control Control = null;

        // Публичное только для чтения поле HitTestCode типа HitTestCode, представляющее код тестирования попадания
        public readonly HitTestCode HitTestCode = HitTestCode.HTNOWHERE;

        // Приватное поле isNonClientArea типа bool, используемое для определения не клиентской области окна
        private bool isNonClientArea = false;

        // Публичный конструктор класса MouseHookEventArgs, инициализирующий поля значениями, переданными из MouseHook
        public MouseHookEventArgs(MouseButtons button, int x, int y, Control control, HitTestCode hitTestCode)
        {
            Button = button;
            X = x;
            Y = y;
            Control = control;
            HitTestCode = hitTestCode;
        }

        // Внутренний метод SetNonClient(), помогающий определить, произошло ли событие вне клиентской области окна
        internal void SetNonClient() { isNonClientArea = true; }
    }
}