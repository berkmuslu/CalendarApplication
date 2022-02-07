using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Assignment5
{

    interface CalendarFeature
    {

        public void Write();
        public void Read();

    }

    class BinaryFeature : CalendarFeature
    {

        public void Write() {

            string path = AppDomain.CurrentDomain.BaseDirectory + @"\saves\binary";
            DirectoryInfo di = Directory.CreateDirectory(path);

            FileStream fs = new FileStream(AppDomain.CurrentDomain.BaseDirectory + @"\saves\binary\DataFile.dat", FileMode.Create);

            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                formatter.Serialize(fs, Program.calendarList);
            }
            catch (SerializationException e)
            {
                Console.WriteLine("Failed to serialize. Reason: " + e.Message);
                Console.ReadLine();
            }
            finally
            {
                fs.Close();
            }

        }

        public void Read() {


            FileStream fs = new FileStream(AppDomain.CurrentDomain.BaseDirectory + @"\saves\binary\DataFile.dat", FileMode.Open);
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();

                Program.calendarList = (List<Calendar>)formatter.Deserialize(fs);
            }
            catch (SerializationException e)
            {
                Console.WriteLine("Failed to deserialize. Reason: " + e.Message);
                throw;
            }
            finally
            {
                fs.Close();
            }
          

        }

    }


    class JsonFeature : CalendarFeature
    {
        public void Read()
        {
            try { 
            string jsonString = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"\saves\json\calendars.json");
            Program.calendarList = JsonSerializer.Deserialize<List<Calendar>>(jsonString);

              

                for (int i = 0; i < Program.calendarList.Count; i++)
                {
                    jsonString = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"\saves\json\activities"+ i +".json");
                    Program.calendarList[i].activities = JsonSerializer.Deserialize<List<Activity>>(jsonString);



                    jsonString = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"\saves\json\dates" + i + ".json");
                    Program.calendarList[i].dates = JsonSerializer.Deserialize<List<DateObj>>(jsonString);
                }


            }
            catch(Exception e)
            {
               
            }

    }

    public void Write()
        {

            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + @"\saves\json";
                DirectoryInfo di = Directory.CreateDirectory(path);

                di = new DirectoryInfo(path);

                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }


                string json = JsonSerializer.Serialize(Program.calendarList);
                File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"\saves\json\calendars.json", json);

                for (int i = 0; i < Program.calendarList.Count; i++)
                {

                    json = JsonSerializer.Serialize(Program.calendarList[i].activities);
                    File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"\saves\json\activities" + i + ".json", json);

                    json = JsonSerializer.Serialize(Program.calendarList[i].dates);
                    File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"\saves\json\dates" + i + ".json", json);

                }

            }
            catch(Exception e)
            {

            }








            }
    }

    class XMLFeature : CalendarFeature
    {
        public void Read()
        {
            try { 
           
            XmlSerializer serializer =
            new XmlSerializer(typeof(List<Calendar>));

            // Declare an object variable of the type to be deserialized.
            List<Calendar> i;

            using (Stream reader = new FileStream(AppDomain.CurrentDomain.BaseDirectory + @"\saves\xml\calendars.xml", FileMode.Open))
            {
                // Call the Deserialize method to restore the object's state.
                i = (List<Calendar>)serializer.Deserialize(reader);
            }

            Program.calendarList = i;
            }catch(Exception e)
            {

            }

        }

        public void Write()
        {


            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + @"\saves\xml";
                DirectoryInfo di = Directory.CreateDirectory(path);

                di = new DirectoryInfo(path);

                foreach (FileInfo files in di.GetFiles())
                {
                    files.Delete();
                }

                path = AppDomain.CurrentDomain.BaseDirectory + @"\saves\xml";
                di = Directory.CreateDirectory(path);



                XmlSerializer xs = new XmlSerializer(typeof(List<Calendar>));
                TextWriter txtWriter = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + @"\saves\xml\calendars.xml");
                xs.Serialize(txtWriter, Program.calendarList);
                
                txtWriter.Close();




            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
            }

        }
    }

    [Serializable]
    public class Program
    {

        public static List<Calendar> calendarList = new List<Calendar>();

        static int calendarID = -1;

        static void Main(string[] args)
        {


            calendarList.Add(new Calendar("Calendar 1"));
            calendarList.Add(new Calendar("Calendar 2"));
            calendarList.Add(new Calendar("test1"));
            calendarList.Add(new Calendar("test2"));
       
            calendarList[0].addTempAct(new Activity("Test4", "14:30", "16:30"), "20.11.2021");
            calendarList[0].addTempAct(new Activity("Test1", "18:30", "19:30"), "22.11.2021");
            calendarList[0].addTempAct(new Activity("Test3", "17:30", "18:30"), "23.11.2021");

            calendarList[1].addTempAct(new Activity("Test2", "12:30", "13:30"), "20.11.2021");
            calendarList[1].addTempAct(new Activity("Test5", "12:30", "14:00"), "21.11.2021");

            JsonFeature jsonFeature = new JsonFeature();
            XMLFeature xmlFeature = new XMLFeature();
            BinaryFeature binaryFeature = new BinaryFeature();

            xmlFeature.Read();

            

        FirstMenu:
            calendarID = -1;
            Console.Clear();

            Console.WriteLine("----------------------------");
            Console.WriteLine("Calendar App");
            Console.WriteLine("----------------------------");
            Console.WriteLine("1- Choose a Calendar");
            Console.WriteLine("2- Add a Calendar");
            Console.WriteLine("3- Remove a Calendar");
            Console.WriteLine("4- Save All Changes");
            Console.WriteLine("5- Import Calendar");
            Console.WriteLine("6- Exit");
            Console.WriteLine("----------------------------");
            Console.Write("Option: ");

            int opt = Convert.ToInt32(Console.ReadLine());
            switch (opt) { 
                case 1: //List 

                    chooseCalendar();

                    if(calendarID != -1)
                    {
                        goto CalendarMenu;
                    }

                    goto FirstMenu;

                case 2://Add

                    addCalendar();
                    goto FirstMenu;

                case 3://Remove

                    removeCalendar();
                    goto FirstMenu;

                case 4:
                    saveAll();
                    goto FirstMenu;

                case 5:
                    importCalendar();
                    goto FirstMenu;

                case 6:
                    jsonFeature.Write();
                    xmlFeature.Write();
              
                    Environment.Exit(1);
                    goto FirstMenu;

                default:
                    Console.WriteLine("Wrong Input!");
                    goto FirstMenu;

            }

        CalendarMenu:
            Console.Clear();
            Console.WriteLine(calendarList[calendarID].ToString());
            
            int option;

            Console.WriteLine("\nCalendar Menu");
            Console.WriteLine("----------------------------");
            Console.WriteLine("1-) Schedule an Activity");
            Console.WriteLine("2-) Unschedule an Activity");
            Console.WriteLine("3-) Search an Activity");
            Console.WriteLine("4-) Update an Activity");
            Console.WriteLine("5-) Share an Activity");
            Console.WriteLine("6-) See Scheduled Activities");
            Console.WriteLine("7-) See Calendar");
            Console.WriteLine("8-) Return Main Menu");
            Console.WriteLine("----------------------------");
            Console.Write("Option: ");
            option = Convert.ToInt32(Console.ReadLine());

            switch (option)
            {

                case 1:
                    Console.Clear();
                    calendarList[calendarID].scheduleActivity();
                    Console.WriteLine("(Press Any Key To Continue!)");
                    Console.ReadLine();
                    Console.Clear();
                    goto CalendarMenu;


                case 2:
                    Console.Clear();
                    calendarList[calendarID].unscheduleActivity();
                    Console.WriteLine("(Press Any Key To Continue!)");
                    Console.ReadLine();
                    Console.Clear();
                    goto CalendarMenu;

                case 3:
                    Console.Clear();
                    Console.WriteLine("Enter the name of the activity");
                    string key = Console.ReadLine();
                    calendarList[0].Search(key);
                    goto CalendarMenu;

                case 4:
                    Console.Clear();
                    calendarList[calendarID].Update();
                    goto CalendarMenu;

                case 5:
                    Console.Clear();
                    calendarList[calendarID].Share();
                    goto CalendarMenu;

                case 6:

                    Console.Clear();
                    calendarList[calendarID].showScheduled();
                    Console.WriteLine("(Press Any Key To Continue!)");
                    Console.ReadLine();
                    Console.Clear();
                    goto CalendarMenu;


                case 7:
                    Console.Clear();
                    calendarList[calendarID].showCalendar();
                    Console.WriteLine("(Press Any Key To Continue!)");
                    Console.ReadLine();
                    Console.Clear();
                    goto CalendarMenu;
                    

                case 8:

                    goto FirstMenu;

                default:
                    goto CalendarMenu;

            
           }


            void importCalendar()
            {
                Console.Clear();
                Console.WriteLine("***Import XML, JSON or Binary files to /saves/json/ or /saves/xml/ or /saves/binary/ to import***");
                Console.WriteLine("-----------------------");
                Console.WriteLine("Choose an import format");
                Console.WriteLine("-----------------------");
                Console.WriteLine("1-) JSON Format");
                Console.WriteLine("2-) XML Format");
                Console.WriteLine("3-) Binary Format");
                Console.WriteLine("-----------------------");
                Console.WriteLine("Input: ");
                int inp = Convert.ToInt32(Console.ReadLine());

                if (inp == 1)
                {
                    jsonFeature.Read();
                }
                else if (inp == 2)
                {
                    xmlFeature.Read();

                }else if(inp == 3)
                {
                    binaryFeature.Read();
                }
                else
                {
                    return;
                }




                Console.WriteLine("Calendars are imported.");

                Console.ReadLine();

            }


            void saveAll()
            {
                Console.Clear();


                Console.WriteLine("-----------------------");
                Console.WriteLine("Choose a saving format");
                Console.WriteLine("-----------------------");
                Console.WriteLine("1-) JSON Format");
                Console.WriteLine("2-) XML Format");
                Console.WriteLine("3-) Binary Format");
                Console.WriteLine("4-) All");
                Console.WriteLine("-----------------------");
                Console.WriteLine("Input: ");
                int inp = Convert.ToInt32(Console.ReadLine());

                if (inp == 1)
                {
                    jsonFeature.Write();
                }
                else if (inp == 2)
                {
                    xmlFeature.Write();
                }
                else if (inp == 3)
                {
                    binaryFeature.Write();

                }
                else if (inp == 4)
                {
                    jsonFeature.Write();
                    xmlFeature.Write();
                    binaryFeature.Write();
                }
                else
                {
                    return;
                }

                Console.WriteLine("Calendars are saved.");
                Console.ReadLine();

            }

            void chooseCalendar()
            {
                try { 
                Console.Clear();
                int calID = 0;
                Console.WriteLine("---------------------");
                Console.WriteLine("Calendar List");
                Console.WriteLine("---------------------");
                foreach (var calendar in calendarList)
                {
                    Console.WriteLine((calID + 1) + "-)" + calendar.CalendarName);
                    calID++;

                }
                Console.WriteLine("---------------------");


                Console.Write("Enter Calendar ID: ");
                int tempID = Convert.ToInt32(Console.ReadLine()) - 1;

                if (!(tempID < 0) && !(tempID >= calendarList.Count) ) { 
                    calendarID = tempID;
                    Console.WriteLine(calendarList[calendarID].CalendarName + " is selected!");

                }
                else
                {
                    Console.WriteLine("Wrong Input!");
                    Console.ReadLine();
                    Console.Clear();
                    
                }
                }
                catch (System.FormatException)
                {
                    Console.WriteLine("Wrong Format!");
                    Console.ReadLine();
                }


            }

            void addCalendar()
            {
                Console.Clear();
                Console.WriteLine("---------------------");
                Console.WriteLine("Add Calendar");
                Console.WriteLine("---------------------");
                try { 
                Console.WriteLine("Enter Calendar Name: ");
                string name = Console.ReadLine();
                calendarList.Add(new Calendar(name));
                Console.WriteLine("Calendar Added Successfully!");
                Console.ReadLine();
                }
                catch (System.FormatException)
                {
                    Console.WriteLine("Format Error!");
                }
            }

            void removeCalendar()
            {
                Console.Clear();

                int calID = 0;
                Console.WriteLine("---------------------");
                Console.WriteLine("Remove a Calendar");
                Console.WriteLine("---------------------");
                foreach (var calendar in calendarList)
                {
                    Console.WriteLine((calID + 1) + "-)" + calendar.CalendarName);
                    calID++;

                }
                Console.WriteLine("---------------------");


                Console.Write("Enter Calendar ID: ");

                int tempID = Convert.ToInt32(Console.ReadLine()) - 1;

                if (!(tempID < 0) && !(tempID >= calendarList.Count))
                {

                    Console.WriteLine(calendarList[tempID].CalendarName + " is removed!");
                    calendarList.RemoveAt(tempID);

                }
                else
                {
                    Console.WriteLine("Wrong Input!");
                    Console.ReadLine();
                    Console.Clear();

                }
            }



        }
       




    }



}