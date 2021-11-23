using System;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
namespace SingleThreadScanningPort
{
    public partial class Form1 : Form
    {
        
        private bool[] ports = new bool[65536];//所有端口号
        private static int port=0;//当前端口号
        private static int count = 0;//开放端口号数量

        public Form1()
        {
            InitializeComponent();
            //CheckForIllegalCrossThreadCalls设置为false；然后就能安全的访问窗体控件
            CheckForIllegalCrossThreadCalls = false;

            //初始化进度显示为空
            label2.Text = "";

            //停止扫描按钮为不可用
            stopScanning.Enabled = false;
        }

        private void beginScanning_Click(object sender, EventArgs e)
        {
            //检查端口号
            if (int.Parse(beginPortText.Text) < 0 || int.Parse(beginPortText.Text) > int.Parse(endPortText.Text) || int.Parse(endPortText.Text) > 65565)
            {
                messages.Items.Add("端口错误!");
                return;
            }

            //新建线程执行扫描端口函数
            Thread procss = new Thread(new ThreadStart(ScanningPort));
            procss.Start();

            //设置进度条最大值最小值分别为结束端口和起始端口
            progressBar1.Maximum = int.Parse(endPortText.Text) - int.Parse(beginPortText.Text);
            progressBar1.Minimum = 0;

            //判断是否为继续扫描
            if (port == 0)
            {
                messages.Items.Clear();
                messages.Items.Add("开始扫描.......");
            }

            else
                messages.Items.Add("继续扫描......");

            //开始扫描禁用,停止扫描启用
            beginScanning.Enabled = false;
            stopScanning.Enabled = true;
        }
        public void ScanningPort()
        {
            int start;
            int end = int.Parse(endPortText.Text);

            //判断是否为继续扫描,如果是则继续扫描,否则重新扫描
            if (port != 0)
                start = port;
            else
                start = int.Parse(beginPortText.Text);

            messages.Items.Add("起始端口" + start);
            messages.Items.Add("结束端口" + end);


            for (int i = start; i <= end; i++)
            {

                //按下停止扫描后开始扫描按钮启用,此时停止扫描
                if (beginScanning.Enabled)
                    break;
                port = i;

                //新建线程进行扫描
                Thread thread = new Thread(Scanning);
                thread.Start();

                //主线程休眠10ms
                System.Threading.Thread.Sleep(10);

                //修改进度条的值
                progressBar1.Value = i- int.Parse(beginPortText.Text);

                //显示端口号以及进度
                label2.Text = "正在扫描端口: " + i+"  进度: "+Math.Round(( (i - int.Parse(beginPortText.Text)) *100.0 / progressBar1.Maximum),2)+"%";
                progressBar1.PerformStep();
            }

            if (port != 0)
                beginScanning.Text = "继续扫描";
            else
            {
                messages.Items.Add("端口扫描结束");
                messages.Items.Add("共有 " + count + " 个端口开放");
            }

            beginScanning.Enabled = true;
            stopScanning.Enabled = false;
           

            //判断是否扫描完毕
            if (int.Parse(endPortText.Text) == port)
            {
                port = 0;
                beginScanning.Text = "开始扫描";
            }

        }


        public void Scanning()
        {
            this.ports[port] = true;
            try
            {
                TcpClient tmp = new TcpClient(ipAddressText.Text, port);
                messages.Items.Add("端口" + port + "开放");
                count++;
            }

            catch (System.Exception ex)
            {
               
            }
        }

        private void stopScanning_Click(object sender, EventArgs e)
        {
            //按下停止按钮后,开始按钮和停止按钮状态翻转
            beginScanning.Enabled = true;
            stopScanning.Enabled = false;
        }
    }
}
