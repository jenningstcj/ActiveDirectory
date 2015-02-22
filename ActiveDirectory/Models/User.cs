using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;

namespace ActiveDirectory.Models
{
    /// <summary>
    /// Object class for user accounts in Active Directory
    /// </summary>
    public class User
    {
        /// <summary>
        /// Number of failed login attempts
        /// </summary>
        public int BadLogonCount { get; set; }

        /// <summary>
        /// Description of user - also used for Title
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Display Name
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Distinguished name declaring name and location of the user
        /// </summary>
        public string DistinguishedName { get; set; }

        /// <summary>
        /// Email Address
        /// </summary>
        public string EmailAddress { get; set; }

        /// <summary>
        /// Employee Id
        /// </summary>
        public string EmployeeId { get; set; }

        /// <summary>
        /// Bool declaring whether the account is enabled or not.
        /// </summary>
        public bool? Enabled { get; set; }

        /// <summary>
        /// First Name / Given Name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Path of home directory
        /// </summary>
        public string HomeDirectory { get; set; }

        /// <summary>
        /// Drive letter of home directory
        /// </summary>
        public string HomeDrive { get; set; }

        /// <summary>
        /// Bool declaring whether the account is locked out or not due to a number of failed login attempts
        /// </summary>
        public bool IsAccountLockedOut { get; set; }

        /// <summary>
        /// Date of last failed login attempt
        /// </summary>
        public DateTime? LastBadPasswordAttempt { get; set; }

        /// <summary>
        /// Date of last successful login attempt
        /// </summary>
        public DateTime? LastLogon { get; set; }

        /// <summary>
        /// Date of last password reset
        /// </summary>
        public DateTime? LastPasswordSet { get; set; }

        /// <summary>
        /// Middle Name
        /// </summary>
        public string MiddleName { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// SamAccountName/Username
        /// </summary>
        public string SamAccountName { get; set; }

        /// <summary>
        /// Last Name or Surname
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// UserPrincipalName ie username@domain
        /// </summary>
        public string UserPrincipalName { get; set; }

        /// <summary>
        /// Telephone number
        /// </summary>
        public string TelephoneNumber { get; set; }

        /// <summary>
        /// Name of Office
        /// </summary>
        public string OfficeName { get; set; }

        /// <summary>
        /// Organization Unit
        /// </summary>
        public string OU { get; set; }

        /// <summary>
        /// Groups the user belongs to
        /// </summary>
        public List<Principal> Groups { get; set; }

        /// <summary>
        /// Company name
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// Manager
        /// </summary>
        public User Manager { get; set; }

        /// <summary>
        /// Department
        /// </summary>
        public string Department { get; set; }
    }
}
