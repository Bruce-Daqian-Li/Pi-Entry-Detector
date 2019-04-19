using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EDD.Models;
using System.Net.WebSockets;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using EDD.Utils;

namespace EDD.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Route("/status")]
        public IActionResult Status()
        {
            var x = InternalCommunication.CurrentTimeRange;
            string a = x.StartAt.TimeOfDay.ToString();
            string b = x.EndAt.TimeOfDay.ToString();
            return Json(new
            {
                entry = InternalCommunication.CurrentEntry,
                range = new
                {
                    id = x.Id,
                    startAt = a,
                    endAt = b
                }
            });
        }

        public IActionResult AboutUs()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
