namespace InputRecorder
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            ckKeyTyped = new CheckBox();
            btnHook = new Button();
            txtFilter = new TextBox();
            btnFile = new Button();
            txtFile = new TextBox();
            lstFilter = new ListBox();
            btnFilter = new Button();
            txtLog = new TextBox();
            lstFilterSwitch = new ListBox();
            txtFilterSwitch = new TextBox();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            lbHook = new Label();
            SuspendLayout();
            // 
            // ckKeyTyped
            // 
            ckKeyTyped.AutoSize = true;
            ckKeyTyped.Location = new Point(12, 12);
            ckKeyTyped.Name = "ckKeyTyped";
            ckKeyTyped.Size = new Size(131, 21);
            ckKeyTyped.TabIndex = 0;
            ckKeyTyped.Text = "KeyTypedEnabled";
            ckKeyTyped.UseVisualStyleBackColor = true;
            ckKeyTyped.CheckedChanged += ckKeyTyped_CheckedChanged;
            // 
            // btnHook
            // 
            btnHook.Location = new Point(492, 12);
            btnHook.Name = "btnHook";
            btnHook.Size = new Size(80, 24);
            btnHook.TabIndex = 2;
            btnHook.Text = "Run Hook";
            btnHook.UseVisualStyleBackColor = true;
            btnHook.Click += btnHook_Click;
            // 
            // txtFilter
            // 
            txtFilter.Location = new Point(372, 42);
            txtFilter.Name = "txtFilter";
            txtFilter.Size = new Size(200, 23);
            txtFilter.TabIndex = 9;
            // 
            // btnFile
            // 
            btnFile.Location = new Point(12, 145);
            btnFile.Name = "btnFile";
            btnFile.Size = new Size(79, 24);
            btnFile.TabIndex = 11;
            btnFile.Text = "Output File";
            btnFile.UseVisualStyleBackColor = true;
            btnFile.Click += btnFile_Click;
            // 
            // txtFile
            // 
            txtFile.Location = new Point(97, 146);
            txtFile.Name = "txtFile";
            txtFile.Size = new Size(475, 23);
            txtFile.TabIndex = 12;
            txtFile.Leave += txtFile_Leave;
            // 
            // lstFilter
            // 
            lstFilter.FormattingEnabled = true;
            lstFilter.Location = new Point(372, 68);
            lstFilter.Name = "lstFilter";
            lstFilter.Size = new Size(200, 72);
            lstFilter.TabIndex = 10;
            // 
            // btnFilter
            // 
            btnFilter.Location = new Point(12, 39);
            btnFilter.Name = "btnFilter";
            btnFilter.Size = new Size(148, 24);
            btnFilter.TabIndex = 6;
            btnFilter.Text = "Add Filter Regex";
            btnFilter.UseVisualStyleBackColor = true;
            // 
            // txtLog
            // 
            txtLog.Location = new Point(12, 175);
            txtLog.Multiline = true;
            txtLog.Name = "txtLog";
            txtLog.ReadOnly = true;
            txtLog.ScrollBars = ScrollBars.Vertical;
            txtLog.Size = new Size(560, 174);
            txtLog.TabIndex = 13;
            // 
            // lstFilterSwitch
            // 
            lstFilterSwitch.FormattingEnabled = true;
            lstFilterSwitch.Location = new Point(166, 68);
            lstFilterSwitch.Name = "lstFilterSwitch";
            lstFilterSwitch.Size = new Size(200, 72);
            lstFilterSwitch.TabIndex = 8;
            // 
            // txtFilterSwitch
            // 
            txtFilterSwitch.Location = new Point(166, 42);
            txtFilterSwitch.Name = "txtFilterSwitch";
            txtFilterSwitch.Size = new Size(200, 23);
            txtFilterSwitch.TabIndex = 7;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 76);
            label1.Name = "label1";
            label1.Size = new Size(143, 17);
            label1.TabIndex = 3;
            label1.Text = "Combined keys are OK";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 93);
            label2.Name = "label2";
            label2.Size = new Size(107, 17);
            label2.TabIndex = 4;
            label2.Text = "Left for start/end";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 110);
            label3.Name = "label3";
            label3.Size = new Size(101, 17);
            label3.TabIndex = 5;
            label3.Text = "Right for output";
            // 
            // lbHook
            // 
            lbHook.AutoSize = true;
            lbHook.Location = new Point(242, 16);
            lbHook.Name = "lbHook";
            lbHook.Size = new Size(244, 17);
            lbHook.TabIndex = 1;
            lbHook.Text = "Ending hook won't save unstopped data";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(584, 361);
            Controls.Add(lbHook);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(lstFilterSwitch);
            Controls.Add(txtFilterSwitch);
            Controls.Add(txtLog);
            Controls.Add(btnFilter);
            Controls.Add(lstFilter);
            Controls.Add(txtFile);
            Controls.Add(btnFile);
            Controls.Add(txtFilter);
            Controls.Add(btnHook);
            Controls.Add(ckKeyTyped);
            Name = "MainForm";
            Text = "Input Recorder";
            FormClosing += MainForm_FormClosing;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private CheckBox ckKeyTyped;
        private Button btnHook;
        private TextBox txtFilter;
        private Button btnFile;
        private TextBox txtFile;
        private ListBox lstFilter;
        private Button btnFilter;
        private TextBox txtLog;
        private ListBox lstFilterSwitch;
        private TextBox txtFilterSwitch;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label lbHook;
    }
}
