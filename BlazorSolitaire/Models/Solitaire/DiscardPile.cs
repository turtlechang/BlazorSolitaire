using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorSolitaire.Models.Solitaire
{
    public class DiscardPile : PileBase
    {
        // 我們需要一種方法來從棄牌堆中獲取所有卡片，
        // 以便我們可以將它們重新添加到抽牌堆中。
        public List<Card> GetAll()
        {
            // 把一個列舉 IEnumerable 轉換為列表 List 的方法
            return Cards.ToList();
        }
    }
}
