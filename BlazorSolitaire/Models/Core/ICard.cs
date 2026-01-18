namespace BlazorSolitaire.Models.Core
{
    public interface ICard
    {
        bool IsVisible { get; set; }
        string ImageName { get; set; }
    }
}
