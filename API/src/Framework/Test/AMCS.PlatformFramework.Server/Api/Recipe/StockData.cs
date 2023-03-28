using AMCS.PlatformFramework.Entity.Api.Recipe;
using AMCS.PlatformFramework.Entity.Api.Settings;
using System.Collections.Generic;

namespace AMCS.PlatformFramework.Server.Api.Recipe
{
  internal static class StockData
  {
    public static Dictionary<string, int> GetCourses()
    {
      var result = new Dictionary<string, int>();
      result.Add("Starter", (int)Course.STARTER);
      result.Add("Main", (int)Course.MAIN);
      result.Add("Dessert", (int)Course.DESSERT);
      result.Add("FAKE", (int)Course.FAKE);
      return result;
    }

    public static Dictionary<string, int> GetDifficultyLevels()
    {
      var result = new Dictionary<string, int>();
      result.Add("Easy", (int)DifficultyLevel.EASY);
      result.Add("Medium", (int)DifficultyLevel.MEDIUM);
      result.Add("Hard", (int)DifficultyLevel.HARD);
      return result;
    }

    public static Dictionary<string, int> GetSpiceGoods()
    {
      var result = new Dictionary<string, int>();

      result.Add("Coarse salt", (int)Measurement.TEASPOON);
      result.Add("Red chili flakes", (int)Measurement.TEASPOON);
      result.Add("Black peppercorns", (int)Measurement.TEASPOON);
      result.Add("Coriander", (int)Measurement.TEASPOON);
      result.Add("Fennel seeds", (int)Measurement.TEASPOON);
      result.Add("Paprika", (int)Measurement.TEASPOON);
      result.Add("Oregano", (int)Measurement.TEASPOON);
      result.Add("Turmeric", (int)Measurement.TEASPOON);
      result.Add("Whole nutmeg", (int)Measurement.EACH);
      result.Add("Bay leaves", (int)Measurement.EACH);
      result.Add("Cayenne pepper", (int)Measurement.TEASPOON);
      result.Add("Thyme", (int)Measurement.HANDFULL);
      result.Add("Cinnamon", (int)Measurement.TEASPOON);
      result.Add("Ground Pepper", (int)Measurement.TEASPOON);
      return result;
    }

    public static Dictionary<string, int> GetPantryGoods()
    {
      var result = new Dictionary<string, int>();

      result.Add("Bread crumbs", (int)Measurement.HANDFULL);
      result.Add("Pasta", (int)Measurement.GRAMS);
      result.Add("Couscous", (int)Measurement.GRAMS);
      result.Add("Rice", (int)Measurement.GRAMS);
      result.Add("All-purpose flour", (int)Measurement.GRAMS);
      result.Add("White sugar", (int)Measurement.GRAMS);
      result.Add("Brown sugar", (int)Measurement.GRAMS);
      result.Add("Powdered sugar", (int)Measurement.POUND);
      result.Add("Baking powder", (int)Measurement.GRAMS);
      result.Add("Active dry yeast", (int)Measurement.TEASPOON);
      result.Add("Chicken stock", (int)Measurement.LITER);
      result.Add("Beef stock", (int)Measurement.LITER);
      return result;
    }

    public static Dictionary<string, int> GetRefrigeratorGoods()
    {
      var result = new Dictionary<string, int>();
      result.Add("Milk", (int)Measurement.CUP);
      result.Add("Butter", (int)Measurement.GRAMS);
      result.Add("Double cream", (int)Measurement.CUP);
      result.Add("Eggs", (int)Measurement.EACH);
      result.Add("Parmesan", (int)Measurement.HANDFULL);
      result.Add("Bacon", (int)Measurement.GRAMS);
      result.Add("Parsley", (int)Measurement.TEASPOON);
      result.Add("Celery", (int)Measurement.EACH);
      result.Add("Carrots", (int)Measurement.EACH);
      result.Add("Lemons", (int)Measurement.EACH);
      result.Add("Limes", (int)Measurement.EACH);
      result.Add("Orange juice", (int)Measurement.CUP);
      result.Add("Apple juice", (int)Measurement.CUP);
      result.Add("Chicken Breasts", (int)Measurement.GRAMS);
      return result;
    }

    public static Dictionary<string, int> GetCondimentGoods()
    {
      var result = new Dictionary<string, int>();
      result.Add("Ketchup", (int)Measurement.TABLESPOON);
      result.Add("Mayonnaise", (int)Measurement.TABLESPOON);
      result.Add("Extra virgin olive oil", (int)Measurement.TABLESPOON);
      result.Add("Vegetable oil", (int)Measurement.TABLESPOON);
      result.Add("Olive oil", (int)Measurement.TABLESPOON);
      result.Add("Vinegar", (int)Measurement.TEASPOON);
      result.Add("Mustard", (int)Measurement.TEASPOON);
      result.Add("Honey", (int)Measurement.TEASPOON);
      result.Add("Brown Sauce", (int)Measurement.TABLESPOON);
      result.Add("Horse radish", (int)Measurement.TABLESPOON);
      result.Add("Tartar Sauce", (int)Measurement.TABLESPOON);
      return result;
    }

    public static Dictionary<string, int> GetVegitableGoods()
    {
      var result = new Dictionary<string, int>();
      
      result.Add("Garlic", (int)Measurement.EACH);
      result.Add("Shallots", (int)Measurement.EACH);
      result.Add("Potatoes", (int)Measurement.EACH);
      result.Add("Onions", (int)Measurement.EACH);
      result.Add("Tomatoes", (int)Measurement.EACH);
      result.Add("Carrots", (int)Measurement.EACH);
      result.Add("Celery", (int)Measurement.EACH);
      result.Add("Broccoli", (int)Measurement.EACH);
      result.Add("Cauliflower", (int)Measurement.EACH);
      return result;
    }

    public static Dictionary<string, int> GetCannedGoods()
    {
      var result = new Dictionary<string, int>();

      result.Add("Tomatoes — diced", (int)Measurement.GRAMS);
      result.Add("Tomato sauce", (int)Measurement.GRAMS);
      result.Add("Tomato paste", (int)Measurement.GRAMS);
      result.Add("Tomatoes — crushed", (int)Measurement.GRAMS);
      result.Add("Beans", (int)Measurement.GRAMS);
      result.Add("Peaches - in syrup", (int)Measurement.GRAMS);
      result.Add("Peaches - in water", (int)Measurement.GRAMS); 
      result.Add("Mixed Vegetables", (int)Measurement.GRAMS);
      result.Add("Gravy", (int)Measurement.TABLESPOON);
      return result;
    }

    public static IList<ApiIngredientType> GetIngredientTypes()
    {
      IList<ApiIngredientType> ingredientTypes = new List<ApiIngredientType>()
      {
          new ApiIngredientType()
        {
          ApiIngredientTypeId = (int)IngredientType.CANNEDGOODS,
          Description = IngredientType.CANNEDGOODS.ToString()
        },
          new ApiIngredientType()
        {
          ApiIngredientTypeId = (int)IngredientType.CONDIMENTS,
          Description = IngredientType.CONDIMENTS.ToString()
        },
          new ApiIngredientType()
        {
          ApiIngredientTypeId = (int)IngredientType.PANTRY,
          Description = IngredientType.PANTRY.ToString()
        },
          new ApiIngredientType()
        {
          ApiIngredientTypeId = (int)IngredientType.REFRIGERATOR,
          Description = IngredientType.REFRIGERATOR.ToString()
        },
          new ApiIngredientType()
        {
          ApiIngredientTypeId = (int)IngredientType.SPICES,
          Description = IngredientType.SPICES.ToString()
        },
          new ApiIngredientType()
        {
          ApiIngredientTypeId = (int)IngredientType.VEGETABLES,
          Description = IngredientType.VEGETABLES.ToString()
        }
      };

      return ingredientTypes;
    }

    public static IList<ApiMeasurement> GetMeasurements()
    {
      IList<ApiMeasurement> measurements = new List<ApiMeasurement>() 
      {
        new ApiMeasurement()
        {
          ApiMeasurementId = (int)Measurement.CUP,
          Description = Measurement.CUP.ToString()
        },
        new ApiMeasurement()
        {
          ApiMeasurementId = (int)Measurement.EACH,
          Description = Measurement.EACH.ToString()
        },
        new ApiMeasurement()
        {
          ApiMeasurementId = (int)Measurement.GRAMS,
          Description = Measurement.GRAMS.ToString()
        },
        new ApiMeasurement()
        {
          ApiMeasurementId = (int)Measurement.HANDFULL,
          Description = Measurement.HANDFULL.ToString()
        },
        new ApiMeasurement()
        {
          ApiMeasurementId = (int)Measurement.LITER,
          Description = Measurement.LITER.ToString()
        },
        new ApiMeasurement()
        {
          ApiMeasurementId = (int)Measurement.POUND,
          Description = Measurement.POUND.ToString()
        },
        new ApiMeasurement()
        {
          ApiMeasurementId = (int)Measurement.TABLESPOON,
          Description = Measurement.TABLESPOON.ToString()
        },
        new ApiMeasurement()
        {
          ApiMeasurementId = (int)Measurement.TEASPOON,
          Description = Measurement.TEASPOON.ToString()
        }
      };

      return measurements;
    }

    public static ApiRecipe GetOvenBakedChickenBites()
    {
      var ingredients = new List<ApiIngredient>();

      ingredients.Add(new ApiIngredient()
      {
        IngredientId = 1,
        Name = "Chicken Breasts",
        Amount = 1,
        Measurement = Measurement.POUND.ToString(),
        MeasurementId = (int)Measurement.POUND,
        Type = IngredientType.REFRIGERATOR.ToString(),
        TypeId = (int)IngredientType.REFRIGERATOR
      });

      ingredients.Add(new ApiIngredient()
      {
        IngredientId = 2,
        Name = "Olive oil",
        Amount = 1,
        Measurement = Measurement.TABLESPOON.ToString(),
        MeasurementId = (int)Measurement.TABLESPOON,
        Type = IngredientType.CONDIMENTS.ToString(),
        TypeId = (int)IngredientType.CONDIMENTS
      });

      ingredients.Add(new ApiIngredient()
      {
        IngredientId = 3,
        Name = "Salt",
        Amount = 1,
        Measurement = Measurement.TEASPOON.ToString(),
        MeasurementId = (int)Measurement.TEASPOON,
        Type = IngredientType.CONDIMENTS.ToString(),
        TypeId = (int)IngredientType.CONDIMENTS
      });

      ingredients.Add(new ApiIngredient()
      {
        IngredientId = 4,
        Name = "Ground Pepper",
        Amount = 0.25,
        Measurement = Measurement.TEASPOON.ToString(),
        MeasurementId = (int)Measurement.TEASPOON,
        Type = IngredientType.CONDIMENTS.ToString(),
        TypeId = (int)IngredientType.CONDIMENTS
      });

      ingredients.Add(new ApiIngredient()
      {
        IngredientId = 5,
        Name = "Paprika",
        Amount = 0.5,
        Measurement = Measurement.TEASPOON.ToString(),
        MeasurementId = (int)Measurement.TEASPOON,
        Type = IngredientType.CONDIMENTS.ToString(),
        TypeId = (int)IngredientType.CONDIMENTS
      });

      ingredients.Add(new ApiIngredient()
      {
        IngredientId = 6,
        Name = "Cayenne Pepper",
        Amount = 0.125,
        Measurement = Measurement.TEASPOON.ToString(),
        MeasurementId = (int)Measurement.TEASPOON,
        Type = IngredientType.CONDIMENTS.ToString(),
        TypeId = (int)IngredientType.CONDIMENTS
      });

      ingredients.Add(new ApiIngredient()
      {
        IngredientId = 7,
        Name = "Garlic Powder",
        Amount = 0.5,
        Measurement = Measurement.TEASPOON.ToString(),
        MeasurementId = (int)Measurement.TEASPOON,
        Type = IngredientType.CONDIMENTS.ToString(),
        TypeId = (int)IngredientType.CONDIMENTS
      });

      ingredients.Add(new ApiIngredient()
      {
        IngredientId = 8,
        Name = "Onion Powder",
        Amount = 0.5,
        Measurement = Measurement.TEASPOON.ToString(),
        MeasurementId = (int)Measurement.TEASPOON,
        Type = IngredientType.CONDIMENTS.ToString(),
        TypeId = (int)IngredientType.CONDIMENTS
      });

      ingredients.Add(new ApiIngredient()
      {
        IngredientId = 9,
        Name = "Chicken Stock",
        Amount = 1,
        Measurement = Measurement.TEASPOON.ToString(),
        MeasurementId = (int)Measurement.TEASPOON,
        Type = IngredientType.CONDIMENTS.ToString(),
        TypeId = (int)IngredientType.CONDIMENTS,
        Optional = true
      });

      ingredients.Add(new ApiIngredient()
      {
        IngredientId = 10,
        Name = "Italian Seasoning",
        Amount = 1,
        Measurement = Measurement.TEASPOON.ToString(),
        MeasurementId = (int)Measurement.TEASPOON,
        Type = IngredientType.CONDIMENTS.ToString(),
        TypeId = (int)IngredientType.CONDIMENTS
      });

      ingredients.Add(new ApiIngredient()
      {
        IngredientId = 11,
        Name = "Parsley",
        Amount = 1,
        Measurement = Measurement.EACH.ToString(),
        MeasurementId = (int)Measurement.EACH,
        Type = IngredientType.VEGETABLES.ToString(),
        TypeId = (int)IngredientType.VEGETABLES
      });


      return new ApiRecipe()
      {
        RecipeId = 1,
        Name = "Oven baked chicken bites",
        DinnerCourseId = (int?)Course.MAIN,
        Method = @"1. To make the oven-baked chicken bites: Preheat your oven to 420°F  (220ºC) conventional. Cut chicken into 1-inch cubes.
    2. In a small bowl, combine all seasonings. Arrange chicken in a baking dish and sprinkle with the seasoning mix. Drizzle a tablespoon olive oil and toss chicken cubes with the seasoning to coat on all sides. Speck seasoned chicken with butter cubes.
    3. Bake seasoned chicken in the preheated oven for 16-18 minutes, or until internal temperature reaches 165°F (75°C) using a meat thermometer. After 8-10 minutes, stir the chicken bites with a spoon for even baking. Once done, the chicken bites should be golden with crisp edges. For crispier chicken bites, switch to broil on high for the last 2 minutes of cooking until golden.
    4. Remove the baking dish from the oven, sprinkle chicken with fresh chopped parsley and let rest for 5 minutes before serving. Serve the oven-baked chicken bites over cauliflower rice with a good drizzle of the pan juices and a slice of lemon. Enjoy!",
        Ingredients = ingredients
      };
    }

    public static ApiRecipe GetCarrotCorianderSoup()
    {
      var ingredients = new List<ApiIngredient>();

      ingredients.Add(new ApiIngredient()
      {
        IngredientId = 12,
        Name = "Vegetable oil",
        Amount = 1,
        Measurement = Measurement.TABLESPOON.ToString(),
        MeasurementId = (int)Measurement.TABLESPOON,
        Type = IngredientType.CONDIMENTS.ToString(),
        TypeId = (int)IngredientType.CONDIMENTS
      });

      ingredients.Add(new ApiIngredient()
      {
        IngredientId = 3,
        Name = "Onion",
        Amount = 1,
        Measurement = Measurement.EACH.ToString(),
        MeasurementId = (int)Measurement.EACH,
        Type = IngredientType.VEGETABLES.ToString(),
        TypeId = (int)IngredientType.VEGETABLES
      });

      ingredients.Add(new ApiIngredient()
      {
        IngredientId = 14,
        Name = "Ground Coriander",
        Amount = 1,
        Measurement = Measurement.TEASPOON.ToString(),
        MeasurementId = (int)Measurement.TEASPOON,
        Type = IngredientType.CONDIMENTS.ToString(),
        TypeId = (int)IngredientType.CONDIMENTS
      });

      ingredients.Add(new ApiIngredient()
      {
        IngredientId = 15,
        Name = "Potato",
        Amount = 1,
        Measurement = Measurement.EACH.ToString(),
        MeasurementId = (int)Measurement.EACH,
        Type = IngredientType.VEGETABLES.ToString(),
        TypeId = (int)IngredientType.VEGETABLES
      });

      ingredients.Add(new ApiIngredient()
      {
        IngredientId = 16,
        Name = "Carrots",
        Amount = 450,
        Measurement = Measurement.GRAMS.ToString(),
        MeasurementId = (int)Measurement.GRAMS,
        Type = IngredientType.VEGETABLES.ToString(),
        TypeId = (int)IngredientType.VEGETABLES
      });

      ingredients.Add(new ApiIngredient()
      {
        IngredientId = 17,
        Name = "Vegetable or chicken stock",
        Amount = 1.2,
        Measurement = Measurement.LITER.ToString(),
        MeasurementId = (int)Measurement.LITER,
        Type = IngredientType.PANTRY.ToString(),
        TypeId = (int)IngredientType.PANTRY
      });

      ingredients.Add(new ApiIngredient()
      {
        IngredientId = 18,
        Name = "Coriander",
        Amount = 1,
        Measurement = Measurement.HANDFULL.ToString(),
        MeasurementId = (int)Measurement.HANDFULL,
        Type = IngredientType.PANTRY.ToString(),
        TypeId = (int)IngredientType.PANTRY
      });


      return new ApiRecipe()
      {
        RecipeId = 2,
        Name = "Carrot & coriander soup",
        DinnerCourseId = (int?)Course.MAIN,
        Method = @"1. Heat 1 tbsp vegetable oil in a large pan, add 1 chopped onion, then fry for 5 mins until softened.
    2. Stir in 1 tsp ground coriander and 1 chopped potato, then cook for 1 min.
    3. Add the 450g peeled and chopped carrots and 1.2l vegetable or chicken stock, bring to the boil, then reduce the heat
    4. Cover and cook for 20 mins until the carrots are tender.
    5. Tip into a food processor with a handful of coriander then blitz until smooth (you may need to do this in two batches). Return to pan, taste, add salt if necessary, then reheat to serve.",
        Ingredients = ingredients
      };
    }

    public static ApiRecipe GetBoiledEgg()
    {
      return new ApiRecipe()
      {
        RecipeId = 3,
        Name = "Boiled Egg",
        DinnerCourseId = (int?)Course.DESSERT,
        Method = @"1. Bring a pan of water to the boil.
                   2. Add egg.
                   3. Boil for 5 minutes.",
        Ingredients = new List<ApiIngredient>()
        {
          new ApiIngredient()
          {
            Name = "Egg",
            Amount = 1,
            Type = IngredientType.REFRIGERATOR.ToString(),
            TypeId = (int)IngredientType.REFRIGERATOR
          }
        }
      };
    }

    public static ApiRecipe GetBeansOnToast()
    {
      return new ApiRecipe()
      {
        RecipeId = 4,
        Name = "Beans on Toast",
        DinnerCourseId = (int?)Course.STARTER,
        Method = @"1. Heat up beans in microwave.
                   2. Make toast in toaster.
                   3. Serve.",
        Ingredients = new List<ApiIngredient>()
        {
          new ApiIngredient()
          {
            Name = "Bread",
            Amount = 1,
            Type = IngredientType.PANTRY.ToString(),
            TypeId = (int)IngredientType.PANTRY
          },
          new ApiIngredient()
          {
            Name = "Beans",
            Amount = 1,
            Type = IngredientType.PANTRY.ToString(),
            TypeId = (int)IngredientType.PANTRY
          }
        }
      };
    }

    public static ApiRecipe GetPorridge()
    {
      return new ApiRecipe()
      {
        RecipeId = 5,
        Name = "Porridge",
        DinnerCourseId = (int?)Course.STARTER,
        Method = @"1. Add oats to bowl.
                   2. Add water to bowl.
                   3. Microwave for 2 minutes.",
        Ingredients = new List<ApiIngredient>()
        {
          new ApiIngredient()
          {
            Name = "Porridge Oats",
            Amount = 0.5,
            Measurement = Measurement.CUP.ToString(),
            MeasurementId = (int)Measurement.CUP,
            Type = IngredientType.REFRIGERATOR.ToString(),
            TypeId = (int)IngredientType.REFRIGERATOR
          }
        }
      };
    }

    public static ApiRecipe GetCheeseSandwich()
    {
      return new ApiRecipe()
      {
        RecipeId = 6,
        Name = "Cheese Sandwich",
        DinnerCourseId = (int?)Course.STARTER,
        Method = @"1. Slice cheese.
                   2. Spread butter on bread.
                   3. Add cheese.",
        Ingredients = new List<ApiIngredient>()
        {
          new ApiIngredient()
          {
            Name = "Cheese",
            Amount = 25,
            Measurement = Measurement.GRAMS.ToString(),
            MeasurementId = (int)Measurement.GRAMS,
            Type = IngredientType.REFRIGERATOR.ToString(),
            TypeId = (int)IngredientType.REFRIGERATOR
          }
        }
      };
    }

    public static List<ApiMapExample> MapExamples { get; set; }

    public static List<ApiExampleSettingBrowser> ExampleSettingsBrowsers { get; set; }

    public static List<ApiExampleSetting> ExampleSettings { get; set; }

    public static List<ApiExampleSettingLookup> ExampleSettingLookups { get; set; }
  }
}
