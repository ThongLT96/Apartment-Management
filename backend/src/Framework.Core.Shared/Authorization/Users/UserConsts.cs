namespace Framework.Authorization.Users
{
    public class UserConsts
    {
        public const int MaxPhoneNumberLength = 24;
        public const int MaxGenderLength = 25;
        public const int MaxIDNumberLength = 12;
        public const int MinPlainPasswordLength = 6;
        public const int MaxPlainPasswordLength = 20;

        public const string PhoneNumberRegex = @"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$";
    }
}
