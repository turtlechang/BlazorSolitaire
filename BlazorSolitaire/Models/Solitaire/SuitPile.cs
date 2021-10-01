
using BlazorSolitaire.Models.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorSolitaire.Models.Solitaire
{
    // 花色牌堆只能在他們的花色中允許一個特定的值。
    // 如果牌堆是空的，則該牌堆只能接受一張 Ace；
    // 如果它已經有一張 Ace，它只能接受一張 Two；
    // 依此類推，直到通過在其上放置 King 來完成該牌堆。
    public class SuitPile : PileBase
    {
        // 花色
        public CardSuit Suit { get; set; }

        // 讓我們創建一個屬性，
        // SuitPile 該屬性將檢查最上面的卡片，
        // 並返回該牌堆目前正在尋找的數字
        public CardValue AllowedValue
        {
            get
            {
                var topCard = Last();
                if (topCard == null) return CardValue.Ace;

                int currentValue = (int)topCard.Value;
                return (CardValue)(currentValue + 1);
            }
        }

        public SuitPile(CardSuit suit)
        {
            Suit = suit;
        }

        // 我們還將包括一個簡單的便捷方法，
        // 該方法返回 bool 指定牌堆是否完整（即最上面的牌是 King 國王）
        public bool IsComplete
        {
            get
            {
                return (int)AllowedValue == 14;
            }
        }
    }
}
