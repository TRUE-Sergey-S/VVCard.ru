using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using NLog;
using vvcard.CustomException;
using vvcard.Models;

namespace vvcard
{
    public class CardStore : IRepository
    {
        private readonly ApplicationContext _dbContext;
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        
        public CardStore(ApplicationContext context)
        {
            _dbContext = context;
            Log.Debug("CardStore Create");
        }

        public async Task<List<Card>> GetCardByIdAsync(string cardId)
        {
            Log.Debug($"GetCardByIdAsync start. CardId: {cardId}");
            var cards = new List<Card>();
            Card card;
            if (int.TryParse(cardId, out var parsId))
            {
                card = await _dbContext.Cards.FirstOrDefaultAsync(x => x.Id == parsId);
                Log.Debug($"GetCardByIdAsync start. CardId int:{cardId}");
            }
            else
            {
                card = await _dbContext.Cards.FirstOrDefaultAsync(x => x.PublicID == cardId);
                Log.Debug($"GetCardByIdAsync start. CardId string:{cardId}");
            }
            if (card != null)
            {
                card.cardFields = await _dbContext.CardFields.Where(x => x.CardId == card.Id).OrderBy(x => x.Order).ToListAsync();
                cards.Add(card);
                Log.Debug($"GetCardByIdAsync. CardId:{cardId}. Add CardField.Count:{card.cardFields.Count}");
            }
            Log.Debug($"GetCardByIdAsync completed. CardId:{cardId}.");
            return cards;
        }

        //Now not use
        public async Task<List<Card>> GetRandomCardAsync(int count)
        {
            //Stored procedure, return random Cards
            var cards = await _dbContext.Cards.FromSqlRaw("GetRandomCard {0}", count).ToListAsync();
            if (cards != null)
            {
                foreach (var item in cards)
                {
                    item.cardFields = await GetCardFieldsAsync(item.Id);
                }
            }
            return cards;
        }

        public async Task<List<CardField>> GetCardFieldsAsync(int cardId)
        {
            Log.Debug($"GetCardFieldsAsync start. CardId: {cardId}");
            var cardFields = await _dbContext.CardFields.Where(x => x.CardId == cardId).OrderBy(x => x.Order).ToListAsync();
            Log.Debug($"GetCardFieldsAsync conpleted. CardId: {cardId}. CardField.Count:{cardFields.Count}.");
            return cardFields;
        }

        public async Task<bool> CheckCardNameAsync(string publicId, User user)
        {
            Log.Debug($"CheckCardNameAsync start. publicId: {publicId}");
            var publicIddb = await _dbContext.Cards.Select(e => e.PublicID).FirstOrDefaultAsync(x => x == publicId);
            if (publicIddb == null)
            {
                Log.Debug($"CheckCardNameAsync completed. publicId: {publicId} not found in database");
                return true;
            }
            var userPublicalIds = await _dbContext.Cards.Where(x => x.UserId == user.Id).Select(x => x.PublicID)
                .ToArrayAsync();
            foreach (var item in userPublicalIds)
            {
                if (item == publicId)
                {
                    Log.Debug(
                        $"CheckCardNameAsync completed. publicId: {publicId} found in database this is user`s publicId");
                    return true;
                }
            }
            Log.Debug($"CheckCardNameAsync completed. publicId: {publicId} found in database");
            return false;
        }

        public async Task<List<Card>> GetAllPersonalCardAsync(User user)
        {
            Log.Debug($"GetAllPersonalCardAsync start. userName: {user.UserName}");
            var cards = await _dbContext.Cards.Where(x => x.UserId == user.Id).ToListAsync();
            foreach (var item in cards)
            {
                item.cardFields = await GetCardFieldsAsync(item.Id);
                item.visitСounter = await _dbContext.VisitСounters.Where(c => c.CardId == item.Id).ToListAsync();
                foreach (var itemCardField in item.cardFields)
                {
                    var tempCardField = await _dbContext.ClickCardFields.Where(x => x.CardFieldId == itemCardField.Id)
                        .ToListAsync();
                    if (tempCardField != null)
                    {
                        itemCardField.ClickCardFields = tempCardField;
                    }
                }
            }

            return cards;
        }

        public async Task<bool> CreateNewCardAsync(Card card, User user)
        {
            Log.Debug($"CreateNewCardAsync start. userName: {user.UserName}");
            var dbCard = await _dbContext.Cards.FirstOrDefaultAsync(x => x.PublicID == card.PublicID);
            if (dbCard != null) return false;
            for (int i = 0; i < card.cardFields.Count; i++)
            {
                card.cardFields[i].Order = i;
            }

            card.UserId = user.Id;
            await _dbContext.Cards.AddAsync(card);
            await _dbContext.SaveChangesAsync();
            Log.Debug($"CreateNewCardAsync completed. userName: {user.UserName}");
            return true;
        }

        public async Task<Card> GetEditCardAsync(int cardId, User user)
        {
            Log.Debug($"GetEditCardAsync start. CardId:{cardId} userName: {user.UserName}");
            var userCardId = await _dbContext.Cards.Where(x => x.UserId == user.Id).Select(x => x.Id).ToArrayAsync();
            foreach (var item in userCardId)
            {
                if (item == cardId)
                {
                    var card = await _dbContext.Cards.FirstOrDefaultAsync(x => x.Id == cardId);
                    card.cardFields = await _dbContext.CardFields.Where(x => x.CardId == cardId).OrderBy(x => x.Order)
                        .ToListAsync();
                    Log.Debug($"GetEditCardAsync completed. CardId:{cardId} userName:{user.UserName}");
                    return card;
                }
            }
            Log.Warn($"GetEditCardAsync not completed. CardId:{cardId} userName:{user.UserName}");
            return null;
        }

        public async Task<bool> EditCardAsync(Card card, User user)
        {
            Log.Debug($"EditCardAsync start. CardId:{card.Id} userName: {user.UserName}");
            var dbCard = await _dbContext.Cards
                .Include(c=>c.cardFields)
                .FirstOrDefaultAsync(x => x.Id == card.Id && x.UserId == user.Id);
            if (await CheckCardNameAsync(card.PublicID, user))
            {
                if (dbCard != null)
                {
                    for (int i = 0; i < card.cardFields.Count; i++)
                    {
                        card.cardFields[i].Order = i;
                    }
                    if (card.cardFields.Count == dbCard.cardFields.Count)
                    {
                        for (int i = 0; i < card.cardFields.Count; i++)
                        {
                            dbCard.cardFields[i].FieldName = card.cardFields[i].FieldName;
                            dbCard.cardFields[i].FieldValue = card.cardFields[i].FieldValue;
                            dbCard.cardFields[i].FieldType = card.cardFields[i].FieldType;
                        }
                    }
                    else 
                    {
                        dbCard.cardFields = card.cardFields;
                    }
                    dbCard.IsPrivate = card.IsPrivate;
                    dbCard.Name = card.Name;
                    dbCard.PublicID = card.PublicID;
                    _dbContext.Cards.Update(dbCard);
                    await _dbContext.SaveChangesAsync();
                    Log.Debug($"EditCardAsync completed. CardId:{card.Id} userName:{user.UserName}");
                    return true;
                }
            }
            Log.Warn($"EditCardAsync not completed. CardId:{card.Id} userName:{user.UserName}");
            return false;
        }

        public async Task<Card> GetDeleteCardAsync(int cardId, User user)
        {
            Log.Debug($"GetDeleteCardAsync start. CardId:{cardId} userName: {user.UserName}");
            var dbCard = await _dbContext.Cards.FirstOrDefaultAsync(x => x.Id == cardId && x.UserId == user.Id);
            if (dbCard == null)return null;
            Log.Debug($"GetDeleteCardAsync completed. CardId:{cardId} userName: {user.UserName}");
            return await _dbContext.Cards.FirstOrDefaultAsync(x => x.Id == cardId);
        }

        public async Task<bool> DeleteCardAsync(Card card, User user)
        {
            Log.Debug($"DeleteCardAsync start. CardId:{card.Id} userName: {user.UserName}");
            var deleteCard = await _dbContext.Cards.FirstOrDefaultAsync(x => x.Id == card.Id && x.UserId == user.Id);
            if (deleteCard == null) return false;
            _dbContext.Cards.Remove(deleteCard);
            await _dbContext.SaveChangesAsync();
            Log.Debug($"DeleteCardAsync completed. userName: {user.UserName}");
            return true;
        }

        public async Task AddNewVisitAsync(string clientIp, int cardId, string userAgentString)
        {
            Log.Debug($"AddNewVisitAsync start. clientIp:{clientIp} CardId:{cardId} userAgentString: {userAgentString}");
            var visitCounter = new VisitСounter
                {DateTime = DateTime.Now, Ip = clientIp, CardId = cardId, UserAgentString = userAgentString};
            _dbContext.VisitСounters.Add(visitCounter);
            await _dbContext.SaveChangesAsync();
            Log.Debug($"AddNewVisitAsync completed.CardId:{cardId}");
        }

        public async Task<List<VisitСounter>> GetVisits(User user, int cardId, string format ) {
            Log.Debug($"GetVisits start. CardId:{cardId} userName: {user.UserName}");
            var responseFromServer = "";
            JObject jsonresponse;
            var card = await _dbContext.Cards.FirstOrDefaultAsync(x => x.Id == cardId && x.UserId == user.Id);
                List<VisitСounter> visits;
                if (card != null)
                {
                    switch (format)
                    {
                        case "Day":
                            visits = await _dbContext.VisitСounters.Where(d => d.CardId == cardId && d.DateTime.Day == DateTime.Now.Day && d.DateTime.Month == DateTime.Now.Month && d.DateTime.Year == DateTime.Now.Year).ToListAsync();
                            break;
                        case "Month":
                            visits = await _dbContext.VisitСounters.Where(d => d.CardId == cardId && d.DateTime.Month == DateTime.Now.Month && d.DateTime.Year == DateTime.Now.Year).ToListAsync();
                            break;
                        default: visits = await _dbContext.VisitСounters.Where(x => x.CardId == cardId).ToListAsync();
                            break;
                    }
                    bool needVisitСounterCityParseAndSaveToDb = false;
                    foreach (var item in visits)
                    {
                        //TODO Отрефачить это говно
                        //TODO Добавить проверку на уникальность Ip адресса
                        if (item.City == null)
                        {
                            var request = WebRequest.Create("http://ipwhois.app/json/" + item.Ip);
                            var response = await request.GetResponseAsync();
                            await using (Stream dataStream = response.GetResponseStream())
                            {
                                if (dataStream != null)
                                {
                                    var reader = new StreamReader(dataStream);
                                    //ТОДО Отрефачить ReadToEnd
                                    responseFromServer = reader.ReadToEnd();
                                }

                                jsonresponse = JObject.Parse(responseFromServer);
                            }
                            var city = (string)jsonresponse["city"];
                            item.City = city;
                            item.JsonResponse = responseFromServer;
                            _dbContext.VisitСounters.Update(item);
                            needVisitСounterCityParseAndSaveToDb = true;
                        }
                    }
                    if (needVisitСounterCityParseAndSaveToDb)
                    {
                        await _dbContext.SaveChangesAsync();
                    }
                    Log.Debug($"GetVisits completed. CardId:{cardId} userName: {user.UserName}");
                    return visits;
                }
                return new List<VisitСounter>();
        }
        
        public async Task AddCardFieldClick(int cardFieldId, string ip) 
        { 
            Log.Debug($"AddCardFieldClick start. cardFieldId:{cardFieldId} ip: {ip}");
            await _dbContext.ClickCardFields.AddAsync(new ClickCardField { CardFieldId = cardFieldId, Ip = ip, DateTime = DateTime.Now });
            await _dbContext.SaveChangesAsync();
            Log.Debug($"AddCardFieldClick completed. cardFieldId:{cardFieldId} ip: {ip}");
        }

        public async Task AddLastLogInDataAsync(string userName)
        {
            Log.Debug($"AddLastLogInDataAsync start. userName:{userName}");
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.UserName == userName);
            if (user != null)
            {
                user.LastLogInData = DateTime.Now.ToString();
                await _dbContext.SaveChangesAsync();
                Log.Debug($"AddLastLogInDataAsync completed. userName:{userName}");
            }
        }

        public Task SendAdminExceptionEmailMessageAsync(string exceptionMessage)
        {
            //TODO добавить отправку сообщения
            return null;
        }
        
        public async Task<User> GetUserAsync(string userName)
        {
            Log.Debug($"GetUserAsync start. userName:{userName}");
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.UserName == userName);
            if (user == null)
            {
                throw new UserNotFoundException(userName);
            }
            Log.Debug($"GetUserAsync completed. userName:{userName}, userId:{user.Id}");
            return user;
        }
    }
}