using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveDirectory
{
    interface IADFunctions
    {
        List<GroupPrincipal> GetAllSecurityGroups(string domain);
        List<UserPrincipal> FindUsersByLastName(string domain, string lastName);
        bool CreateNewUser(UserPrincipal user);
        bool UnlockUserAccount(string distinguishedName);
        bool CheckUsernameAvailability(string username);

        bool DisableUserAccount(string distinguishedName);
        bool EnableUserAccount(string distinguishedName);

        bool AddUserToGroup(string userDistinguishedName, string groupDistinguishedName);
        bool RemoveUserFromGroup(string userDistinguishedName, string groupDistinguishedName);

        bool MoveUserToNewOU(string userDistinguishedName, string OU);

    }
}
