using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using NLog;
using vvcard.Models;

namespace vvcard.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRepository _repository;
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        
        public HomeController(IRepository repository)
        {
            this._repository = repository;
            Log.Debug("CardController created");
        }

        public async Task<ActionResult> Index(string cardId)
        {
            Log.Debug($"Index start. CardId:{cardId}. User: {User.Identity?.Name ?? "Anonymous"}");
            try
            {
                var cards = new List<Card>();
                if (cardId == null)
                {
                    Log.Debug($"Index completed. CardId:null. User: {User.Identity?.Name ?? "Anonymous"}");
                    return View(cards);
                }

                cards = await _repository.GetCardByIdAsync(cardId);
                if (cards.Count == 0)
                {
                    cards.Add(new Card {Name = "Неверный ID"});
                    Log.Warn($"Index completed. wrong CardId:{cardId}. User: {User.Identity?.Name ?? "Anonymous"}");
                    return View(cards);
                }

                if (HttpContext.Connection.RemoteIpAddress != null)
                {
                    var ip = HttpContext.Connection.RemoteIpAddress.ToString();
                    var userAgent = HttpContext.Request.Headers["User-Agent"];
                    if (!ServiceAndBotDetector.Check(userAgent))
                    {
                        Log.Debug($"Index not a bot. CardId:{cardId}. User: {User.Identity?.Name ?? "Anonymous"}");
                        await _repository.AddNewVisitAsync(ip, cards[0].Id, userAgent);
                    }
                    else
                    {
                        Log.Debug($"Index is a bot. CardId:{cardId}. User: {User.Identity?.Name ?? "Anonymous"}");
                    }
                }
                Log.Warn($"Index completed. CardId:{cardId}. User: {User.Identity?.Name ?? "Anonymous"}");
                return View(cards);
            }
            catch (Exception e)
            {
                var exceptionString = $"Occurred exception. Data:{e.Data}, Message:{e.Message}, StackTrace:{e.StackTrace}. User: {User.Identity?.Name}";
                await _repository.SendAdminExceptionEmailMessageAsync(exceptionString);
                Log.Error(exceptionString);
            }

            return RedirectToAction("ExceptionBadRequest", "Home");
        }

        public IActionResult Help()
        {
            Log.Debug($"Help request. User: {User.Identity?.Name ?? "Anonymous"}");
            return View();
        }
        
        public IActionResult RoadMap()
        {
            Log.Debug($"RoadMap request. User: {User.Identity?.Name ?? "Anonymous"}");
            return View();
        }
        
        public IActionResult PrivacyPolicy()
        {
            Log.Debug($"PrivacyPolicy request. User: {User.Identity?.Name ?? "Anonymous"}");
            return View();
        }

        public IActionResult ExceptionBadRequest()
        {
            Log.Debug($"ExceptionBadRequest redirect. User: {User.Identity?.Name ?? "Anonymous"}");
            return View();
        }
    }
}

