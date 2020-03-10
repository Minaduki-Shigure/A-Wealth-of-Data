using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace compute
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog f1 = new OpenFileDialog();
            f1.Filter = "数据文件(csv)|*.csv";
            if (f1.ShowDialog() != DialogResult.OK) return;
            textBox1.Text = f1.FileName;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Application.ExitThread();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "" || textBox2.Text == "")
            {
                MessageBox.Show("not enough parameters");
                return;
            }
            List<string> data = new List<string>();
            StreamReader fk = new StreamReader(textBox1.Text, Encoding.UTF8);
            string tem = "";
            while ((tem = fk.ReadLine()) != null)
            {
                string[] abc = tem.Split(new string[] { "," }, StringSplitOptions.None);
                if (abc.Length == 4)
                {
                    data.Add(abc[0]); data.Add(abc[1]); data.Add(abc[2]); data.Add(abc[3]);
                }
                else
                {
                    MessageBox.Show("Error");
                    fk.Close();
                }
            }
            fk.Close();
            List<string> ttc = new List<string>();    //保存中间数据
            List<string> time = new List<string>();    //保存时间相同的数据---用于计算
            //算法提取时间相同是数据生成List<string>，计算后整合到原记录中
            for (int k = 0; k < data.Count; k += 4)
            {
                if (time.Count == 0)    //空集合，加一组数据
                {
                    time.Add(data[k + 0]);  //ID
                    time.Add(data[k + 1]);  //时间
                    time.Add(data[k + 2]);  //计算数据1
                    time.Add(data[k + 3]);  //计算数据2
                }
                else                        //遍历已有的数据，看看他们的ID段是否相同
                {
                    if (time[0] == data[k + 0])    //ID段相同，追加数据
                    {
                        time.Add(data[k + 0]);  //ID
                        time.Add(data[k + 1]);  //时间
                        time.Add(data[k + 2]);  //计算数据1
                        time.Add(data[k + 3]);  //计算数据2
                    }
                    else                       //不相同，需要先计算time中的数据，再清空time，将当前数据加进去
                    {
                        newData(time, ttc);
                        time.Clear();
                        time.Add(data[k + 0]);  //ID
                        time.Add(data[k + 1]);  //时间
                        time.Add(data[k + 2]);  //计算数据1
                        time.Add(data[k + 3]);  //计算数据2
                    }
                }
            }

            //相同产品时间相同的记录取最后一个
            List<string> bm = new List<string>();
            List<string> ax = new List<string>();
            for (int f = 0; f < ttc.Count; f += 7)
            {
                if (ax.Count == 0)   //加一个
                {
                    ax.Add(ttc[f + 0]); ax.Add(ttc[f + 1]); ax.Add(ttc[f + 2]); ax.Add(ttc[f + 3]); ax.Add(ttc[f + 4]); ax.Add(ttc[f + 5]); ax.Add(ttc[f + 6]);
                }
                else
                {
                    if (ttc[f + 0] == ax[0] && ttc[f + 1] == ax[1])    //相同数据仅保留最后一个
                    {
                        ax.Clear();    //清空原来的，加新的
                        ax.Add(ttc[f + 0]); ax.Add(ttc[f + 1]); ax.Add(ttc[f + 2]); ax.Add(ttc[f + 3]); ax.Add(ttc[f + 4]); ax.Add(ttc[f + 5]); ax.Add(ttc[f + 6]);
                    }
                    else          //将ax中的数据追加到bm中，清空ax,再写入现在的数据
                    {
                        bm.Add(ax[0]); bm.Add(ax[1]); bm.Add(ax[2]); bm.Add(ax[3]); bm.Add(ax[4]); bm.Add(ax[5]); bm.Add(ax[6]);
                        ax.Clear();
                        ax.Add(ttc[f + 0]); ax.Add(ttc[f + 1]); ax.Add(ttc[f + 2]); ax.Add(ttc[f + 3]); ax.Add(ttc[f + 4]); ax.Add(ttc[f + 5]); ax.Add(ttc[f + 6]);
                    }
                }
            }
            //将bm写入文件
            StreamWriter fw = new StreamWriter(textBox1.Text.Substring(0, textBox1.Text.LastIndexOf("\\") + 1) + textBox2.Text + ".csv", false, Encoding.UTF8);
            for (int i = 0; i < bm.Count; i+=7)
                fw.WriteLine(bm[i+0]+","+bm[i+1]+","+bm[i+2]+","+bm[i+3]+","+bm[i+4]+","+bm[i+5]+","+bm[i+6]);
            fw.Close();
            MessageBox.Show("OK!");
        }

        private void newData(List<string> def, List<string> c99)
        {
            double a = 0, b = 0;
            for (int i = 0; i < def.Count; i += 4)
            {
                a += Convert.ToDouble(def[i + 2]) * Convert.ToDouble(def[i + 3]);
                b += Convert.ToDouble(def[i + 3]);
                c99.Add(def[i + 0]);
                c99.Add(def[i + 1]);
                c99.Add(def[i + 2]);
                c99.Add(def[i + 3]);
                c99.Add(Convert.ToString(a));    //C*D的合计
                c99.Add(Convert.ToString(b));    //D合计
                c99.Add(Convert.ToString(a / b * 1.0));   //加权平均
            }
        }
    }
}
