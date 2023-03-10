using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaUI.Core.Definitions;
using FluentAssertions;

namespace AddressBook.Tests.Windows
{
    public class ContactWindow : Window
    {
        public ContactWindow(FrameworkAutomationElementBase frameworkAutomationElement) : base(frameworkAutomationElement)
        {
        }
        private Tab Tab =>
            FindFirstDescendant(cf => cf.ByControlType(ControlType.Tab))
            .AsTab();

        public NameAndEmailTabField NameAndEmailTabField => Tab.FindFirstChild(
            cf => cf.ByName("Name and E-mail")).As<NameAndEmailTabField>();

        public PhoneTabField PhoneTabField => Tab.FindFirstChild(
            cf => cf.ByName("Phone Numbers")).As<PhoneTabField>();

        public TabItem WebsitesTabField => Tab.FindFirstChild(
            cf => cf.ByName("Websites")).As<TabItem>();

        public TabItem LocationsTabField => Tab.FindFirstChild(
            cf => cf.ByName("Locations")).As<TabItem>();

        public TabItem JobTabField => Tab.FindFirstChild(
            cf => cf.ByName("Jobs")).As<TabItem>();

        public NotesTabItem NotesTabField => Tab.FindFirstChild(
            cf => cf.ByName("Notes")).As<NotesTabItem>();

        public Button SaveChangesButton => FindFirstChild(
            cf => cf.ByName("Save Changes")).AsButton();
    }

    public class NameAndEmailTabField : AutomationElement
    {
        public NameAndEmailTabField(FrameworkAutomationElementBase frameworkAutomationElement) :
            base(frameworkAutomationElement)
        { }

        private List<TextBox> textBoxes => FindAllDescendants(
            cf => cf.ByClassName("TextBox"))
            .Select(e => e.As<TextBox>()).ToList();

        public TextBox FormattedName => textBoxes[0];
        public TextBox FirstName => textBoxes[1];
        public TextBox MiddleName => textBoxes[2];
        public TextBox LastName => textBoxes[3];
        public TextBox Nickname => textBoxes[4];
        public TextBox Email => textBoxes[5];
    }

    public class PhoneTabField : AutomationElement
    {
        public PhoneTabField(FrameworkAutomationElementBase frameworkAutomationElement) :
            base(frameworkAutomationElement)
        { }

        private List<TextBox> textBoxes => FindAllDescendants(
            cf => cf.ByClassName("TextBox"))
            .Select(e => e.As<TextBox>()).ToList();

        public TextBox HomePhone => textBoxes[0];
        public TextBox HomeFax => textBoxes[1];
        public TextBox WorkPhone => textBoxes[2];
        public TextBox WorkFax => textBoxes[3];
        public TextBox OtherCell => textBoxes[4];
        public TextBox OtherPager => textBoxes[5];
    }

    public class NotesTabItem : AutomationElement
    {
        public NotesTabItem(FrameworkAutomationElementBase frameworkAutomationElement) :
            base(frameworkAutomationElement)
        { }

        private List<TextBox> textBoxes => FindAllDescendants(
            cf => cf.ByClassName("TextBox"))
            .Select(e => e.As<TextBox>()).ToList();

        public TextBox Notes => textBoxes[0];
    }
}
