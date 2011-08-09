using EyePatch.Core.Models;
using EyePatch.Core.Models.Forms;

namespace EyePatch.Core.Services
{
    public interface IApplicationService
    {
        void Initialise();
        void Install(InstallationForm install);
        bool SignIn(string userName, string password);
        void SignOut();
    }
}