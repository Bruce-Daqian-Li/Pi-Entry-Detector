using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Abstractions;
using System;
using System.Linq;
using System.Collections.Concurrent;
using System.Collections.Generic;
using EDD.Models;
using System.Threading;

namespace EDD.Utils
{
    public static class InternalCommunication
    {
        private static int SENSOR_GPIO_PIN = (int)BcmPin.Gpio04;
        private static Thread recieveWorker;
        private static DateTime currentRangeStartAt { get; set; }

        public static List<EntryInfo> EntryRecord;
        public static EntryInfo CurrentEntry;
        public static TimeRange CurrentTimeRange;

        public static void Initialise()
        {
            L.I("Initialising GPIO Reader...");
            L.W("Starting Communication Bus...");
            EntryRecord = new List<EntryInfo>();
            CurrentEntry = new EntryInfo();
            CurrentTimeRange = new TimeRange();

            if (!Program.DEBUG_MODE)
            {
                InitialisePin();
                recieveWorker = new Thread(CoreProcess);
                recieveWorker.Start();
            }
        }
        public static void ProcessTimeEntry(TimeRange range)
        {
            L.D("Processing a new TimeRange");
            L.I("CurrentEntry Started At: " + currentRangeStartAt.ToNormalString());
            if (DateTime.Now.Subtract(currentRangeStartAt).TotalSeconds <= 300)
            {
                L.D("Adding No." + (CurrentEntry.Records.Count + 1) + " Time Range to current entry log");
                CurrentEntry.Frequency++;
                CurrentEntry.Records.Add(range);
                CurrentEntry.FirstSeen = CurrentEntry.Records.First().StartAt;
                CurrentEntry.LastSeen = range.StartAt;
            }
            else
            {
                EntryRecord.Add(CurrentEntry);
                CurrentEntry = new EntryInfo();
                CurrentEntry.Records = new List<TimeRange>();
                currentRangeStartAt = DateTime.Now;
            }
            CurrentTimeRange = range;
        }
        //Here comes the very core operation of EDD Project
        public static void CoreProcess()
        {
            var pin = Pi.Gpio[SENSOR_GPIO_PIN];
            L.I("Starting Core GPIO Detection Process...");
            while (true)
            {
                TimeRange _currentRange = ListenTimeRange(pin, currentRangeStartAt);
                ProcessTimeEntry(_currentRange);
            }
        }

        private static TimeRange ListenTimeRange(IGpioPin pin, DateTime rangeStart)
        {
            TimeRange _currentRange = new TimeRange();
            var status = pin.Read();
            if (status)
            {
                //If we already have a HIGH...
                L.D("Already have HIGH in GPIO" + SENSOR_GPIO_PIN);
                _currentRange.StartAt = rangeStart;
            }
            else
            {
                L.D("Waiting a HIGH in GPIO" + SENSOR_GPIO_PIN);
                //If not, wait for a HIGH
                pin.WaitForValue(GpioPinValue.High, 1000 * (299 - (int)DateTime.Now.Subtract(rangeStart).TotalSeconds));
                //We got it just now....
                _currentRange.StartAt = DateTime.Now;
            }
            //Vise Versa
            status = pin.Read();
            //status = pin.Value;
            if (!status)
            {
                L.D("Already have a LOW in GPIO" + SENSOR_GPIO_PIN);
                _currentRange.EndAt = DateTime.Now;
            }
            else
            {
                L.D("Waiting a LOW on GPIO" + SENSOR_GPIO_PIN);
                pin.WaitForValue(GpioPinValue.Low, 1000 * (299 - (int)DateTime.Now.Subtract(rangeStart).TotalSeconds));
                _currentRange.EndAt = DateTime.Now;
            }

            return _currentRange;
        }

        private static void InitialisePin()
        {

            Pi.Gpio[SENSOR_GPIO_PIN].InputPullMode = GpioPinResistorPullMode.PullUp;
            L.W("Setting GPIO Pin:" + SENSOR_GPIO_PIN + " to mode: Input");
            Pi.Gpio[SENSOR_GPIO_PIN].PinMode = GpioPinDriveMode.Input;
        }
    }
}