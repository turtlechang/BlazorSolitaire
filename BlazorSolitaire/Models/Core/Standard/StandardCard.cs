using BlazorSolitaire.Models.Common.Enums;

namespace BlazorSolitaire.Models.Core.Standard
{
    public class StandardCard : BaseCard
    {
        public CardSuit Suit { get; set; }
        public CardValue Value { get; set; }

        public bool IsRed
        {
            get
            {
                return Suit == CardSuit.Diamonds || Suit == CardSuit.Hearts;
            }
        }

        public bool IsBlack
        {
            get
            {
                return !IsRed;
            }
        }
    }
}
