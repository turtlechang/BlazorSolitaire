using BlazorSolitaire.Models.Core;
using System.Collections.Generic;
using System.Linq;

namespace BlazorSolitaire.Models.BigTwo
{
    public class BigTwoGame
    {
        public List<ICard> Player1Hand { get; set; } = new List<ICard>();
        public List<ICard> Player2Hand { get; set; } = new List<ICard>();
        public List<ICard> Player3Hand { get; set; } = new List<ICard>();
        public List<ICard> Player4Hand { get; set; } = new List<ICard>();

        private BigTwoDeck _deck;

        public BigTwoGame()
        {
            _deck = new BigTwoDeck();
        }

        public void Deal()
        {
            // 清空手牌
            Player1Hand.Clear();
            Player2Hand.Clear();
            Player3Hand.Clear();
            Player4Hand.Clear();

            _deck.Shuffle();

            // 發牌 (4人，每人13張)
            while (_deck.Count > 0)
            {
                Player1Hand.Add(_deck.Draw());
                if (_deck.Count > 0) Player2Hand.Add(_deck.Draw());
                if (_deck.Count > 0) Player3Hand.Add(_deck.Draw());
                if (_deck.Count > 0) Player4Hand.Add(_deck.Draw());
            }

            // 排序手牌
            SortHand(Player1Hand);
            SortHand(Player2Hand);
            SortHand(Player3Hand);
            SortHand(Player4Hand);
        }

        private void SortHand(List<ICard> hand)
        {
            // 轉換為 BigTwoCard 進行排序，再轉回 ICard
            // 這裡需要型別轉換，因為介面是 ICard
            var sorted = hand.OfType<BigTwoCard>().OrderBy(c => c).Cast<ICard>().ToList();
            hand.Clear();
            hand.AddRange(sorted);
        }
    }
}
