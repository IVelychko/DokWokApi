using DokWokApi.DAL.Entities;

namespace DokWokApiTests.EqualityComparers;

public class ProductEqualityComparer : IEqualityComparer<Product?>
{
    public bool Equals(Product? x, Product? y)
    {
        if (ReferenceEquals(x, y))
        {
            return true;
        }

        if (x is null || y is null)
        {
            return false;
        }

        return x.Id == y.Id
            && x.Name == y.Name
            && x.Description == y.Description
            && x.CategoryId == y.CategoryId
            && x.Price == y.Price;
    }

    public int GetHashCode(Product obj)
    {
        return obj.Name.GetHashCode();
    }
}
