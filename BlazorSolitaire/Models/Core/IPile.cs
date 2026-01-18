using System.Collections.Generic;

namespace BlazorSolitaire.Models.Core
{
    public interface IPile
    {
        void Add(ICard card);
        void Remove(ICard card);
        void Clear();
        IEnumerable<ICard> GetAll();
    }
}
