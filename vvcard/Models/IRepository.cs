
using System.Collections.Generic;
using System.Threading.Tasks;

namespace vvcard.Models
{
    public interface IRepository
    {
        Task<List<Card>> GetCardByIdAsync(string cardId);
        Task<List<Card>> GetRandomCardAsync(int count);
        Task<bool> CheckCardNameAsync(string publicId, User user);
        Task<List<Card>> GetAllPersonalCardAsync(User user);
        Task<List<CardField>> GetCardFieldsAsync(int cardId);
        Task<bool> CreateNewCardAsync(Card card, User user);
        Task<Card> GetEditCardAsync(int cardId, User user);
        Task<bool> EditCardAsync(Card card, User user);
        Task<Card> GetDeleteCardAsync(int cardId, User user);
        Task<bool> DeleteCardAsync(Card card, User user);
        Task AddNewVisitAsync(string clientIp, int cardId, string userAgentString);
        Task<List<VisitСounter>> GetVisits(User user, int cardId, string format);
        Task AddCardFieldClick(int cardFieldId, string Ip);
        Task AddLastLogInDataAsync(string userName);
        Task SendAdminExceptionEmailMessageAsync(string exceptionMessage);
        Task<User> GetUserAsync(string userName);
    }
}
