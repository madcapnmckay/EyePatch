using System.Web.Security;

namespace EyePatch.Core.Services
{
    public interface IMembershipService
    {
        int MinPasswordLength { get; }

        bool ValidateUser(string userName, string password);
        MembershipCreateStatus CreateUser(string userName, string password, string email);
        bool ChangePassword(string userName, string oldPassword, string newPassword);
        bool CreateRole(string roleName);
        bool AddUserToRole(string roleName, string userName);
    }
}