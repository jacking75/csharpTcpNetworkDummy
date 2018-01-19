﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace SimpleTcpEchoDummyClient
{
    public partial class MainForm : Form
    {
        TcpDummyClientsLib.ModuleConnectOnly DummyConnectOnly = new TcpDummyClientsLib.ModuleConnectOnly();

        static System.Collections.Concurrent.ConcurrentQueue<string> logMsgQueue = new System.Collections.Concurrent.ConcurrentQueue<string>();

        System.Windows.Threading.DispatcherTimer dispatcherUITimer;
        System.Windows.Threading.DispatcherTimer dispatcherLogTimer;


        public MainForm()
        {
            InitializeComponent();

            dispatcherUITimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherUITimer.Tick += new EventHandler(Update);
            dispatcherUITimer.Interval = new TimeSpan(0, 0, 0, 1);
            dispatcherUITimer.Start();

            dispatcherLogTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherLogTimer.Tick += new EventHandler(LogPrint);
            dispatcherLogTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            dispatcherLogTimer.Start();
        }

        TestConfig GetTestConfig()
        {
            return new TestConfig
            {
                DummyCount = textBox3.Text.ToInt32(),
                RemoteIP = textBoxIP.Text,
                RemotePort = textBoxPort.Text.ToUInt16()
            };
        }

        struct TestConfig
        {
            public int DummyCount;
            public string RemoteIP;
            public UInt16 RemotePort;
        }

        void AddLog(string msg)
        {
            logMsgQueue.Enqueue(msg);
        }

        // 접속만.... - 접속 하기
        private async void button1_Click(object sender, EventArgs e)
        {
            var config = GetTestConfig();

            var result = await Task.Run(async () => await DummyConnectOnly.Start(config.DummyCount, config.RemoteIP, config.RemotePort));

            AddLog(result);
        }

        // 접속만.... - 접속 끊기
        private async void button2_Click(object sender, EventArgs e)
        {
            var result = await Task.Run(async () => await DummyConnectOnly.End());

            AddLog(result);
        }

        void Update(object sender, EventArgs e)
        {
            try
            {
                textBox1.Text = DummyConnectOnly.CurrentConnectedCount().ToString();
            }
            catch (Exception ex)
            {
                AddLog(ex.Message);
            }
        }

        void LogPrint(object sender, EventArgs e)
        {
            // 너무 이 작업만 할 수 없으므로 일정 작업 이상을 하면 일단 패스한다.
            int logWorkCount = 0;

            while (true)
            {
                string msg;

                if (logMsgQueue.TryDequeue(out msg) == false)
                {
                    break;
                }

                ++logWorkCount;

                if (listBoxLog.Items.Count > 512)
                {
                    listBoxLog.Items.Clear();
                }

                listBoxLog.Items.Add(msg);
                listBoxLog.SelectedIndex = listBoxLog.Items.Count - 1;
                
                if (logWorkCount > 16)
                {
                    break;
                }
            }
        }
    }
}