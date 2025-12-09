using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApp3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public class Server
        {
            public int CurrentLoad { get; set; }
            public int MaxLoad { get; set; }
            public string ServerName { get; set; }
            private bool isCrashed;

            static public double totalTraffic = 0;

            public Server(string name, int maxLd, int currLd)
            {
                MaxLoad = maxLd;
                CurrentLoad = currLd;
                ServerName = name;
            }

            public delegate void ServerStatusHandler(string logMessage);

            private ServerStatusHandler systemLog;

            public void ConnectLogger(ServerStatusHandler methodToCall)
            {
                systemLog += methodToCall;
            }

            public void IncreaseLoad(int requests)
            {
                if (isCrashed)
                {
                    if (systemLog != null)
                        systemLog("CRITICAL: Сервер не відповідає (System Down)!");
                }
                else
                {
                    totalTraffic += Math.Abs(requests);

                    CurrentLoad += requests;

                    if (systemLog != null)
                    {
                        if ((MaxLoad - CurrentLoad <= 10) && (CurrentLoad < MaxLoad))
                        {
                            systemLog("WARNING: Високе навантаження CPU! Можливий перегрів.");
                        }
                        else if (CurrentLoad >= MaxLoad)
                        {
                            isCrashed = true;
                            systemLog("ERROR 500: Сервер впав через перевантаження!");
                        }
                        else
                        {
                            systemLog($"OK: Поточне навантаження CPU: {CurrentLoad}%");
                        }
                    }
                }
            }
        }

        public void OnServerLog(string msg)
        {
            label1.Text = label1.Text + msg + " \n";
        }

        public void OnTrafficStats(string msg)
        {
            label2.Text = label2.Text + "Оброблено запитів: " + Server.totalTraffic.ToString() + " MB \n";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            label1.Text = "";
            label2.Text = "";
            Server.totalTraffic = 0;

            Server myServer = new Server("Main_Database_Server", 100, 10);

            Server.ServerStatusHandler logger1 = new Server.ServerStatusHandler(OnServerLog);
            Server.ServerStatusHandler logger2 = new Server.ServerStatusHandler(OnTrafficStats);

            myServer.ConnectLogger(logger1);
            myServer.ConnectLogger(logger2);

            OnServerLog("--- Запуск стрес-тесту сервера ---");

            for (int i = 0; i < 11; i++)
            {
                myServer.IncreaseLoad(9);
            }
        }
    }
}