using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorSolitaire.Models.Solitaire
{
    public class PileBase
    {

        protected List<Card> Cards { get; set; } = new List<Card>();

        // 增加一張 Card 到 Cards內 
        public void Add(Card card)
        {
            if (card != null)
                Cards.Add(card);
        }

        // 判斷 Cards 是否包含任何 Card
        public bool Any()
        {
            return Cards.Any();
        }

        // 移除並傳回在 Cards 頂端的 Card。
        public Card Pop()
        {
            var lastCard = Cards.LastOrDefault();
            if (lastCard != null)
            {
                Cards.Remove(lastCard);
            }

            return lastCard;
        }

        // 傳回 Cards 的最後一張 Card；如果找不到任何項目，則傳回預設值。
        public Card Last()
        {
            return Cards.LastOrDefault();
        }

        public void RemoveIfExists(Card card)
        {
            var matchingCard = Cards.FirstOrDefault(x => x.Suit == card.Suit && x.Value == card.Value);
            if (matchingCard != null)
                Cards.Remove(matchingCard);
        }

        public bool Contains(Card card)
        {
            return Cards.Any(x => x.Suit == card.Suit && x.Value == card.Value);
        }
    }
}
