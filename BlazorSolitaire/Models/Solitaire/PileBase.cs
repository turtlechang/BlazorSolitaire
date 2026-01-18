using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorSolitaire.Models.Solitaire
{
    public class PileBase : BlazorSolitaire.Models.Core.IPile
    {
        // 請注意，我們收集的卡片 PileBase 是 List<Card>，
        // 而不是 Stack<Card>，因為 List<Card> 更容易於交互。
        protected List<Card> Cards { get; set; } = new List<Card>();

        // 增加一張 Card 到 Cards內 
        public void Add(Card card)
        {
            if (card != null)
                Cards.Add(card); // 將物件加入至 List<T> 的末端。
        }

        // 判斷 Cards 是否包含任何 Card
        public bool Any()
        {
            return Cards.Any();
        }

        // 移除並傳回在 Cards 頂端的 Card。
        public Card Pop()
        {            
            var lastCard = Cards.LastOrDefault(); // 傳回序列的最後一個項目；如果找不到任何項目，則傳回預設值。
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

        // 最後，我們需要一個方法來從一堆卡片中取出一張卡片，如果它存在的話。
        // 當卡片從一個牌堆拖到另一個牌堆，或是從棄牌堆到牌堆...等等時，將使用此方法。
        public void RemoveIfExists(Card card)
        {
            var matchingCard = Cards.FirstOrDefault(x => x.Suit == card.Suit && x.Value == card.Value);

            if (matchingCard != null)
                Cards.Remove(matchingCard);
        }

        // Suit Piles 和 Stacks 需要知道特定的牌是否在特定的牌堆中。
        // 我們可以實現一個 Contains() 方法來通過花色和數字找到一張牌
        public bool Contains(Card card)
        {
            return Cards.Any(x => x.Suit == card.Suit && x.Value == card.Value);
        }
        public void Add(Models.Core.ICard card)
        {
            if (card is Card c)
            {
                Add(c);
            }
        }

        public void Remove(Models.Core.ICard card)
        {
             if (card is Card c)
             {
                 RemoveIfExists(c);
             }
        }

        public void Clear()
        {
            Cards.Clear();
        }

        public IEnumerable<Models.Core.ICard> GetAll()
        {
            return Cards.Cast<Models.Core.ICard>();
        }
    }
}
