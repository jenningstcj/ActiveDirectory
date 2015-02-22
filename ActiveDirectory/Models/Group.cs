using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;

namespace ActiveDirectory.Models
{
    /// <summary>
    /// Object class for Security/Distrbution groups in Active Directory
    /// </summary>
    public class Group
    {
        /// <summary>
        /// Description of group
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Distinguished name declaring name and location of group
        /// </summary>
        public string DistinguishedName { get; set; }
        
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Members of the group
        /// </summary>
        public List<Principal> Members { get; set; }

        /// <summary>
        /// SamAccountName/Username
        /// </summary>
        public string SamAccountName { get; set; }

        /// <summary>
        /// Organization Unit
        /// </summary>
        public string OU { get; set; }
    }
}
