namespace BookConv
{
    partial class Form1
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.inputTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.outputTextBox = new System.Windows.Forms.TextBox();
            this.convButton = new System.Windows.Forms.Button();
            this.inputSelectButton = new System.Windows.Forms.Button();
            this.outputSelectButton = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.SuspendLayout();
            // 
            // inputTextBox
            // 
            this.inputTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.inputTextBox.Location = new System.Drawing.Point(87, 14);
            this.inputTextBox.Name = "inputTextBox";
            this.inputTextBox.Size = new System.Drawing.Size(259, 19);
            this.inputTextBox.TabIndex = 0;
            this.inputTextBox.DragDrop += new System.Windows.Forms.DragEventHandler(this.inputTextBox_DragDrop);
            this.inputTextBox.DragEnter += new System.Windows.Forms.DragEventHandler(this.inputTextBox_DragEnter);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "入力";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "出力";
            // 
            // outputTextBox
            // 
            this.outputTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.outputTextBox.Location = new System.Drawing.Point(87, 48);
            this.outputTextBox.Name = "outputTextBox";
            this.outputTextBox.Size = new System.Drawing.Size(259, 19);
            this.outputTextBox.TabIndex = 3;
            this.outputTextBox.DragDrop += new System.Windows.Forms.DragEventHandler(this.inputTextBox_DragDrop);
            this.outputTextBox.DragEnter += new System.Windows.Forms.DragEventHandler(this.inputTextBox_DragEnter);
            // 
            // convButton
            // 
            this.convButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.convButton.Location = new System.Drawing.Point(342, 171);
            this.convButton.Name = "convButton";
            this.convButton.Size = new System.Drawing.Size(75, 23);
            this.convButton.TabIndex = 4;
            this.convButton.Text = "変換";
            this.convButton.UseVisualStyleBackColor = true;
            this.convButton.Click += new System.EventHandler(this.convButton_Click);
            // 
            // inputSelectButton
            // 
            this.inputSelectButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.inputSelectButton.Location = new System.Drawing.Point(387, 12);
            this.inputSelectButton.Name = "inputSelectButton";
            this.inputSelectButton.Size = new System.Drawing.Size(30, 23);
            this.inputSelectButton.TabIndex = 5;
            this.inputSelectButton.Text = "...";
            this.inputSelectButton.UseVisualStyleBackColor = true;
            this.inputSelectButton.Click += new System.EventHandler(this.inputSelectButton_Click);
            // 
            // outputSelectButton
            // 
            this.outputSelectButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.outputSelectButton.Location = new System.Drawing.Point(387, 46);
            this.outputSelectButton.Name = "outputSelectButton";
            this.outputSelectButton.Size = new System.Drawing.Size(30, 23);
            this.outputSelectButton.TabIndex = 6;
            this.outputSelectButton.Text = "...";
            this.outputSelectButton.UseVisualStyleBackColor = true;
            this.outputSelectButton.Click += new System.EventHandler(this.outputSelectButton_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.Filter = "ShogiGUI定跡ファイル(*.sbk)|*.sbk";
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.Filter = "Apery Book(*.bin)|*.bin|すべてのファイル|*.*";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(429, 206);
            this.Controls.Add(this.outputSelectButton);
            this.Controls.Add(this.inputSelectButton);
            this.Controls.Add(this.convButton);
            this.Controls.Add(this.outputTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.inputTextBox);
            this.Name = "Form1";
            this.Text = "定跡変換";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox inputTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox outputTextBox;
        private System.Windows.Forms.Button convButton;
        private System.Windows.Forms.Button inputSelectButton;
        private System.Windows.Forms.Button outputSelectButton;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
    }
}

