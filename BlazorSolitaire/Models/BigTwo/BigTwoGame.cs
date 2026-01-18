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
        public List<ICard> LastPlayedCards { get; set; } = new List<ICard>();

        public int CurrentPlayerIndex { get; private set; } // 0: User, 1: CPU2, 2: CPU3, 3: CPU4
        public int PassCount { get; private set; }
        public string GameStatusMessage { get; set; } = "";
        public bool IsGameOver { get; private set; }

        private BigTwoDeck _deck;

        public BigTwoGame()
        {
            _deck = new BigTwoDeck();
        }

        public void Deal()
        {
            Player1Hand.Clear();
            Player2Hand.Clear();
            Player3Hand.Clear();
            Player4Hand.Clear();
            LastPlayedCards = new List<ICard>();
            PassCount = 0;
            IsGameOver = false;

            _deck.Shuffle();

            while (_deck.Count > 0)
            {
                Player1Hand.Add(_deck.Draw());
                if (_deck.Count > 0) Player2Hand.Add(_deck.Draw());
                if (_deck.Count > 0) Player3Hand.Add(_deck.Draw());
                if (_deck.Count > 0) Player4Hand.Add(_deck.Draw());
            }

            SortHand(Player1Hand);
            SortHand(Player2Hand);
            SortHand(Player3Hand);
            SortHand(Player4Hand);

            DetermineFirstPlayer();
        }

        private void DetermineFirstPlayer()
        {
            // Find 3 of Clubs
            if (HasThreeOfClubs(Player1Hand)) CurrentPlayerIndex = 0;
            else if (HasThreeOfClubs(Player2Hand)) CurrentPlayerIndex = 1;
            else if (HasThreeOfClubs(Player3Hand)) CurrentPlayerIndex = 2;
            else CurrentPlayerIndex = 3;

            GameStatusMessage = $"玩家 {GetPlayerName(CurrentPlayerIndex)} 持有梅花3，優先出牌";
        }

        private bool HasThreeOfClubs(List<ICard> hand)
        {
            return hand.OfType<BigTwoCard>().Any(c => c.Suit == BlazorSolitaire.Models.Common.Enums.CardSuit.Clubs && c.Value == BlazorSolitaire.Models.Common.Enums.CardValue.Three);
        }

        private void SortHand(List<ICard> hand)
        {
            var sorted = hand.OfType<BigTwoCard>().OrderBy(c => c).Cast<ICard>().ToList();
            hand.Clear();
            hand.AddRange(sorted);
        }

        public bool Play(List<ICard> cards)
        {
            if (IsGameOver) return false;
            
            // Basic Validation
            if (cards == null || !cards.Any()) return false;
            
            // Rule validation
            var handType = BigTwoRuleEngine.DetermineHandType(cards);
            if (handType.Type == HandType.Invalid)
            {
                GameStatusMessage = "無效的牌型";
                return false;
            }

            // Check if can beat passed cards (unless new round)
            bool isNewRound = LastPlayedCards == null || LastPlayedCards.Count == 0;
            if (!isNewRound)
            {
                if (!BigTwoRuleEngine.CanBeat(cards, LastPlayedCards))
                {
                    GameStatusMessage = "你的牌太小了";
                    return false;
                }
            }
            else
            {
                 // Check 3 of Clubs rule for first turn
                 // If it is the VERY first turn (everyone has 13 cards), must play 3 of Clubs
                 // We can check if all hands are full (13 * 4 = 52) but easier to flag it?
                 // Or just check if the player holds 3 of clubs, they MUST include it.
                 // Simplification: If you hold 3 Clubs, you must play it.
                 var currentHand = GetCurrentHand();
                 if (HasThreeOfClubs(currentHand))
                 {
                     if (!HasThreeOfClubs(cards))
                     {
                          GameStatusMessage = "第一手必須打出梅花3";
                          return false;
                     }
                 }
            }

            // Execute Play
            var hand = GetCurrentHand();
            foreach (var card in cards)
            {
                var cardToRemove = hand.FirstOrDefault(c => c == card);
                if (cardToRemove != null) hand.Remove(cardToRemove);
            }

            LastPlayedCards = cards;
            PassCount = 0;

            CheckWinner();
            if (!IsGameOver)
            {
                NextTurn();
            }
            return true;
        }

        public void Pass()
        {
            if (IsGameOver) return;
            if (LastPlayedCards == null || LastPlayedCards.Count == 0)
            {
                GameStatusMessage = "你擁有發牌權，不能 Pass";
                return;
            }

            PassCount++;
            if (PassCount >= 3)
            {
                LastPlayedCards = new List<ICard>(); // New Round
            }
            
            NextTurn();
        }

        private void NextTurn()
        {
            CurrentPlayerIndex = (CurrentPlayerIndex + 1) % 4;
            GameStatusMessage = $"輪到 {GetPlayerName(CurrentPlayerIndex)}";
        }

        private void CheckWinner()
        {
            if (Player1Hand.Count == 0) { IsGameOver = true; GameStatusMessage = "玩家獲勝！"; }
            else if (Player2Hand.Count == 0) { IsGameOver = true; GameStatusMessage = "CPU 2 獲勝！"; }
            else if (Player3Hand.Count == 0) { IsGameOver = true; GameStatusMessage = "CPU 3 獲勝！"; }
            else if (Player4Hand.Count == 0) { IsGameOver = true; GameStatusMessage = "CPU 4 獲勝！"; }
        }

        public List<ICard> GetCurrentHand()
        {
            switch(CurrentPlayerIndex)
            {
                case 0: return Player1Hand;
                case 1: return Player2Hand;
                case 2: return Player3Hand;
                case 3: return Player4Hand;
                default: return new List<ICard>();
            }
        }

        private string GetPlayerName(int index) => index == 0 ? "You" : $"CPU {index + 1}";

        // AI Logic
        public void AutoPlay()
        {
            if (IsGameOver) return;
            
            var hand = GetCurrentHand();
            bool isNewRound = LastPlayedCards == null || LastPlayedCards.Count == 0;
            
            // Simple AI: Try to find smallest valid play
            List<ICard> cardsToPlay = null;

            // 1. New Round: Play smallest single (or 3 of Clubs if held)
            if (isNewRound)
            {
                // Must play 3 of Clubs if have it
                if (HasThreeOfClubs(hand))
                {
                     // Find combo with 3 of Clubs? Simplest: Single 3 Clubs
                     var c3 = hand.OfType<BigTwoCard>().First(c => c.Suit == BlazorSolitaire.Models.Common.Enums.CardSuit.Clubs && c.Value == BlazorSolitaire.Models.Common.Enums.CardValue.Three);
                     cardsToPlay = new List<ICard> { c3 };
                }
                else
                {
                    // Play smallest single
                    cardsToPlay = new List<ICard> { hand[0] };
                }
            }
            else
            {
                // Try to beat current hand
                // AI currently only supports Single play for simplicity, needs expansion for Pairs/etc later
                // Or try to find any single card that beats
                if (LastPlayedCards.Count == 1)
                {
                    var validCard = hand.OfType<BigTwoCard>().FirstOrDefault(c => BigTwoRuleEngine.CanBeat(new List<ICard>{c}, LastPlayedCards));
                    if (validCard != null)
                    {
                        cardsToPlay = new List<ICard> { validCard };
                    }
                }
                // TODO: Support Pairs/Straight AI
            }

            if (cardsToPlay != null)
            {
                Play(cardsToPlay);
            }
            else
            {
                Pass();
            }
        }

    }
}
