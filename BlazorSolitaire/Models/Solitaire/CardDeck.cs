using BlazorSolitaire.Models.Common;
using BlazorSolitaire.Models.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorSolitaire.Models.Solitaire
{    
    public class CardDeck
    {
        protected Stack<Card> Cards { get; set; } = new Stack<Card>();

        // 牌堆的卡片數量
        public int Count
        {
            get
            {
                return Cards.Count;
            }
        }

        // 加入到牌堆中
        public void Add(Card card)
        {
            Cards.Push(card);
        }

        // 初始化牌堆並隨機洗牌
        public CardDeck()
        {
            List<Card> cards = new List<Card>();

            foreach (CardSuit suit in Enum.GetValues(typeof(CardSuit)))
            {
                foreach (CardValue value in (CardValue[])Enum.GetValues(typeof(CardValue)))
                {
                    //For each suit and value, create and insert a new Card object.
                    Card newCard = new Card()
                    {
                        Suit = suit,
                        Value = value,
                        ImageName = "card" + suit.GetDisplayName() + value.GetDisplayName()
                    };

                    cards.Add(newCard);
                }
            }

            var array = cards.ToArray();

            Random rnd = new Random();

            //Step 1: For each unshuffled item in the collection ( 對於集合中的每個未洗牌的項目 )
            for (int n = array.Count() - 1; n > 0; n--)
            {
                //Step 2: Randomly pick an item which has not been shuffled ( 隨機選擇一個未被洗牌的項目 )
                int k = rnd.Next(n + 1);

                //Step 3: Swap the selected item with the last "unstruck" letter in the collection ( 用集合中最後一個“脫離的”的字母交換所選項目 )
                Card temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }

            for (int n = 0; n < array.Count(); n++)
            {
                Cards.Push(array[n]);
            }
        }

        // 在牌堆頂部抽一張牌 顯示
        public Card Draw()
        {
            var card = Cards.Pop();
            card.IsVisible = true;
            return card;
        }

        // 在牌堆頂部抽一張牌 不顯示
        public Card DrawHidden()
        {
            var card = Cards.Pop();
            card.IsVisible = false;
            return card;
        }
    }
}
