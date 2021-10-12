using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using vvcard.Models;

namespace vvcard.Controllers
{
    [Authorize]
    public class CardController : Controller
    {
        IRepository repository;
        public CardController(IRepository repo)
        {
            repository = repo;
        }
        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> CheckPublicID(string PublicID)
        {
            if (await repository.CheckCardNameAsync(PublicID, User.Identity.Name))
            {
                return Json(true);
            }
            return Json(false);
        }

        public async Task<IActionResult> ShowPersonalCard()
        {
            List<Card> cards = await repository.GetAllPersonalCardAsync(User.Identity.Name);
            if (cards.Count == 0)
            {
                return RedirectToAction("Create", "Card");
            }
            return View(cards);
        }
        [HttpGet]
        public ActionResult Create()
        {
            Card card = new Card();
            card.cardFields = new List<CardField>();
            return View(card);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Card card)
        {
            if (ModelState.IsValid)
            {
                if (await repository.CreateNewCardAsync(card, User.Identity.Name))
                {
                    return RedirectToAction("ShowPersonalCard", "Card");
                }
                ModelState.AddModelError("", "Такой адрес уже зарезервирован");
            }
            return View(card);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            Card card = await repository.GetEditCardAsync(id, User.Identity.Name);
            if (card != null)
            {
                return View(card);
            }
            return RedirectToAction("ShowPersonalCard", "Card");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Card card)
        {
            if (ModelState.IsValid)
            {
                if (await repository.EditCardAsync(card, User.Identity.Name))
                {
                    return RedirectToAction("ShowPersonalCard", "Card");
                }
                ModelState.AddModelError("", "Что то пошло нетак");
            }
            return View(card);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            Card card = await repository.GetDeleteCardAsync(id, User.Identity.Name);
            if (card != null)
            {
                return View(card);
            }
            return RedirectToAction("ShowPersonalCard", "Card");
        }

        [HttpPost]
        public async Task<ActionResult> Delete(Card card)
        {
            if (await repository.DeleteCardAsync(card, User.Identity.Name))
            {
                ModelState.AddModelError("", "Карта удалена");
            }
            return RedirectToAction("ShowPersonalCard", "Card");
        }
        [HttpGet]
        public async Task<IActionResult> ShowVisits(int id, string format)
        {
            List<VisitСounter> visitСounter = await repository.GetVisits(User.Identity.Name, id, format);
            return View(visitСounter);
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task AddCardFieldClick(int Id) {
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            await repository.AddCardFieldClick(Id, ip);
        }
    }
}
