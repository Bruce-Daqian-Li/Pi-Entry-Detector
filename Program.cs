using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EDD.Models;
using EDD.Utils;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Unosquare.RaspberryIO;
using Unosquare.WiringPi;

namespace EDD
{
    public class Program
    {

        public readonly static bool DEBUG_MODE = false;

        //This is the main entry of the project
        public static void Main(string[] args)
        {
            L.InitLog();
            L._LogLevel = LogLevel.DBG;
            L.W("Starting EDD Project...");
            L.D("Initialising BootStrapWiringPi");

            if (DEBUG_MODE) DEBUG();
            else InitialiseGPIO();

            L.D("Starting WebServer...");
            CreateWebHostBuilder(args).Build().Run();
        }

        public static void InitialiseGPIO()
        {
            Pi.Init<BootstrapWiringPi>();
            InternalCommunication.Initialise();
        }

        public static void DEBUG()
        {
            InternalCommunication.Initialise();
            L.W("Generate Fake Data");

            Thread fakeAdder = new Thread(FakeAddWorker);
            fakeAdder.Start();

            void FakeAddWorker()
            {
                while (true)
                {
                    InternalCommunication.ProcessTimeEntry(new TimeRange() { StartAt = DateTime.Now, EndAt = DateTime.Now });
                    Thread.Sleep(new Random().Next(5, 15) * 1000);
                }
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
