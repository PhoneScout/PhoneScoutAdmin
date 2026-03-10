using PhoneScoutAdmin.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PhoneScoutAdmin.ViewModels.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Collections.Generic;
    using System.Linq;

    [TestClass]
    public class PartsTests
    {
        private List<string> _originalParts;

        private List<string> Parts;

        private bool ArePartsChanged()
        {
            return !_originalParts.OrderBy(x => x)
                                  .SequenceEqual(Parts.OrderBy(x => x));
        }

        [TestMethod]
        public void PartsAreNotChanged_ShouldReturnFalse()
        {
            _originalParts = new List<string> { "Screen", "Battery", "Button" };
            Parts = new List<string> { "Battery", "Button", "Screen" }; // Same elements, different order

            bool result = ArePartsChanged();

            Assert.IsFalse(result); // Parts are the same
        }

        [TestMethod]
        public void PartsAreChanged_ShouldReturnTrue()
        {
            _originalParts = new List<string> { "Screen", "Battery", "Button" };
            Parts = new List<string> { "Screen", "Battery", "Camera" }; // One element changed

            bool result = ArePartsChanged();

            Assert.IsTrue(result); // Parts changed
        }

        [TestMethod]
        public void PartsAreChanged_WhenCountIsDifferent_ShouldReturnTrue()
        {
            _originalParts = new List<string> { "Screen", "Battery", "Button" };
            Parts = new List<string> { "Screen", "Battery" }; // Missing one element

            bool result = ArePartsChanged();

            Assert.IsTrue(result); // Parts changed
        }
    }
}

[TestClass]
public class PriceTests
{
    [TestMethod]
    public void PriceIsNotChanged()
    {
        decimal originalPrice = 100m;
        decimal currentPrice = 100m;

        bool isChanged = currentPrice != originalPrice;

        Assert.IsFalse(isChanged); // Prices are the same
    }

    [TestMethod]
    public void PriceIsChanged()
    {
        decimal originalPrice = 100m;
        decimal currentPrice = 150m;

        bool isChanged = currentPrice != originalPrice;

        Assert.IsTrue(isChanged); // Prices are different
    }
}