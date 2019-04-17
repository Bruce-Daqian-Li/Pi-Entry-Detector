using System;
using System.Collections.Generic;

namespace EDD.Models
{
    public class EntryInfo
    {
        public EntryInfo()
        {
            Id = Guid.NewGuid();
        }
        public Guid Id { get; set; }
        public DateTime FirstSeen { get; set; }
        public int Frequency { get; set; }
        public List<TimeRange> Records { get; set; }
        public DateTime LastSeen { get; set; }
    }
    public class TimeRange
    {
        public TimeRange()
        {
            Id = Guid.NewGuid();
        }
        public Guid Id { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
    }
}