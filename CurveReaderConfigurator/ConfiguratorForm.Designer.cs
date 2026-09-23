namespace CurveReaderConfigurator
{
    partial class ConfiguratorForm
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.configurationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.languageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.englishToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.spanishToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fanControlPathToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewHelpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.profileLabel = new System.Windows.Forms.Label();
            this.profilesComboBox = new System.Windows.Forms.ComboBox();
            this.profileConfigPanel = new System.Windows.Forms.Panel();
            this.refreshProfilesButton = new System.Windows.Forms.Button();
            this.mainToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.configurationToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(9, 4, 0, 4);
            this.menuStrip1.Size = new System.Drawing.Size(784, 27);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(60, 19);
            this.fileToolStripMenuItem.Text = "Archivo";
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.saveToolStripMenuItem.Text = "Guardar";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.SaveToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(113, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.exitToolStripMenuItem.Text = "Salir";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItem_Click);
            // 
            // configurationToolStripMenuItem
            // 
            this.configurationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.languageToolStripMenuItem,
            this.fanControlPathToolStripMenuItem});
            this.configurationToolStripMenuItem.Name = "configurationToolStripMenuItem";
            this.configurationToolStripMenuItem.Size = new System.Drawing.Size(95, 19);
            this.configurationToolStripMenuItem.Text = "Configuracion";
            // 
            // languageToolStripMenuItem
            // 
            this.languageToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.englishToolStripMenuItem,
            this.spanishToolStripMenuItem});
            this.languageToolStripMenuItem.Name = "languageToolStripMenuItem";
            this.languageToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
            this.languageToolStripMenuItem.Text = "Idioma";
            // 
            // englishToolStripMenuItem
            // 
            this.englishToolStripMenuItem.Name = "englishToolStripMenuItem";
            this.englishToolStripMenuItem.Size = new System.Drawing.Size(115, 22);
            this.englishToolStripMenuItem.Text = "Inglés";
            this.englishToolStripMenuItem.Click += new System.EventHandler(this.EnglishToolStripMenuItem_Click);
            // 
            // spanishToolStripMenuItem
            // 
            this.spanishToolStripMenuItem.Name = "spanishToolStripMenuItem";
            this.spanishToolStripMenuItem.Size = new System.Drawing.Size(115, 22);
            this.spanishToolStripMenuItem.Text = "Español";
            this.spanishToolStripMenuItem.Click += new System.EventHandler(this.SpanishToolStripMenuItem_Click);
            // 
            // fanControlPathToolStripMenuItem
            // 
            this.fanControlPathToolStripMenuItem.Name = "fanControlPathToolStripMenuItem";
            this.fanControlPathToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
            this.fanControlPathToolStripMenuItem.Text = "Especificar Ruta Fan Control";
            this.fanControlPathToolStripMenuItem.Click += new System.EventHandler(this.FanControlPathToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewHelpToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(53, 19);
            this.helpToolStripMenuItem.Text = "Ayuda";
            // 
            // viewHelpToolStripMenuItem
            // 
            this.viewHelpToolStripMenuItem.Name = "viewHelpToolStripMenuItem";
            this.viewHelpToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.viewHelpToolStripMenuItem.Text = "Ver la Ayuda";
            this.viewHelpToolStripMenuItem.Click += new System.EventHandler(this.ViewHelpToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.aboutToolStripMenuItem.Text = "Acerca de...";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.AboutToolStripMenuItem_Click);
            // 
            // profileLabel
            // 
            this.profileLabel.AutoSize = true;
            this.profileLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.profileLabel.Location = new System.Drawing.Point(59, 57);
            this.profileLabel.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.profileLabel.Name = "profileLabel";
            this.profileLabel.Size = new System.Drawing.Size(62, 26);
            this.profileLabel.TabIndex = 1;
            this.profileLabel.Text = "Perfil";
            // 
            // profilesComboBox
            // 
            this.profilesComboBox.BackColor = System.Drawing.SystemColors.Window;
            this.profilesComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.profilesComboBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.profilesComboBox.ForeColor = System.Drawing.SystemColors.WindowText;
            this.profilesComboBox.FormattingEnabled = true;
            this.profilesComboBox.Location = new System.Drawing.Point(140, 57);
            this.profilesComboBox.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.profilesComboBox.Name = "profilesComboBox";
            this.profilesComboBox.Size = new System.Drawing.Size(230, 29);
            this.profilesComboBox.TabIndex = 2;
            this.profilesComboBox.SelectedIndexChanged += new System.EventHandler(this.ProfilesComboBox_SelectedIndexChanged);
            // 
            // profileConfigPanel
            // 
            this.profileConfigPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.profileConfigPanel.AutoScroll = true;
            this.profileConfigPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.profileConfigPanel.Location = new System.Drawing.Point(50, 125);
            this.profileConfigPanel.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.profileConfigPanel.Name = "profileConfigPanel";
            this.profileConfigPanel.Size = new System.Drawing.Size(685, 400);
            this.profileConfigPanel.TabIndex = 3;
            // 
            // refreshProfilesButton
            // 
            this.refreshProfilesButton.Font = new System.Drawing.Font("Segoe UI Symbol", 12F);
            this.refreshProfilesButton.Location = new System.Drawing.Point(390, 57);
            this.refreshProfilesButton.Name = "refreshProfilesButton";
            this.refreshProfilesButton.Size = new System.Drawing.Size(29, 29);
            this.refreshProfilesButton.TabIndex = 4;
            this.refreshProfilesButton.Text = "↻";
            this.refreshProfilesButton.UseVisualStyleBackColor = true;
            this.refreshProfilesButton.Click += new System.EventHandler(this.RefreshProfilesButton_Click);
            // 
            // ConfiguratorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.refreshProfilesButton);
            this.Controls.Add(this.profileConfigPanel);
            this.Controls.Add(this.profilesComboBox);
            this.Controls.Add(this.profileLabel);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "ConfiguratorForm";
            this.Text = "CurveReader Configurator";
            this.Load += new System.EventHandler(this.ConfiguratorForm_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewHelpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem configurationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fanControlPathToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem languageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem spanishToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem englishToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.Label profileLabel;
        private System.Windows.Forms.ComboBox profilesComboBox;
        private System.Windows.Forms.Panel profileConfigPanel;
        private System.Windows.Forms.Button refreshProfilesButton;
        private System.Windows.Forms.ToolTip mainToolTip;
    }
}

