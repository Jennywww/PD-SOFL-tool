
namespace XMLFormsApp
{
    partial class FeaturePage
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.featureTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // featureTextBox
            // 
            this.featureTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.featureTextBox.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.featureTextBox.Location = new System.Drawing.Point(0, 0);
            this.featureTextBox.Multiline = true;
            this.featureTextBox.Name = "featureTextBox";
            this.featureTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.featureTextBox.Size = new System.Drawing.Size(219, 188);
            this.featureTextBox.TabIndex = 0;
            // 
            // FeaturePage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.featureTextBox);
            this.Name = "FeaturePage";
            this.Size = new System.Drawing.Size(219, 188);
            this.Load += new System.EventHandler(this.FeaturePage_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox featureTextBox;
    }
}
