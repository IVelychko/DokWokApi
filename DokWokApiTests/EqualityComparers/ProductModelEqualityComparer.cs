using DokWokApi.BLL.Models;

namespace DokWokApiTests.EqualityComparers;

public class ProductModelEqualityComparer : IEqualityComparer<ProductModel?>
{
    public bool Equals(ProductModel? x, ProductModel? y)
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
            && x.CategoryName == y.CategoryName
            && x.Price == y.Price;
    }

    public int GetHashCode(ProductModel obj)
    {
        return obj.Name.GetHashCode();
    }
}
