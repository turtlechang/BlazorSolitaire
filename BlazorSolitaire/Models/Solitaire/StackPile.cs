using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorSolitaire.Models.Solitaire
{
    // Stacks 牌堆區必須確保：
    // 卡片按照交替顏色放置，例如: 黑-紅-黑-紅-黑
    // 卡片按照降序排列，例如: Jack-19-8-7
    // 只有 King 國王可以放置在空牌堆上。

    // 最難實現的功能是：
    // 子牌堆可以從一堆移動到另一堆，前提是它們遵循顏色和排序規則。

    public class StackPile : PileBase
    {
        // 我們還需要一種特殊方法，它可以獲取給定牌堆中特定卡片的索引。
        // 當我們實現將子牌堆從一個牌堆拖到另一個牌堆時，我們將使用這種方法
        public int IndexOf(Card card)
        {
            var matchingCard = Cards.FirstOrDefault(x => x.Suit == card.Suit && x.Value == card.Value);
            if (matchingCard != null)
                return Cards.IndexOf(matchingCard);

            return 0;
        }

        // 一種檢查牌堆中的所有卡片是否都未被隱藏的方法。
        // 這將用於我們將在本系列的第 6 部分中創建的自動完成功能。
        public bool HasNoHiddenCards()
        {
            return !Any() || Cards.All(x => x.IsVisible);
        }

        // 一個返回牌堆中卡片數量的方法，因此我們可以知道牌堆是否為空
        public int Count()
        {
            return Cards.Count();
        }

        // 一種獲取所有卡片的方法，以便我們可以將它們顯示給用戶
        public List<Card> GetAllCards()
        {
            return Cards;
        }
    }
}
