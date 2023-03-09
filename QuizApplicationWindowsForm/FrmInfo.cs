using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace QuizApplicationWindowsForm
{
    public partial class FrmInfo : Form
    {
        public FrmInfo()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string executableLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string FilePath = Path.Combine(executableLocation, "Information.txt");

            if (!(File.Exists(FilePath)))
            {
                using (StreamWriter writer = new StreamWriter(FilePath))
                {
                    writer.WriteLine(textBox1.Text.ToString());
                }

            }
            else
            {
                File.Delete(FilePath);
                using (StreamWriter writer = new StreamWriter(FilePath))
                {
                    writer.WriteLine(textBox1.Text.ToString());
                }

            }

            FrmReadyForExam fr = new FrmReadyForExam();
            fr.Show();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void FrmInfo_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
