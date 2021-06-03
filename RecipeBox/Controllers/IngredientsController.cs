using Microsoft.AspNetCore.Mvc;
using RecipeBox.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Security.Claims;

namespace RecipeBox.Controllers
{
  [Authorize]
  public class IngredientsController : Controller
  {
    private readonly RecipeBoxContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    public IngredientsController(UserManager<ApplicationUser> userManager, RecipeBoxContext db)
    {
      _userManager = userManager;
      _db = db;
    }
    public async Task<ActionResult> Index()
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      var userIngredients = _db.Ingredients.Where(entry => entry.User.Id == currentUser.Id).ToList();
      return View(userIngredients);
    }
    public async Task<ActionResult> Create()
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      var userRecipes = _db.Recipes.Where(entry => entry.User.Id == currentUser.Id).ToList();
      ViewBag.RecipeId = new SelectList(userRecipes, "RecipeId", "Title");
      return View();
    }
    [HttpPost]
    public async Task<ActionResult> Create(Ingredient ingredient, int RecipeId)
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      ingredient.User = currentUser;
      _db.Ingredients.Add(ingredient);
      _db.SaveChanges();
      if (RecipeId != 0)
      {
        _db.IngredientRecipes.Add(new IngredientRecipe() { RecipeId = RecipeId, IngredientId = ingredient.IngredientId });
      }
      _db.SaveChanges();
      return RedirectToAction("Index");
    }
    public ActionResult Details(int id)
    {
      var thisIngredient = _db.Ingredients
      .Include(ingredient => ingredient.JoinIR)
      .ThenInclude(join => join.Recipe)
      .FirstOrDefault(ingredient => ingredient.IngredientId == id);
      return View(thisIngredient);
    }
    public ActionResult Edit(int id)
    {
      var thisIngredient = _db.Ingredients.FirstOrDefault(ingredient => ingredient.IngredientId == id);
      ViewBag.RecipeId = new SelectList(_db.Ingredients, "IngredientId", "Title");
      return View(thisIngredient);
    }
    [HttpPost]
    public ActionResult Edit(Ingredient ingredient, int RecipeId)
    {
      if (RecipeId != 0)
      {
        _db.IngredientRecipes.Add(new IngredientRecipe() { RecipeId = RecipeId, IngredientId = ingredient.IngredientId });
      }
      _db.Entry(ingredient).State = EntityState.Modified;
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    public async Task<ActionResult> AddRecipe(int id)
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      var userRecipes = _db.Recipes.Where(entry => entry.User.Id == currentUser.Id).ToList();
      var thisIngredient = _db.Ingredients.FirstOrDefault(ingredient => ingredient.IngredientId == id);
      ViewBag.RecipeId = new SelectList(userRecipes, "RecipeId", "Title");
      return View(thisIngredient);
    }

    // needs to be updated
    [HttpPost]
    public ActionResult AddRecipe(Recipe recipe, int IngredientId)
    {
      if (IngredientId != 0)
      {
        _db.IngredientRecipes.Add(new IngredientRecipe() { IngredientId = IngredientId, RecipeId = recipe.RecipeId });
      }
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    public ActionResult Delete(int id)
    {
      var thisIngredient = _db.Ingredients.FirstOrDefault(ingredient => ingredient.IngredientId == id);
      return View(thisIngredient);
    }
    [HttpPost, ActionName("Delete")]
    public ActionResult DeleteConfirmed(int id)
    {
      var thisIngredient = _db.Ingredients.FirstOrDefault(ingredient => ingredient.IngredientId == id);
      _db.Ingredients.Remove(thisIngredient);
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
  }
}