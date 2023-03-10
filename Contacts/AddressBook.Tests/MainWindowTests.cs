using AddressBook.Tests.Windows;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.UIA3;
using FluentAssertions;
using Microsoft.Communications.Contacts;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace AddressBook.Tests
{
    [Apartment(ApartmentState.STA)]
    public class MainWindowTests
    {
        ContactManager contactManager;
        private AutomationBase automation;
        private Application app;


        [SetUp]
        public void Setup()
        {
            contactManager = new ContactManager();
            ClearContacts();

            automation = new UIA3Automation();
            app = FlaUI.Core.Application
                .Launch(@"..\..\..\..\AddressBook\AddressBook.exe");
        }

        [TearDown]
        public void Clear()
        {
            app?.Close();
            automation?.Dispose();

            ClearContacts();
        }

        [Test]
        public void ReadContactList()
        {
            var contacts = GenerateContacts(10).ToList();
            var mainWindow = app.GetMainWindow(automation).As<MainWindow>();

            var contactNames = mainWindow.Contacts.Select(c => c.ContactName);
            var expectedNames = contacts.Select(c => c.Names.First().FormattedName);

            contactNames.Should().BeEquivalentTo(expectedNames);
        }

        [Test]
        public void OpenFirstContact()
        {
            var contacts = GenerateContacts(10).ToList();
            var mainWindow = app.GetMainWindow(automation).As<MainWindow>();

            var contactWindow = mainWindow.Contacts.First().OpenContactWindow();
            contactWindow.Close();
        }
        [Test]
        public void AddNewContact()
        {
            var contact = new TestContact(GenerateContact());
            var mainWindow = app.GetMainWindow(automation).As<MainWindow>();

            var newContactWindow = mainWindow.OpenWindow("_newContactButton");

            newContactWindow.NameAndEmailTabField.FormattedName.Text = contact.Name;
            newContactWindow.NameAndEmailTabField.Email.Text = contact.Email;
            newContactWindow.PhoneTabField.Click();
            newContactWindow.PhoneTabField.OtherCell.Text = contact.PhoneNumber;
            newContactWindow.NotesTabField.Click();
            newContactWindow.NotesTabField.Notes.Text = contact.Notes;
            newContactWindow.SaveChangesButton.Click();

            var addedContact = new TestContact(contactManager.GetContactCollection().First());

            addedContact.Should().BeEquivalentTo(contact);

            mainWindow.Close();
        }
        [Test]
        public void FindContact()
        {
            var contacts = GenerateContacts(20).ToList();
            var mainWindow = app.GetMainWindow(automation).As<MainWindow>();

            var contactWindow = mainWindow.Contacts.First().OpenContactWindow();
            var searchString = contacts.First().Names.First().FormattedName[0].ToString();
            mainWindow.Search.Text = searchString;
            Thread.Sleep(1000);
            mainWindow.Contacts.Where(c => c.IsOffscreen == false).Count()
                .Should().Be(contacts.Select(c => c.Names.First())
                .Select(n => n.FormattedName)
                .Where(fn => fn.StartsWith(searchString)).Count());
        }
        [Test]
        public void EditContact()
        {
            GenerateContacts(1).ToList();
            var contact = new TestContact(GenerateContact());
            var mainWindow = app.GetMainWindow(automation).As<MainWindow>();

            var contactWindow = mainWindow.Contacts.First().OpenContactWindow();
            contactWindow.NameAndEmailTabField.FormattedName.Text = contact.Name;
            contactWindow.NameAndEmailTabField.Email.Text = contact.Email;
            contactWindow.PhoneTabField.Click();
            contactWindow.PhoneTabField.OtherCell.Text = contact.PhoneNumber;
            contactWindow.NotesTabField.Click();
            contactWindow.NotesTabField.Notes.Text = contact.Notes;
            contactWindow.SaveChangesButton.Click();

            var editedContact = new TestContact(contactManager.GetContactCollection().First());

            editedContact.Should().BeEquivalentTo(contact);

            mainWindow.Close();
        }
        [Test]
        public void DeleteContact()
        {
            GenerateContacts(1).ToList();
            var mainWindow = app.GetMainWindow(automation).As<MainWindow>();
            mainWindow.Contacts.First().Click();
            mainWindow.OpenWindow("_deleteContactButton");
            mainWindow.Contacts.Should().BeEmpty();
        }

        private void ClearContacts()
        {
            var contacts = contactManager.GetContactCollection();
            foreach (var contact in contacts)
            {
                contactManager.Remove(contact.Id);
            }
        }
        private Contact GenerateContact()
        {
            var user = new Bogus.Person();

            var contact = new Contact();
            contact.Names.Add(new Name(user.FirstName, "", user.LastName,
                NameCatenationOrder.GivenFamily));
            contact.EmailAddresses.Add(user.Email);
            contact.PhoneNumbers.Add(new PhoneNumber(user.Phone));
            contact.Notes = user.Company.ToString() + " " + user.Website.ToString();

            return contact;
        }

        private IEnumerable<Contact> GenerateContacts(int count)
        {
            for (uint i = 0; i < count; i++)
            {
                var person = new Bogus.Person();

                var contact = new Contact();
                contact.Names.Add(new Name(person.FirstName, "", person.LastName,
                    NameCatenationOrder.GivenFamily));

                contactManager.AddContact(contact);

                yield return contact;
            }

        }
    }
}