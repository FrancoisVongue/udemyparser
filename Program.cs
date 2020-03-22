using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp;
using System.Net;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;

namespace UdemyParser
{
    public class Tutorial
    {
        public string Name { get; set; }
        public string Duration { get; set; }
        public TimeSpan DurationTimeSpan
        {
            get
            {
                var hours = int.Parse(this.Duration.Split(':')[0]);
                var minutes = int.Parse(this.Duration.Split(':')[1]);
                return new TimeSpan(hours, minutes, 0);
            }
        }
        public bool IsFree { get; set; }
    }
    
    class Program
    {
        static void Main(string[] args)
        {
            string source;
            using ( WebClient client = new WebClient() ) 
            {
                client.Headers.Add("User-Agent","Mozilla/5.0 (X11; U; Linux i686) Gecko/20071127 Firefox/2.0.0.11");
                source = client.DownloadString("https://www.udemy.com/course/learn-flutter-dart-to-build-ios-android-apps");
            }
            
            var config = Configuration.Default;
            var context = BrowsingContext.New(config);
            var parser = context.GetService<IHtmlParser>();
            var document = parser.ParseDocument(source);
            
            List<Tutorial> tutorials = document.QuerySelectorAll(".lecture-container--preview")
                .Select(lecture => new Tutorial()
                {
                    Name = lecture
                        .GetElementsByClassName("left-content")[0]
                        .GetElementsByClassName("top")[0]
                        .GetElementsByClassName("title")[0]
                        .FirstElementChild
                        .InnerHtml.Trim(),
                    Duration = lecture
                        .GetElementsByClassName("details")[0]
                        .GetElementsByClassName("content-summary")[0]
                        .InnerHtml.Trim()
                })
                .ToList();
            
            tutorials.Sort((tutorial1, tutorial2) =>
                (int)(tutorial2.DurationTimeSpan.TotalMinutes - tutorial1.DurationTimeSpan.TotalMinutes));
            
            foreach (var tutorial in tutorials)
            {
                Console.WriteLine(tutorial.Name + " - " + tutorial.Duration);
            }
        }
    }
}