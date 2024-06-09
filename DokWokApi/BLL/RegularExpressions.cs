namespace DokWokApi.BLL;

public static class RegularExpressions
{
    public const string FirstName = @"^[A-Za-zÀ-ÖØ-öø-ÿА-Яа-яЁёІіЇїЄєҐґ]+$";

    public const string PhoneNumber = @"^(?:\+38)?(0\d{9})$";

    public const string Address = @"^[a-zA-Zа-яА-ЯёЁіїІЇєЄґҐ0-9.,\-()№\s]+$";

    public const string PaymentType = @"^(cash|card)$";

    public const string Guid = @"^[{(]?[0-9A-Fa-f]{8}[-]?[0-9A-Fa-f]{4}[-]?[0-9A-Fa-f]{4}[-]?[0-9A-Fa-f]{4}[-]?[0-9A-Fa-f]{12}[)}]?$";

    public const string OrderStatus = $@"^({OrderStatuses.BeingProcessed}|{OrderStatuses.Completed}|{OrderStatuses.Cancelled})$";

    public const string RegularString = @"^[a-zA-Zа-яА-ЯёЁіїєґІЇЄҐ0-9\s.,!?""'@#%&*()\-_=+;:/\\[\]`~]*$";

    public const string Password = @"^(?=.*[A-Z])(?=.*\d)[A-Za-z\d_-]{6,}$";

    public const string UserName = @"^[a-zA-Z0-9_-]{5,}$";
}
