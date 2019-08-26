using CSBaseLib;
using NPSBDummyLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace TcpDummyClient
{
    public partial class MainForm : Form
    {
        NPSBDummyLib.DummyManager DummyManager = new NPSBDummyLib.DummyManager();
        
        System.Collections.Concurrent.ConcurrentQueue<ReportData> logMsgQueue;

        System.Windows.Threading.DispatcherTimer dispatcherUITimer;
        System.Windows.Threading.DispatcherTimer dispatcherLogTimer;


        public MainForm()
        {
            InitializeComponent();

            Init();
        }

        void Init()
        {
            DummyManager.LogFunc = AddLog;
            
            logMsgQueue = new System.Collections.Concurrent.ConcurrentQueue<ReportData>();

            dispatcherUITimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherUITimer.Tick += new EventHandler(UpdateUI);
            dispatcherUITimer.Interval = new TimeSpan(0, 0, 0, 1);
            dispatcherUITimer.Start();

            dispatcherLogTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherLogTimer.Tick += new EventHandler(UpdateLogPrint);
            dispatcherLogTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            dispatcherLogTimer.Start();
        }

        void AddLog(ReportData msg)
        {
            logMsgQueue.Enqueue(msg);
        }

        void AddLog(string msg)
        {
            logMsgQueue.Enqueue(new ReportData(msg));
        }

        void testResultToUILog(Int64 testIndex, TestConfig config)
        {
            var testResult = DummyManager.GetTestResult(testIndex, config);
            foreach (var report in testResult)
            {
                AddLog(report);
            } 
        }

        #region 테스트 설정 값
        TestConfig GetTestBaseConfig()
        {
            var config = new NPSBDummyLib.TestConfig
            {
                ActionIntervalTime = textBox3.Text.ToInt32(),
                ActionRepeatCount = textBox12.Text.ToInt32(),
                DummyIntervalTime = textBox14.Text.ToInt32(),
                LimitActionTime = textBox9.Text.ToInt32(),
                RoomNumber = textBox2.Text.ToInt32(),
                ChatMessage = textBox6.Text,
                MaxVaildActionRecvCount = 3,
            };

            return config;
        }

        DummyInfo GetBaseDummyInfo()
        {
            var dummyInfo = new NPSBDummyLib.DummyInfo
            {
                RmoteIP = textBoxIP.Text,
                RemotePort = textBoxPort.Text.ToInt32(),
                DummyCount = textBoxDummyCount.Text.ToInt32(),
                PacketSizeMax = textBox13.Text.ToInt32(),
                IsRecvDetailProc = (checkBox2.CheckState == CheckState.Checked) ? true : false,
            };

            return dummyInfo;
        }

        NPSBDummyLib.EchoCondition GetTestEchoConfig()
        {
            var config = GetTestBaseConfig();
            var repeatCount = textBox8.Text.ToInt32();
            var repeatTimeSec = textBox7.Text.ToInt32();

            var echoCondi = new NPSBDummyLib.EchoCondition();
            echoCondi.IP = DummyManager.GetDummyInfo.RmoteIP;
            echoCondi.Port = DummyManager.GetDummyInfo.RemotePort;
            echoCondi.PacketSizeMin = textBox10.Text.ToInt32();
            echoCondi.PacketSizeMax = textBox11.Text.ToInt32();
            echoCondi.Set(repeatCount, repeatTimeSec);
            
            return echoCondi;
        }
        /*
        TcpDummyClientsLib.TestRepeatConnDisConnConfig GetTestRepeatConnDisConnConfig()
        {
            return new TcpDummyClientsLib.TestRepeatConnDisConnConfig
            {
                DummyCount = textBox4.Text.ToInt32(),
                RemoteIP = textBoxIP.Text,
                RemotePort = textBoxPort.Text.ToUInt16(),

                RepeatCount = textBox5.Text.ToInt32(),
                RepeatTime = DateTime.Now.AddSeconds(textBox2.Text.ToInt32()),
            };
        }
                
        TcpDummyClientsLib.TestSimpleEchoConfig GetTestTestSimpleEchoConfig()
        {
            return new TcpDummyClientsLib.TestSimpleEchoConfig
            {
                DummyCount = textBox9.Text.ToInt32(),
                RemoteIP = textBoxIP.Text,
                RemotePort = textBoxPort.Text.ToUInt16(),

                RepeatCount = textBox8.Text.ToInt32(),
                RepeatTime = DateTime.Now.AddSeconds(textBox7.Text.ToInt32()),

                MinSendDataSize = textBox10.Text.ToInt32(),
                MaxSendDataSize = textBox11.Text.ToInt32(),

                LogFunc = AddLog,
            };
        }
        */
        #endregion


        #region 접속만하기
        // 접속만....연결하기
        private async void button1_Click(object sender, EventArgs e)
        {
            var config = GetTestBaseConfig();
            DummyManager.SetDummyInfo = GetBaseDummyInfo();
            config.ActionCase = NPSBDummyLib.TestCase.ONLY_CONNECT;

            DummyManager.Prepare(config);

            var testUniqueId = DateTime.Now.Ticks;
            await DummyManager.TestConnectOnlyAsync(testUniqueId);

            testResultToUILog(testUniqueId, config);
        }

        // 접속만.... - 접속 끊기
        private void button2_Click(object sender, EventArgs e)
        {
            AddLog($"End - {DummyManager.CurrentTestCase()}");
            DummyManager.EndTest();

        }
        #endregion

        #region 접속/끊기 반복
        // 테스트 시작 - 접속/끊기 반복
        private async void button4_Click(object sender, EventArgs e)
        {
            var config = GetTestBaseConfig();
            config.ActionCase = NPSBDummyLib.TestCase.REPEAT_CONNECT;
            DummyManager.SetDummyInfo = GetBaseDummyInfo();

            var testUniqueIndex = DateTime.Now.Ticks;

            DummyManager.Prepare(config);
                        
            await DummyManager.TestRepeatConnectAsync(testUniqueIndex);
            
            testResultToUILog(testUniqueIndex, config);
            AddLog($"End - {config.ActionCase}");
            DummyManager.EndTest();
            
        }
        // 테스트 중단 - 접속/끊기 반복
        private void button3_Click(object sender, EventArgs e)
        {
            AddLog($"End - {DummyManager.CurrentTestCase()}");
            DummyManager.EndTest();
        }
        #endregion


        #region 에코 테스트
        private async void button9_Click(object sender, EventArgs e)
        {
            var config = GetTestBaseConfig();
            config.ActionCase = NPSBDummyLib.TestCase.ECHO;
            DummyManager.SetDummyInfo = GetBaseDummyInfo();
           
            DummyManager.Prepare(config);


            var testUniqueIndex = DateTime.Now.Ticks;
            var echoCondi = GetTestEchoConfig();

            AddLog($"Start - {DummyManager.CurrentTestCase()}, Condi_Count:{echoCondi.EchoCount}, Condi_Time:{echoCondi.EchoTime}");
            await DummyManager.TestRepeatEchoAsync(testUniqueIndex, echoCondi);
            
            testResultToUILog(testUniqueIndex, config);
            AddLog($"End - {DummyManager.CurrentTestCase()}");

            DummyManager.EndTest();
        }

        private async void button8_Click(object sender, EventArgs e)
        {
            await Task.CompletedTask;

            //var result = await Task.Run(async () => await DummySimpleEcho.End());

            //AddLog(result);
        }
        #endregion


        #region Test
        // Test 로그인
        private async void button5_Click(object sender, EventArgs e)
        {
            var testUniqueIndex = DateTime.Now.Ticks;
            var config = GetTestBaseConfig();

            DummyManager.Prepare(config);           
            await DummyManager.TestLoginAsync(testUniqueIndex, config);

            testResultToUILog(testUniqueIndex, config);            
            DummyManager.EndTest();
        }

        // Test 액션 모두 삭제
        private void button6_Click(object sender, EventArgs e)
        {
            listBoxAction.Items.Clear();
            //AddLog("액션 모두 삭제");
        }

        // Test 보내기
        private void button7_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(textBox6.Text))
            {
                return;
            }

            var config = GetTestBaseConfig();
            listBoxAction.Items.Add(new ActionData(TestCase.ACTION_ROOM_CHAT, "방채팅", config));

            //DevTestgSendReceive.SendData(textBox6.Text);
        }
        #endregion


        void UpdateUI(object sender, EventArgs e)
        {
            try
            {
                textBox1.Text = NPSBDummyLib.DummyManager.ConnectedDummyCount().ToString();   
            }
            catch (Exception ex)
            {
                AddLog(ex.Message);
            }
        }

        void UpdateLogPrint(object sender, EventArgs e)
        {
            // 너무 이 작업만 할 수 없으므로 일정 작업 이상을 하면 일단 패스한다.
            int logWorkCount = 0;

            while (true)
            {
                if (logMsgQueue.TryDequeue(out var report) == false)
                {
                    break;
                }

                ++logWorkCount;

                if (listBoxLog.Items.Count > 512)
                {
                    listBoxLog.Items.RemoveAt(0);
                    //listBoxLog.Items.Clear();
                }
                
                listBoxLog.Items.Add(report);
                listBoxLog.SelectedIndex = listBoxLog.Items.Count - 1;

                if (logWorkCount > 16)
                {
                    break;
                }
            }
        }

        private async void buttonStartAction_Click(object sender, EventArgs e)
        {
            DummyManager.Init();
            var dummyInfo = GetBaseDummyInfo();
            DummyManager.SetDummyInfo = dummyInfo;

            var totalRepeatCount = textBox4.Text.ToInt32();
            var testUniqueId = DateTime.Now.Ticks;

            if (listBoxAction.Items.Count > 0)
            {
                for (int repeatCount = 0; repeatCount < totalRepeatCount; ++repeatCount)
                {
                    int selectedIdx = 0;
                    var actionList = listBoxAction.Items.Cast<ActionData>().ToList();

                    foreach (ActionData item in actionList)
                    {
                        var config = item.Config;

                        listBoxAction.SetSelected(selectedIdx, true);
                        DummyManager.Prepare(item.Config);
                        for (int repeatCnt = 0; repeatCnt < item.Config.ActionRepeatCount; ++repeatCnt)
                        {
                            await DummyManager.RunAction(testUniqueId, item.TestCase, config);
                            testResultToUILog(testUniqueId, config);

                            await Task.Delay(config.ActionIntervalTime);
                        }

                        ++selectedIdx;
                    }
                }

                DummyManager.Clear();
            }
        }

        private void buttonDeleteAction_Click(object sender, EventArgs e)
        {
            if (listBoxAction.SelectedIndex == -1)
            {
                return;
            }

            var index = listBoxAction.SelectedIndex;

            listBoxAction.Items.RemoveAt(index);
        }

        private void listBoxLog_DoubleClick(object sender, EventArgs e)
        {
            if (listBoxLog.SelectedIndex == -1)
            {
                return;
            }

            ReportData curItem = (ReportData)listBoxLog.SelectedItem;
            if (curItem.Detail != null)
            {
                MessageBox.Show(curItem.Detail);
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            var config = GetTestBaseConfig();
            listBoxAction.Items.Add(new ActionData(TestCase.ACTION_ROOM_ENTER, "방입장", config));
        }

        private void button11_Click(object sender, EventArgs e)
        {
            var config = GetTestBaseConfig();
            listBoxAction.Items.Add(new ActionData(TestCase.ACTION_ROOM_LEAVE, "방퇴장", config));
        }

        private void listBoxAction_DoubleClick(object sender, EventArgs e)
        {
            if (listBoxAction.SelectedIndex == -1)
            {
                return;
            }

            ActionData curItem = (ActionData)listBoxAction.SelectedItem;
            if (curItem.Config != null)
            {
                MessageBox.Show(curItem.Config.ToString());
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            MessageBox.Show(DummyManager.ToPacketStat());
        }

        private void button13_Click(object sender, EventArgs e)
        {
            listBoxLog.Items.Clear();
        }

        private void button14_Click(object sender, EventArgs e)
        {
            var config = GetTestBaseConfig();
            listBoxAction.Items.Add(new ActionData(TestCase.ACTION_CONNECT, "연결시도", config));
        }

        private void button15_Click(object sender, EventArgs e)
        {
            var config = GetTestBaseConfig();
            listBoxAction.Items.Add(new ActionData(TestCase.ACTION_DISCONNECT, "연결해제", config));
        }
    }
}
