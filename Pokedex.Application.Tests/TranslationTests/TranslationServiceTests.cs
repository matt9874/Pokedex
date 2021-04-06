using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pokedex.Application.Interfaces;
using Pokedex.Application.Translation;
using Pokedex.Application.Translation.Dtos;
using Pokedex.Domain;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Pokedex.Application.Tests.TranslationTests
{
    [TestClass]
    public class TranslationServiceTests
    {
        private Mock<ITranslationTypeDecider> _mockTranslationTypeDecider;
        private Mock<IReader<TranslationRequest, TranslationResult>> _mockTranslator;
        private Mock<Func<TranslationType, IReader<TranslationRequest, TranslationResult>>> _mockTranslatorFactory;
        private TranslationService _translationService;

        [TestInitialize]
        public void TestInit()
        {
            _mockTranslationTypeDecider = new Mock<ITranslationTypeDecider>();
            _mockTranslator = new Mock<IReader<TranslationRequest, TranslationResult>>();
            _mockTranslatorFactory = new Mock<Func<TranslationType,IReader<TranslationRequest, TranslationResult>>>();
            _mockTranslatorFactory.Setup(tf => tf.Invoke(It.IsAny<TranslationType>()))
                .Returns(_mockTranslator.Object);
            _translationService = new TranslationService(_mockTranslationTypeDecider.Object, _mockTranslatorFactory.Object);
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
        public async Task TranslateDescription_DescriptionCanBeTranslated_TranslationTypeDeciderWasCalledOnce()
        {
            string description = "Hello, good evening and welcome.";
            Domain.Pokemon pokemon = new Domain.Pokemon("n", new TranslatedText(description), "h", true);
            _mockTranslationTypeDecider.Setup(ttd => ttd.DecideTranslationType(pokemon))
                .Returns(TranslationType.Yoda);

            Domain.Pokemon translatedPokemon = await _translationService.TranslateDescription(pokemon);

            _mockTranslationTypeDecider.Verify(tf => tf.DecideTranslationType(pokemon), Times.Once);
        }

        [TestMethod]
        public async Task TranslateDescription_DescriptionCanBeTranslated_TranslatorWasCalledOnceWithCorrectRequest()
        {
            string description = "Hello, good evening and welcome.";
            Domain.Pokemon pokemon = new Domain.Pokemon("n", new TranslatedText(description), "h", true);
            TranslationType translationType = TranslationType.Yoda;
            _mockTranslationTypeDecider.Setup(ttd => ttd.DecideTranslationType(pokemon))
                .Returns(translationType);

            Domain.Pokemon translatedPokemon = await _translationService.TranslateDescription(pokemon);

            _mockTranslator.Verify(t => t.Read(It.Is<TranslationRequest>(tr=>tr.Text==description && tr.Type== translationType)), 
                Times.Once);
        }

        [TestMethod]
        public async Task TranslateDescription_DescriptionCanBeTranslated_ReturnedPokemonHasTranlatedDescription()
        {
            string description = "Hello, good evening and welcome.";
            Domain.Pokemon pokemon = new Domain.Pokemon("n", new TranslatedText(description), "h", true);

            TranslationType translationType = TranslationType.Yoda;
            _mockTranslationTypeDecider.Setup(ttd => ttd.DecideTranslationType(pokemon))
                .Returns(translationType);

            string translatedDescription = "Welcome, good evening and hello";
            _mockTranslator.Setup(t => t.Read(It.IsAny<TranslationRequest>()))
                .ReturnsAsync(new TranslationResult() {Contents = new Contents() { Translated = translatedDescription} });

            Domain.Pokemon translatedPokemon = await _translationService.TranslateDescription(pokemon);

            Assert.AreEqual(translatedDescription, translatedPokemon.Description.Text);
        }

        [TestMethod]
        public async Task TranslateDescription_DescriptionCanBeTranslated_ReturnedPokemonHasCorrectTranslationType()
        {
            string description = "Hello, good evening and welcome.";
            Domain.Pokemon pokemon = new Domain.Pokemon("n", new TranslatedText(description), "h", true);
            
            var translationType = TranslationType.Yoda;
            _mockTranslationTypeDecider.Setup(tf => tf.DecideTranslationType(pokemon))
                .Returns(translationType);

            string translatedDescription = "Welcome, good evening and hello";
            _mockTranslator.Setup(t => t.Read(It.IsAny<TranslationRequest>()))
                .ReturnsAsync(new TranslationResult() { Contents = new Contents() { Translated = translatedDescription } });

            Domain.Pokemon translatedPokemon = await _translationService.TranslateDescription(pokemon);

            Assert.AreEqual(translationType, translatedPokemon.Description.TranslationType);
        }

        [TestMethod]
        public async Task TranslateDescription_ThrowsHttpRequestException_ReturnedPokemonHasInputDescription()
        {
            string description = "Hello, good evening and welcome.";
            Domain.Pokemon pokemon = new Domain.Pokemon("n", new TranslatedText(description), "h", true);

            var translationType = TranslationType.Yoda;
            _mockTranslationTypeDecider.Setup(tf => tf.DecideTranslationType(pokemon))
                .Returns(translationType);

            _mockTranslator.Setup(t => t.Read(It.IsAny<TranslationRequest>())).ThrowsAsync(new HttpRequestException());

            Domain.Pokemon translatedPokemon = await _translationService.TranslateDescription(pokemon);

            Assert.AreEqual(description, translatedPokemon.Description.Text);
        }

        [TestMethod]
        public async Task TranslateDescription_ThrowsHttpRequestException_ReturnedPokemontDescriptionHasTranslationTypeOfNone()
        {
            string description = "Hello, good evening and welcome.";
            Domain.Pokemon pokemon = new Domain.Pokemon("n", new TranslatedText(description), "h", true);

            var translationType = TranslationType.Yoda;
            _mockTranslationTypeDecider.Setup(tf => tf.DecideTranslationType(pokemon))
                .Returns(translationType);

            _mockTranslator.Setup(t => t.Read(It.IsAny<TranslationRequest>())).ThrowsAsync(new HttpRequestException());

            Domain.Pokemon translatedPokemon = await _translationService.TranslateDescription(pokemon);

            Assert.AreEqual(TranslationType.None, translatedPokemon.Description.TranslationType);
        }
    }
}
