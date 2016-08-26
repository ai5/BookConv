using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using ShogiLib;

namespace BookConv
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void inputTextBox_DragDrop(object sender, DragEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            
            string[] fileName = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            textBox.Text = fileName[0];
        }

        private void inputTextBox_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void inputSelectButton_Click(object sender, EventArgs e)
        {
            if (this.inputTextBox.Text != string.Empty)
            {
                this.openFileDialog1.FileName = this.inputTextBox.Text;
            }

            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.inputTextBox.Text = this.openFileDialog1.FileName;
            }
        }

        private void outputSelectButton_Click(object sender, EventArgs e)
        {
            if (this.outputTextBox.Text != string.Empty)
            {
                this.saveFileDialog1.FileName = this.outputTextBox.Text;
            }

            this.saveFileDialog1.FilterIndex = this.comboBox1.SelectedIndex + 1;

            if (this.saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.outputTextBox.Text = this.saveFileDialog1.FileName;
            }
        }

        private void convButton_Click(object sender, EventArgs e)
        {
            if (this.inputTextBox.Text == string.Empty)
            {
                MessageBox.Show("入力ファイルを設定してください");
                return;
            }

            if (this.outputTextBox.Text == string.Empty)
            {
                MessageBox.Show("出力ファイルを設定してください");
                return;
            }

            Cursor keep = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;

            try
            {
                // 面倒なのでここでやる
                SBook book = SBook.Load(this.inputTextBox.Text);

                if (this.comboBox1.SelectedIndex == (int)BookFormat.YaneuraOu2016)
                {
                    book.ExportYaneuraOUbook(this.outputTextBox.Text);
                }
                else if (this.comboBox1.SelectedIndex == (int)BookFormat.Gikou)
                {
                    book.ExportGikou(this.outputTextBox.Text);
                }
                else
                {
                    book.ExportAperyBook(this.outputTextBox.Text);
                }

                Cursor.Current = keep;

                MessageBox.Show("完了", "メッセージ", MessageBoxButtons.OK);
            }
            catch (Exception ex)
            {
                Cursor.Current = keep;
                MessageBox.Show(ex.Message, "エラー", MessageBoxButtons.OK);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Settings.Load();

            this.inputTextBox.Text = Settings.InputFile;
            this.outputTextBox.Text = Settings.OutputFile;
            if (Settings.BookFormat >= 0 && (int)Settings.BookFormat < this.comboBox1.Items.Count)
            {
                this.comboBox1.SelectedIndex = (int)Settings.BookFormat;
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Settings.InputFile = this.inputTextBox.Text;
            Settings.OutputFile = this.outputTextBox.Text;
            Settings.BookFormat = (BookFormat)this.comboBox1.SelectedIndex;
            Settings.Save();
        }
    }
}
