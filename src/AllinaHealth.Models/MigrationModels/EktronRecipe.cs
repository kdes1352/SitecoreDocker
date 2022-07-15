using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using AllinaHealth.Models.Extensions;

namespace AllinaHealth.Models.MigrationModels
{
    public class EktronRecipe : AEktronMigrationModel<EktronRecipe>
    {
        public string RecipesName { get; set; }
        public string RecipesDescription { get; set; }
        public string RecipesIngredients { get; set; }
        public string RecipesDirections { get; set; }
        public List<string> RecipesContains { get; set; }
        public string RecipesServings { get; set; }
        public EktronOptionGallery OptionGallery { get; set; }

        public EktronRecipe()
        {

        }

        public override EktronRecipe FromXmlString(EktronInfo ei)
        {
            var doc = XDocument.Parse(ei.Data);

            var elements = doc.Descendants("Recipes").ToList();

            return new EktronRecipe
            {
                RecipesName = elements[0].Element("RecipesName").ToStringWtihHtml(),
                RecipesDescription = elements[0].Element("RecipesDescription").ToStringWtihHtml(),
                RecipesIngredients = elements[0].Element("RecipesIngredients").ToStringWtihHtml(),
                RecipesDirections = elements[0].Element("RecipesDirections").ToStringWtihHtml(),
                RecipesContains = elements[0].Elements("RecipesContains").Select(x => x.ToStringWtihHtml()).ToList(),
                RecipesServings = elements[0].Element("RecipesServings").ToStringWtihHtml(),
                OptionGallery = new EktronOptionGallery
                {
                    Photo = elements[0]?.Element("OptionGallery")?.Element("Photo")?.Element("img") != null ? elements[0]?.Element("OptionGallery")?.Element("Photo")?.Element("img")?.Attribute("src")?.Value : string.Empty,
                    PhotoCaption = elements[0]?.Element("OptionGallery")?.Element("PhotoCaption").ToStringWtihHtml(),
                    NutritionImages = elements[0]?.Element("OptionGallery")?.Element("NutritionImages").ToStringWtihHtml(),
                    RecipeTip = elements[0]?.Element("OptionGallery")?.Element("RecipeTip").ToStringWtihHtml()
                }
            };

        }
    }

    public class EktronOptionGallery
    {
        public string Photo { get; set; }
        public string PhotoCaption { get; set; }
        public string NutritionImages { get; set; }
        public string RecipeTip { get; set; }
    }
}