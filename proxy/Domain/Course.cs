using System.Text.Json;
using MongoDB.Bson.Serialization.Attributes;

namespace video_streamming_proxy.Domain;

public enum CourseStatus
{
    Active,
    Inactive,

}
public class Chapter
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    
    public string Thumbnail { get; set; }

    public Media Media { get; set; }

    public string GetThumbnailImage()
    {            
        return $"data:image/png;base64,{Thumbnail}";    
    }
}

public enum Plan
{
    Monthly,
    Quarterly,
    Semianual,
    Yearly,
}

public enum Currency
{
    BRL,
    USD
}

public class Price
{
    public string Id { get; set; }
    public decimal Amount { get; set; }
    public Plan Plan { get; set; }
    public DateTime CreatedAt { get; set; }
    public int Active { get; set; }
    public string CourseId { get; set; }

    public Currency Currency { get; set; } = Currency.BRL;

    public override bool Equals(object? obj)
    {
        if (obj == null)
            return false;

        return ((obj as Price).Plan == Plan && (obj as Price).Amount == Amount);
    }
}
public class Course
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    public DateTime CreatedAt { get; set; }
    public CourseStatus Status { get; set; }
    public string Slug { get; set; }

    public string Thumbnail { get; set; }

    public IEnumerable<Chapter> Chapters { get; set; } = new List<Chapter>();
    
    public decimal Amount { get; set; }
    
    public IEnumerable<Price> Prices { get; set; }

    public Price? GetInitialPrice()
    {
        return Prices.FirstOrDefault();
    }

    public IEnumerable<Price> GetAvailablePrices()
    {
        if (Prices == null)
            return Array.Empty<Price>();
        
        return Prices.Where(p => p.Active == 1).ToList();
    }

    public Price? GetPriceByPlan(Plan plan)
    {
        return Prices.FirstOrDefault(x => x.Plan == plan);
    }

    public string GetThumbnailImage()
    {            
        return $"data:image/png;base64,{Thumbnail}";    
    }

    public void AddPrice(Price price)
    {
        if (Prices == null || !Prices.Any())
            Prices = new[] { price };
        else if (!Prices.Any(x => x.Plan == price.Plan))
        {
            Prices =Prices.Append(price);
        }
        else
        {
            var groupedByPlan = Prices.GroupBy(x => x.Plan);
            var newPrice = groupedByPlan
                .First(x => x.Key == price.Plan)
                .Select(y => new Price
                {
                    Active = 0,
                    CreatedAt = y.CreatedAt,
                    Amount = y.Amount,
                    CourseId = y.CourseId,
                    Id = y.Id,
                    Plan = y.Plan
                }).Append(price);
            
            var values = groupedByPlan.ToDictionary(g => g.Key, g => g.ToList());
            values[price.Plan] = newPrice.ToList();
            Prices = values.SelectMany(x => x.Value);
        }
    }
}