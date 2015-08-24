using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Data.Entity;
using Parser.Models;
using System.IO;
using System.Windows.Forms.DataVisualization.Charting;
using System.Drawing;

namespace Parser
{
    class Program
    {
        static FbContext db = new FbContext();
        static void Main(string[] args)
        {
            Run();
        }

        static void Run()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("+------------------------------------------------------+");
                Console.WriteLine("|               What do you want to do?                |");
                Console.WriteLine("+---+--------------------------------------------------+");
                Console.WriteLine("| 1 |  Seed Database from messages.htm                 |");
                Console.WriteLine("| 2 |  Print 10 first messages in db                   |");
                Console.WriteLine("| 3 |  List all the conversations                      |");
                Console.WriteLine("| 4 |  Join duplicates (Not working, sorry :()         |");
                Console.WriteLine("+---+--------------------------------------------------+");

                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.D1:
                        {
                            SeedDb();
                            break;
                        }
                    case ConsoleKey.D2:
                        {
                            for (int i = 1; i < 20; i++)
                            {
                                Console.WriteLine(db.FbThreads.First().Messages.ElementAt(i).Author);
                                Console.WriteLine(db.FbThreads.First().Messages.ElementAt(i).TimeSent);
                                Console.WriteLine(db.FbThreads.First().Messages.ElementAt(i).Text);
                                Console.WriteLine("==============");
                            }
                            break;
                        }
                    case ConsoleKey.D3:
                        {
                            ListAll();
                            break;
                        }
                    case ConsoleKey.D4:
                        {
                            Fix();
                            break;
                        }
                }
            }
        }

        static void Fix()
        {
            //not implemented
        }

        static void ListAll()
        {
            if (db.FbThreads.Count() < 0)
            {
                Console.WriteLine("Database is empty! Are you sure you loaded your messages?");
                return;
            }
            Console.Clear();
            Console.WriteLine("+---+----------------------------------------------------+");
            Console.WriteLine("|ID |                     Participants                   |");
            Console.WriteLine("+---+----------------------------------------------------+");
            int total = 0;
            foreach (var t in db.FbThreads.OrderBy(d => d.Participants))
            {
                Console.WriteLine(String.Format("|{0,3}|{1,52}|", t.FbThreadId, t.Participants));
                total += t.MessageCount;
            }
            Console.WriteLine("+---+------------+---------------------------------------+");
            Console.WriteLine(string.Format("| Total Messages:|{0,39}|", total));
            Console.WriteLine("+---+----------------------------------------------------+");
            int sel;
            Console.WriteLine("Enter Conversation ID to get more info:");
            int.TryParse(Console.ReadLine(), out sel);
            if (sel == 0) { return; }
            Console.Clear();
            var th = db.FbThreads.Where(d => d.FbThreadId == sel).First();
            Console.WriteLine("+------------+-------------------------------------------+");
            Console.WriteLine(string.Format("|Participants| {0,18} and {1,19}|", db.FbUsers.First().Name, th.Participants.ToString()));
                          Console.WriteLine("+------------+--+----------------------------------------+");
            Console.WriteLine(string.Format("|Total messages:|{0,40}|",th.MessageCount));   
            Console.WriteLine(string.Format("|First message: |{0,40}|", th.Messages.OrderBy(d => d.TimeSent).First().TimeSent));
            Console.WriteLine(string.Format("|Last message:  |{0,40}|", th.Messages.OrderByDescending(d => d.TimeSent).First().TimeSent));
            var MpD = th.MessageCount / (th.Messages.OrderByDescending(d => d.TimeSent).First().TimeSent - th.Messages.OrderBy(d => d.TimeSent).First().TimeSent).TotalDays;
            Console.WriteLine(string.Format("|Average MpD:   |{0,40}|", MpD));
            Console.WriteLine(string.Format("|Busiest day:   |{0,40}|", th.Messages.OrderBy(d => d.TimeSent.Day).First().TimeSent.Date));
            Console.WriteLine(string.Format("| with total:   |{0,40}|", th.Messages.GroupBy(d => d.TimeSent.Day).First().Count() + " messages"));
            Console.WriteLine("+---------------+----------------------------------------+");
            Console.WriteLine();
            Console.WriteLine("Press 1 to export file to .txt or any other key to return");
            switch (Console.ReadKey().Key)
            {
                case ConsoleKey.D1:
                    {
                        ExportThread(sel);
                        Console.WriteLine("\nDone!");
                        break;
                    }
                default:
                    {
                        return;
                    }
            }
        }

        static void ShowStats(int sel)
        {

        }

        static void ExportThread(int sel)
        {
            var _thread = db.FbThreads.Where(d => d.FbThreadId == sel).First();
            using (StreamWriter sw = new StreamWriter(_thread.Participants + ".txt"))
            {
                foreach (var m in _thread.Messages.OrderBy(d => d.TimeSent))
                {
                    sw.WriteLine("==========================");
                    sw.WriteLine(m.Author + " , " + m.TimeSent);
                    sw.WriteLine();
                    sw.WriteLine(m.Text);

                }
            }
        }

        static void SeedDb()
        {
            HtmlDocument htm = new HtmlDocument();
            Console.WriteLine("\n\nLoading messages.htm...");
            try
            {
                htm.Load("messages.htm");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error occured" + e.ToString());
                return;
            } 
            Console.WriteLine("Done!");
            Console.WriteLine();
            Console.WriteLine("Parsing Content...");

            var content = htm.DocumentNode.Descendants("div").Where(d => d.Attributes.Contains("class") && d.Attributes["class"].Value.Contains("contents")).First();
            var user = new FbUser() { Name = content.Descendants("h1").First().InnerText };
            db.FbUsers.Add(user);
            var threads = content.Descendants("div").Where(d => d.Attributes.Contains("class") && d.Attributes["class"].Value.Contains("thread"));

            Console.WriteLine(String.Format("Done! Found {0} conversations. Parsing messages...", threads.Count()));
            foreach (var t in threads)
            {
                List<FbMessage> _messages = new List<FbMessage>();
                var ttmsgs = t.Descendants("div").Where(d => d.Attributes.Contains("class") && d.Attributes["class"].Value.Contains("message") && !d.Attributes["class"].Value.Contains("message_header"));
                foreach (var m in ttmsgs)
                {
                    string _author = m.Descendants("span").Where(d => d.Attributes.Contains("class") && d.Attributes["class"].Value.Contains("user")).First().InnerText;
                    string _text = m.NextSibling.InnerText;
                    DateTime _timeSent = MetaToTime(m.Descendants("span").Where(d => d.Attributes.Contains("class") && d.Attributes["class"].Value.Contains("meta")).First().InnerText);
                    _messages.Add(new FbMessage() { Author = _author, Text = _text, TimeSent = _timeSent });
                }
                int _id = threads.ToList().IndexOf(t);
                string _participants = t.FirstChild.InnerText.Replace(user.Name + ", ", "").Replace(", " + user.Name, "");
                db.FbThreads.Add(new FbThread() { FbThreadId = _id, Messages = _messages, Participants = _participants });
                db.SaveChanges();
                Console.Write('#');
            }
            Console.WriteLine("\nDone!");
            Console.WriteLine();
            CreateChart();
            Console.WriteLine("Press any key to continue!");
            Console.ReadKey();
        }

        static DateTime MetaToTime(string meta)
        {
            int ind1 = meta.IndexOf(',') + 2;
            int year = int.Parse(meta.Substring(ind1, 4));

            int ind2 = meta.IndexOf("gada") + 5;
            int day = int.Parse(meta.Substring(ind2, 2).Replace('.', ' '));

            int ind3 = meta.IndexOf(day.ToString()) + 2;
            int month = 1;
            string metaCut = meta.Substring(ind3 + 1);
            if (metaCut.Contains("janvāris"))           { month = 1; }
            else if (metaCut.Contains("februāris"))     { month = 2; }
            else if (metaCut.Contains("marts"))         { month = 3; }
            else if (metaCut.Contains("aprīlis"))       { month = 4; }
            else if (metaCut.Contains("maijs"))         { month = 5; }
            else if (metaCut.Contains("jūnijs"))        { month = 6; }
            else if (metaCut.Contains("jūlijs"))        { month = 7; }
            else if (metaCut.Contains("augusts"))       { month = 8; }
            else if (metaCut.Contains("septembris"))    { month = 9; }
            else if (metaCut.Contains("oktobris"))      { month = 10; }
            else if (metaCut.Contains("novembris"))     { month = 11; }
            else if (metaCut.Contains("decembris"))     { month = 12; }

            //int ind4 = metaCut.IndexOf('.') + 2;
            int ind4 = metaCut.IndexOf(':') - 2;
            int hours = int.Parse(metaCut.Substring(ind4, 2));
            int minutes = int.Parse(metaCut.Substring(ind4 + 3, 2));

            DateTime time = new DateTime(year, month, day, hours, minutes, 0);
            return time;
        }

        static void CreateChart()
        {
            
        }

    }
}
