using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using vvcard.Models;

namespace vvcard.Controllers
{
    public class HomeController : Controller
    {
        IRepository repository;
        public HomeController(IRepository repo)
        {
            repository = repo;
        }
        public async Task<ActionResult> Index(string id)
        {
            List<Card> cards = new List<Card>();
            if (id == null)
            {
                cards = await repository.GetRandomCardAsync(3);
                return View(cards);
            }
            else
            {
                cards = await repository.GetCardByIdAsync(id);
            }
            if (cards.Count == 0)
            {
                cards.Add(new Card { Name = "Неверный ID" });
                return View(cards);
            }
            else
            {
                var ip = HttpContext.Connection.RemoteIpAddress.ToString();
                var userAgent = HttpContext.Request.Headers["User-Agent"];
                if (!ServiceAndBotDetector.Check(userAgent))
                {
                    await repository.AddNewVisitAsync(ip, cards[0].Id, userAgent);
                }
            }
            return View(cards);
        }
        public IActionResult Help()
        {
            return View();
        }
        public IActionResult RoadMap()
        {
            return View();
        }
    }
}

