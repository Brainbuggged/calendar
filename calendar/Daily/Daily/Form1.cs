using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Drawing.Imaging;
using System.Collections;
using System.Web;

namespace Daily
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            
        }

        public string mainEvents = @"Events.dat";

        public void LoadPage()
        {
            Uri url = new Uri("https://www.life-moon.pp.ru/");
            //код для вставки в событие Click

            webBrowser1.Url = url;
            webBrowser1.ScriptErrorsSuppressed = true;
            

            MessageBox.Show(webBrowser1.IsBusy.ToString(), "IsBusy");
        }
      


       

        public class Event
        {
            private DateTime date;

            private string occasion;

            public DateTime Date
            {
                get { return date; }
                set { date = value; }
            }
            public string Occasion
            {
                get { return occasion; }
                set { occasion = value; }
            }
            public void Read(BinaryReader reader)
            {
                long data = reader.ReadInt64();
                date = DateTime.FromBinary(data);
                occasion = reader.ReadString();
                dates.Add(date);
            }

            public void Write(BinaryWriter writer)
            {       
                writer.Write(date.Date.ToBinary());
                writer.Write(occasion);     
            }
        }
        List<Event> events = new List<Event>();
        static  List<DateTime> dates = new List<DateTime>();

        private void Form1_Load(object sender, EventArgs e)
        {
          
            Text = "Ежедневник 1.0";
            LoadPage();
            BinaryReader reader = new BinaryReader(File.Open(mainEvents, FileMode.Open));
            while (reader.PeekChar() != -1)
            {
                Event newEvent = new Event();
                newEvent.Read(reader);
                events.Add(newEvent);
               
            }
            
                foreach (Event o in events)
                {
                   monthCalendar1.AddBoldedDate(o.Date);
                   monthCalendar1.UpdateBoldedDates();

                }
                monthCalendar1_DateSelected(null, new DateRangeEventArgs(DateTime.Now.Date, DateTime.Now.Date));
                monthCalendar1.UpdateBoldedDates();
                reader.Close();
            

        }
       

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

            FileStream fileStream = File.Open(mainEvents, FileMode.Open);
            fileStream.SetLength(0);
            fileStream.Close();
            BinaryWriter writer = new BinaryWriter(File.Open(mainEvents, FileMode.OpenOrCreate));
                foreach (Event j in events)
                {
                    j.Write(writer);
                }      
                events.Clear();
                dates.Clear();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBox1.Text)==false)
            {
                if (dates.Contains(dateTimePicker1.Value.Date)==false)
                {
                    Event newEvent = new Event();
                    newEvent.Date = dateTimePicker1.Value.Date;
                    newEvent.Occasion = textBox1.Text;
                    events.Add(newEvent);
                    dates.Add(dateTimePicker1.Value.Date);
                    monthCalendar1.AddBoldedDate(dateTimePicker1.Value.Date);
                    monthCalendar1.UpdateBoldedDates();
                    textBox1.Text = "";
                }
                else MessageBox.Show("Выберите другой день. Этот уже занят", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else MessageBox.Show("Заполните поле текста, пожалуйста.", "Ошибка",MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void monthCalendar1_DateSelected(object sender, DateRangeEventArgs e)
        {
            
            bool found = false;
            foreach (Event o in events)
            {
                if (monthCalendar1.SelectionRange.Start.Date == o.Date.Date)
                {
                    label2.Text = o.Occasion;               
                    found = true;
                }
            }
            if (!found)
            {
                label2.Text = "Событие отстутсвует.";           
            }  
        }

        private void button2_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < events.Count; i++)
            {
                if (monthCalendar1.SelectionRange.Start.Date == events[i].Date.Date)
                {               
                        monthCalendar1.RemoveBoldedDate(monthCalendar1.SelectionRange.Start.Date);
                        monthCalendar1.UpdateBoldedDates();
                        events.RemoveAt(i);
                       
                        label2.Text = "Событие удалено!";
                }
            }
        }

        public string GetHtmlPage(string url)
        {

            string HtmlText = string.Empty;
            HttpWebRequest myHttpWebrequest = (HttpWebRequest)HttpWebRequest.Create(url);
            HttpWebResponse myHttpWebresponse = (HttpWebResponse)myHttpWebrequest.GetResponse();
            StreamReader strm = new StreamReader(myHttpWebresponse.GetResponseStream());
            HtmlText = strm.ReadToEnd();
            return HtmlText;
        }

        private void label2_Click(object sender, EventArgs e)
        {
            
        }


    }
    
}

