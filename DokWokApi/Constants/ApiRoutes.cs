namespace DokWokApi.Constants;

public static class ApiRoutes
{
    private const string Base = "api";

    public static class Orders
    {
        public const string Group = Base + "/orders";
        public const string GetAllOrderLinesByOrderId = "{orderId:long}/order-lines";
        public const string GetById = "{id:long}";
        public const string AddDelivery = "delivery";
        public const string AddTakeaway = "takeaway";
        public const string DeleteById = "{id:long}";
    }

    public static class OrderLines
    {
        public const string Group = Base + "/order-lines";
        public const string GetById = "{id:long}";
        public const string GetByOrderAndProductIds = "{orderId}/{productId}";
        public const string DeleteById = "{id:long}";
    }

    public static class Products
    {
        public const string Group = Base + "/products";
        public const string GetById = "{id:long}";
        public const string DeleteById = "{id:long}";
        public const string IsNameTaken = "name/{name}";
    }

    public static class ProductCategories
    {
        public const string Group = Base + "/categories";
        public const string GetAllProductsByCategoryId = "{categoryId:long}/products";
        public const string GetById = "{id:long}";
        public const string DeleteById = "{id:long}";
        public const string IsNameTaken = "name/{name}";
    }

    public static class Shops
    {
        public const string Group = Base + "/shops";
        public const string GetById = "{id:long}";
        public const string GetByAddress = "{street}/{building}";
        public const string DeleteById = "{id:long}";
        public const string IsAddressTaken = "address/{street}/{building}";
    }

    public static class Users
    {
        public const string Group = Base + "/users";
        // public const string GetAllCustomers = "customers";
        public const string GetAllOrdersByUserId = "{id:long}/orders";
        // public const string GetUserByUserName = "username/{userName}";
        public const string GetUserById = "{id:long}";
        // public const string GetCustomerById = "customers/id/{id}";
        public const string UpdateCustomerPassword = "password";
        public const string UpdateCustomerPasswordAsAdmin = "password/as-admin";
        public const string DeleteUserById = "{id:long}";
        public const string IsCustomerUserNameTaken = "username/{userName}";
        public const string IsCustomerEmailTaken = "email/{email}";
        public const string IsCustomerPhoneNumberTaken = "phone/{phoneNumber}";
    }

    public static class Auth
    {
        public const string Group = Base + "/auth";
        public const string Login = "login";
        public const string RegisterUser = "register";
        public const string RefreshToken = "refresh-token";
        public const string LogOut = "logout";
    }

    public static class Roles
    {
        public const string Group = Base + "/roles";
        public const string GetAllUsersByRoleName = "{roleName}/users";
        public const string GetUserByRoleNameAndUserId = "{roleName}/users/{userId:long}";
    }
}
