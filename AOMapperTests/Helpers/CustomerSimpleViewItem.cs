using System;
using System.Collections.Generic;

namespace AOMapperTests.Helpers
{
    public class CustomerSimpleViewItem3 : CustomerSimpleViewItem
    {
        public string NumberOfOrders { get; set; }
    }

    public class CustomerSimpleViewItem4 : CustomerSimpleViewItem
    {
        public Dictionary<string, string> DateTimes { get; set; } 
    }

    public class CustomerSimpleViewItem5 : CustomerSimpleViewItem
    {
        public DateTime[] DateTimes { get; set; }
    }

    public class CustomerSimpleViewItem
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        //}

        //public virtual string SubName { get; set; }

        //public virtual CustomerSubViewItem SubSubItem { get; set; }

        //public int NumberOfOrders { get; set; }

        protected bool Equals(CustomerSimpleViewItem other)
        {
            return string.Equals(FirstName, other.FirstName) && string.Equals(LastName, other.LastName) && DateOfBirth.Equals(other.DateOfBirth);// && NumberOfOrders == other.NumberOfOrders;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CustomerSimpleViewItem)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (FirstName != null ? FirstName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (LastName != null ? LastName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ DateOfBirth.GetHashCode();
                //hashCode = (hashCode * 397) ^ (SubName != null ? SubName.GetHashCode() : 0);
                //hashCode = (hashCode * 397) ^ NumberOfOrders;
                return hashCode;
            }
        }
    }
}