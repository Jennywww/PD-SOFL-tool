
namespace XMLFormsApp
{
    partial class ProcessForm
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
            this.nameTextBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.preButton = new System.Windows.Forms.Button();
            this.NextButton = new System.Windows.Forms.Button();
            this.preRadioButton = new System.Windows.Forms.RadioButton();
            this.label4 = new System.Windows.Forms.Label();
            this.operationButton = new System.Windows.Forms.Button();
            this.postTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.outputTextBox = new System.Windows.Forms.TextBox();
            this.inputTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // nameTextBox
            // 
            this.nameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.nameTextBox.Location = new System.Drawing.Point(128, 30);
            this.nameTextBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.nameTextBox.Name = "nameTextBox";
            this.nameTextBox.Size = new System.Drawing.Size(229, 21);
            this.nameTextBox.TabIndex = 47;
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(68, 34);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(38, 15);
            this.label5.TabIndex = 46;
            this.label5.Text = "Name:";
            // 
            // preButton
            // 
            this.preButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.preButton.Location = new System.Drawing.Point(8, 215);
            this.preButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.preButton.Name = "preButton";
            this.preButton.Size = new System.Drawing.Size(52, 29);
            this.preButton.TabIndex = 45;
            this.preButton.Text = "pre";
            this.preButton.UseVisualStyleBackColor = true;
            this.preButton.Click += new System.EventHandler(this.preButton_Click);
            // 
            // NextButton
            // 
            this.NextButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.NextButton.Location = new System.Drawing.Point(378, 215);
            this.NextButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.NextButton.Name = "NextButton";
            this.NextButton.Size = new System.Drawing.Size(54, 29);
            this.NextButton.TabIndex = 44;
            this.NextButton.Text = "Next";
            this.NextButton.UseVisualStyleBackColor = true;
            this.NextButton.Click += new System.EventHandler(this.NextButton_Click);
            // 
            // preRadioButton
            // 
            this.preRadioButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.preRadioButton.AutoSize = true;
            this.preRadioButton.Location = new System.Drawing.Point(128, 191);
            this.preRadioButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.preRadioButton.Name = "preRadioButton";
            this.preRadioButton.Size = new System.Drawing.Size(14, 13);
            this.preRadioButton.TabIndex = 43;
            this.preRadioButton.TabStop = true;
            this.preRadioButton.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(68, 188);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(26, 15);
            this.label4.TabIndex = 42;
            this.label4.Text = "Pre:";
            // 
            // operationButton
            // 
            this.operationButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.operationButton.Location = new System.Drawing.Point(344, 505);
            this.operationButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.operationButton.Name = "operationButton";
            this.operationButton.Size = new System.Drawing.Size(87, 29);
            this.operationButton.TabIndex = 41;
            this.operationButton.Text = "删除";
            this.operationButton.UseVisualStyleBackColor = true;
            this.operationButton.Click += new System.EventHandler(this.operationButton_Click);
            // 
            // postTextBox
            // 
            this.postTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.postTextBox.Location = new System.Drawing.Point(128, 235);
            this.postTextBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.postTextBox.Multiline = true;
            this.postTextBox.Name = "postTextBox";
            this.postTextBox.Size = new System.Drawing.Size(229, 262);
            this.postTextBox.TabIndex = 40;
            this.postTextBox.Visible = false;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(68, 239);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(32, 15);
            this.label3.TabIndex = 39;
            this.label3.Text = "Post:";
            // 
            // outputTextBox
            // 
            this.outputTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.outputTextBox.Location = new System.Drawing.Point(128, 138);
            this.outputTextBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.outputTextBox.Name = "outputTextBox";
            this.outputTextBox.Size = new System.Drawing.Size(229, 21);
            this.outputTextBox.TabIndex = 38;
            // 
            // inputTextBox
            // 
            this.inputTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.inputTextBox.Location = new System.Drawing.Point(128, 84);
            this.inputTextBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.inputTextBox.Name = "inputTextBox";
            this.inputTextBox.Size = new System.Drawing.Size(229, 21);
            this.inputTextBox.TabIndex = 37;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(68, 136);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 15);
            this.label2.TabIndex = 36;
            this.label2.Text = "Output:";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(68, 85);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 15);
            this.label1.TabIndex = 35;
            this.label1.Text = "Input:";
            // 
            // ProcessForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(439, 562);
            this.Controls.Add(this.nameTextBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.preButton);
            this.Controls.Add(this.NextButton);
            this.Controls.Add(this.preRadioButton);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.operationButton);
            this.Controls.Add(this.postTextBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.outputTextBox);
            this.Controls.Add(this.inputTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "ProcessForm";
            this.Text = "ProcessForm";
            this.Load += new System.EventHandler(this.ProcessForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox nameTextBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button preButton;
        private System.Windows.Forms.Button NextButton;
        private System.Windows.Forms.RadioButton preRadioButton;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button operationButton;
        private System.Windows.Forms.TextBox postTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox outputTextBox;
        private System.Windows.Forms.TextBox inputTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
    }
}