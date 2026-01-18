using BlazorSolitaire.Models.Common.Enums;
using BlazorSolitaire.Models.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlazorSolitaire.Models.BigTwo
{
    public class BigTwoDeck : IDeck
    {
        private Stack<ICard> _cards = new Stack<ICard>();

        public int Count => _cards.Count;

        public BigTwoDeck()
        {
            Initialize();
        }

        private void Initialize()
        {
            var cards = new List<BigTwoCard>();
            foreach (CardSuit suit in Enum.GetValues(typeof(CardSuit)))
            {
                foreach (CardValue value in Enum.GetValues(typeof(CardValue)))
                {
                    cards.Add(new BigTwoCard
                    {
                        Suit = suit,
                        Value = value,
                        ImageName = $"card{suit}{value}", // 注意：需確認圖片命名規則是否需要調整，目前沿用 Solitaire 規則
                        IsVisible = true // 預設發牌可見，或由 Game 控制
                    });
                }
            }
            
            // 洗牌
            var rng = new Random();
            var shuffledIndex = Enumerable.Range(0, cards.Count).OrderBy(x => rng.Next()).ToList();
            
            _cards.Clear();
            foreach(var idx in shuffledIndex)
            {
                _cards.Push(cards[idx]);
            }
        }

        public void Add(ICard card)
        {
            _cards.Push(card);
        }

        public ICard Draw()
        {
            if (_cards.Count > 0)
                return _cards.Pop();
            return null;
        }

        public void Shuffle()
        {
            // 簡單重置並洗牌
            Initialize();
        }
    }
}
