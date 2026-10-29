using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace splitbill.Model
{
    public class Refund(Activity activity, Expense expense, Participant debitor, Participant creditor, decimal amount)
    {
        //Non modifiable ; tous  des get
        public Participant Debitor { get; } = debitor;
        public Participant Creditor { get; } = creditor;
        public Activity Activity { get; } = activity;
        public Expense Expense { get; } = expense;
        public decimal Amount { get; } = amount;
    }
}
