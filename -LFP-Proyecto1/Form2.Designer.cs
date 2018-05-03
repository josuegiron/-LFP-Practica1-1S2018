namespace _LFP_Proyecto1
{
    partial class Seleccion
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
            this.tabA = new System.Windows.Forms.ComboBox();
            this.tabB = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // tabA
            // 
            this.tabA.FormattingEnabled = true;
            this.tabA.Location = new System.Drawing.Point(38, 78);
            this.tabA.Name = "tabA";
            this.tabA.Size = new System.Drawing.Size(170, 21);
            this.tabA.TabIndex = 0;
            // 
            // tabB
            // 
            this.tabB.FormattingEnabled = true;
            this.tabB.Location = new System.Drawing.Point(269, 78);
            this.tabB.Name = "tabB";
            this.tabB.Size = new System.Drawing.Size(170, 21);
            this.tabB.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(363, 135);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Comparar";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Seleccion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(481, 182);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.tabB);
            this.Controls.Add(this.tabA);
            this.MaximizeBox = false;
            this.Name = "Seleccion";
            this.Text = "Seleccionar lenguajes para comparar";
            this.Load += new System.EventHandler(this.Form2_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox tabA;
        private System.Windows.Forms.ComboBox tabB;
        private System.Windows.Forms.Button button1;
    }
}