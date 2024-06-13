using System;
using System.Windows.Forms;

/* Представляет статический класс HookManager, который предоставляет события для отслеживания активности мыши на глобальном уровне в приложении Windows Forms.
 * Для мыши (Mouse) реализованы события для отслеживания перемещения, кликов, нажатия и отпускания кнопок, вращения колеса и двойных кликов.
 * Каждое событие включает методы add и remove, чтобы подписывать и отписывать обработчики событий. Например, MouseMove, MouseClick, MouseDown, MouseUp, MouseWheel, MouseDoubleClick.
 * Этот код использует глобальные хуки Windows для перехвата событий мыши вне пределов текущего приложения. 
 * Он также реализует логику для обработки двойных кликов мышью, используя таймер для определения интервала между двумя последовательными кликами. */

namespace MouseHookSample
{
    public static partial class HookManager
    {
        private static event MouseEventHandler s_MouseMove; // Событие для перемещения мыши

        public static event MouseEventHandler MouseMove // Событие для перемещения мыши
        {
            add
            {
                EnsureSubscribedToGlobalMouseEvents(); // Убедиться в подписке на глобальные события мыши
                s_MouseMove += value; // Добавить обработчик события
            }

            remove
            {
                s_MouseMove -= value; // Удалить обработчик события
                TryUnsubscribeFromGlobalMouseEvents(); // Попытаться отписаться от глобальных событий мыши
            }
        }

        private static event EventHandler<MouseEventExtArgs> s_MouseMoveExt; // Событие для расширенного перемещения мыши

        public static event EventHandler<MouseEventExtArgs> MouseMoveExt // Событие для расширенного перемещения мыши
        {
            add
            {
                EnsureSubscribedToGlobalMouseEvents(); // Убедиться в подписке на глобальные события мыши
                s_MouseMoveExt += value; // Добавить обработчик события
            }

            remove
            {
                s_MouseMoveExt -= value; // Удалить обработчик события
                TryUnsubscribeFromGlobalMouseEvents(); // Попытаться отписаться от глобальных событий мыши
            }
        }

        private static event MouseEventHandler s_MouseClick; // Событие для клика мыши

        public static event MouseEventHandler MouseClick // Событие для клика мыши
        {
            add
            {
                EnsureSubscribedToGlobalMouseEvents(); // Убедиться в подписке на глобальные события мыши
                s_MouseClick += value; // Добавить обработчик события
            }
            remove
            {
                s_MouseClick -= value; // Удалить обработчик события
                TryUnsubscribeFromGlobalMouseEvents(); // Попытаться отписаться от глобальных событий мыши
            }
        }

        // Объявление события для дополнительного клика мыши
        private static event EventHandler<MouseEventExtArgs> s_MouseClickExt;

        // Событие для дополнительного клика мыши
        public static event EventHandler<MouseEventExtArgs> MouseClickExt
        {
            // Добавление обработчика события
            add
            {
                EnsureSubscribedToGlobalMouseEvents(); // Гарантирует подписку на глобальные события мыши
                s_MouseClickExt += value;
            }
            // Удаление обработчика события
            remove
            {
                s_MouseClickExt -= value;
                TryUnsubscribeFromGlobalMouseEvents(); // Пытается отписаться от глобальных событий мыши
            }
        }

        // Объявление события для нажатия кнопки мыши
        private static event MouseEventHandlerEx s_MouseDown;

        // Событие для нажатия кнопки мыши
        public static event MouseEventHandlerEx MouseDown
        {
            // Добавление обработчика события
            add
            {
                EnsureSubscribedToGlobalMouseEvents(); // Гарантирует подписку на глобальные события мыши
                s_MouseDown += value;
            }
            // Удаление обработчика события
            remove
            {
                s_MouseDown -= value;
                TryUnsubscribeFromGlobalMouseEvents(); // Пытается отписаться от глобальных событий мыши
            }
        }

        // Объявление события для отпускания кнопки мыши
        private static event MouseEventHandler s_MouseUp;

        // Событие для отпускания кнопки мыши
        public static event MouseEventHandler MouseUp
        {
            // Добавление обработчика события
            add
            {
                EnsureSubscribedToGlobalMouseEvents(); // Гарантирует подписку на глобальные события мыши
                s_MouseUp += value;
            }
            // Удаление обработчика события
            remove
            {
                s_MouseUp -= value;
                TryUnsubscribeFromGlobalMouseEvents(); // Пытается отписаться от глобальных событий мыши
            }
        }

        // Объявление события для обработки прокрутки колеса мыши
        private static event MouseEventHandlerEx s_MouseWheel;

        // Событие для обработки прокрутки колеса мыши
        public static event MouseEventHandlerEx MouseWheel
        {
            // Добавление обработчика события
            add
            {
                EnsureSubscribedToGlobalMouseEvents(); // Гарантирует подписку на глобальные события мыши
                s_MouseWheel += value;
            }
            // Удаление обработчика события
            remove
            {
                s_MouseWheel -= value;
                TryUnsubscribeFromGlobalMouseEvents(); // Пытается отписаться от глобальных событий мыши
            }
        }

        // Объявление события для двойного клика мыши
        private static event MouseEventHandler s_MouseDoubleClick;

        // Событие для двойного клика мыши
        public static event MouseEventHandler MouseDoubleClick
        {
            // Добавление обработчика события
            add
            {
                EnsureSubscribedToGlobalMouseEvents(); // Гарантирует подписку на глобальные события мыши
                if (s_MouseDoubleClick == null)
                {
                    // Инициализация таймера для двойных кликов
                    s_DoubleClickTimer = new Timer
                    {
                        Interval = GetDoubleClickTime(), // Устанавливает интервал времени для двойного клика
                        Enabled = false // Отключает таймер
                    };
                    s_DoubleClickTimer.Tick += DoubleClickTimeElapsed; // Добавляет обработчик события истечения времени таймера
                    MouseUp += OnMouseUp; // Добавляет обработчик события отпускания кнопки мыши
                }
                s_MouseDoubleClick += value; // Добавляет обработчик для двойного клика мыши
            }
            // Удаление обработчика события
            remove
            {
                if (s_MouseDoubleClick != null)
                {
                    s_MouseDoubleClick -= value; // Удаляет обработчик для двойного клика мыши
                    if (s_MouseDoubleClick == null)
                    {
                        MouseUp -= OnMouseUp; // Удаляет обработчик события отпускания кнопки мыши
                        s_DoubleClickTimer.Tick -= DoubleClickTimeElapsed; // Удаляет обработчик события истечения времени таймера
                        s_DoubleClickTimer = null; // Обнуляет таймер для двойных кликов
                    }
                }
                TryUnsubscribeFromGlobalMouseEvents(); // Пытается отписаться от глобальных событий мыши
            }
        }

        // Переменная для хранения предыдущей нажатой кнопки мыши
        private static MouseButtons s_PrevClickedButton;

        // Таймер для отслеживания двойных кликов мыши
        private static Timer s_DoubleClickTimer;

        // Метод, вызываемый при истечении времени таймера двойных кликов
        private static void DoubleClickTimeElapsed(object sender, EventArgs e)
        {
            s_DoubleClickTimer.Enabled = false; // Отключает таймер двойных кликов
            s_PrevClickedButton = MouseButtons.None; // Сбрасывает предыдущую нажатую кнопку мыши
        }

        // Метод, вызываемый при отпускании кнопки мыши
        private static void OnMouseUp(object sender, MouseEventArgs e)
        {
            // Если количество кликов меньше 1, просто выйти из метода
            if (e.Clicks < 1) { return; }

            // Если текущая кнопка совпадает с предыдущей
            if (e.Button.Equals(s_PrevClickedButton))
            {
                // Если есть подписчики на событие MouseDoubleClick
                if (s_MouseDoubleClick != null)
                {
                    s_MouseDoubleClick.Invoke(null, e); // Вызвать событие MouseDoubleClick
                }
                s_DoubleClickTimer.Enabled = false; // Отключает таймер двойных кликов
                s_PrevClickedButton = MouseButtons.None; // Сбрасывает предыдущую нажатую кнопку мыши
            }
            else
            {
                s_DoubleClickTimer.Enabled = true; // Включает таймер двойных кликов
                s_PrevClickedButton = e.Button; // Сохраняет текущую кнопку мыши
            }
        }
    }
}
