using BlazorSolitaire.Models.Common.Enums;
using BlazorSolitaire.Models.Core.Standard;
using System;

namespace BlazorSolitaire.Models.BigTwo
{
    public class BigTwoCard : StandardCard, IComparable<BigTwoCard>
    {
        // 權重計算屬性
        public int Weight
        {
            get
            {
                // 處理數字權重
                // 3(3) < ... < K(13) < A(14) < 2(15)
                int valWeight = (int)Value;
                if (Value == CardValue.Ace) valWeight = 14;
                else if (Value == CardValue.Two) valWeight = 15;

                // 處理花色權重
                // 梅花(0) < 方塊(1) < 愛心(2) < 黑桃(3) (這裡使用常見規則，具體可依需求調整)
                // CardSuit Enum: Hearts(0), Clubs(1), Diamonds(2), Spades(3)
                // 調整為: Clubs(0) < Diamonds(1) < Hearts(2) < Spades(3)
                int suitWeight = 0;
                switch (Suit)
                {
                    case CardSuit.Clubs: suitWeight = 0; break;
                    case CardSuit.Diamonds: suitWeight = 1; break;
                    case CardSuit.Hearts: suitWeight = 2; break;
                    case CardSuit.Spades: suitWeight = 3; break;
                }

                // 綜合權重: 數字 * 4 + 花色
                return valWeight * 4 + suitWeight;
            }
        }

        public int CompareTo(BigTwoCard other)
        {
            if (other == null) return 1;
            return this.Weight.CompareTo(other.Weight);
        }
    }
}
