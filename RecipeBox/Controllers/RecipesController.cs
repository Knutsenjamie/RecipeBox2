using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeBox.Models;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;


namespace RecipeBox.Controllers
{
  [Authorize]
  public class RecipesController : Controller
  {
    private readonly RecipeBoxContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public RecipesController(UserManager<ApplicationUser> userManager, RecipeBoxContext db)
    {
      _userManager = userManager;
      _db = db;
    }
    
    [AllowAnonymous]
    public async Task<ActionResult> Index()
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      var userRecipes = _db.Recipes.Where(entry => entry.User.Id == currentUser.Id).ToList();
      var allRecipes = _db.Recipes;
      return View(userRecipes);
      // else { return View(); }
    }

    public ActionResult Create()
    {
      ViewBag.IngredientId = new SelectList(_db.Ingredients, "IngredientId", "IngredientName");
      return View();
    }

    // // Overload with three ingredient arguments
    // [HttpPost, ActionName("Create")]
    // public async Task<ActionResult> CreateThree(Recipe recipe, int IngredientId1, int IngredientId2, int IngredientId3)
    // {
    //   var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    //   var currentUser = await _userManager.FindByIdAsync(userId);
    //   recipe.User = currentUser;
    //   _db.Recipes.Add(recipe);
    //   _db.SaveChanges();
    //   if (IngredientId1 != 0 && IngredientId2 != 0 && IngredientId3 != 0)
    //   {
    //     _db.IngredientRecipes.Add(new IngredientRecipe() { IngredientId = IngredientId1, RecipeId = recipe.RecipeId });
    //     _db.IngredientRecipes.Add(new IngredientRecipe() { IngredientId = IngredientId2, RecipeId = recipe.RecipeId });
    //     _db.IngredientRecipes.Add(new IngredientRecipe() { IngredientId = IngredientId3, RecipeId = recipe.RecipeId });
    //   }
    //   _db.SaveChanges();
    //   return RedirectToAction("Index");
    // }

    // // Overload with two ingredient arguments
    // [HttpPost, ActionName("Create")]
    // public async Task<ActionResult> CreateTwo(Recipe recipe, int IngredientId1, int IngredientId2)
    // {
    //   var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    //   var currentUser = await _userManager.FindByIdAsync(userId);
    //   recipe.User = currentUser;
    //   _db.Recipes.Add(recipe);
    //   _db.SaveChanges();
    //   if (IngredientId1 != 0 && IngredientId2 != 0)
    //   {
    //     _db.IngredientRecipes.Add(new IngredientRecipe() { IngredientId = IngredientId1, RecipeId = recipe.RecipeId });
    //     _db.IngredientRecipes.Add(new IngredientRecipe() { IngredientId = IngredientId2, RecipeId = recipe.RecipeId });
    //   }
    //   _db.SaveChanges();
    //   return RedirectToAction("Index");
    // }

    // Overload with one ingredient argument
    [HttpPost]
    public async Task<ActionResult> Create(Recipe recipe, int IngredientId1)
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      recipe.User = currentUser;
      _db.Recipes.Add(recipe);
      _db.SaveChanges();
      if (IngredientId1 != 0)
      {
        _db.IngredientRecipes.Add(new IngredientRecipe() { IngredientId = IngredientId1, RecipeId = recipe.RecipeId });
      }
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    public ActionResult Details(int id)
    {
      var thisRecipe = _db.Recipes
            .Include(recipe => recipe.JoinIR)
            .ThenInclude(join => join.Ingredient)
            .Include(recipe => recipe.JoinRR)
            .ThenInclude(join => join.Rating)
            .FirstOrDefault(recipe => recipe.RecipeId == id);
      return View(thisRecipe);
    }

    public ActionResult Edit(int id)
    {
      var thisRecipe = _db.Recipes.FirstOrDefault(recipe => recipe.RecipeId == id);
      ViewBag.RatingId = new SelectList(_db.Ratings, "RatingId", "LucyStar");
      ViewBag.IngredientId = new SelectList(_db.Ingredients, "IngredientId", "IngredientId");
      return View(thisRecipe);
    }

    [HttpPost]
    public ActionResult Edit(Recipe recipe, int IngredientId, int RatingId)
    {
      if (IngredientId != 0 && RatingId != 0)
      {
        _db.IngredientRecipes.Add(new IngredientRecipe() { IngredientId = IngredientId, RecipeId = recipe.RecipeId });
        _db.RatingRecipes.Add(new RatingRecipe() { RatingId = RatingId, RecipeId = recipe.RecipeId });
      }
      else if (IngredientId != 0 && RatingId < 1)
      {
        _db.IngredientRecipes.Add(new IngredientRecipe() { IngredientId = IngredientId, RecipeId = recipe.RecipeId });
      }
      else if (IngredientId < 1 && RatingId != 0)
      {
        _db.RatingRecipes.Add(new RatingRecipe() { RatingId = RatingId, RecipeId = recipe.RecipeId });
      }
      _db.Entry(recipe).State = EntityState.Modified;
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    public ActionResult AddIngredient(int id)
    {
      var thisRecipe = _db.Recipes.FirstOrDefault(recipe => recipe.RecipeId == id);
      ViewBag.IngredientId = new SelectList(_db.Ingredients, "IngredientId", "Name");
      return View(thisRecipe);
    }

    [HttpPost]
    public ActionResult AddIngredient(Recipe recipe, int IngredientId)
    {
      if (IngredientId != 0)
      {
        _db.IngredientRecipes.Add(new IngredientRecipe() { IngredientId = IngredientId, RecipeId = recipe.RecipeId });
      }
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    public ActionResult AddRating(int id)
    {
      var thisRecipe = _db.Recipes.FirstOrDefault(recipe => recipe.RecipeId == id);
      ViewBag.RatingId = new SelectList(_db.Ratings, "RatingId", "LucyStar");
      return View(thisRecipe);
    }

    [HttpPost]
    public ActionResult AddRating(Recipe recipe, int RatingId)
    {
      if (RatingId != 0)
      {
        _db.RatingRecipes.Add(new RatingRecipe() { RatingId = RatingId, RecipeId = recipe.RecipeId });
      }
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    public ActionResult Delete(int id)
    {
      var thisRecipe = _db.Recipes.FirstOrDefault(recipe => recipe.RecipeId == id);
      return View(thisRecipe);
    }

    [HttpPost, ActionName("Delete")]
    public ActionResult DeleteConfirmed(int id)
    {
      var thisRecipe = _db.Recipes.FirstOrDefault(recipe => recipe.RecipeId == id);
      _db.Recipes.Remove(thisRecipe);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    [HttpPost]
    public ActionResult DeleteIngredient(int joinId)
    {
      var joinEntry = _db.IngredientRecipes.FirstOrDefault(entry => entry.IngredientRecipeId == joinId);
      _db.IngredientRecipes.Remove(joinEntry);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    [HttpPost]
    public ActionResult DeleteRating(int joinId)
    {
      var joinEntry = _db.RatingRecipes.FirstOrDefault(entry => entry.RatingRecipeId == joinId);
      _db.RatingRecipes.Remove(joinEntry);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }
  }
}