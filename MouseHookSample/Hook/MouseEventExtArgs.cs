using System.Windows.Forms;

/* Представляет класс MouseEventExtArgs, который расширяет класс MouseEventArgs из пространства имен System.Windows.Forms. 
 * Этот класс предоставляет дополнительную функциональность для обработки событий мыши, таких как MouseClickExt и MouseMoveExt.
 * 
 * Основные моменты этого класса:
 * 
 * Он имеет конструкторы, которые расширяют базовый класс MouseEventArgs, позволяя передавать дополнительные аргументы, такие как buttons (нажатие кнопок мыши), clicks (количество кликов), 
 * x и y (координаты мыши) и delta (для колеса прокрутки).
 * 
 * Handled: Это свойство типа bool, которое может быть установлено в true внутри обработчика события для предотвращения дальнейшей обработки события в других приложениях. 
 * Это полезно, если вы хотите отменить дальнейшую обработку события мыши после того, как оно было обработано в вашем приложении.
 * 
 * Этот класс помогает расширить стандартные события мыши и добавить дополнительную логику обработки этих событий в вашем приложении, позволяя управлять поведением обработки событий мыши. */

namespace MouseHookSample
{
    // Класс, расширяющий MouseEventArgs и добавляющий дополнительную функциональность
    public class MouseEventExtArgs : MouseEventArgs
    {
        // Конструктор, расширяющий базовый класс MouseEventArgs для передачи дополнительных аргументов
        public MouseEventExtArgs(MouseButtons buttons, int clicks, int x, int y, int delta)
            : base(buttons, clicks, x, y, delta)
        { }

        // Внутренний конструктор, использующий базовый конструктор MouseEventArgs
        internal MouseEventExtArgs(MouseEventArgs e) : base(e.Button, e.Clicks, e.X, e.Y, e.Delta)
        { }

        private bool m_Handled; // Приватное поле типа bool

        // Свойство Handled типа bool, позволяющее управлять дальнейшей обработкой события
        public bool Handled
        {
            get { return m_Handled; }
            set { m_Handled = value; }
        }
    }
}