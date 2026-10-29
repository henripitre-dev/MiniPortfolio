//using splitbill.Service.XMLDataModel;
using splitbill.Model;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml.Linq;

namespace splitbill.Service
{
    public class XMLService
    {
        /// <summary>
        /// Loads participants and activities from an XML file.
        /// Returns empty collections (never throws) if the file is missing or malformed.
        /// </summary>
        public static (ObservableCollection<Participant> participants,
                        ObservableCollection<Activity> activities)


        Load(string path)
        {
            var participants = new ObservableCollection<Participant>();
            var activities = new ObservableCollection<Activity>();

            if (!File.Exists(path))
            {
                // Create directory + empty XML skeleton, then return empty collections
                string? dir = Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(dir))
                    Directory.CreateDirectory(dir);

                new XDocument(
                    new XDeclaration("1.0", "utf-8", null),
                    new XElement("SplitBillData",
                        new XElement("Participants"),
                        new XElement("Activities")))
                    .Save(path);

                return (participants, activities);
            }

            XDocument doc;
            try { doc = XDocument.Load(path); }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"XML Load failed: {ex.Message}");
                return (participants, activities);
            }

            XElement? root = doc.Root;
            if (root == null) return (participants, activities);

            // 1. Participants
            foreach (XElement el in root.Element("Participants")
                                        ?.Elements("Participant")
                                        ?? Enumerable.Empty<XElement>())
            {
                string? username = (string?)el.Attribute("username");
                string? pwd = (string?)el.Attribute("passwordhash");
                if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(pwd))
                    participants.Add(new Participant(username, pwd, true));
            }

            // Helper: look up a Participant by username (returns null if not found)
            Participant? Resolve(string? username) =>
                string.IsNullOrWhiteSpace(username)
                    ? null
                    : participants.FirstOrDefault(p => p.Username == username);

            // 2. Activities
            foreach (XElement actEl in root.Element("Activities")
                                           ?.Elements("Activity")
                                           ?? Enumerable.Empty<XElement>())
            {
                string name = (string?)actEl.Attribute("name") ?? string.Empty;
                string description = (string?)actEl.Attribute("description") ?? string.Empty;
                bool isActive = ParseBool(actEl.Attribute("isActive"), defaultValue: true);
                string? ownerName = (string?)actEl.Attribute("owner");

                Participant? owner = Resolve(ownerName);
                if (owner == null) continue; // owner is mandatory

                // Secondary participants
                var secondaryParticipants = actEl
                    .Element("Participants")
                    ?.Elements("Ref")
                    .Select(r => Resolve((string?)r.Attribute("username")))
                    .OfType<Participant>()
                    .ToList() ?? [];

                var activity = new Activity(name, owner, secondaryParticipants, description);

                // Sync active state (Activity starts active; call SwitchState to deactivate)
                if (!isActive && activity.IsActive)
                    activity.SwitchState();

                // 3. Expenses 
                foreach (XElement expEl in actEl.Element("Expenses")
                                                ?.Elements("Expense")
                                                ?? Enumerable.Empty<XElement>())
                {
                    string expName = (string?)expEl.Attribute("name") ?? string.Empty;
                    string expDescription = (string?)expEl.Attribute("description") ?? string.Empty;
                    decimal amount = ParseDecimal(expEl.Attribute("amount"));
                    bool expIsActive = ParseBool(expEl.Attribute("isActive"), defaultValue: true);
                    string? expOwnerName = (string?)expEl.Attribute("owner");

                    Participant? expOwner = Resolve(expOwnerName);
                    if (expOwner == null) continue;

                    var expParticipants = expEl
                        .Element("Participants")
                        ?.Elements("Ref")
                        .Select(r => Resolve((string?)r.Attribute("username")))
                        .OfType<Participant>()
                        .ToList() ?? [];

                    activity.AddExpense(new Expense(activity, expName, amount, expOwner, expParticipants, expDescription));

                    // Sync active state of the newly added expense
                    var addedExpense = activity.Expenses[^1];
                    if (!expIsActive && addedExpense.IsActive)
                        addedExpense.SwitchState();
                }

                // 4. Refunds
                foreach (XElement refEl in actEl.Element("Refunds")
                                                ?.Elements("Refund")
                                                ?? Enumerable.Empty<XElement>())
                {
                    string refDebitorName = (string?)refEl.Attribute("debitor") ?? string.Empty;
                    string refCreditorName = (string?)refEl.Attribute("creditor") ?? string.Empty;
                    decimal amount = ParseDecimal(refEl.Attribute("amount"));
                    string? refExpenseName = (string?)refEl.Attribute("expense");

                    Participant? refDebitor = Resolve(refDebitorName);
                    if (refDebitor == null) continue;

                    Participant? refCreditor = Resolve(refCreditorName);
                    if (refCreditor == null) continue;


                    Expense? expense = activity.Expenses
    .FirstOrDefault(e => e.Name == refExpenseName);

                    if (expense == null) continue;

                    activity.AddRefund(expense, refDebitor, amount);

                    
                }

                activities.Add(activity);
            }

            return (participants, activities);
        }


        /// <summary>
        /// Serializes participants and activities to an XML file.
        /// Creates or overwrites the file at <paramref name="path"/>.
        /// </summary>
        public static void Save(string path,
                                IEnumerable<Participant> participants,
                                IEnumerable<Activity> activities)
        {
            var doc = new XDocument(
                new XDeclaration("1.0", "utf-8", null),
                new XElement("SplitBillData",

                    // Participants
                    new XElement("Participants",
                        participants.Select(p =>
                            new XElement("Participant",
                                new XAttribute("username", p.Username),
                                new XAttribute("passwordhash", p.PasswordHash)))),

                    // Activities 
                    new XElement("Activities",
                        activities.Select(a =>
                            new XElement("Activity",
                                new XAttribute("name", a.Name),
                                new XAttribute("owner", a.Owner.Username),
                                new XAttribute("description", a.Description ?? string.Empty),
                                new XAttribute("isActive", a.IsActive),

                                new XElement("Participants",
                                    a.Participants.Select(p =>
                                        new XElement("Ref",
                                            new XAttribute("username", p.Username)))),

                                new XElement("Expenses",
                                    a.Expenses.Select(e =>
                                        new XElement("Expense",
                                            new XAttribute("name", e.Name),
                                            new XAttribute("amount", e.Amount),
                                            new XAttribute("owner", e.Owner.Username),
                                            new XAttribute("description", e.Description ?? string.Empty),
                                            new XAttribute("isActive", e.IsActive),

                                            new XElement("Participants",
                                                e.Participants.Select(p =>
                                                    new XElement("Ref",
                                                        new XAttribute("username", p.Username))))))),

                                new XElement("Refunds",
                                    a.Refunds.Select(r =>
                                        new XElement("Refund",
                                        new XAttribute("amount", r.Amount),
                                        new XAttribute("debitor", r.Debitor.Username),
                                        new XAttribute("creditor", r.Creditor.Username),
                                        new XAttribute("expense", r.Expense.Name))))
                                )))));

            // Ensure the directory exists
            string? dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir))
                Directory.CreateDirectory(dir);

            doc.Save(path);
        }


        private static bool ParseBool(XAttribute? attr, bool defaultValue = true)
        {
            if (attr == null) return defaultValue;
            return bool.TryParse(attr.Value, out bool result) ? result : defaultValue;
        }

        private static decimal ParseDecimal(XAttribute? attr)
        {
            if (attr == null) return 0m;
            return decimal.TryParse(attr.Value,
                       System.Globalization.NumberStyles.Any,
                       System.Globalization.CultureInfo.InvariantCulture,
                       out decimal result)
                   ? result : 0m;
        }
    }
}
