using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Collections;
using System.Diagnostics;
using WebApplication3.Models;

namespace WebApplication3.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public IActionResult SalesSummary()
        {
            object vret;
            string connectionString = "Host=34.101.160.134;Database=test;User Id=campuss;Password=my_password; ";
            using (NpgsqlConnection cnn = new NpgsqlConnection(connectionString))
            {
                cnn.Open();
                var items = new ArrayList();
                using (var com = cnn.CreateCommand())
                {
                    com.CommandText = "select item_type, sum(units_sold) as solds, sum(units_sold * unit_price) as revenue from salesorder s\r\ngroup by item_type";
                    using (var reader = com.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            items.Add(new
                            {
                                item = Convert.ToString(reader["item_type"]),
                                units = Convert.ToInt64(reader["solds"]),
                                revenue = Convert.ToDecimal(reader["revenue"])
                            });
                        }
                        reader.Close();
                    }
                    com.Dispose();
                }
                var countries = new ArrayList();
                using (var com = cnn.CreateCommand())
                {
                    com.CommandText = "select country, sum(units_sold) as solds, sum(units_sold * unit_price) as revenue from salesorder s\r\ngroup by country";
                    using (var reader = com.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            countries.Add(new
                            {
                                country = Convert.ToString(reader["country"]),
                                units = Convert.ToInt64(reader["solds"]),
                                revenue = Convert.ToDecimal(reader["revenue"])
                            });
                        }
                        reader.Close();
                    }
                    com.Dispose();
                }
                vret = new
                {
                    status = "OK",
                    items,
                    countries
                };
                cnn.Dispose();
            }
            return Json(vret);
        }
    }
}
