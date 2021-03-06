using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment5
{
    [Serializable]

    public class Activity
    {
        public int repID = -1;
        bool isShared;
        private string name;
        private string start, end;
        private TimeSpan start_time, end_time;

        public Activity(string name, string start, string end)
        {
            this.name = name;
            this.start = start;
            this.end = end;
            this.start_time = transformTime(start);
            this.end_time = transformTime(end);
        }

        public Activity(int repID,string name, string start, string end)
        {
            this.repID = repID;
            this.name = name;
            this.start = start;
            this.end = end;
            this.start_time = transformTime(start);
            this.end_time = transformTime(end);
        }


        public Activity()
        {

        }

        public bool IsShared
        {
            get { return isShared; }
            set { isShared = value; }
        }
        public override string ToString()
        {
            return "Activity Name: " + this.name + "\nStarting Hour: " + this.start + "\nEnding Hour: " + this.end;
        }

        private TimeSpan transformTime(string hour)
        {

            string[] hoursDivided = hour.Split(":");
            
            TimeSpan hours = new TimeSpan(Convert.ToInt32(hoursDivided[0]), Convert.ToInt32(hoursDivided[1]), 0);

            return hours;
        }

        public string Start
        {
            get { return start; }
            set { start = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string End
        {
            get { return end; }
            set { end = value; }
        }

        public TimeSpan Start_Time
        {
            get { return start_time; }
            set { start_time = value; }
        }

        public TimeSpan End_Time
        {
            get { return end_time; }
            set { end_time = value; }
        }

    }
}
