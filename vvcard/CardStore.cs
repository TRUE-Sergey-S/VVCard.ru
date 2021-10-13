using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Helpers;
using vvcard.Models;

namespace vvcard
{
    public class CardStore : IRepository
    {
        private ApplicationContext db;

        public CardStore(ApplicationContext context)
        {
            db = context;
        }

        public async Task<List<Card>> GetCardByIdAsync(string id)
        {
            List<Card> cards = new List<Card>();
            Card card;
            int parsId;
            if (int.TryParse(id, out parsId))
            {
                card = await db.Cards.FirstOrDefaultAsync(x => x.Id == parsId);
            }
            else
            {
                card = await db.Cards.FirstOrDefaultAsync(x => x.PublicID == id);
            }
            if (card != null)
            {
                card.cardFields = await db.CardFields.Where(x => x.CardId == card.Id).OrderBy(x => x.Order).ToListAsync();
                cards.Add(card);
            }
            return cards;
        }

        public async Task<List<Card>> GetRandomCardAsync(int count)
        {
            //Stored procedure, return random Cards
            var cards = await db.Cards.FromSqlRaw("GetRandomCard {0}", count).ToListAsync();
            if (cards != null)
            {
                foreach (var item in cards)
                {
                    item.cardFields = await GetCardFieldsAsync(item.Id);
                }
            }
            return cards;
        }

        public async Task<List<CardField>> GetCardFieldsAsync(int cardID)
        {

            List<CardField> cardFields = await db.CardFields.Where(x => x.CardId == cardID).OrderBy(x => x.Order).ToListAsync();
            return cardFields;
        }

        public async Task<bool> CheckCardNameAsync(string id, string userName)
        {
            string publicIDDB = await db.Cards.Select(e => e.PublicID).FirstOrDefaultAsync(x => x == id);
            if (publicIDDB == null)
            {
                return true;
            }
            User user = await db.Users.FirstOrDefaultAsync(x => x.UserName == userName);
            if (user != null)
            {
                string[] publicalIDs = await db.Cards.Where(x => x.UserId == user.Id).Select(x => x.PublicID).ToArrayAsync();
                foreach (var item in publicalIDs)
                {
                    if (item == id)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public async Task<List<Card>> GetAllPersonalCardAsync(string userName)
        {
            List<Card> cards = new List<Card>();
            User user = await db.Users.FirstOrDefaultAsync(x => x.UserName == userName);
            if (user != null)
            {
                cards = await db.Cards.Where(x => x.UserId == user.Id).ToListAsync();
                foreach (var item in cards)
                {
                    item.cardFields = await GetCardFieldsAsync(item.Id);
                    item.visitСounter = await db.VisitСounters.Where(c => c.CardId == item.Id).ToListAsync();
                    foreach (var itemCardField in item.cardFields)
                    {
                        var tempCardField = await db.ClickCardFields.Where(x => x.CardFieldId == itemCardField.Id).ToListAsync();
                        if (tempCardField != null)
                        {
                            itemCardField.ClickCardFields = tempCardField;
                        }
                    }
                }
            }
            return cards;
        }

        public async Task<bool> CreateNewCardAsync(Card card, string userName)
        {
            Card dbcard = await db.Cards.FirstOrDefaultAsync(x => x.PublicID == card.PublicID);
            if (dbcard == null)
            {
                User user = await db.Users.FirstOrDefaultAsync(x => x.UserName == userName);
                if (user != null)
                {
                    for (int i = 0; i < card.cardFields.Count; i++)
                    {
                        card.cardFields[i].Order = i;
                    }
                    card.UserId = user.Id;
                    await db.Cards.AddAsync(card);
                    await db.SaveChangesAsync();
                    return true;
                }
            }
            return false;
        }

        public async Task<Card> GetEditCardAsync(int id, string userName)
        {
            User user = await db.Users.FirstOrDefaultAsync(x => x.UserName == userName);
            if (user != null)
            {
                int[] userCardId = await db.Cards.Where(x => x.UserId == user.Id).Select(x => x.Id).ToArrayAsync();
                foreach (var item in userCardId)
                {
                    if (item == id)
                    {
                        Card card = await db.Cards.FirstOrDefaultAsync(x => x.Id == id);
                        card.cardFields = await db.CardFields.Where(x => x.CardId == id).OrderBy(x => x.Order).ToListAsync();
                        return card;
                    }
                }
            }
            return null;
        }

        public async Task<bool> EditCardAsync(Card card, string userName)
        {
            User user = await db.Users.FirstOrDefaultAsync(x => x.UserName == userName);
            Card dbCard = await db.Cards.FirstOrDefaultAsync(x => x.Id == card.Id && x.UserId == user.Id);
            if (await CheckCardNameAsync(card.PublicID, userName))
            {
                if (dbCard != null)
                {
                    for (int i = 0; i < card.cardFields.Count; i++)
                    {
                        card.cardFields[i].Order = i;
                    }
                    var cardFields = await db.CardFields.Where(x => x.CardId == dbCard.Id).ToListAsync();
                    dbCard.cardFields = card.cardFields;
                    dbCard.IsPrivate = card.IsPrivate;
                    dbCard.Name = card.Name;
                    dbCard.PublicID = card.PublicID;
                    db.Cards.Update(dbCard);
                    await db.SaveChangesAsync();
                    return true;
                }
            }
            return false;
        }

        public async Task<Card> GetDeleteCardAsync(int id, string userName)
        {
            User user = await db.Users.FirstOrDefaultAsync(x => x.UserName == userName);
            Card dbCard = await db.Cards.FirstOrDefaultAsync(x => x.Id == id && x.UserId == user.Id);
            if (dbCard != null)
            {
                return await db.Cards.FirstOrDefaultAsync(x => x.Id == id);
            }
            return null;
        }

        public async Task<bool> DeleteCardAsync(Card card, string userName)
        {
            User user = await db.Users.FirstOrDefaultAsync(x => x.UserName == userName);
            if (user != null)
            {
                Card deletecard = await db.Cards.FirstOrDefaultAsync(x => x.Id == card.Id && x.UserId == user.Id);
                if (deletecard != null)
                {
                    db.Cards.Remove(deletecard);
                    await db.SaveChangesAsync();
                    return true;
                }
            }
            return false;
        }

        public async Task AddNewVisitAsync(string ClientIp, int cardID, string userAgentString)
        {
            VisitСounter visitСounter = new VisitСounter() { DateTime = DateTime.Now, Ip = ClientIp, CardId = cardID, UserAgentString = userAgentString };
            db.VisitСounters.Add(visitСounter);
            await db.SaveChangesAsync();
        }

        public async Task<List<VisitСounter>> GetVisits(string userName, int cardId, string format ) {
            string responseFromServer = "";
            JObject jsonresponse;
            User user = await db.Users.FirstOrDefaultAsync(x => x.UserName == userName);
            if (user != null)
            {
                var card = await db.Cards.FirstOrDefaultAsync(x => x.Id == cardId && x.UserId == user.Id);
                List<VisitСounter> visits;
                if (card != null)
                {
                    switch (format)
                    {
                        case "Day":
                            visits = await db.VisitСounters.Where(d => d.CardId == cardId && d.DateTime.Day == DateTime.Now.Day && d.DateTime.Month == DateTime.Now.Month && d.DateTime.Year == DateTime.Now.Year).ToListAsync();
                            break;
                        case "Mouth":
                            visits = await db.VisitСounters.Where(d => d.CardId == cardId && d.DateTime.Month == DateTime.Now.Month && d.DateTime.Year == DateTime.Now.Year).ToListAsync();
                            break;
                        default: visits = await db.VisitСounters.Where(x => x.CardId == cardId).ToListAsync();
                            break;
                    }
                    bool needVisitСounter_City_ParseAndSaveToDB = false;
                    foreach (var item in visits)
                    {
                        if (item.City == null)
                        {
                            var request = WebRequest.Create("http://ipwhois.app/json/" + item.Ip);
                            var response = await request.GetResponseAsync();
                            await using (Stream dataStream = response.GetResponseStream())
                            {
                                var reader = new StreamReader(dataStream);
                                responseFromServer = reader.ReadToEnd();
                                jsonresponse = JObject.Parse(responseFromServer);
                            }
                            string city = (string)jsonresponse["city"];
                            item.City = city;
                            item.JsonResponse = responseFromServer;
                            db.VisitСounters.Update(item);
                            needVisitСounter_City_ParseAndSaveToDB = true;
                        }
                    }
                    if (needVisitСounter_City_ParseAndSaveToDB)
                    {
                        await db.SaveChangesAsync();
                    }
                    return visits;
                }
            }
            return new List<VisitСounter>();
        }
        
        public async Task AddCardFieldClick(int cardFieldId, string ip) {
           await db.ClickCardFields.AddAsync(new ClickCardField { CardFieldId = cardFieldId, Ip = ip, DateTime = DateTime.Now });
           await db.SaveChangesAsync();
        }
    }
}