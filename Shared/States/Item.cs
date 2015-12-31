using System;
using System.Collections.Generic;

namespace Shared.States
{
    public class Item : IEquatable<Item>
    {
        public Item(int id)
        {
            Id = id;
        }

        public int Id { get; private set; }

        public bool Equals(Item other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Id, other.Id);
        }

        public Item Copy()
        {
            return new Item(Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Item)obj);
        }

        public override int GetHashCode()
        {
            return (Id.GetHashCode());
        }

        private sealed class ItemEqualityComparer : IEqualityComparer<Item>
        {
            public bool Equals(Item x, Item y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return Equals(x.Id, y.Id);
            }

            public int GetHashCode(Item obj)
            {
                return (obj.Id.GetHashCode());
            }
        }

        private static readonly IEqualityComparer<Item> ItemComparerInstance = new ItemEqualityComparer();

        public static IEqualityComparer<Item> ItemComparer
        {
            get { return ItemComparerInstance; }
        }
    }
}
