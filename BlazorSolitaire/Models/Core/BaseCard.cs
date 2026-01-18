namespace BlazorSolitaire.Models.Core
{
    public abstract class BaseCard : ICard
    {
        public bool IsVisible { get; set; }
        public string ImageName { get; set; }
    }
}
