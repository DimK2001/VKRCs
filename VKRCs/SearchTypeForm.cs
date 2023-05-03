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
            analyzer.Analyze(data, 0, this);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            analyzer.Analyze(data, 1, this);
        }

        public void PrintResult(string _message)
        {
            result.Text += _message;
        }

        private void result_Click(object sender, EventArgs e)
        {

        }
    }
}
