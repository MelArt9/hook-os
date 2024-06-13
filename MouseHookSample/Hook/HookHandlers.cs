using System.Threading;
using System.Windows.Forms;

/* Обрабатывает события нажатия правой кнопки мыши. Он отслеживает два события: нажатие и отпускание кнопки. 
 * Если пользователь нажимает правую кнопку мыши, код обрабатывает это событие дважды (для нажатия и отпускания). 
 * Он выполняет вывод сообщения только при первом нажатии, игнорируя последующее отпускание кнопки. */

namespace MouseHookSample
{
    internal static class HookHandlers
    {
        private static int iKeyPress; // Переменная, отслеживающая текущее состояние нажатия кнопки

        // Обработчик события нажатия кнопки мыши (вызывается при нажатии и отпускании кнопки)
        public static void HookManagerOnMouseClickExt(object sender, MouseEventExtArgs mouseEventExtArgs)
        {
            // Проверка, что нажата правая кнопка мыши
            if (mouseEventExtArgs.Button == MouseButtons.Right)
            {
                // Если iKeyPress равен 1, значит это первое нажатие кнопки
                if (iKeyPress == 1)
                {
                    // Запуск нового потока для выполнения действия
                    new Thread(() => { ExecuteAction(); }).Start();

                    // Пометить событие как обработанное
                    mouseEventExtArgs.Handled = true;

                    // Сброс переменной iKeyPress для следующего нажатия кнопки
                    iKeyPress = 0;

                    // Возврат из метода
                    return;
                }
                // Если iKeyPress не равен 1, значит это второе нажатие кнопки
                else
                {
                    // Установка значения iKeyPress в 1 для отслеживания первого нажатия кнопки в следующий раз
                    iKeyPress = 1;

                    // Пометить событие как обработанное
                    mouseEventExtArgs.Handled = true;

                    // Возврат из метода
                    return;
                }
            }
        }

        // Метод для выполнения определенного действия при нажатии правой кнопки мыши
        private static void ExecuteAction()
        {
            MouseInputAction.DisplayMessage("Вы щелкнули правой кнопкой мыши");
        }
    }
}