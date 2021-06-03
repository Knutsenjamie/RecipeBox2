using System.Collections.Generic;

namespace RecipeBox.Models
{
  public class Rating
  {
    public int RatingId { get; set; }
    public int LucyStar { get; set; }

    public Rating()
    {
      this.JoinRR = new HashSet<RatingRecipe>();
    }

    public virtual ICollection<RatingRecipe> JoinRR { get; }
    public virtual ApplicationUser User { get; set; }
  }
}
