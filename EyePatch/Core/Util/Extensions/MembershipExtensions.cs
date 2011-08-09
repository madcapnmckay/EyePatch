using System.Web.Security;

namespace EyePatch.Core.Util.Extensions
{
    public static class MembershipExtensions
    {
        public static void GetRulesExceptions(this MembershipCreateStatus createStatus)
        {
            /*switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    throw new RulesException("Username", "Username already exists. Please enter a different user name.");

                case MembershipCreateStatus.DuplicateEmail:
                    throw new RulesException("EmailAddress",
                                             "A username for that e-mail address already exists. Please enter a different e-mail address.");

                case MembershipCreateStatus.InvalidPassword:
                    throw new RulesException("Password", "The password provided is invalid. Please enter a valid password value.");

                case MembershipCreateStatus.InvalidEmail:
                    throw new RulesException("EmailAddress", "The e-mail address provided is invalid. Please check the value and try again.");

                case MembershipCreateStatus.InvalidAnswer:
                    throw new RulesException("Answer", "The password retrieval answer provided is invalid. Please check the value and try again.");

                case MembershipCreateStatus.InvalidQuestion:
                    throw new RulesException("Question", "The password retrieval question provided is invalid. Please check the value and try again.");

                case MembershipCreateStatus.InvalidUserName:
                    throw new RulesException("Username", "The user name provided is invalid. Please check the value and try again.");

                case MembershipCreateStatus.ProviderError:
                    throw new RulesException("", "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.");

                case MembershipCreateStatus.UserRejected:
                    throw new RulesException("", "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.");

                default:
                    throw new RulesException("", "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.");
            }*/
        } 
    }
}