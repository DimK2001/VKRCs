using System;
using System.Threading;
using System.Windows.Forms;

namespace VKRCs
{
    public partial class CreateBaseForm : Form
    {
        public CreateBaseForm()
        {
            InitializeComponent();
            Thread thread1 = new Thread(createBase);
            thread1.Start();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void VKRForm_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }

        private void createBase()
        {
            Thread.Sleep(1000);
            Analyzer analyzer = new Analyzer();
            analyzer.CreateBase();
            label1.Invoke(changeText);
        }

        private void changeText()
        {
            label1.Text = "База данных успешно создана.";
        }
    }
}
