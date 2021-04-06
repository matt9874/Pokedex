using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pokedex.Application.Translation;
using Pokedex.Domain;

namespace Pokedex.Application.Tests.TranslationTests
{
    [TestClass]
    public class TranslationTypeDeciderTests
    {
        private TranslationTypeDecider _translatorFactory;

        [TestInitialize]
        public void TestInit()
        {
            _translatorFactory = new TranslationTypeDecider();
        }

        [TestMethod]
        public void DecideTranslationType_IsLegendary_TranslatorTypeEqualsYoda()
        {
            var pokemon = new Domain.Pokemon("name", new TranslatedText("description"), "waters-edge", true);
            TranslationType translationType = _translatorFactory.DecideTranslationType(pokemon);
            Assert.AreEqual(TranslationType.Yoda, translationType);
        }

        [TestMethod]
        public void DecideTranslationType_CaveHabitat_TranslatorTypeEqualsYoda()
        {
            var pokemon = new Domain.Pokemon("name", new TranslatedText("description"), "cave", false);
            TranslationType translationType = _translatorFactory.DecideTranslationType(pokemon);
            Assert.AreEqual(TranslationType.Yoda, translationType);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        [DataRow("waters-edge")]
        [DataRow("some-new-habitat")]
        public void DecideTranslationType_NotLegendaryOrCaveHabitat_TranslatorTypeEqualsShakespeare(string habitat)
        {
            var pokemon = new Domain.Pokemon("name", new TranslatedText("description"), habitat, false);
            TranslationType translationType = _translatorFactory.DecideTranslationType(pokemon);
            Assert.AreEqual(TranslationType.Shakespeare, translationType);
        }
    }
}
