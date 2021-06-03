using System.ComponentModel.DataAnnotations;

namespace RecipeBox.Models
{
  public class IngredientRecipe
  {
    [Key]
    public int IngredientRecipeId { get; set; }
    public int RecipeId { get; set; }
    public int IngredientId { get; set; }
    public virtual Recipe Recipe { get; set; }
    public virtual Ingredient Ingredient { get; set; }
  }
}