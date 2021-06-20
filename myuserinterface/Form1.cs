using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
public struct mydata
{
    public double ovs;
    public double t1;
    public double t2;
    public double t3;
    public double t4;
    public IntPtr msg;
};

namespace myuserinterface
{
    public partial class Form1 : Form
    {
        #region Dll definition


        [DllImport(@"dlltest.dll", EntryPoint = "expret", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.Struct)]
        public extern unsafe static mydata expret([In] [MarshalAsAttribute(UnmanagedType.LPStr)]string csvfile, 
            char type, int rate, [Out][MarshalAsAttribute(UnmanagedType.LPArray, ArraySubType = UnmanagedType.R4)] double[] exportvolts,
            [Out] [MarshalAsAttribute(UnmanagedType.LPArray, ArraySubType = UnmanagedType.R4)] double[] exporttimes,
            [Out] [MarshalAsAttribute(UnmanagedType.LPArray, ArraySubType = UnmanagedType.R4)] double[] ovst1234);
        #endregion
        #region Variable definition
        private char type = 'A';
        private int rate = 106;
        private string csvpath = string.Empty;
        #endregion
        public Form1()
        {
            InitializeComponent();
            comboBox1.Items.Add('A');
            comboBox1.Items.Add('B');
            comboBox2.Items.Add(106);
            comboBox2.Items.Add(212);
            comboBox2.Items.Add(424);
            comboBox2.Items.Add(848);
        }
        void initArray(ref double[] x,ref double[] y)
        {
            for (var i = 0; i < x.Length; i++)
                x[i] = 0;
            for (var j = 0; j < x.Length; j++)
                y[j] = 0;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            double[] t = new double[20000];
            double[] v = new double[20000];

            initArray(ref t, ref v);
            double[] myovst1234 = new double[5];
            double timevalue = 0;
            
            string msg = string.Empty;
            mydata mdata = new mydata();

            if (csvpath != string.Empty)
            {
                InitChart();
                mdata = expret(csvpath, type, rate, v, t, myovst1234);
                IntPtr pnt = Marshal.AllocHGlobal(Marshal.SizeOf(mdata));
                // Copy the struct to unmanaged memory.
                Marshal.StructureToPtr(mdata, pnt, false);
                mydata _mdata;
                _mdata = (mydata)Marshal.PtrToStructure(pnt, typeof(mydata));
                string imsg = null;
                imsg = Marshal.PtrToStringAnsi(_mdata.msg);
                Marshal.FreeHGlobal(pnt);
                if (imsg == string.Empty)
                {
                    for (int i = 0; i < t.Length; i++)
                    {
                        timevalue = t[i] * 1000000;
                        chart1.Series["volttime"].Points.AddXY(timevalue, v[i]);
                    }
                    label3.Text = "OVS: " + mdata.ovs.ToString();
                    label4.Text = "T1: " + mdata.t1.ToString();
                    label5.Text = "T2: " + mdata.t2.ToString();
                    label6.Text = "T3: " + mdata.t3.ToString();
                    label7.Text = "T4: " + mdata.t4.ToString();
                    chart1.ChartAreas["ChartArea1"].AxisY.Title = "Volt";
                    chart1.ChartAreas["ChartArea1"].AxisX.Title = "Sample Interval e-006";
                    chart1.ChartAreas["ChartArea1"].AxisX.IntervalAutoMode = System.Windows.Forms.DataVisualization.Charting.IntervalAutoMode.VariableCount;


                    this.WindowState = FormWindowState.Maximized;
                }
                else
                   MessageBox.Show(imsg);
            }
            else
                MessageBox.Show("please Load the Data file");
        }
        void InitChart()
        {
            chart1.Series["volttime"].Points.Clear();
        }
        private void button3_Click(object sender, EventArgs e)
        {
            
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                csvpath = openFileDialog1.FileName;
                button1.Enabled = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            type = (char)comboBox1.SelectedItem;
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            rate = (int)comboBox2.SelectedItem;
        }
    }
}
