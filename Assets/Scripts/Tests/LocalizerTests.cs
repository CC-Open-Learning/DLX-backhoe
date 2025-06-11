using NUnit.Framework;
using RemoteEducation.Localization;
using System;
using UnityEngine;

public class LocalizerTests
{
    private const string SUCCESS_STRING = "This Unit Test Was Successful.";
    private const string SUCCESS_STRING_FRENCH = "This Unit Test Was Successful but in French.";
    private const string TEST_TOKEN = "ENGINE.TEST";
    private const string TEST_MODULE_TOKEN = "VALIDMODULE.VALIDTOKEN";
    private const string SUCCESS_STRING_MODULE = "This Unit Test Was Successful with a valid new module.";
    private const string MISSING_TOKEN_VALID_MODULE = "validmodule.tokenThatDoesNotExist";

    private LanguageLoaderForUnitTests languageLoader;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        languageLoader = new LanguageLoaderForUnitTests();
    }

    [SetUp]
    public void SetUp()
    {
        Localizer.Reset(languageLoader);
        Localizer.SetLanguage("english", false);
    }

    /// <summary>Tests that swapping from one language to a nonexistant language does not break anything.</summary>
    /// <remarks>Our expected outcome is that the original language is kept.</remarks>
    [Test]
    public void SetLanguage_MissingLanguage_LocalizeUsesLastLanguage()
    {
        Localizer.SetLanguage("aLanguageThatDoesNotExist", false);

        var str = Localizer.Localize(TEST_TOKEN);

        Assert.AreEqual(SUCCESS_STRING, str);
    }

    /// <summary>Tests that swapping from one language to an empty language string does not break anything.</summary>
    /// <remarks>Our expected outcome is that the original language is kept.</remarks>
    [Test]
    public void SetLanguage_EmptyLanguage_LocalizeUsesLastLanguage()
    {
        Localizer.SetLanguage("", false);

        var str = Localizer.Localize(TEST_TOKEN);

        Assert.AreEqual(SUCCESS_STRING, str);
    }

    /// <summary>Tests that swapping from one language to a <see langword="null"/> throws a <see cref="NullReferenceException"/>.</summary>
    [Test]
    public void SetLanguage_NullLanguage_ThrowsNullRef()
    {
        Assert.Throws<NullReferenceException>(() => Localizer.SetLanguage(null, false));
    }

    /// <summary>Tests that a known existing language can be loaded and read from.</summary>
    [Test]
    public void Localize_ValidToken_LoadsCorrectString()
    {
        var str = Localizer.Localize(TEST_TOKEN);

        Assert.AreEqual(SUCCESS_STRING, str);
    }

    /// <summary>Tests that a valid token does not need to be in its proper case to be found.</summary>
    [Test]
    public void Localize_ValidTokenWithDifferentCase_LoadsCorrectString()
    {
        var str = Localizer.Localize(TEST_TOKEN.ToLower());

        Assert.AreEqual(SUCCESS_STRING, str);
    }

    /// <summary>Tests that we can swap languages and get our proper test string in that language.</summary>
    [Test]
    public void SetLanguage_ValidLanguageSwap_LocalizeUsesLastLanguage()
    {
        Localizer.SetLanguage("french", false);

        var str = Localizer.Localize(TEST_TOKEN);

        Assert.AreEqual(SUCCESS_STRING_FRENCH, str);
    }

    /// <summary>Tests that a missing token returns the error string.</summary>
    [Test]
    public void Localize_InvalidToken_ErrorString()
    {
        var str = Localizer.Localize("tokenThatDoesNotExist");

        Assert.AreEqual(Localizer.ERRORSTR, str);
    }

    /// <summary>Tests that a valid token for an unloaded module loads that module and returns the string value.</summary>
    [Test]
    public void Localize_UnloadedValidModule_LoadsModule()
    {
        var str = Localizer.Localize(TEST_MODULE_TOKEN);

        Assert.AreEqual(SUCCESS_STRING_MODULE, str);
    }

    /// <summary>Tests that a missing token from a valid, unloaded module returns the error string.</summary>
    [Test]
    public void Localize_UnloadedValidModule_InvalidToken_LoadsModule_ReturnsErrorString()
    {
        var str = Localizer.Localize(MISSING_TOKEN_VALID_MODULE);

        Assert.AreEqual(Localizer.ERRORSTR, str);
    }

    /// <summary>Tests that a missing token returns the original string.</summary>
    [Test]
    public void LocalizePassthrough_InvalidToken_ReturnsInputString()
    {
        var token = "tokenThatDoesNotExist";

        var str = Localizer.LocalizePassthrough(token);

        Assert.AreEqual(token, str);
    }

    /// <summary>Tests that a missing token returns the original string.</summary>
    [Test]
    public void LocalizePassthrough_UnloadedValidModule_InvalidToken_LoadsModule_ReturnsInputString()
    {
        var str = Localizer.LocalizePassthrough(MISSING_TOKEN_VALID_MODULE);

        Assert.AreEqual(MISSING_TOKEN_VALID_MODULE, str);
    }

    /// <summary>Tests that a null token throws an exception.</summary>
    [Test]
    public void Localize_NullToken_ThrowsNullRef()
    {
        string token = null;

        Assert.Throws<NullReferenceException>(() => Localizer.Localize(token));
    }

    /// <summary>Tests that a null token throws an exeption.</summary>
    [Test]
    public void LocalizePassthrough_NullToken_ThrowsNullRef()
    {
        string token = null;

        Assert.Throws<NullReferenceException>(() => Localizer.LocalizePassthrough(token));
    }
}
public class LanguageLoaderForUnitTests : ILanguageLoader
{
    /// <summary>Loads a language with the exact strings we want.</summary>
    public string[] LoadLanguage(string subPath)
    {
        switch (subPath.ToLower())
        {
            case "strings/engine/english":
                return new string[] { "\"test\" \"This Unit Test Was Successful.\"", "\"test2\" \"test2.\"" };

            case "strings/engine/french":
                return new string[] { "\"test\" \"This Unit Test Was Successful but in French.\"", "\"test2\" \"test2.\"" };

            case "strings/validmodule/english":
                return new string[] { "\"VALIDTOKEN\" \"This Unit Test Was Successful with a valid new module.\"", "\"test2\" \"test2.\"" };

            default:
                return null;
        }
    }
}

