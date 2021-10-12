using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace vvcard.Models
{
    public interface IRepository
    {
        Task<List<Card>> GetCardByIdAsync(string id);
        Task<List<Card>> GetRandomCardAsync(int count);
        Task<bool> CheckCardNameAsync(string id, string userName);
        Task<List<Card>> GetAllPersonalCardAsync(string userName);
        Task<List<CardField>> GetCardFieldsAsync(int cardID);
        Task<bool> CreateNewCardAsync(Card card, string userName);
        Task<Card> GetEditCardAsync(int cardID, string userName);
        Task<bool> EditCardAsync(Card card, string userName);
        Task<Card> GetDeleteCardAsync(int id, string userName);
        Task<bool> DeleteCardAsync(Card card, string userName);
        Task AddNewVisitAsync(string ClientIp, int cardID, string userAgentString);
        Task<List<VisitСounter>> GetVisits(string userName, int cardId, string format);
        Task AddCardFieldClick(int cardFieldId, string Ip);
    }
}
