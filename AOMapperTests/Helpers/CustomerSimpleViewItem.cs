using System;
using System.Collections.Generic;

namespace AOMapperTests.Helpers
{
    public class CustomerSimpleViewItem3 : CustomerSimpleViewItem
    {
        public string NumberOfOrders { get; set; }
    }

    public class CustomerSimpleViewItem6 : CustomerSimpleViewItem3
    {
        public int Color { get; set; }
        public double Cast { get; set; }
    }

    public class CustomerSimpleViewItem4 : CustomerSimpleViewItem
    {
        public Dictionary<string, string> DateTimes { get; set; }
    }

    public class CustomerSimpleViewItem5 : CustomerSimpleViewItem
    {
        public DateTime[] DateTimes { get; set; }

        //public virtual CustomerSubViewItem2 SubViewItem2 { get; set; }
    }

    public class CustomerSimpleViewItem7 : CustomerSimpleViewItem
    {
        public List<DateTime> DateTimes { get; set; }        
    }

    public class CustomerSimpleViewItem10 : CustomerSimpleViewItem
    {
        public SimpleObjectViewItem[] ViewItems { get; set; }
    }

    public class CustomerSimpleViewItem8 : CustomerSimpleViewItem
    {
        public List<string> DateTimes { get; set; }
    }

    public class CustomerSimpleViewItem9 : CustomerSimpleViewItem
    {
        public string[] DateTimes { get; set; }
    }

    public class CustomerSimpleViewItem
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        //}

        public virtual string SubName { get; set; }
        public virtual string SubDescription { get; set; }

        public virtual CustomerSubViewItem SubSubItem { get; set; }

        //public int NumberOfOrders { get; set; }

        public CustomerSimpleViewItem()
        {
            
        }

        public CustomerSimpleViewItem(string dest)
        {
            SubDescription = dest;
        }

        protected bool Equals(CustomerSimpleViewItem other)
        {
            return string.Equals(FirstName, other.FirstName) && string.Equals(LastName, other.LastName) &&
                   DateOfBirth.Equals(other.DateOfBirth); // && NumberOfOrders == other.NumberOfOrders;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((CustomerSimpleViewItem) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = FirstName != null ? FirstName.GetHashCode() : 0;
                hashCode = (hashCode*397) ^ (LastName != null ? LastName.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ DateOfBirth.GetHashCode();
                //hashCode = (hashCode * 397) ^ (SubName != null ? SubName.GetHashCode() : 0);
                //hashCode = (hashCode * 397) ^ NumberOfOrders;
                return hashCode;
            }
        }
    }
}