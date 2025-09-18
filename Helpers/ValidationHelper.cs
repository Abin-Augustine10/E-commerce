using System.Text.RegularExpressions;

namespace ShopZone.Helpers
{
    public static class ValidationHelper
    {
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                return emailRegex.IsMatch(email);
            }
            catch
            {
                return false;
            }
        }

        public static bool IsValidPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;

            var phoneRegex = new Regex(@"^\+?[1-9]\d{1,14}$");
            return phoneRegex.IsMatch(phone.Replace(" ", "").Replace("-", ""));
        }

        public static bool IsEmail(string input)
        {
            return input?.Contains("@") == true;
        }

        public static bool IsValidRole(string role)
        {
            var validRoles = new[] { "Buyer", "Seller", "DeliveryPartner" };
            return validRoles.Contains(role);
        }

        public static bool IsValidOrderStatus(string status)
        {
            var validStatuses = new[] { "Pending", "Paid", "Assigned", "OutForDelivery", "Delivered", "Cancelled" };
            return validStatuses.Contains(status);
        }
    }
}
