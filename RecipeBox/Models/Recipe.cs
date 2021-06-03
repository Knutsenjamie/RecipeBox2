using System;
using System.Collections.Generic;


namespace RecipeBox.Models
{
  public class Recipe
  {
    public Recipe()
    {
      this.JoinIR = new HashSet<IngredientRecipe>();
      this.JoinRR = new HashSet<RatingRecipe>();
    }
    public int RecipeId { get; set; }
    public string Title { get; set; }
    public string Instructions { get; set; }
    public string Style { get; set; }
    public string Course { get; set; }
    public DateTime PrepDate { get; set; }
    public int PrepTime { get; set; }
    public virtual ApplicationUser User { get; set; }
    public virtual ICollection<IngredientRecipe> JoinIR { get; }
    public virtual ICollection<RatingRecipe> JoinRR { get; }
  }
}