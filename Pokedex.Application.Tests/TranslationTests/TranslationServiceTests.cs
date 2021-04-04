using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pokedex.Application.Translation;
using Pokedex.Domain;
using System.Net.Http;
using System.Threading.Tasks;

namespace Pokedex.Application.Tests.TranslationTests
{
    [TestClass]
    public class TranslationServiceTests
    {
        private Mock<ITranslatorFactory> _mockTranslatorFactory;
        private TranslationService _translationService;

        [TestInitialize]
        public void TestInit()
        {
            _mockTranslatorFactory = new Mock<ITranslatorFactory>();
            _translationService = new TranslationService(_mockTranslatorFactory.Object);
        }

        [TestMethod]
        public async Task TranslateDescription_NullPokemon_NullPokemonReturned()
        {
            Domain.Pokemon pokemon = null;

            Domain.Pokemon translatedPokemon = await _translationService.TranslateDescription(pokemon);

            Assert.IsNull(translatedPokemon);
        }

        [TestMethod]
        public async Task TranslateDescription_DescriptionIsNull_DescriptionIsNull()
        {
            Domain.Pokemon pokemon = new Domain.Pokemon("n", null, "h", true);

            Domain.Pokemon translatedPokemon = await _translationService.TranslateDescription(pokemon);

            Assert.IsNull(translatedPokemon.Description);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public async Task TranslateDescription_DescriptionCannotBeTranslated_PokemonHasInputDescription(string description)
        {
            Domain.Pokemon pokemon = new Domain.Pokemon("n", new TranslatedText(description), "h", true);

            Domain.Pokemon translatedPokemon = await _translationService.TranslateDescription(pokemon);

            Assert.AreEqual(pokemon.Description, translatedPokemon.Description);
        }

        [TestMethod]
        public async Task TranslateDescription_DescriptionCanBeTranslated_TranslatorFactoryWasCalledOnce()
        {
            string description = "Hello, good evening and welcome.";
            Domain.Pokemon pokemon = new Domain.Pokemon("n", new TranslatedText(description), "h", true);

            var mockTranslator = new Mock<ITranslator>();

            _mockTranslatorFactory.Setup(tf => tf.CreateTranslator(pokemon))
                .Returns(mockTranslator.Object);

            Domain.Pokemon translatedPokemon = await _translationService.TranslateDescription(pokemon);

            _mockTranslatorFactory.Verify(tf => tf.CreateTranslator(pokemon), Times.Once);
        }

        [TestMethod]
        public async Task TranslateDescription_DescriptionCanBeTranslated_TranslatorWasCalledOnce()
        {
            string description = "Hello, good evening and welcome.";
            Domain.Pokemon pokemon = new Domain.Pokemon("n", new TranslatedText(description), "h", true);
            var mockTranslator = new Mock<ITranslator>();
            
            _mockTranslatorFactory.Setup(tf => tf.CreateTranslator(pokemon))
                .Returns(mockTranslator.Object);

            Domain.Pokemon translatedPokemon = await _translationService.TranslateDescription(pokemon);

            mockTranslator.Verify(t => t.Translate(description), Times.Once);
        }

        [TestMethod]
        public async Task TranslateDescription_DescriptionCanBeTranslated_ReturnedPokemonHasTranlatedDescription()
        {
            string description = "Hello, good evening and welcome.";
            Domain.Pokemon pokemon = new Domain.Pokemon("n", new TranslatedText(description), "h", true);
            var mockTranslator = new Mock<ITranslator>();
            string translatedDescription = "Welcome, good evening and hello";
            mockTranslator.Setup(t => t.Translate(description))
                .ReturnsAsync(translatedDescription);
            _mockTranslatorFactory.Setup(tf => tf.CreateTranslator(pokemon))
                .Returns(mockTranslator.Object);

            Domain.Pokemon translatedPokemon = await _translationService.TranslateDescription(pokemon);

            Assert.AreEqual(translatedDescription, translatedPokemon.Description.Text);
        }

        [TestMethod]
        public async Task TranslateDescription_DescriptionCanBeTranslated_ReturnedPokemonHasCorrectTranslationType()
        {
            string description = "Hello, good evening and welcome.";
            Domain.Pokemon pokemon = new Domain.Pokemon("n", new TranslatedText(description), "h", true);
            var mockTranslator = new Mock<ITranslator>();
            var translationType = TranslationType.Yoda;
            mockTranslator.Setup(t => t.Type)
                .Returns(translationType);
            _mockTranslatorFactory.Setup(tf => tf.CreateTranslator(pokemon))
                .Returns(mockTranslator.Object);

            Domain.Pokemon translatedPokemon = await _translationService.TranslateDescription(pokemon);

            Assert.AreEqual(translationType, translatedPokemon.Description.TranslationType);
        }

        [TestMethod]
        public async Task TranslateDescription_ThrowsHttpRequestException_ReturnedPokemonHasInputDescription()
        {
            string description = "Hello, good evening and welcome.";
            Domain.Pokemon pokemon = new Domain.Pokemon("n", new TranslatedText(description), "h", true);
            var mockTranslator = new Mock<ITranslator>();
            var translationType = TranslationType.Yoda;
            mockTranslator.Setup(t => t.Type)
                .Returns(translationType);
            mockTranslator.Setup(t => t.Translate(It.IsAny<string>())).ThrowsAsync(new HttpRequestException());
            _mockTranslatorFactory.Setup(tf => tf.CreateTranslator(pokemon))
                .Returns(mockTranslator.Object);

            Domain.Pokemon translatedPokemon = await _translationService.TranslateDescription(pokemon);

            Assert.AreEqual(description, translatedPokemon.Description.Text);
        }

        [TestMethod]
        public async Task TranslateDescription_ThrowsHttpRequestException_ReturnedPokemontDescriptionHasTranslationTypeOfNone()
        {
            string description = "Hello, good evening and welcome.";
            Domain.Pokemon pokemon = new Domain.Pokemon("n", new TranslatedText(description), "h", true);
            var mockTranslator = new Mock<ITranslator>();
            var translationType = TranslationType.Yoda;
            mockTranslator.Setup(t => t.Type)
                .Returns(translationType);
            mockTranslator.Setup(t => t.Translate(It.IsAny<string>())).ThrowsAsync(new HttpRequestException());
            _mockTranslatorFactory.Setup(tf => tf.CreateTranslator(pokemon))
                .Returns(mockTranslator.Object);

            Domain.Pokemon translatedPokemon = await _translationService.TranslateDescription(pokemon);

            Assert.AreEqual(TranslationType.None, translatedPokemon.Description.TranslationType);
        }
    }
}
