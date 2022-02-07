using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Assignment5
{
    [Serializable]
    public class DateObj
    {
        string date;

        public DateObj(string x)
        {
            date = x;
        }

        public DateObj()
        {

        }

        public string Date
        {
            get { return date; }
            set { date = value; }
        }
    }

    [Serializable]
    public class Calendar
    {
        public static int repeatingID = 1;
        

        string calendarName;
        public List<Activity> activities = new List<Activity>();
        public List<DateObj> dates = new List<DateObj>();
      

        public Calendar()
        {
            calendarName = "Empty";
        }

        public Calendar(string name)
        {
            calendarName = name;
        }

        public string CalendarName {
            get { return calendarName; }
            set { calendarName = value; }
        }
        public void scheduleActivity()
        {
            Console.WriteLine("-------------------------");
            Console.WriteLine("Schedule an Activity");
            Console.WriteLine("-------------------------");

            try {
                Console.WriteLine("Enter The Date: (DD.MM.YYYY)");

                string date = Console.ReadLine();
                DateTime dTCurrent = DateTime.Today;
                DateTime inputDate = DateTime.ParseExact(date, "dd.MM.yyyy", CultureInfo.InvariantCulture);
                int result = DateTime.Compare(dTCurrent, inputDate);

                if (result == 1)
                {
                    throw (new PastDateException());


                }
                else if (result == 0)
                {

                    Console.WriteLine("Enter Activity Name: ");
                    string activity_name = Console.ReadLine();

                    Console.WriteLine("Enter Starting Time: (HH:MM)");
                    string start_time = Console.ReadLine();

                    string[] time;
                    string[] time1;

                    time = start_time.Split(":");

                    TimeSpan hourMinute = DateTime.Now.TimeOfDay;
                    if (hourMinute >= new TimeSpan(Convert.ToInt16(time[0]), Convert.ToInt16(time[1]), 0))
                    {
                        throw (new PastDateException());
                    }
                    else {

                        hourMinute = new TimeSpan(Convert.ToInt16(time[0]), Convert.ToInt16(time[1]), 0);

                        Console.WriteLine("Enter Ending Time: (HH:MM)");
                        string end_time = Console.ReadLine();
                        time1 = end_time.Split(":");

                        if (new TimeSpan(00, 00, 00) <= new TimeSpan(Convert.ToInt16(time1[0]), Convert.ToInt16(time1[1]), 0))
                        {
                            throw (new SameDayException());
                        }


                        if (new TimeSpan(Convert.ToInt16(time1[0]), Convert.ToInt16(time1[1]), 0) <= new TimeSpan(Convert.ToInt16(time[0]), Convert.ToInt16(time[1]), 0))
                        {
                            throw (new EndTimeGreaterThanStartTime());
                        }
                        else
                        {

                            if (checkClash(date, start_time, end_time))
                            {
                                throw (new ClashException(getAct(date, start_time, end_time)));
                            }
                            else
                            {
                                addActivity(new Activity(activity_name, start_time, end_time), date);
                            }

                        }

                    }

                }
                else
                {

                    Console.WriteLine("Enter Activity Name: ");
                    string activity_name = Console.ReadLine();


                    Console.WriteLine("Enter Starting Time: (HH:MM)");
                    string start_time = Console.ReadLine();

                    string[] time;
                    time = start_time.Split(":");

                    TimeSpan hourMinute;
                    hourMinute = new TimeSpan(Convert.ToInt16(time[0]), Convert.ToInt16(time[1]), 0);


                    Console.WriteLine("Enter Ending Time: (HH:MM)");
                    string end_time = Console.ReadLine();
                    time = end_time.Split(":");

                    if (hourMinute >= new TimeSpan(Convert.ToInt16(time[0]), Convert.ToInt16(time[1]), 0))
                    {
                        throw (new SameDayException());
                    }
                    else
                    {

                        if (checkClash(date, start_time, end_time))
                        {
                            throw (new ClashException(getAct(date, start_time, end_time)));
                        }
                        else
                        {
                            addActivity(new Activity(activity_name, start_time, end_time), date);
                        }

                    }

                }
            }
            catch (System.FormatException)
            {
                Console.WriteLine("You entered the format wrong!");
            }
            catch (PastDateException)
            {
                Console.WriteLine("You can't schedule to a past date!");
            }
            catch (SameDayException)
            {
                Console.WriteLine("The activity must end in the same day!");
            }
            catch (EndTimeGreaterThanStartTime)
            {
                Console.WriteLine("Ending time can not be earlier than the start time!");
            }
            catch (ClashException e)
            {
                Console.WriteLine(e.Message);


                Update(e.act);



            }
        }
        public void Repeat(Activity act, string date, int id)
        {
            Activity repAct = new RepeatingActivity(repeatingID,act.Name,act.Start,act.End);

            activities[activities.Count - 1] = repAct;
            Program.calendarList[id].activities[Program.calendarList[id].activities.Count - 1] = repAct;




            Console.WriteLine("How many weeks do you want to repeat: ");
            int repeat = Convert.ToInt32(Console.ReadLine());
            
            string[] myDate = date.Split(".");
            for (int i = 0; i < repeat; i++)
            {
                

                if(checkDays(myDate[1], myDate[2]) == 31)
                {

                    if (Convert.ToInt32(myDate[0]) + 7 <= 31)
                    {
                        myDate[0] = (Convert.ToInt32(myDate[0]) + 7).ToString();

                        if (myDate[0].Length < 2)
                        {
                            myDate[0] = "0" + myDate[0];
                        } 

                        if(!checkClash((myDate[0] + "." + myDate[1] + "." + myDate[2]), act.Start, act.End)){
                            activities.Add(repAct);
                            dates.Add(new DateObj(myDate[0] + "." + myDate[1] + "." + myDate[2]));
                        }

                        if (!checkClash((myDate[0] + "." + myDate[1] + "." + myDate[2]), act.Start, act.End,Program.calendarList[id]))
                        {
                            Program.calendarList[id].activities.Add(repAct);
                            Program.calendarList[id].dates.Add(new DateObj(myDate[0] + "." + myDate[1] + "." + myDate[2]));
                        }


                        



                    }
                    else
                    {
                        myDate[0] = (Convert.ToInt32(myDate[0])+7 - 31).ToString();

                        if (myDate[0].Length < 2)
                        {
                            myDate[0] = "0" + myDate[0];
                        }

                        if (Convert.ToInt32(myDate[1]) + 1 <= 12)
                        {
                            myDate[1] = (Convert.ToInt32(myDate[1]) + 1).ToString();
                            repeat--;
                            

                            if (!checkClash((myDate[0] + "." + myDate[1] + "." + myDate[2]), act.Start, act.End))
                            {
                                activities.Add(repAct);
                                dates.Add(new DateObj(myDate[0] + "." + myDate[1] + "." + myDate[2]));
                            }

                            if (!checkClash((myDate[0] + "." + myDate[1] + "." + myDate[2]), act.Start, act.End, Program.calendarList[id]))
                            {
                                Program.calendarList[id].activities.Add(repAct);
                                Program.calendarList[id].dates.Add(new DateObj(myDate[0] + "." + myDate[1] + "." + myDate[2]));
                            }

                        }
                        else
                        {

                            myDate[1] = "01";
                            int year = Convert.ToInt32(myDate[2]);
                            year++;
                            myDate[2] = year.ToString();
                            Console.WriteLine(myDate[2]);
                            Console.WriteLine(myDate[1]);
                            Console.WriteLine(myDate[0]);
                            repeat--;
                          

                            if (!checkClash((myDate[0] + "." + myDate[1] + "." + myDate[2]), act.Start, act.End))
                            {
                                activities.Add(repAct);
                                dates.Add(new DateObj(myDate[0] + "." + myDate[1] + "." + myDate[2]));
                            }

                            if (!checkClash((myDate[0] + "." + myDate[1] + "." + myDate[2]), act.Start, act.End, Program.calendarList[id]))
                            {
                                Program.calendarList[id].activities.Add(repAct);
                                Program.calendarList[id].dates.Add(new DateObj(myDate[0] + "." + myDate[1] + "." + myDate[2]));
                            }




                        }
                    }


                }
                else if(checkDays(myDate[1], myDate[2]) == 30)
                {

                    if (Convert.ToInt32(myDate[0]) + 7 <= 30)
                    {
                        myDate[0] = (Convert.ToInt32(myDate[0]) + 7).ToString();

                        if (myDate[0].Length < 2)
                        {
                            myDate[0] = "0" + myDate[0];
                        }

                        if (!checkClash((myDate[0] + "." + myDate[1] + "." + myDate[2]), act.Start, act.End))
                        {
                            activities.Add(repAct);
                            dates.Add(new DateObj(myDate[0] + "." + myDate[1] + "." + myDate[2]));
                        }

                        if (!checkClash((myDate[0] + "." + myDate[1] + "." + myDate[2]), act.Start, act.End, Program.calendarList[id]))
                        {
                            Program.calendarList[id].activities.Add(repAct);
                            Program.calendarList[id].dates.Add(new DateObj(myDate[0] + "." + myDate[1] + "." + myDate[2]));
                        }

                    }
                    else
                    {
                        myDate[0] = (Convert.ToInt32(myDate[0])+7 - 30).ToString();

                        if (myDate[0].Length < 2)
                        {
                            myDate[0] = "0" + myDate[0];
                        }

                        if (Convert.ToInt32(myDate[1]) + 1 <= 12)
                        {
                            myDate[1] = (Convert.ToInt32(myDate[1]) + 1).ToString();
                            repeat--;
                            if (!checkClash((myDate[0] + "." + myDate[1] + "." + myDate[2]), act.Start, act.End))
                            {
                                activities.Add(repAct);
                                dates.Add(new DateObj(myDate[0] + "." + myDate[1] + "." + myDate[2]));
                            }

                            if (!checkClash((myDate[0] + "." + myDate[1] + "." + myDate[2]), act.Start, act.End, Program.calendarList[id]))
                            {
                                Program.calendarList[id].activities.Add(repAct);
                                Program.calendarList[id].dates.Add(new DateObj(myDate[0] + "." + myDate[1] + "." + myDate[2]));
                            }

                        }
                        else
                        {

                            myDate[1] = "01";
                            int year = Convert.ToInt32(myDate[2]);
                            year++;
                            myDate[2] = year.ToString();
                            Console.WriteLine(myDate[2]);
                            Console.WriteLine(myDate[1]);
                            Console.WriteLine(myDate[0]);
                            repeat--;
                            if (!checkClash((myDate[0] + "." + myDate[1] + "." + myDate[2]), act.Start, act.End))
                            {
                                activities.Add(repAct);
                                dates.Add(new DateObj(myDate[0] + "." + myDate[1] + "." + myDate[2]));
                            }

                            if (!checkClash((myDate[0] + "." + myDate[1] + "." + myDate[2]), act.Start, act.End, Program.calendarList[id]))
                            {
                                Program.calendarList[id].activities.Add(repAct);
                                Program.calendarList[id].dates.Add(new DateObj(myDate[0] + "." + myDate[1] + "." + myDate[2]));
                            }


                        }
                    }

                }

               

            }

            repeatingID++;
        }

        public void Repeat(Activity act, string date)
        {

            Activity repAct = new RepeatingActivity(repeatingID,act.Name, act.Start, act.End);
            activities[activities.Count - 1] = repAct;

           

            Console.WriteLine("How many weeks do you want to repeat: ");
            int repeat = Convert.ToInt32(Console.ReadLine());

            string[] myDate = date.Split(".");
            for (int i = 0; i < repeat; i++)
            {


                if (checkDays(myDate[1], myDate[2]) == 31)
                {

                    if (Convert.ToInt32(myDate[0]) + 7 <= 31)
                    {
                        myDate[0] = (Convert.ToInt32(myDate[0]) + 7).ToString();

                        if (myDate[0].Length < 2)
                        {
                            myDate[0] = "0" + myDate[0];
                        }

                        if (!checkClash((myDate[0] + "." + myDate[1] + "." + myDate[2]), act.Start, act.End))
                        {
                            activities.Add(repAct);
                            dates.Add(new DateObj(myDate[0] + "." + myDate[1] + "." + myDate[2]));
                        }


                    }
                    else
                    {
                        myDate[0] = (Convert.ToInt32(myDate[0]) + 7 - 31).ToString();

                        if (myDate[0].Length < 2)
                        {
                            myDate[0] = "0" + myDate[0];
                        }

                        if (Convert.ToInt32(myDate[1]) + 1 <= 12)
                        {
                            myDate[1] = (Convert.ToInt32(myDate[1]) + 1).ToString();
                            repeat--;
                            if (!checkClash((myDate[0] + "." + myDate[1] + "." + myDate[2]), act.Start, act.End))
                            {
                                activities.Add(repAct);
                                dates.Add(new DateObj(myDate[0] + "." + myDate[1] + "." + myDate[2]));
                            }

                        }
                        else
                        {

                            myDate[1] = "01";
                            int year = Convert.ToInt32(myDate[2]);
                            year++;
                            myDate[2] = year.ToString();
                            Console.WriteLine(myDate[2]);
                            Console.WriteLine(myDate[1]);
                            Console.WriteLine(myDate[0]);
                            repeat--;
                            if (!checkClash((myDate[0] + "." + myDate[1] + "." + myDate[2]), act.Start, act.End))
                            {
                                activities.Add(repAct);
                                dates.Add(new DateObj(myDate[0] + "." + myDate[1] + "." + myDate[2]));
                            }




                        }
                    }


                }
                else if (checkDays(myDate[1], myDate[2]) == 30)
                {

                    if (Convert.ToInt32(myDate[0]) + 7 <= 30)
                    {
                        myDate[0] = (Convert.ToInt32(myDate[0]) + 7).ToString();

                        if (myDate[0].Length < 2)
                        {
                            myDate[0] = "0" + myDate[0];
                        }

                        if (!checkClash((myDate[0] + "." + myDate[1] + "." + myDate[2]), act.Start, act.End))
                        {
                            activities.Add(repAct);
                            dates.Add(new DateObj(myDate[0] + "." + myDate[1] + "." + myDate[2]));
                        }

                    }
                    else
                    {
                        myDate[0] = (Convert.ToInt32(myDate[0]) + 7 - 30).ToString();

                        if (myDate[0].Length < 2)
                        {
                            myDate[0] = "0" + myDate[0];
                        }

                        if (Convert.ToInt32(myDate[1]) + 1 <= 12)
                        {
                            myDate[1] = (Convert.ToInt32(myDate[1]) + 1).ToString();
                            repeat--;
                            if (!checkClash((myDate[0] + "." + myDate[1] + "." + myDate[2]), act.Start, act.End))
                            {
                                activities.Add(repAct);
                                dates.Add(new DateObj(myDate[0] + "." + myDate[1] + "." + myDate[2]));
                            }

                        }
                        else
                        {

                            myDate[1] = "01";
                            int year = Convert.ToInt32(myDate[2]);
                            year++;
                            myDate[2] = year.ToString();
                            Console.WriteLine(myDate[2]);
                            Console.WriteLine(myDate[1]);
                            Console.WriteLine(myDate[0]);
                            repeat--;
                            if (!checkClash((myDate[0] + "." + myDate[1] + "." + myDate[2]), act.Start, act.End))
                            {
                                activities.Add(repAct);
                                dates.Add(new DateObj(myDate[0] + "." + myDate[1] + "." + myDate[2]));
                            }


                        }
                    }

                }



            }
            repeatingID++;

        }

        int checkDays(string mon, string yea)
        {
            int month = Convert.ToInt32(mon);
            int year = Convert.ToInt32(yea);
            return DateTime.DaysInMonth(year, month);

        }
        public void Search(string name)
        {
            Console.Clear();
            int item_cnt = 0;
            int cnt = 0;

            foreach (var key in activities)
            {

                if (key.Name.ToLower().Contains(name.ToLower())) {

                    Console.WriteLine(key.ToString());
                    Console.WriteLine("Date: " + dates[cnt]);
                    Console.WriteLine();
                    item_cnt++;

                }
                cnt++;
            }
            Console.WriteLine(cnt + " activities found.");

            Console.ReadLine();
            Console.Clear();

        }
        public void Update(Activity act)
        {
            try
            {
                Console.WriteLine("------------------");
                Console.WriteLine("1-) Change Date");
                Console.WriteLine("2-) Change Time");
                Console.WriteLine("------------------");
                Console.Write("Option: ");

                int opt = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine();
                int cnt = 0;
                if (opt == 1)
                {
                    Console.WriteLine("Enter new date: ");
                    string newDate = Console.ReadLine();

                    if (!checkClash(newDate, act.Start, act.End))
                    {
                        foreach (var item in activities)
                        {
                            if (item == act)
                            {
                                
                                dates[cnt].Date = newDate;
                                break;
                            }
                            cnt++;


                        }

                    }

                    Console.WriteLine("Activity is updated!");
                    Console.ReadLine();

                }

                if (opt == 2)
                {
                    Console.WriteLine("Enter the starting time: ");
                    string stTime = Console.ReadLine();


                    Console.WriteLine("Enter the ending time: ");
                    string endTime = Console.ReadLine();


                    string[] time;
                    time = stTime.Split(":");

                    TimeSpan hourMinute;
                    hourMinute = new TimeSpan(Convert.ToInt16(time[0]), Convert.ToInt16(time[1]), 0);



                    time = endTime.Split(":");

                    if (hourMinute >= new TimeSpan(Convert.ToInt16(time[0]), Convert.ToInt16(time[1]), 0))
                    {
                        throw (new SameDayException());
                    }
                    else
                    {



                        foreach (var item in activities)
                        {
                            if (item == act)
                            {

                                break;
                            }
                            cnt++;


                        }

                        if (!checkClash(dates[cnt].Date, stTime, endTime, act))
                        {
                            activities[cnt].Start = stTime;
                            activities[cnt].Start_Time = new TimeSpan(Convert.ToInt32(stTime.Split(":")[0]), Convert.ToInt32(stTime.Split(":")[1]), 0);

                            activities[cnt].End = endTime;
                            activities[cnt].End_Time = new TimeSpan(Convert.ToInt32(endTime.Split(":")[0]), Convert.ToInt32(endTime.Split(":")[1]), 0);
                            Console.WriteLine("Activity is updated!");


                        }
                        else
                        {
                            throw (new ClashException(getAct(dates[cnt].Date, stTime, endTime)));
                        }
                    }
                }
            }






            catch (System.FormatException)
            {
                Console.WriteLine("You entered the format wrong!");
                Console.ReadLine();
                Console.Clear();
            }
            catch (PastDateException)
            {
                Console.WriteLine("You can't schedule to a past date!");
                Console.ReadLine();
                Console.Clear();


            }
            catch (SameDayException)
            {
                Console.WriteLine("The activity must end in the same day!");
                Console.ReadLine();
                Console.Clear();

            }
            catch (EndTimeGreaterThanStartTime)
            {
                Console.WriteLine("Ending time can not be earlier than the start time!");
                Console.ReadLine();
                Console.Clear();

            }
            catch (ClashException e)
            {
                Console.WriteLine(e.Message);

                Console.Clear();

            }


        }

  

        public void Update()
        {
            try {
                Console.Write("Enter the activity's date you want to update: ");
                string date = Console.ReadLine();
                Console.WriteLine();
                Console.Clear();
                bool noItem = true;

                List<int> activityID = new List<int>();
                int cnt = 1;
                int id_cnt = 0;
                

                foreach (var item in dates)
                {
                    if (item.Date.Equals(date))
                    {
                        Console.WriteLine(cnt + "-)");
                        Console.WriteLine(activities[id_cnt].ToString());
                        activityID.Add(id_cnt);
                        noItem = false;
                        cnt++;
                        Console.WriteLine();
                    }
                    id_cnt++;
                }

                if (noItem)
                {
                    Console.WriteLine("There is no scheduled activity on " + date);
                    Console.ReadLine();
                    Console.Clear();
                }
                else
                {
                    Console.Write("Enter the activity's ID you want to update: ");
                    int updatedID = Convert.ToInt32(Console.ReadLine());

                    if (updatedID == -1)
                    {
                        Console.WriteLine("Updating Canceled!");
                    }
                    else if (updatedID > cnt || updatedID <= 0)
                    {
                        Console.WriteLine("There is no ID with " + updatedID);
                    }
                    else
                    {

                        Console.WriteLine("1-) Change Name");
                        Console.WriteLine("2-) Change Date");
                        Console.WriteLine("3-) Change Time");

                        Console.WriteLine("Option: ");

                        int opt = Convert.ToInt32(Console.ReadLine());

                        if (opt == 1)
                        {
                            Console.WriteLine("Enter the new name: ");
                            string newName = Console.ReadLine();
                            activities[activityID[updatedID - 1]].Name = newName;


                            Console.WriteLine("Activity is updated!");
                            Console.ReadLine();

                        }

                        if (opt == 2)
                        {
                            Console.WriteLine("Enter the new date: ");
                            string newDate = Console.ReadLine();
                            

                            if (checkClash(newDate, activities[activityID[updatedID - 1]].Start, activities[activityID[updatedID - 1]].End)) {

                                throw (new ClashException());


                            }
                            else
                            {


                                dates[activityID[updatedID - 1]].Date = newDate;
                                

                                Console.WriteLine("Activity is updated!");



                            }

                            Console.ReadLine();
                            Console.Clear();




                        }


                        if (opt == 3)
                        {
                            Console.WriteLine("Enter the starting time: ");
                            string stTime = Console.ReadLine();


                            Console.WriteLine("Enter the ending time: ");
                            string endTime = Console.ReadLine();


                            string[] time;
                            time = stTime.Split(":");

                            TimeSpan hourMinute;
                            hourMinute = new TimeSpan(Convert.ToInt16(time[0]), Convert.ToInt16(time[1]), 0);



                            time = endTime.Split(":");

                            if (hourMinute >= new TimeSpan(Convert.ToInt16(time[0]), Convert.ToInt16(time[1]), 0))
                            {
                                throw (new SameDayException());
                            }
                            else {

                                if (!checkClash(dates[activityID[updatedID - 1]].Date, stTime, endTime, activities[activityID[updatedID - 1]])) {
                                    activities[activityID[updatedID - 1]].Start = stTime;
                                    activities[activityID[updatedID - 1]].Start_Time = new TimeSpan(Convert.ToInt32(stTime.Split(":")[0]), Convert.ToInt32(stTime.Split(":")[1]), 0);

                                    activities[activityID[updatedID - 1]].End = endTime;
                                    activities[activityID[updatedID - 1]].End_Time = new TimeSpan(Convert.ToInt32(endTime.Split(":")[0]), Convert.ToInt32(endTime.Split(":")[1]), 0);
                                    Console.WriteLine("Activity is updated!");


                                }
                                else
                                {
                                    throw (new ClashException(getAct(date, stTime, endTime)));
                                }
                            }

                        }




                    }

                }
            }
            catch (System.FormatException)
            {
                Console.WriteLine("You entered the format wrong!");
                Console.ReadLine();
                Console.Clear();
            }
            catch (PastDateException)
            {
                Console.WriteLine("You can't schedule to a past date!");
                Console.ReadLine();
                Console.Clear();


            }
            catch (SameDayException)
            {
                Console.WriteLine("The activity must end in the same day!");
                Console.ReadLine();
                Console.Clear();

            }
            catch (EndTimeGreaterThanStartTime)
            {
                Console.WriteLine("Ending time can not be earlier than the start time!");
                Console.ReadLine();
                Console.Clear();

            }
            catch (ClashException e)
            {
                Console.WriteLine(e.Message);

                Console.Clear();

            }


        }


     

        public void unscheduleActivity()
        {
            Console.WriteLine("Unschedule Options");
            Console.WriteLine("------------------------");
            Console.WriteLine("1-) Remove all activities");
            Console.WriteLine("2-) Remove an activity/repeating activity");
            Console.WriteLine("------------------------");
            Console.Write("Options: ");

            int unOpt = Convert.ToInt32(Console.ReadLine());

            if (unOpt == 1)
            {
                List<Activity> delAct = new List<Activity>();

                List<int> repeatID = new List<int>();
                Activity typeCheckAct = new RepeatingActivity(-1, "", "00:00", "00:00");


                foreach (var item in activities)
                {
                    if(item.IsShared == true) { 
                    delAct.Add(item);
                    }
                }

                foreach (var item in activities)
                {

                    if (item.GetType() == typeCheckAct.GetType()) { 
                        repeatID.Add(item.repID);
                    }

                }



                foreach (var item in delAct)
                {

                        foreach (var cal in Program.calendarList)
                        {
                           
                        
                            if(cal != this) { 
                            foreach (var calAct in cal.activities)
                            {

                                if (calAct == item)
                                 {

                                    int x = cal.activities.IndexOf(calAct);
                                    cal.dates.RemoveAt(x);
                                    cal.activities.Remove(calAct);
                                    break;

                                 }

                           }
                        }


                    }
                }
                



                foreach (var id in repeatID)
                {


                    int idToRemove = id;

                    foreach (var cal in Program.calendarList)
                    {


                        int local_cnt = 0;

                        List<int> itemID = new List<int>();

                        foreach (var item in cal.activities)
                        {

                            if (item.repID == idToRemove)
                            {
                                itemID.Add(local_cnt);
                            }

                            local_cnt++;
                        }

                        itemID.Sort();
                        itemID.Reverse();

                        foreach (var item in itemID)
                        {

                            cal.activities.RemoveAt(item);
                            cal.dates.RemoveAt(item);
                        }






                    }

                }

                for (int i = activities.Count() - 1; 0 <= i; i--)
                {

                    activities.RemoveAt(i);
                    dates.RemoveAt(i);

                }

            }
            else
            {

                Console.Clear();
                Console.Write("Enter the unwanted activity's date: ");
                string date = Console.ReadLine();
                Console.WriteLine();
                Console.Clear();
                bool noItem = true;

                List<int> activityID = new List<int>();
                int cnt = 1;
                int id_cnt = 0;

                foreach (var item in dates)
                {
                    if (item.Date.Equals(date))
                    {
                        Console.WriteLine(cnt + "-)");
                        Console.WriteLine(activities[id_cnt].ToString());
                        activityID.Add(id_cnt);
                        noItem = false;
                        cnt++;
                        Console.WriteLine();
                    }
                    id_cnt++;
                }

                if (noItem)
                {
                    Console.WriteLine("There is no scheduled activity on " + date);
                }
                else
                {
                    Console.Write("Enter the unwanted activity's ID: ");
                    int unwantedID = Convert.ToInt32(Console.ReadLine());

                    if (unwantedID == -1)
                    {
                        Console.WriteLine("Unschedule Canceled!");
                    }
                    else if (unwantedID > cnt)
                    {
                        Console.WriteLine("There is no ID with " + unwantedID);
                    }
                    else
                    {
                        Activity typeCheckAct = new RepeatingActivity(-1, "", "00:00", "00:00");


                        if (activities[activityID[unwantedID - 1]].GetType() != typeCheckAct.GetType())
                        {
                            activities.Remove(activities[activityID[unwantedID - 1]]);
                            Console.WriteLine("Activity is unscheduled!");


                        }
                        else
                        {

                            Console.WriteLine("Unschedule Options:");
                            Console.WriteLine("------------------------");
                            Console.WriteLine("1-) Unschedule entire set");
                            Console.WriteLine("2-) Unschedule single entity");
                            Console.WriteLine("------------------------");
                            Console.Write("Options: ");

                            int delOpt = Convert.ToInt32(Console.ReadLine());

                            if (delOpt == 1)
                            {
                                int idToRemove = activities[activityID[unwantedID - 1]].repID;

                                foreach (var cal in Program.calendarList)
                                {


                                    int local_cnt = 0;

                                    List<int> itemID = new List<int>();

                                    foreach (var item in cal.activities)
                                    {

                                        if (item.repID == idToRemove)
                                        {
                                            itemID.Add(local_cnt);
                                        }

                                        local_cnt++;
                                    }

                                    itemID.Sort();
                                    itemID.Reverse();

                                    foreach (var item in itemID)
                                    {

                                        cal.activities.RemoveAt(item);
                                        cal.dates.RemoveAt(item);
                                    }




                                }

                            }
                            else if (delOpt == 2)
                            {
                                int idToRemove = activities[activityID[unwantedID - 1]].repID;

                                foreach (var cal in Program.calendarList)
                                {


                                    int local_cnt = 0;

                                    List<int> itemID = new List<int>();

                                    foreach (var item in cal.activities)
                                    {

                                        if (item.repID == idToRemove && cal.dates[local_cnt].Date.Equals(date))
                                        {
                                            itemID.Add(local_cnt);
                                        }

                                        local_cnt++;
                                    }

                                    itemID.Sort();
                                    itemID.Reverse();

                                    foreach (var item in itemID)
                                    {

                                        cal.activities.RemoveAt(item);
                                        cal.dates.RemoveAt(item);
                                    }




                                }
                            }
                            else
                            {
                                Console.WriteLine("Error!");
                            }








                        }
                    }

                }
            }
        }
        TimeSpan stringToTimeSpan(string time)
        {
            string[] timeArray = time.Split(":");

            return new TimeSpan(Convert.ToInt32(timeArray[0]), Convert.ToInt32(timeArray[1]), 0);
        }
        Activity getAct(string date, string starting_time, string ending_time)
        {

            TimeSpan starting_hour = stringToTimeSpan(starting_time);
            TimeSpan ending_hour = stringToTimeSpan(ending_time);
            int cnt = 0;

            foreach (var item in dates)
            {

                if (item.Equals(date))
                {

                    if (starting_hour >= activities[cnt].Start_Time && starting_hour < activities[cnt].End_Time)
                    {
                        return activities[cnt];

                    }

                    if (ending_hour > activities[cnt].Start_Time && ending_hour <= activities[cnt].End_Time)
                    {

                        return activities[cnt];
                    }

                    if (starting_hour <= activities[cnt].Start_Time && ending_hour >= activities[cnt].End_Time)
                    {

                        return activities[cnt];
                    }

                }
                cnt++;

            }

            return null;

        }
        bool checkClash(string date, string starting_time, string ending_time)
        {
            bool clash = false;
            int cnt = 0;

            TimeSpan starting_hour = stringToTimeSpan(starting_time);
            TimeSpan ending_hour = stringToTimeSpan(ending_time);

            foreach (var item in dates)
            {

                if (item.Equals(date))
                {

                    if (starting_hour >= activities[cnt].Start_Time && starting_hour < activities[cnt].End_Time)
                    {
                        Console.WriteLine();

                        clash = true;
                        break;
                    }

                    if (ending_hour > activities[cnt].Start_Time && ending_hour <= activities[cnt].End_Time)
                    {
                        Console.WriteLine();

                        clash = true;
                        break;
                    }

                    if (starting_hour <= activities[cnt].Start_Time && ending_hour >= activities[cnt].End_Time)
                    {
                        Console.WriteLine();

                        clash = true;
                        break;
                    }

                }
                cnt++;
            }

            return clash;
        }
        bool checkClash(string date, string starting_time, string ending_time, Calendar cal)
        {
            bool clash = false;

            TimeSpan starting_hour = stringToTimeSpan(starting_time);
            TimeSpan ending_hour = stringToTimeSpan(ending_time);
            int cnt = 0;

            foreach (var item in cal.dates)
            {

                if (item.Equals(date))
                {

                    if (starting_hour >= activities[cnt].Start_Time && starting_hour < activities[cnt].End_Time)
                    {
                        Console.WriteLine();

                        clash = true;
                        break;
                    }

                    if (ending_hour > activities[cnt].Start_Time && ending_hour <= activities[cnt].End_Time)
                    {
                        Console.WriteLine();

                        clash = true;
                        break;
                    }

                    if (starting_hour <= activities[cnt].Start_Time && ending_hour >= activities[cnt].End_Time)
                    {
                        Console.WriteLine();

                        clash = true;
                        break;
                    }

                }
                cnt++;
            }

            return clash;
        }
        bool checkClash(string date, string starting_time, string ending_time, Activity act)
        {
            bool clash = false;

            TimeSpan starting_hour = stringToTimeSpan(starting_time);
            TimeSpan ending_hour = stringToTimeSpan(ending_time);
            int cnt = 0;

            foreach (var item in activities)
            {
                if (item != act) {
                    if (dates[cnt].Equals(date))
                    {

                        if (starting_hour >= item.Start_Time && starting_hour < item.End_Time)
                        {
                            clash = true;
                            break;
                        }

                        if (ending_hour > item.Start_Time && ending_hour <= item.End_Time)
                        {
                            clash = true;
                            break;
                        }

                        if (starting_hour <= item.Start_Time && ending_hour >= item.End_Time)
                        {
                            clash = true;
                            break;
                        }

                    }
                }
                cnt++;
            }

            return clash;
        }

        static bool isShare;


        public void addActivity(Activity activity, string date)
        {
            activities.Add(activity);
            dates.Add(new DateObj(date));

            Console.WriteLine("---------------------------------");
            Console.WriteLine("Activity Scheduled!");
            Console.WriteLine("---------------------------------");
            Console.WriteLine("Do you want to share this activity?");
            Console.WriteLine("1-) Yes");
            Console.WriteLine("2-) No");
            Console.WriteLine("---------------------------------");
            Console.Write("Option: ");

            isShare = false;

            int id = -1;
            try
            { 
            int opt = Convert.ToInt32(Console.ReadLine());

            if(opt == 1)
            {

                    id = Share(activity, dates[dates.Count - 1]);

                    
       


                }
                Console.WriteLine("---------------------------------");
                Console.WriteLine("Is this a repeating activity?");
                Console.WriteLine("---------------------------------");
                Console.WriteLine("1-) Yes");
                Console.WriteLine("2-) No");
                Console.WriteLine("---------------------------------");
                Console.Write("Option: ");
                opt = Convert.ToInt32(Console.ReadLine());

                if (opt == 1)
                {

                    if (isShare) {
                        Repeat(activity, date, id);

                    }
                    else
                    {
                        Repeat(activity, date);

                    }
                }
               

            }
            catch (System.FormatException)
            {
                Console.WriteLine("Wrong Format1");
            }

        }
        public void addTempAct(Activity activity, string date)
        {
            activities.Add(activity);
            dates.Add(new DateObj(date));
        }
        public void showScheduled()
        {

            List<String> dateList = new List<String>();

            foreach (var item in dates)
            {
                if (!dateList.Contains(item.Date))
                {
                    dateList.Add(item.Date);
                }
            }

            dateList.Sort();

            Console.WriteLine("Scheduled Activities");
            Console.WriteLine("------------------------");
            for (int i = 0; i < dateList.Count; i++)
            {
                Console.WriteLine(dateList[i]);
                Console.WriteLine("----------------");
                List<String> startingHours = new List<String>();

                for (int j = 0; j < activities.Count; j++)
                {
                    if (dates[j].Date.Equals(dateList[i]))
                    {

                        startingHours.Add(activities[j].Start);


                    }
                }

                startingHours.Sort();


                for (int l = 0; l < startingHours.Count; l++)
                {

                    for (int m = 0; m < activities.Count; m++)
                    {

                        if (dates[m].Date.Equals(dateList[i]))
                        {

                            if (activities[m].Start.Equals(startingHours[l]))
                            {
                                Console.WriteLine(activities[m].ToString());
                                Console.WriteLine();
                            }

                        }

                    }





                }




            }

        }
        public override string ToString()
        {

            if (activities.Count == 0 || activities.Count == 1)
            {
                return "You have " + activities.Count + " scheduled activity.";
            }
            else
            {
                return "You have " + activities.Count + " scheduled activities.";
            }
        }
        public void showCalendar()
        {
            Console.WriteLine("Which year do you want to show?");
            int year = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine();

            string[] months = { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };

            for (int i = 0; i < months.Length; i++)
            {

                Console.WriteLine(months[i] + "\n------------------------");
                int daysOfTheMonth = DateTime.DaysInMonth(year, i + 1);


                for (int j = 1; j <= daysOfTheMonth; j++)
                {

                    if (j % 7 != 0)
                    {
                        Console.Write(j + " ");
                    }
                    else
                    {
                        Console.Write(j);
                        Console.WriteLine();
                    }

                }

                Console.WriteLine("\n");

            }

        }

        
      

        public void Share()
        {

            Console.Write("Enter the activity's date you want to update: ");
            string date = Console.ReadLine();
            Console.WriteLine();
            Console.Clear();
            bool noItem = true;

            List<int> activityID = new List<int>();
            int cnt = 1;
            int id_cnt = 0;

            foreach (var item in dates)
            {

                if (item.Date.Equals(date))
                {
                    Console.WriteLine(cnt + "-)");
                    Console.WriteLine(activities[id_cnt].ToString());
                    activityID.Add(id_cnt);
                    noItem = false;
                    cnt++;
                    Console.WriteLine();
                }
                id_cnt++;
            }

            if (noItem)
            {
                Console.WriteLine("There is no scheduled activity on " + date);
                Console.ReadLine();
                Console.Clear();
            }
            else
            {
                Console.Write("Enter the activity's ID you want to share: ");
                int updatedID = Convert.ToInt32(Console.ReadLine());

                if (updatedID == -1)
                {
                    Console.WriteLine("Sharing Canceled!");
                }
                else if (updatedID > cnt || updatedID <= 0)
                {
                    Console.WriteLine("There is no ID with " + updatedID);
                }
                else
                {

                    updatedID--;

                    try
                    {
                        Console.Clear();
                        int calICnt = 0;
                        Console.WriteLine("---------------------");
                        Console.WriteLine("Calendar List");
                        Console.WriteLine("---------------------");
                        foreach (var calendar in Program.calendarList)
                        {

                            Console.WriteLine((calICnt + 1) + "-)" + calendar.CalendarName);
                            calICnt++;

                        }
                        Console.WriteLine("---------------------");


                        Console.Write("Enter Calendar ID: ");
                        int calID = Convert.ToInt32(Console.ReadLine()) - 1;

                        if (Program.calendarList[calID] != this) {

                            if (!(calID < 0) && !(calID >= Program.calendarList.Count))
                            {

                                if (!checkClash(dates[updatedID].Date, activities[updatedID].Start, activities[updatedID].End, Program.calendarList[calID])) {


                                    Random rand = new Random();
                                    int prob = rand.Next(100);

                                    if(prob <= 50) { 
                                    activities[updatedID].IsShared = true;
                                    
                                    Program.calendarList[calID].dates.Add(dates[updatedID]);
                                    Program.calendarList[calID].activities.Add(activities[updatedID]);
                       

                                    Console.WriteLine("Activity Shared!");
                                    }
                                    else
                                    {
                                        throw new ShareException();
                                    }

                                }
                                else
                                {
                                    Console.Write("There is a clash!");
                                    Console.ReadLine();
                                }

                            }
                            else
                            {
                                Console.WriteLine("Wrong Input!");
                                Console.ReadLine();
                                Console.Clear();

                            }
                        }
                        else
                        {
                            Console.WriteLine("You can't share activity with the same calendar!");
                            Console.ReadLine();
                        }
                    }
                    catch (System.FormatException)
                    {
                        Console.WriteLine("Wrong Format!");
                        Console.ReadLine();
                    }catch (ShareException e)
                    {
                        Console.WriteLine(e.Message);
                        Console.ReadLine();
                    }







                }

            }


        }
        public int Share(Activity act, DateObj date)
        {

            act.IsShared = true;
            int calID = -1;
            try
            {
                Console.Clear();
                int calICnt = 0;
                Console.WriteLine("---------------------");
                Console.WriteLine("Calendar List");
                Console.WriteLine("---------------------");
                foreach (var calendar in Program.calendarList)
                {

                    Console.WriteLine((calICnt + 1) + "-)" + calendar.CalendarName);
                    calICnt++;

                }
                Console.WriteLine("---------------------");


                Console.Write("Enter Calendar ID: ");
                calID = Convert.ToInt32(Console.ReadLine()) - 1;

                Random rand = new Random();
                int prob = rand.Next(100);


                if(prob <= 50)
                {
                    Console.WriteLine("Share request is rejected by the Calendar Owner!");

                }
                else
                {


                    isShare = true;

                if (Program.calendarList[calID] != this)
                {

                    if (!(calID < 0) && !(calID >= Program.calendarList.Count))
                    {

                        if (!checkClash(date.Date, act.Start, act.End, Program.calendarList[calID]))
                        {

                            Program.calendarList[calID].activities.Add(act);
                            Program.calendarList[calID].dates.Add(date);

                            Program.calendarList[calID].activities[Program.calendarList[calID].activities.Count -1 ] = act;
                            Program.calendarList[calID].dates[Program.calendarList[calID].dates.Count -1] = date;
                            Console.WriteLine("Activity Shared!");

                        }
                        else
                        {
                            Console.Write("There is a clash!");
                            Console.ReadLine();
                        }

                    }
                    else
                    {
                        Console.WriteLine("Wrong Input!");
                        Console.ReadLine();
                        Console.Clear();

                    }
                }
                else
                {
                    Console.WriteLine("You can't share activity with the same calendar!");
                    Console.ReadLine();
                }


              }
            }
            catch (System.FormatException)
            {
                Console.WriteLine("Wrong Format!");
                Console.ReadLine();

            }
            catch (ShareException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();

            }
            return calID;

        }

        public class ShareException : Exception
        {
            public ShareException()
            {

                Console.WriteLine("Share request is rejected by the Calendar Owner!");

            }


        }

        public class ClashException : Exception
        {
            public Activity act;
            public ClashException(Activity act) : base(String.Format("Activity clashes with:\n{0}", act))
            {
                this.act = act;

            }

            public ClashException()
            {

            }
        }
        public class SameDayException : Exception
        {
            public SameDayException(string message) : base(message)
            {
            }

            public SameDayException()
            {

            }
        }
        public class EndTimeGreaterThanStartTime : Exception
        {
            public EndTimeGreaterThanStartTime(string message) : base(message)
            {
            }

            public EndTimeGreaterThanStartTime()
            {

            }
        }
        public class PastDateException : Exception
        {
            public PastDateException(string message) : base(message)
            {
            }

            public PastDateException()
            {

            }
        }




    }




      }
 

