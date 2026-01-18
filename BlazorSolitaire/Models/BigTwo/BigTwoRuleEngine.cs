using BlazorSolitaire.Models.Common.Enums;
using BlazorSolitaire.Models.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlazorSolitaire.Models.BigTwo
{
    public enum HandType
    {
        Invalid,
        Single,         // 單張
        Pair,           // 對子
        Straight,       // 順子
        Flush,          // 同花
        FullHouse,      // 葫蘆
        FourOfAKind,    // 鐵支
        StraightFlush   // 同花順
    }

    public static class BigTwoRuleEngine
    {
        // 驗證是否為合法牌型，並回傳牌型與主要權重(用於比大小)
        public static (HandType Type, int Weight) DetermineHandType(List<ICard> cards)
        {
            if (cards == null || cards.Count == 0) return (HandType.Invalid, 0);
            
            var bCards = cards.OfType<BigTwoCard>().OrderBy(c => c.Weight).ToList();
            if (bCards.Count != cards.Count) return (HandType.Invalid, 0); // 含非 BigTwoCard

            int count = bCards.Count;

            if (count == 1)
            {
                return (HandType.Single, bCards[0].Weight);
            }
            else if (count == 2)
            {
                if (bCards[0].Value == bCards[1].Value)
                {
                    // 對子比大小看最大那張 (含花色)
                    return (HandType.Pair, bCards[1].Weight);
                }
            }
            else if (count == 5)
            {
                return CheckFiveCardHand(bCards);
            }

            return (HandType.Invalid, 0);
        }

        private static (HandType Type, int Weight) CheckFiveCardHand(List<BigTwoCard> sortedCards)
        {
            bool isFlush = sortedCards.All(c => c.Suit == sortedCards[0].Suit);
            bool isStraight = IsStraight(sortedCards);

            // 同花順
            if (isFlush && isStraight)
            {
                // 權重看最大張
                return (HandType.StraightFlush, sortedCards.Last().Weight);
            }

            // 鐵支 (4+1)
            // 由於已排序，可能是 AAAA B 或 A BBBB
            var groups = sortedCards.GroupBy(c => c.Value).ToList();
            if (groups.Any(g => g.Count() == 4))
            {
                // 鐵支比大小看四張的點數，權重 + 500 (確保大於葫蘆)
                var fourVal = groups.First(g => g.Count() == 4).Key;
                
                // 找出鐵支中最大的一張作為權重代表 (其實只要看數字，但為了統一用 Weight)
                // 這裡我們重新計算一個虛擬權重：(ValueWeight * 4) + 1000 以確保壓過一般牌型
                // 但為了簡單，我們直接取鐵支中最大那張牌的 Weight，並在 HandType 層級做區隔
                return (HandType.FourOfAKind, sortedCards.First(c => c.Value == fourVal).Weight);
            }

            // 葫蘆 (3+2)
            if (groups.Count == 2 && groups.Any(g => g.Count() == 3))
            {
                var threeVal = groups.First(g => g.Count() == 3).Key;
                // 葫蘆比三張
                return (HandType.FullHouse, sortedCards.Last(c => c.Value == threeVal).Weight);
            }

            // 同花: 只比最大張，若最大張相同比第二大...這裡簡化為比最大張 (標準規則通常是比最大的花色，或比最大張數字)
            // 台灣規則通常：先比花色 (黑桃>...>梅花)，花色相同比數字
            // 但也有規則是：比最大張。
            // 這裡採用：比最大張 (BigTwoCard Weight 已含花色)
            if (isFlush)
            {
                return (HandType.Flush, sortedCards.Last().Weight);
            }

            // 順子: 比最大張
            if (isStraight)
            {
                return (HandType.Straight, sortedCards.Last().Weight);
            }

            return (HandType.Invalid, 0);
        }

        private static bool IsStraight(List<BigTwoCard> sortedCards)
        {
            // 處理 A 2 3 4 5 等特殊順子邏輯 (BigTwoCard Weight 是 3..K, A, 2)
            // 如果單純看 Weight 是不連續的 (因 A, 2 最大)
            // 但若是順子 3 4 5 6 7, Weight 是連續的
            // 2 3 4 5 6 (2最大) -> 不算順子
            // A 2 3 4 5 -> 不算順子
            // 10 J Q K A -> 算
            // J Q K A 2 -> 算 (2最大)
            
            // 簡單檢查：數值連續
            // 需先取得原始數字大小進行檢查 (非權重)
            // 3=3, ..., A=1, 2=2
            // 轉成 1(A), 2(2), 3(3)...13(K) 來檢查連續性
            var values = sortedCards.Select(c => (int)c.Value).OrderBy(v => v).ToList();
            
            // 一般順子 (3 4 5 6 7)
            bool isNormalStraight = true;
            for(int i=0; i<4; i++)
            {
                if (values[i+1] != values[i] + 1)
                {
                    isNormalStraight = false;
                    break;
                }
            }
            if (isNormalStraight) return true;

            // 特殊順子 (10 J Q K A), (J Q K A 2)
            // 10, 11, 12, 13, 1
            // 11, 12, 13, 1, 2
             if (values[0] == 1 && values[1] == 10 && values[2] == 11 && values[3] == 12 && values[4] == 13) return true; // 10-A
             if (values[0] == 1 && values[1] == 2 && values[2] == 11 && values[3] == 12 && values[4] == 13) return true; // J-2 (需確認規則是否允許 JQKA2)

            return false;
        }

        // 比較能不能壓牌
        public static bool CanBeat(List<ICard> handsToPlay, List<ICard> lastHand)
        {
            var myHand = DetermineHandType(handsToPlay);
            if (myHand.Type == HandType.Invalid) return false;

            if (lastHand == null || lastHand.Count == 0) return true; // 先手或對方Pass
            
            var targetHand = DetermineHandType(lastHand);

            // 張數必須相同 (除非是鐵支/同花順? 大老二規則通常鐵支可壓各類順花葫，同花順最大)
            // 這裡簡化：張數必須相同 (大部分規則五張打五張)
            if (handsToPlay.Count != lastHand.Count) return false;

            // 牌型不同時的比較 (只針對五張)
            if (handsToPlay.Count == 5 && myHand.Type != targetHand.Type)
            {
                // 牌型大小：同花順 > 鐵支 > 葫蘆 > 同花 > 順子
                return GetTypeRank(myHand.Type) > GetTypeRank(targetHand.Type);
            }

            // 同牌型比較
            return myHand.Weight > targetHand.Weight;
        }

        private static int GetTypeRank(HandType type)
        {
            switch (type)
            {
                case HandType.StraightFlush: return 5;
                case HandType.FourOfAKind: return 4;
                case HandType.FullHouse: return 3;
                case HandType.Flush: return 2;
                case HandType.Straight: return 1;
                default: return 0;
            }
        }
    }
}
