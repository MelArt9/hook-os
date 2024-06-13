using System;
using System.Threading;
using System.Windows.Forms;

namespace MouseHookSample
{
    static class Program
    {
        // Установка статического метода Main в качестве точки входа приложения
        [STAThread]
        static void Main()
        {
            // Включение визуальных стилей приложения
            Application.EnableVisualStyles();
            // Установка совместимости текста с рендерингом по умолчанию
            Application.SetCompatibleTextRenderingDefault(false);
            // Запуск главной формы приложения
            Application.Run(new Form1());
        }

        // Статическая переменная, отвечающая за поток перехвата мыши
        public static Thread MouseHook = null;
        // Переменная для остановки потока перехвата мыши
        private static bool blnStopMouseHook = false;

        // Метод запуска потока перехвата мыши
        public static void StartMouseHook()
        {
            // Проверка, что поток перехвата мыши еще не создан
            if (MouseHook == null)
            {
                // Установка флага остановки потока в false
                blnStopMouseHook = false;
                // Создание нового потока для перехвата мыши
                MouseHook = new Thread(new ThreadStart(() => { MouseHookThread(); }));
                // Установка режима апартаментной модели для потока
                MouseHook.SetApartmentState(ApartmentState.STA);
                // Запуск потока
                MouseHook.Start();
            }
        }

        // Метод остановки потока перехвата мыши
        public static void StopMouseHook()
        {
            // Проверка, что поток перехвата мыши существует
            if (MouseHook != null)
            {
                // Установка флага остановки потока в true
                blnStopMouseHook = true;
                // Ожидание завершения потока
                MouseHook.Join();
                // Очистка ссылки на поток
                MouseHook = null;
            }
        }

        // Метод потока для перехвата мыши
        private static void MouseHookThread()
        {
            // Присоединение обработчика события нажатия мыши
            HookManager.MouseClickExt += HookHandlers.HookManagerOnMouseClickExt;
            // Цикл перехвата, выполняющийся до остановки потока
            do
            {
                System.Threading.Thread.Sleep(1);
                Application.DoEvents();
            } while (blnStopMouseHook == false);

            // Отсоединение обработчика события нажатия мыши
            HookManager.MouseClickExt -= HookHandlers.HookManagerOnMouseClickExt;
        }
    }
}