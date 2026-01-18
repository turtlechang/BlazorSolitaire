namespace BlazorSolitaire.Models.Core
{
    public interface IDeck
    {
        void Shuffle();
        ICard Draw();
        int Count { get; }
        void Add(ICard card);
    }
}
