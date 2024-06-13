using System;
using System.Windows.Forms;

namespace MouseHookSample
{
    public partial class Form1 : Form
    {
        // Конструктор класса Form1, вызывается при его создании
        public Form1()
        {
            InitializeComponent(); // Инициализация компонентов формы
        }

        // Обработчик нажатия кнопки "Start Mouse Hook"
        private void btnStartMouseHook_Click(object sender, EventArgs e)
        {
            Program.StartMouseHook(); // Запуск перехвата мыши
            // Вывод сообщения с инструкциями пользователю
            MessageBox.Show(
                "Хук мыши был запущен в отдельном потоке. Теперь, когда вы щелкаете правой кнопкой мыши, вы увидите окно сообщения, показывающее, что хук захватил его. Когда вы закроете форму, хук будет выгружен, поэтому не беспокойтесь о нажатии StopMouseHook.");
        }

        // Обработчик нажатия кнопки "Stop Mouse Hook"
        private void btnStopMouseHook_Click(object sender, EventArgs e)
        {
            Program.StopMouseHook(); // Остановка перехвата мыши
        }

        // Обработчик события закрытия формы
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Program.StopMouseHook(); // Остановка перехвата мыши при закрытии формы
        }

        // Обработчик нажатия кнопки "btnGUIProcess"
        private void btnGUIProcess_Click(object sender, EventArgs e)
        {
            // Пауза в выполнении GUI на 5 секунд для демонстрации
            for (int i = 0; i <= 9; i++)
            {
                System.Threading.Thread.Sleep(500);
            }
        }
    }
}