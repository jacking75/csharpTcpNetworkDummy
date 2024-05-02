﻿using System;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace TcpDummyClient
{
    [SupportedOSPlatform("windows10.0.177630")]
    static class Program
    {
        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
