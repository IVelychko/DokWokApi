using DokWokApi.BLL.Models;

namespace DokWokApiTests.EqualityComparers;

public class ProductCategoryModelEqualityComparer : IEqualityComparer<ProductCategoryModel?>
{
    public bool Equals(ProductCategoryModel? x, ProductCategoryModel? y)
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

    public int GetHashCode(ProductCategoryModel obj)
    {
        return obj.Name.GetHashCode();
    }
}
