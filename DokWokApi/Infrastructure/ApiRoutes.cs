namespace DokWokApi.Infrastructure
{
    public static class ApiRoutes
    {
        private const string Base = "api";

        public static class Cart
        {
            public const string Controller = Base + "/cart";
            public const string AddProduct = "item";
            public const string RemoveProduct = "item";
            public const string RemoveLine = "line";
        }

        public static class Orders
        {
            public const string Controller = Base + "/orders";
            public const string GetById = "{id}";
            public const string AddDelivery = "delivery";
            public const string AddTakeaway = "takeaway";
            public const string DeleteById = "{id}";
        }

        public static class OrderLines
        {
            private const string OrderLineBase = "lines";
            public const string GetAll = OrderLineBase;
            public const string GetById = OrderLineBase + "/{id}";
            public const string GetByOrderAndProductIds = OrderLineBase + "/{orderId}/{productId}";
            public const string Add = OrderLineBase;
            public const string Update = OrderLineBase;
            public const string DeleteById = OrderLineBase + "/{id}";
        }

        public static class Products
        {
            public const string Controller = Base + "/products";
            public const string GetById = "{id}";
            public const string DeleteById = "{id}";
            public const string IsNameTaken = "isNameTaken/{name}";
        }

        public static class ProductCategories
        {
            private const string ProductCategoryBase = "categories";
            public const string GetAll = ProductCategoryBase;
            public const string GetById = ProductCategoryBase + "/{id}";
            public const string Add = ProductCategoryBase;
            public const string Update = ProductCategoryBase;
            public const string DeleteById = ProductCategoryBase + "/{id}";
            public const string IsNameTaken = ProductCategoryBase + "/isNameTaken/{name}";
        }

        public static class Shops
        {
            public const string Controller = Base + "/shops";
            public const string GetById = "{id}";
            public const string GetByAddress = "{street}/{building}";
            public const string DeleteById = "{id}";
            public const string IsAddressTaken = "isAddressTaken/{street}/{building}";
        }

        public static class Users
        {
            public const string Controller = Base + "/users";
            public const string GetAllCustomers = "customers";
            public const string GetUserByUserName = "username/{userName}";
            public const string GetUserById = "id/{id}";
            public const string GetCustomerById = "customers/id/{id}";
            public const string UpdateCustomerPassword = "password";
            public const string UpdateCustomerPasswordAsAdmin = "password/asAdmin";
            public const string DeleteUserById = "{id}";
            public const string LoginCustomer = "customers/login";
            public const string LoginAdmin = "admins/login";
            public const string RegisterUser = "register";
            public const string RefreshToken = "refreshToken";
            public const string IsCustomerTokenValid = "customers/isTokenValid";
            public const string IsAdminTokenValid = "admins/isTokenValid";
            public const string GetUserRoles = "roles";
            public const string IsCustomerUserNameTaken = "customers/isUserNameTaken/{userName}";
            public const string IsCustomerEmailTaken = "customers/isEmailTaken/{email}";
            public const string IsCustomerPhoneNumberTaken = "customers/isPhoneTaken/{phoneNumber}";
        }
    }
}
