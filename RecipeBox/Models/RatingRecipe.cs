
namespace RecipeBox.Models
{
  public class RatingRecipe
  {
    public int RatingRecipeId { get; set; }
    public int RatingId { get; set; }
    public int RecipeId { get; set; }
    public virtual Rating Rating { get; set; }
    public virtual Recipe Recipe { get; set; }
  }
}