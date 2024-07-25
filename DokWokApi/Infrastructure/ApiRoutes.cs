namespace DokWokApi.Infrastructure
{
    public static class ApiRoutes
    {
        private const string Base = "api";

        public static class Cart
        {
            public const string Group = Base + "/cart";
            public const string AddProduct = "item";
            public const string RemoveProduct = "item";
            public const string RemoveLine = "line";
        }

        public static class Orders
        {
            public const string Group = Base + "/orders";
            public const string GetById = "{id}";
            public const string AddDelivery = "delivery";
            public const string AddTakeaway = "takeaway";
            public const string DeleteById = "{id}";
        }

        public static class OrderLines
        {
            public const string Group = Orders.Group + "/lines";
            public const string GetById = "{id}";
            public const string GetByOrderAndProductIds = "{orderId}/{productId}";
            public const string DeleteById = "{id}";
        }

        public static class Products
        {
            public const string Group = Base + "/products";
            public const string GetById = "{id}";
            public const string DeleteById = "{id}";
            public const string IsNameTaken = "isNameTaken/{name}";
        }

        public static class ProductCategories
        {
            public const string Group = Products.Group + "/categories";
            public const string GetById = "{id}";
            public const string DeleteById = "{id}";
            public const string IsNameTaken = "isNameTaken/{name}";
        }

        public static class Shops
        {
            public const string Group = Base + "/shops";
            public const string GetById = "{id}";
            public const string GetByAddress = "{street}/{building}";
            public const string DeleteById = "{id}";
            public const string IsAddressTaken = "isAddressTaken/{street}/{building}";
        }

        public static class Users
        {
            public const string Group = Base + "/users";
            public const string GetAllCustomers = "customers";
            public const string GetUserByUserName = "username/{userName}";
            public const string GetUserById = "id/{id}";
            public const string GetCustomerById = "customers/id/{id}";
            public const string UpdateCustomerPassword = "password";
            public const string UpdateCustomerPasswordAsAdmin = "password/asAdmin";
            public const string DeleteUserById = "{id}";
            public const string LoginCustomer = "authorization/customers/login";
            public const string LoginAdmin = "authorization/admins/login";
            public const string RegisterUser = "authorization/register";
            public const string RefreshToken = "authorization/refreshToken";
            public const string IsCustomerTokenValid = "customers/isTokenValid";
            public const string IsAdminTokenValid = "admins/isTokenValid";
            public const string GetUserRoles = "roles";
            public const string IsCustomerUserNameTaken = "customers/isUserNameTaken/{userName}";
            public const string IsCustomerEmailTaken = "customers/isEmailTaken/{email}";
            public const string IsCustomerPhoneNumberTaken = "customers/isPhoneTaken/{phoneNumber}";
        }
    }
}
