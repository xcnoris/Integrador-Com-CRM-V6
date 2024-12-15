namespace Integrador_Com_CRM
{
    partial class Frm_Tela_Principal
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_Tela_Principal));
            TBC_Dados = new TabControl();
            Btn_Salvar = new Button();
            Btn_Sair = new Button();
            notifyIcon1 = new NotifyIcon(components);
            SuspendLayout();
            // 
            // TBC_Dados
            // 
            TBC_Dados.Dock = DockStyle.Top;
            TBC_Dados.Location = new Point(0, 0);
            TBC_Dados.Name = "TBC_Dados";
            TBC_Dados.SelectedIndex = 0;
            TBC_Dados.Size = new Size(834, 446);
            TBC_Dados.TabIndex = 0;
            // 
            // Btn_Salvar
            // 
            Btn_Salvar.Location = new Point(300, 452);
            Btn_Salvar.Name = "Btn_Salvar";
            Btn_Salvar.Size = new Size(113, 33);
            Btn_Salvar.TabIndex = 1;
            Btn_Salvar.Text = "Salvar";
            Btn_Salvar.UseVisualStyleBackColor = true;
            Btn_Salvar.Click += Btn_Salvar_Click;
            // 
            // Btn_Sair
            // 
            Btn_Sair.Location = new Point(428, 452);
            Btn_Sair.Name = "Btn_Sair";
            Btn_Sair.Size = new Size(113, 33);
            Btn_Sair.TabIndex = 2;
            Btn_Sair.Text = "Fechar";
            Btn_Sair.UseVisualStyleBackColor = true;
            Btn_Sair.Click += Btn_Sair_Click;
            // 
            // notifyIcon1
            // 
            notifyIcon1.Icon = (Icon)resources.GetObject("notifyIcon1.Icon");
            notifyIcon1.Text = "Integrador Com CRM";
            notifyIcon1.Visible = true;
            notifyIcon1.MouseDoubleClick += notifyIcon1_MouseDoubleClick;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.ForeColor = SystemColors.Desktop;
            label7.Location = new Point(770, 463);
            label7.Name = "label7";
            label7.Size = new Size(52, 15);
            label7.TabIndex = 9;
            label7.Text = "2.24.12.2";
            // 
            // Frm_Tela_Principal
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ActiveBorder;
            ClientSize = new Size(834, 487);
            Controls.Add(Btn_Sair);
            Controls.Add(Btn_Salvar);
            Controls.Add(TBC_Dados);
            MaximumSize = new Size(850, 526);
            MinimumSize = new Size(850, 526);
            Name = "Frm_Tela_Principal";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Frm_Tela_Principal";
            ResumeLayout(false);
        }

        #endregion

        private TabControl TBC_Dados;
        private Button Btn_Salvar;
        private Button Btn_Sair;
        private NotifyIcon notifyIcon1;
    }
}