using System.Threading.Tasks;
using Core.Entities;

namespace Core.Interfaces
{
    public interface IBasketInterface
    {
         Task <CustomerBasket> getBasketAsync(string id);
         Task <CustomerBasket> UpdateBasket(CustomerBasket basket);

         Task <bool> DeleteBasketAsync(string basketId);
    }
}