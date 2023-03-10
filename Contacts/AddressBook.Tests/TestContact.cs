using Microsoft.Communications.Contacts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddressBook.Tests
{
    public class TestContact
    {
        public string Name { get; }
        public string Email { get; }
        public string PhoneNumber { get; }
        public string Notes { get; }


        public TestContact(Contact contact)
        {
            this.Name = contact.Names[0].FormattedName;
            this.Email = contact.EmailAddresses.Select(e => e.Address)
                .Where(a => a != string.Empty).First();
            this.PhoneNumber = contact.PhoneNumbers.Select(pn => pn.Number)
                .Where(n => n != string.Empty).First();
            this.Notes = contact.Notes;
        }
    }
}
