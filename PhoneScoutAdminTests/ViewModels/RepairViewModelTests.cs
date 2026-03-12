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
        public void PartsAreNotChanged()
        {
            _originalParts = new List<string> { "Képernyő", "Akkumulátor", "Kamera" };
            Parts = new List<string> { "Akkumulátor", "Kamera", "Képernyő" };

            bool result = ArePartsChanged();

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void PartsAreChanged()
        {
            _originalParts = new List<string> { "Képernyő", "Akkumulátor", "Kamera" };
            Parts = new List<string> { "Képernyő", "Akkumulátor", "Camera" }; 

            bool result = ArePartsChanged();

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void PartsAreChanged_WhenCountIsDifferent()
        {
            _originalParts = new List<string> { "Képernyő", "Akkumulátor", "Kamera" };
            Parts = new List<string> { "Képernyő", "Akkumulátor" };

            bool result = ArePartsChanged();

            Assert.IsTrue(result);
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

        Assert.IsFalse(isChanged);
    }

    [TestMethod]
    public void PriceIsChanged()
    {
        decimal originalPrice = 100m;
        decimal currentPrice = 150m;

        bool isChanged = currentPrice != originalPrice;

        Assert.IsTrue(isChanged);
    }
}