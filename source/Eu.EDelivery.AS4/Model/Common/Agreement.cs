﻿using System;
using System.Xml.Serialization;

namespace Eu.EDelivery.AS4.Model.Common
{
    public class Agreement : IEquatable<Agreement>
    {
        public string Value { get; set; }

        public string RefType { get; set; }

        [XmlElement(IsNullable = true)]
        public string PModeId { get; set; }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(Agreement other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return
                string.Equals(this.Value, other.Value, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(this.RefType, other.RefType, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(this.PModeId, other.PModeId, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <returns>
        /// true if the specified object  is equal to the current object; otherwise, false.
        /// </returns>
        /// <param name="obj">The object to compare with the current object. </param>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((Agreement) obj);
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>
        /// A hash code for the current object.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = this.Value != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(this.Value) : 0;
                hashCode = (hashCode * 397)
                           ^ (this.RefType != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(this.RefType) : 0);
                hashCode = (hashCode * 397)
                           ^ (this.PModeId != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(this.PModeId) : 0);
                return hashCode;
            }
        }
    }
}