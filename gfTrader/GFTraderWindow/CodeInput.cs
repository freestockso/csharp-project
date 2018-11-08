using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GFTrader
{
    public partial class CodeInput : Form
    {
        public CodeInput()
        {
            InitializeComponent();
        }

        private void Ok_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.Close();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                textCode.Text = string.Empty;
                this.Close();
            }
        }
        public static string ShowInputBox()
        {
            CodeInput codeInput = new CodeInput();
            codeInput.ShowDialog();
            return codeInput.textCode.Text;
        }
    }
}
