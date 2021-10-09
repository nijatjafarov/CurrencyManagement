using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Net;
using System.IO;
using System.Globalization;
using System.Collections;

namespace XMLTest.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            List<string> valutes = GetValutes();

            return View(valutes);
        }
        [HttpPost]
        public ActionResult Index(string selectedCurrency, DateTime startDate, DateTime finishDate)
        {
            string[] months = { "Yanvar", "Fevral", "Mart", "Aprel", "May", "İyun",
                "İyul", "Avqust", "Sentyabr", "Oktyabr", "Noyabr", "Dekabr" };

            ArrayList totalValues = new ArrayList();
            ArrayList totalDays = new ArrayList();

            int totalYears = finishDate.Year - startDate.Year;

            int totalMonths = finishDate.Month - startDate.Month + totalYears * 12;

            DateTime currentDate = startDate;

            int daysInMonth;

            List<decimal> valuesByMonth = new List<decimal>();
            List<int> daysByMonth = new List<int>();

            if (totalMonths > 0)
            {
                for (int i = 0; i < totalMonths; i++)
                {
                    daysInMonth = DateTime.DaysInMonth(currentDate.Year, currentDate.Month);

                    for (int j = currentDate.Day; j <= daysInMonth; j++)
                    {
                        HttpWebRequest httpWeb = (HttpWebRequest)WebRequest.Create($"https://www.cbar.az/currencies/"
                        + currentDate.ToString("dd.MM.yyyy") + ".xml");

                        Stream str = httpWeb.GetResponse().GetResponseStream();
                        StreamReader sr = new StreamReader(str);
                        string a = sr.ReadToEnd();

                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(a);

                        XmlNode valuteNames = xmlDoc.SelectSingleNode($"//Name[text()='{selectedCurrency}']");
                        decimal value = decimal.Parse(valuteNames.NextSibling.InnerText);
                        valuesByMonth.Add(value);

                        daysByMonth.Add(j);
                        currentDate = currentDate.AddDays(1);
                    }
                    totalValues.Add(valuesByMonth);
                    totalDays.Add(daysByMonth);

                    daysByMonth = new List<int>();
                    valuesByMonth = new List<decimal>();
                }

                for (int j = 0; j < startDate.Day; j++)
                {
                    HttpWebRequest httpWeb = (HttpWebRequest)WebRequest.Create($"https://www.cbar.az/currencies/"
                        + currentDate.ToString("dd.MM.yyyy") + ".xml");

                    Stream str = httpWeb.GetResponse().GetResponseStream();
                    StreamReader sr = new StreamReader(str);
                    string a = sr.ReadToEnd();

                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(a);

                    XmlNode valuteNames = xmlDoc.SelectSingleNode($"//Name[text()='{selectedCurrency}']");
                    decimal value = decimal.Parse(valuteNames.NextSibling.InnerText);
                    valuesByMonth.Add(value);

                    daysByMonth.Add(j + 1);
                    currentDate = currentDate.AddDays(1);
                }
                totalValues.Add(valuesByMonth);
                totalDays.Add(daysByMonth);
            }
            else
            {
                for (int j = startDate.Day; j <= finishDate.Day; j++)
                {
                    HttpWebRequest httpWeb = (HttpWebRequest)WebRequest.Create($"https://www.cbar.az/currencies/"
                    + currentDate.ToString("dd.MM.yyyy") + ".xml");

                    Stream str = httpWeb.GetResponse().GetResponseStream();
                    StreamReader sr = new StreamReader(str);
                    string a = sr.ReadToEnd();

                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(a);

                    XmlNode valuteNames = xmlDoc.SelectSingleNode($"//Name[text()='{selectedCurrency}']");
                    decimal value = decimal.Parse(valuteNames.NextSibling.InnerText);
                    valuesByMonth.Add(value);

                    daysByMonth.Add(j);
                    currentDate = currentDate.AddDays(1);
                }
                totalValues.Add(valuesByMonth);
                totalDays.Add(daysByMonth);
            }

            List<string> selectedMonths = new List<string>();

            int h = 0;
            int startMonth = startDate.Month - 1;
            int year = startDate.Year;
            for (int i = 0; i < totalMonths + 1; i++)
            {
                if (h + startMonth == months.Length)
                {
                    h = 0;
                    startMonth = 0;
                    year += 1;
                }
                selectedMonths.Add(months[startMonth + h] + " - " + year.ToString());
                h++;
            }


            ViewBag.SelectedValute = selectedCurrency;
            ViewBag.StartDate = startDate.ToString("yyyy-MM-dd");
            ViewBag.FinishDate = finishDate.ToString("yyyy-MM-dd");
            ViewBag.SelectedMonths = selectedMonths;
            ViewBag.TotalMonths = totalValues.Count;
            ViewBag.TotalDays = totalDays;
            ViewBag.TotalValues = totalValues;

            List<string> valutes = GetValutes();

            return View(valutes);
        }

        public List<string> GetValutes()
        {
            HttpWebRequest httpWeb = (HttpWebRequest)WebRequest.Create($"https://www.cbar.az/currencies/" + DateTime.Now.ToString("dd.MM.yyyy") + ".xml");
            Stream str = httpWeb.GetResponse().GetResponseStream();
            StreamReader sr = new StreamReader(str);
            string a = sr.ReadToEnd();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(a);

            XmlNodeList valuteNames = xmlDoc.GetElementsByTagName("Name");

            List<string> nameTexts = new List<string>();

            foreach (XmlNode name in valuteNames)
            {
                nameTexts.Add(name.InnerText);
            }

            return nameTexts;
        }
    }
}