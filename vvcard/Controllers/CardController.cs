using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using NLog;
using vvcard.Models;

namespace vvcard.Controllers
{
    [Authorize]
    public class CardController : Controller
    {
        private readonly IRepository _repository;
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public CardController(IRepository repository)
        {
            _repository = repository;
            Log.Debug("CardController created");
        }

        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> CheckPublicId(string publicId)
        {
            Log.Debug($"CheckPublicId start. User: {User.Identity?.Name}");
            try
            {
                var user = await _repository.GetUserAsync(User.Identity?.Name);
                var result = Json(await _repository.CheckCardNameAsync(publicId, user));
                Log.Debug($"CheckPublicId completed return value:{result}. User: {User.Identity?.Name}");
                return result;
            }
            catch (Exception e)
            {
                var exceptionString =
                    $"Occurred exception. Data:{e.Data}, Message:{e.Message}, StackTrace:{e.StackTrace}. User: {User.Identity?.Name}";
                await _repository.SendAdminExceptionEmailMessageAsync(exceptionString);
                Log.Error(exceptionString);
            }

            return RedirectToAction("ExceptionBadRequest", "Home");
        }

        public async Task<IActionResult> ShowPersonalCard()
        {
            Log.Debug($"ShowPersonalCard start. User: {User.Identity?.Name}");
            try
            {
                var user = await _repository.GetUserAsync(User.Identity?.Name);
                var cards = await _repository.GetAllPersonalCardAsync(user);
                if (cards.Count == 0)
                {
                    Log.Debug($"ShowPersonalCard completed. No card, redirect to create new. User: {User.Identity?.Name}");
                    return RedirectToAction("Create", "Card");
                }

                Log.Debug($"ShowPersonalCard completed return cards.Count:{cards.Count}. User: {User.Identity?.Name}");
                return View(cards);
            }
            catch (Exception e)
            {
                var exceptionString =
                    $"Occurred exception. Data:{e.Data}, Message:{e.Message}, StackTrace:{e.StackTrace}. User: {User.Identity?.Name}";
                await _repository.SendAdminExceptionEmailMessageAsync(exceptionString);
                Log.Error(exceptionString);
            }

            return RedirectToAction("ExceptionBadRequest", "Home");
        }

        [HttpGet]
        public async Task<ActionResult> Create()
        {
            Log.Debug($"Create get start. User: {User.Identity?.Name}");
            try
            {
                var card = new Card
                {
                    cardFields = new List<CardField>()
                };
                for (int i = 0; i < 6; i++)
                {
                    card.cardFields.Add(new CardField {FieldType = 2});
                }

                Log.Debug($"Create get completed. User: {User.Identity?.Name}");
                return View(card);
            }
            catch (Exception e)
            {
                var exceptionString =
                    $"Occurred exception. Data:{e.Data}, Message:{e.Message}, StackTrace:{e.StackTrace}. User: {User.Identity?.Name}";
                await _repository.SendAdminExceptionEmailMessageAsync(exceptionString);
                Log.Error(exceptionString);
            }

            return RedirectToAction("ExceptionBadRequest", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Card card)
        {
            Log.Debug($"Create post start. User: {User.Identity?.Name}");
            try
            {

                if (ModelState.IsValid)
                {
                    var user = await _repository.GetUserAsync(User.Identity?.Name);
                    if (await _repository.CreateNewCardAsync(card, user))
                    {
                        Log.Debug($"Create post completed. User: {User.Identity?.Name}");
                        return RedirectToAction("ShowPersonalCard", "Card");
                    }

                    ModelState.AddModelError("", "Такой адрес уже зарезервирован");
                }

                Log.Debug($"Create post not completed. User: {User.Identity?.Name}");
                return View(card);
            }
            catch (Exception e)
            {
                var exceptionString =
                    $"Occurred exception. Data:{e.Data}, Message:{e.Message}, StackTrace:{e.StackTrace}. User: {User.Identity?.Name}";
                await _repository.SendAdminExceptionEmailMessageAsync(exceptionString);
                Log.Error(exceptionString);
            }

            return RedirectToAction("ExceptionBadRequest", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int cardId)
        {
            Log.Debug($"Edit get start. User: {User.Identity?.Name}");
            try
            {
                var user = await _repository.GetUserAsync(User.Identity?.Name);
                var card = await _repository.GetEditCardAsync(cardId, user);
                if (card != null)
                {
                    Log.Debug($"Edit get completed. CardId:{card.Id} User: {User.Identity?.Name}");
                    return View(card);
                }

                Log.Warn($"Edit get not completed. Card is null. User: {User.Identity?.Name}");
                return RedirectToAction("ShowPersonalCard", "Card");
            }
            catch (Exception e)
            {
                var exceptionString =
                    $"Occurred exception. Data:{e.Data}, Message:{e.Message}, StackTrace:{e.StackTrace}. User: {User.Identity?.Name}";
                await _repository.SendAdminExceptionEmailMessageAsync(exceptionString);
                Log.Error(exceptionString);
            }

            return RedirectToAction("ExceptionBadRequest", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Card card)
        {
            Log.Debug($"Edit post start. CardId:{card.Id} User: {User.Identity?.Name}");
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _repository.GetUserAsync(User.Identity?.Name);
                    if (await _repository.EditCardAsync(card, user))
                    {
                        Log.Debug($"Edit post completed. CardId:{card.Id} User: {User.Identity?.Name}");
                        return RedirectToAction("ShowPersonalCard", "Card");
                    }

                    ModelState.AddModelError("", "Что то пошло нетак");
                }

                return View(card);
            }
            catch (Exception e)
            {
                var exceptionString =
                    $"Occurred exception. Data:{e.Data}, Message:{e.Message}, StackTrace:{e.StackTrace}. User: {User.Identity?.Name}";
                await _repository.SendAdminExceptionEmailMessageAsync(exceptionString);
                Log.Error(exceptionString);
            }

            return RedirectToAction("ExceptionBadRequest", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int cardId)
        {
            Log.Debug($"Delete get start. CardId:{cardId} User: {User.Identity?.Name}");
            try
            {
                var user = await _repository.GetUserAsync(User.Identity?.Name);
                var card = await _repository.GetDeleteCardAsync(cardId, user);
                if (card != null)
                {
                    Log.Debug($"Delete get completed. CardId:{cardId} User: {User.Identity?.Name}");
                    return View(card);
                }

                return RedirectToAction("ShowPersonalCard", "Card");
            }
            catch (Exception e)
            {
                var exceptionString =
                    $"Occurred exception. Data:{e.Data}, Message:{e.Message}, StackTrace:{e.StackTrace}. User: {User.Identity?.Name}";
                await _repository.SendAdminExceptionEmailMessageAsync(exceptionString);
                Log.Error(exceptionString);
            }

            return RedirectToAction("ExceptionBadRequest", "Home");
        }

        [HttpPost]
        public async Task<ActionResult> Delete(Card card)
        {
            Log.Debug($"Delete post start. CardId:{card.Id} User: {User.Identity?.Name}");
            try
            {
                var user = await _repository.GetUserAsync(User.Identity?.Name);
                if (await _repository.DeleteCardAsync(card, user))
                {
                    //TODO добавить отправку сообщения
                    ModelState.AddModelError("", "Карта удалена");
                }

                Log.Debug($"Delete post completed. User: {User.Identity?.Name}");
                return RedirectToAction("ShowPersonalCard", "Card");
            }
            catch (Exception e)
            {
                var exceptionString =
                    $"Occurred exception. Data:{e.Data}, Message:{e.Message}, StackTrace:{e.StackTrace}. User: {User.Identity?.Name}";
                await _repository.SendAdminExceptionEmailMessageAsync(exceptionString);
                Log.Error(exceptionString);
            }

            return RedirectToAction("ExceptionBadRequest", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> ShowVisits(int cardId, string format)
        {
            Log.Debug($"ShowVisits start. User: {User.Identity?.Name}");
            try
            {
                var user = await _repository.GetUserAsync(User.Identity?.Name);
                var visitCounters = await _repository.GetVisits(user, cardId, format);
                Log.Debug($"ShowVisits completed. visitCounters.Count: {visitCounters.Count}. User: {User.Identity?.Name}");
                return View(visitCounters);
            }
            catch (Exception e)
            {
                var exceptionString =
                    $"Occurred exception. Data:{e.Data}, Message:{e.Message}, StackTrace:{e.StackTrace}. User: {User.Identity?.Name}";
                await _repository.SendAdminExceptionEmailMessageAsync(exceptionString);
                Log.Error(exceptionString);
            }

            return RedirectToAction("ExceptionBadRequest", "Home");
        }
        
        [AllowAnonymous]
        [HttpPost]
        public async Task AddCardFieldClick(int cardFieldId)
        {
            Log.Debug($"AddCardFieldClick start. User: {User.Identity?.Name ?? "Anonymous"}");
            try
            {
                if (HttpContext.Connection.RemoteIpAddress != null)
                {
                    var ip = HttpContext.Connection.RemoteIpAddress.ToString();
                    await _repository.AddCardFieldClick(cardFieldId, ip);
                }
            }
            catch (Exception e)
            {
                var exceptionString = $"Occurred exception. Data:{e.Data}, Message:{e.Message}, StackTrace:{e.StackTrace}. User: {User.Identity?.Name}";
                await _repository.SendAdminExceptionEmailMessageAsync(exceptionString);
                Log.Error(exceptionString);
            }
        }

        #region Удалить
        // public async Task<ActionResult> Index(string id)
        // {
        //     Log.Debug($"Index start. User: {User.Identity?.Name}");
        //     try
        //     {
        //         if (User.Identity?.Name == null)
        //         {
        //             throw new NullReferenceException("User.Identity is null");
        //         }
        //
        //         List<Card> cards;
        //         if (id == null)
        //         {
        //             cards = await _repository.GetRandomCardAsync(3);
        //             return View(cards);
        //         }
        //
        //         cards = await _repository.GetCardByIdAsync(id);
        //         if (cards.Count == 0)
        //         {
        //             cards.Add(new Card {Name = "Неверный ID"});
        //             return View(cards);
        //         }
        //
        //         if (HttpContext.Connection.RemoteIpAddress == null)
        //         {
        //             throw new NullReferenceException("HttpContext.Connection.RemoteIpAddress is null");
        //         }
        //
        //         var ip = HttpContext.Connection.RemoteIpAddress.ToString();
        //         var userAgent = HttpContext.Request.Headers["User-Agent"];
        //         if (!ServiceAndBotDetector.Check(userAgent))
        //         {
        //             await _repository.AddNewVisitAsync(ip, cards[0].Id, userAgent);
        //         }
        //
        //         return View(cards);
        //     }
        //     catch (Exception e)
        //     {
        //         var exceptionString =
        //             $"Occurred exception. Data:{e.Data}, Message:{e.Message}, StackTrace:{e.StackTrace}. User: {User.Identity?.Name}";
        //         await _repository.SendAdminExceptionEmailMessage(exceptionString);
        //         Log.Error(exceptionString);
        //     }
        //
        //     return RedirectToAction("ExceptionBadRequest", "Home");
        // }
        #endregion
    }
}
