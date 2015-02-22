using ActiveDirectory.Models;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;

namespace ActiveDirectory
{
    /// <summary>
    /// Class consisting of actions that can be performed against an Active Directory domain
    /// </summary>
    public class ActiveDirectory
    {
        private string _domainName;

        /// <summary>
        /// Creates an instance connected to a single domain
        /// </summary>
        /// <param name="domain">The name of the domain</param>
        public ActiveDirectory(string domain)
        {
            _domainName = domain;
        }

        /// <summary>
        /// Returns a list of all Security Groups found in the domain
        /// </summary>
        /// <returns></returns>
        public List<Principal> GetAllSecurityGroups()
        {
            try
            {
                List<Principal> Groups = new List<Principal>();
                using (PrincipalContext ctx = new PrincipalContext(ContextType.Domain, _domainName))
                {
                    using (GroupPrincipal gp = new GroupPrincipal(ctx))
                    {
                        using (PrincipalSearcher ps = new PrincipalSearcher(gp))
                        {
                            foreach (var item in ps.FindAll())
                            {
                                Groups.Add(item);
                            }
                        }
                    }
                }
                return Groups;
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Returns the details of a group including members
        /// </summary>
        /// <param name="groupSamAccountName"></param>
        /// <returns></returns>
        public Group GetGroupDetails(string groupSamAccountName)
        {
            try
            {
                using (PrincipalContext ctx = new PrincipalContext(ContextType.Domain, _domainName))
                {
                    using (GroupPrincipal gp = GroupPrincipal.FindByIdentity(ctx, IdentityType.SamAccountName, groupSamAccountName))
                    {
                        Group group = new Group()
                        {
                            Description = gp.Description,
                            DistinguishedName = gp.DistinguishedName,
                            Name = gp.Name,
                            SamAccountName = gp.SamAccountName,
                        };
                        List<Principal> members = new List<Principal>();
                        foreach (var member in gp.Members)
                        {
                            members.Add(member);
                        }
                        group.Members = members;
                        using (var entry = (DirectoryEntry)gp.GetUnderlyingObject())
                        {
                            using (DirectoryEntry deUserContainer = entry.Parent)
                            {
                                group.OU = deUserContainer.Properties["DistinguishedName"].Value.ToString();
                            }
                        }
                        return group;
                    }
                }
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Finds all User objects in the domain by lastname/surname
        /// </summary>
        /// <param name="lastName">The lastname or surname of users that wish to be found</param>
        /// <returns></returns>
        public List<Principal> FindUsersByLastName(string lastName)
        {
            try
            {
                List<Principal> Users = new List<Principal>();
                using (PrincipalContext ctx = new PrincipalContext(ContextType.Domain, _domainName))
                {
                    using (UserPrincipal up = new UserPrincipal(ctx))
                    {
                        up.Surname = lastName;
                        using (PrincipalSearcher ps = new PrincipalSearcher(up))
                        {
                            foreach (var item in ps.FindAll())
                            {
                                Users.Add(item);
                            }
                        }
                    }
                }
                return Users;
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Returns details of a user for a given username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public User GetUserDetails(string username)
        {
            try
            {
                using (PrincipalContext ctx = new PrincipalContext(ContextType.Domain, _domainName))
                {
                    using (UserPrincipal up = UserPrincipal.FindByIdentity(ctx, IdentityType.SamAccountName, username))
                    {
                        User user = new User()
                        {
                            BadLogonCount = up.BadLogonCount,
                            Description = up.Description,
                            DisplayName = up.DisplayName,
                            DistinguishedName = up.DistinguishedName,
                            EmailAddress = up.EmailAddress,
                            EmployeeId = up.EmployeeId,
                            Enabled = up.Enabled,
                            FirstName = up.GivenName,
                            HomeDirectory = up.HomeDirectory,
                            HomeDrive = up.HomeDrive,
                            IsAccountLockedOut = up.IsAccountLockedOut(),
                            LastBadPasswordAttempt = up.LastBadPasswordAttempt,
                            LastLogon = up.LastLogon,
                            LastPasswordSet = up.LastPasswordSet,
                            MiddleName = up.MiddleName,
                            Name = up.Name,
                            SamAccountName = up.SamAccountName,
                            LastName = up.Surname,
                            UserPrincipalName = up.UserPrincipalName,
                            TelephoneNumber = up.VoiceTelephoneNumber
                        };
                        user.Groups = new List<Principal>();
                        foreach (var group in up.GetGroups())
                        {
                            user.Groups.Add(group);
                        }
                        using (var entry = (DirectoryEntry)up.GetUnderlyingObject())
                        {
                            user.OfficeName = entry.Properties["physicalDeliveryOfficeName"].Value != null ? entry.Properties["physicalDeliveryOfficeName"].Value.ToString() : string.Empty;
                            user.CompanyName = entry.Properties["company"].Value != null ? entry.Properties["company"].Value.ToString() : string.Empty;

                            if (string.IsNullOrEmpty(entry.Properties["manager"].Value.ToString()))
                            {
                                user.Manager = GetUserDetails(entry.Properties["manager"].Value.ToString());
                            }

                            user.Department = entry.Properties["department"].Value != null ? entry.Properties["department"].Value.ToString() : string.Empty;
                            using (DirectoryEntry deUserContainer = entry.Parent)
                            {
                                user.OU = deUserContainer.Properties["DistinguishedName"].Value.ToString();
                            }
                        }
                        return user;
                    }
                }
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Create a new user in the domain
        /// </summary>
        /// <param name="user">Properties of the user to be created.  Properties supported include:
        /// FirstName, MiddleName, LastName, DisplayName, SamAccountName(username), Description,
        /// TelephoneNumber, EmployeeId, OfficeName, CompanyName, Manager, Department</param>
        /// <param name="password">Password to set for user account</param>
        /// <returns></returns>
        public bool CreateNewUser(User user, string password)
        {
            try
            {
                using (PrincipalContext ctx = new PrincipalContext(ContextType.Domain, _domainName, user.OU))
                {
                    using (UserPrincipal up = new UserPrincipal(ctx))
                    {
                        up.GivenName = user.FirstName;
                        if (!string.IsNullOrEmpty(user.MiddleName))
                            up.MiddleName = user.MiddleName;
                        up.Surname = user.LastName;
                        up.DisplayName = user.DisplayName;
                        up.Name = user.DisplayName;
                        up.SamAccountName = user.SamAccountName;
                        up.UserPrincipalName = user.SamAccountName + "@ad.sumg.int";
                        up.Description = user.Description;
                        up.SetPassword(password);
                        up.VoiceTelephoneNumber = user.TelephoneNumber;
                        if (!string.IsNullOrEmpty(user.EmployeeId))
                            up.EmployeeId = user.EmployeeId;
                        up.Enabled = true;
                        up.ExpirePasswordNow();
                        up.Save();
                        if (up.GetUnderlyingObjectType() == typeof(DirectoryEntry))
                        {
                            using (var entry = (DirectoryEntry)up.GetUnderlyingObject())
                            {
                                entry.Properties["physicalDeliveryOfficeName"].Value = user.OfficeName;
                                if (!string.IsNullOrEmpty(user.MiddleName))
                                    entry.Properties["initials"].Value = user.MiddleName.ToCharArray()[0];
                                entry.Properties["title"].Value = user.Description;
                                entry.Properties["company"].Value = user.CompanyName;
                                if (user.Manager != null)
                                    entry.Properties["manager"].Value = user.Manager.DistinguishedName;
                                entry.Properties["department"].Value = user.Department;
                                entry.CommitChanges();
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Unlocks a user account by username
        /// </summary>
        /// <param name="username"></param>
        public void UnlockUserAccount(string username)
        {
            try
            {
                using (PrincipalContext ctx = new PrincipalContext(ContextType.Domain, _domainName))
                {
                    using (UserPrincipal up = UserPrincipal.FindByIdentity(ctx, IdentityType.SamAccountName, username))
                    {
                        up.UnlockAccount();
                        up.Save();
                    }
                }
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Changes a user's password
        /// </summary>
        /// <param name="distinguishedName">A user's DistinguishedName may be found with FindUsersByLastName() or GetUserDetails()</param>
        /// <param name="newPassword">New password</param>
        public void ChangeUserPassword(string distinguishedName, string newPassword)
        {
            try
            {
                using (PrincipalContext ctx = new PrincipalContext(ContextType.Domain, _domainName))
                {
                    using (UserPrincipal up = UserPrincipal.FindByIdentity(ctx, IdentityType.DistinguishedName, distinguishedName))
                    {
                        up.SetPassword(newPassword);
                        up.Save();
                    }
                }
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Checks to see if a username already exists within the domain
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public bool CheckUsernameAvailability(string username)
        {
            try
            {
                using (PrincipalContext ctx = new PrincipalContext(ContextType.Domain, _domainName))
                {
                    using (UserPrincipal up = UserPrincipal.FindByIdentity(ctx, IdentityType.SamAccountName, username))
                    {
                        return true;
                    }
                }
            }
            catch (Exception) { return false; }
        }

        /// <summary>
        /// Disables a user account by the distinguished name
        /// </summary>
        /// <param name="distinguishedName">A user's DistinguishedName may be found with FindUsersByLastName() or GetUserDetails()</param>
        /// <returns></returns>
        public bool DisableUserAccount(string distinguishedName)
        {
            try
            {
                using (PrincipalContext ctx = new PrincipalContext(ContextType.Domain, _domainName))
                {
                    using (UserPrincipal up = UserPrincipal.FindByIdentity(ctx, IdentityType.DistinguishedName, distinguishedName))
                    {
                        up.Enabled = false;
                        up.Save();
                        return true;
                    }
                }
            }
            catch (Exception) { return false; }
        }

        /// <summary>
        /// Enables a user account by the distinguished name
        /// </summary>
        /// <param name="distinguishedName">A user's DistinguishedName may be found with FindUsersByLastName() or GetUserDetails()</param>
        /// <returns></returns>
        public bool EnableUserAccount(string distinguishedName)
        {
            try
            {
                using (PrincipalContext ctx = new PrincipalContext(ContextType.Domain, _domainName))
                {
                    using (UserPrincipal up = UserPrincipal.FindByIdentity(ctx, IdentityType.DistinguishedName, distinguishedName))
                    {
                        up.Enabled = true;
                        up.Save();
                        return true;
                    }
                }
            }
            catch (Exception) { return false; }
        }


        /// <summary>
        /// Adds a user to a group
        /// </summary>
        /// <param name="userDistinguishedName">A user's DistinguishedName may be found with FindUsersByLastName() or GetUserDetails()</param>
        /// <param name="groupDistinguishedName">A group's DistinguishedName may be found with GetAllSecurityGroups() or GetGroupDetails()</param>
        /// <returns></returns>
        public bool AddUserToGroup(string userDistinguishedName, string groupDistinguishedName)
        {
            try
            {
                using (PrincipalContext ctx = new PrincipalContext(ContextType.Domain, _domainName))
                {
                    using (GroupPrincipal gp = GroupPrincipal.FindByIdentity(ctx, IdentityType.DistinguishedName, groupDistinguishedName))
                    {
                        gp.Members.Add(ctx, IdentityType.DistinguishedName, userDistinguishedName);
                        gp.Save();
                        return true;
                    }
                }
            }
            catch (Exception) { return false; }
        }

        /// <summary>
        /// Removes a user from a group
        /// </summary>
        /// <param name="userDistinguishedName">A user's DistinguishedName may be found with FindUsersByLastName() or GetUserDetails()</param>
        /// <param name="groupDistinguishedName">A group's DistinguishedName may be found with GetAllSecurityGroups() or GetGroupDetails()</param>
        /// <returns></returns>
        public bool RemoveUserFromGroup(string userDistinguishedName, string groupDistinguishedName)
        {
            try
            {
                using (PrincipalContext ctx = new PrincipalContext(ContextType.Domain, _domainName))
                {
                    using (GroupPrincipal gp = GroupPrincipal.FindByIdentity(ctx, IdentityType.DistinguishedName, groupDistinguishedName))
                    {
                        gp.Members.Remove(ctx, IdentityType.DistinguishedName, userDistinguishedName);
                        gp.Save();
                        return true;
                    }
                }
            }
            catch (Exception) { return false; }
        }


        //TODO: GET LIST OF OU'S

        /// <summary>
        /// Moves a user or group from one OU to another
        /// </summary>
        /// <param name="userDistinguishedName">Users DistinguishedName may be found with FindUsersByLastName or GetUserDetails</param>
        /// <param name="newOU"></param>
        public void MoveUserToNewOU(string userDistinguishedName, string newOU)
        {
            try
            {
                DirectoryEntry CurrentLocation = new DirectoryEntry(@"LDAP://" + userDistinguishedName);
                CurrentLocation.MoveTo(new DirectoryEntry(@"LDAP://" + newOU));
            }
            catch (Exception) { throw; }
        }
    }
}
