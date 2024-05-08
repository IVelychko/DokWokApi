using DokWokApi.DAL.Entities;

namespace DokWokApiTests.EqualityComparers;

public class ProductCategoryEqualityComparer : IEqualityComparer<ProductCategory?>
{
    public bool Equals(ProductCategory? x, ProductCategory? y)
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
            && x.Name == y.Name;
    }

    public int GetHashCode(ProductCategory obj)
    {
        return obj.Name.GetHashCode();
    }
}
