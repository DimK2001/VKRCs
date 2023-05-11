using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace VKRCs
{
    public partial class SearchTypeForm : Form
    {
        private List<double[]> data = new List<double[]>();
        Analyzer analyzer = new Analyzer();
        public SearchTypeForm(List<double[]> _listenedData)
        {
            data = _listenedData;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            analyzer.Analyze(data, 0, this, tagsBox.Text.Split(';'));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            analyzer.Analyze(data, 1, this, tagsBox.Text.Split(';'));
        }

        public void PrintResult(string _message)
        {
            result.Text = "Результат поиска: " + _message;
        }

        private void result_Click(object sender, EventArgs e)
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
