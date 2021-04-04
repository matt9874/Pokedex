using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pokedex.Application.Interfaces;
using Pokedex.Application.Translation;
using Pokedex.Application.Translation.Dtos;
using Pokedex.Domain;

namespace Pokedex.Application.Tests.TranslationTests
{
    [TestClass]
    public class TranslatorFactoryTests
    {
        private TranslatorFactory _translatorFactory;

        [TestInitialize]
        public void TestInit()
        {
            var mockTranslationReader = new Mock<IReader<TranslationRequest, TranslationResult>>();
            _translatorFactory = new TranslatorFactory(mockTranslationReader.Object);
        }

        [TestMethod]
        public void CreateTranslator_IsLegendary_TranslatorTypeEqualsYoda()
        {
            var pokemon = new Domain.Pokemon("name", new TranslatedText("description"), "waters-edge", true);
            ITranslator translator = _translatorFactory.CreateTranslator(pokemon);
            Assert.AreEqual(TranslationType.Yoda, translator.Type);
        }

        [TestMethod]
        public void CreateTranslator_CaveHabitat_TranslatorTypeEqualsYoda()
        {
            var pokemon = new Domain.Pokemon("name", new TranslatedText("description"), "cave", false);
            ITranslator translator = _translatorFactory.CreateTranslator(pokemon);
            Assert.AreEqual(TranslationType.Yoda, translator.Type);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        [DataRow("waters-edge")]
        [DataRow("some-new-habitat")]
        public void CreateTranslator_NotLegendaryOrCaveHabitat_TranslatorTypeEqualsShakespeare(string habitat)
        {
            var pokemon = new Domain.Pokemon("name", new TranslatedText("description"), habitat, false);
            ITranslator translator = _translatorFactory.CreateTranslator(pokemon);
            Assert.AreEqual(TranslationType.Shakespeare, translator.Type);
        }
    }
}
