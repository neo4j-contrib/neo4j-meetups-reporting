using Meetup.Analytics.Services.Implementations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Meetup.Analytics.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

            return View();
        }

        public ActionResult Analytics(string tag, string city, string from, string to)
        {
            TimeSeries timeSeries = Meetup.Analytics.Services.Implementations.Analytics.GetMeetupAnalytics(tag, city, from, to);

            if (timeSeries != null)
            {
                ViewBag.categories = Newtonsoft.Json.JsonConvert.SerializeObject(timeSeries.categories);
                ViewBag.series = Newtonsoft.Json.JsonConvert.SerializeObject(timeSeries.series);
                ViewBag.tag = tag;
                ViewBag.city = city;
            }
            
            return View();
        }

        public ActionResult AnalyticsGrowthPercent(string tag, string city, string from, string to)
        {
            var timeSeries = Meetup.Analytics.Services.Implementations.Analytics.GetMeetupAnalyticsByGroupAndGrowthPercent(tag, city, from, to);

            if (timeSeries != null)
            {
                ViewBag.categories = Newtonsoft.Json.JsonConvert.SerializeObject(timeSeries.categories);
                ViewBag.series = Newtonsoft.Json.JsonConvert.SerializeObject(timeSeries.series);
                ViewBag.tag = tag;
                ViewBag.city = city;
            }

            return View();
        }

        public ActionResult AnalyticsByWeek(string tag, string city, string from, string to)
        {
            var timeSeries = Meetup.Analytics.Services.Implementations.Analytics.GetMeetupAnalyticsByWeek(tag, city, from, to);

            if (timeSeries != null)
            {
                ViewBag.categories = Newtonsoft.Json.JsonConvert.SerializeObject(timeSeries.categories);
                ViewBag.series = Newtonsoft.Json.JsonConvert.SerializeObject(timeSeries.series);
                ViewBag.tag = tag;
                ViewBag.city = city;
            }

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
