namespace MouseHookSample
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.btnStartMouseHook = new System.Windows.Forms.Button();
            this.btnStopMouseHook = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnGUIProcess = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnStartMouseHook
            // 
            this.btnStartMouseHook.Location = new System.Drawing.Point(334, 119);
            this.btnStartMouseHook.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnStartMouseHook.Name = "btnStartMouseHook";
            this.btnStartMouseHook.Size = new System.Drawing.Size(171, 29);
            this.btnStartMouseHook.TabIndex = 0;
            this.btnStartMouseHook.Text = "Запустить хук мыши";
            this.btnStartMouseHook.UseVisualStyleBackColor = true;
            this.btnStartMouseHook.Click += new System.EventHandler(this.btnStartMouseHook_Click);
            // 
            // btnStopMouseHook
            // 
            this.btnStopMouseHook.Location = new System.Drawing.Point(334, 152);
            this.btnStopMouseHook.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnStopMouseHook.Name = "btnStopMouseHook";
            this.btnStopMouseHook.Size = new System.Drawing.Size(171, 29);
            this.btnStopMouseHook.TabIndex = 1;
            this.btnStopMouseHook.Text = "Отключить хук мыши";
            this.btnStopMouseHook.UseVisualStyleBackColor = true;
            this.btnStopMouseHook.Click += new System.EventHandler(this.btnStopMouseHook_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(31, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(762, 64);
            this.label1.TabIndex = 2;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // btnGUIProcess
            // 
            this.btnGUIProcess.Location = new System.Drawing.Point(168, 321);
            this.btnGUIProcess.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnGUIProcess.Name = "btnGUIProcess";
            this.btnGUIProcess.Size = new System.Drawing.Size(485, 29);
            this.btnGUIProcess.TabIndex = 3;
            this.btnGUIProcess.Text = "Запустите длительный процесс в потоке графического интерфейса пользователя";
            this.btnGUIProcess.UseVisualStyleBackColor = true;
            this.btnGUIProcess.Click += new System.EventHandler(this.btnGUIProcess_Click);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(31, 220);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(762, 64);
            this.label2.TabIndex = 4;
            this.label2.Text = resources.GetString("label2.Text");
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(835, 396);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnGUIProcess);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnStopMouseHook);
            this.Controls.Add(this.btnStartMouseHook);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "Form1";
            this.Text = "ХУК";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnStartMouseHook;
        private System.Windows.Forms.Button btnStopMouseHook;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnGUIProcess;
        private System.Windows.Forms.Label label2;
    }
}

