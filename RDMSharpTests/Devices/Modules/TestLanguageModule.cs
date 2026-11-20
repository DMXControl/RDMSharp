using RDMSharp.Metadata;
using RDMSharp.RDM.Device.Module;
using RDMSharpTests.Devices.Mock;
using System.Text;

namespace RDMSharpTests.RDM.Devices.Modules;

/// <summary>
/// LANGUAGE_CAPABILITIES (E1.20 10.5.7) and LANGUAGE (E1.20 10.5.8).
/// <list type="bullet">
/// <item>LANGUAGE_CAPABILITIES is a packed list of 2 character ISO 639-1 codes.</item>
/// <item>LANGUAGE can only be set to one of these languages, other languages are NACKed with DATA_OUT_OF_RANGE.</item>
/// <item>The Language Code 0x0000 resets the device to its default language.</item>
/// </list>
/// </summary>
public class TestLanguageModule
{
    private LanguageMockDevice? generated;

    private static readonly UID CONTROLLER_UID = new UID(0x1fff + (ushort)ERDM_Parameter.LANGUAGE, 333);
    private static readonly UID DEVCIE_UID = new UID(123 + (ushort)ERDM_Parameter.LANGUAGE, 555);

    private static readonly string[] LANGUAGES = new string[] { "en", "de", "fr" };
    private const string DEFAULT_LANGUAGE = "en";

    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        await MetadataFactory.AwaitInitialize();
    }

    [SetUp]
    public void Setup()
    {
        var defines = MetadataFactory.GetMetadataDefineVersions();
        Assert.That(defines, Is.Not.Null);
        generated = new LanguageMockDevice(DEVCIE_UID);
    }
    [TearDown]
    public void TearDown()
    {
        generated?.Dispose();
        generated = null;
    }

    [Test]
    public void TestDisplayName()
    {
        Assert.That(generated, Is.Not.Null);
        var module = generated.Modules.OfType<LanguageModule>().Single();
        Assert.That(module, Is.Not.Null);
        Assert.That(module.DisplayName, Is.EqualTo("Language"));
    }

    [Test]
    public void TestConstructor()
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new LanguageModule("es", LANGUAGES), "default language has to be one of the languages");
            Assert.Throws<ArgumentException>(() => new LanguageModule("en", "en", "eng"), "ISO 639-1 codes have 2 characters");
            Assert.Throws<ArgumentException>(() => new LanguageModule("en", "en", "e1"), "ISO 639-1 codes are alpha codes");
            Assert.Throws<ArgumentException>(() => new LanguageModule("en", "en", "de", "EN"), "a language must not be used twice");
            Assert.Throws<ArgumentException>(() => new LanguageModule("en"), "at least one language is needed");
        }
    }

    [Test, DependsOnTest(nameof(TestDisplayName))]
    public void TestGetLANGUAGE_CAPABILITIES()
    {
        Assert.That(generated, Is.Not.Null);
        var module = generated.Modules.OfType<LanguageModule>().Single();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(module.Languages, Is.EqualTo(LANGUAGES));
            Assert.That(module.DefaultLanguage, Is.EqualTo(DEFAULT_LANGUAGE));
        }

        RDMMessage? response = generated.ProcessRequestMessage_Internal(createRequest(ERDM_Parameter.LANGUAGE_CAPABILITIES, ERDM_Command.GET_COMMAND));
        Assert.That(response, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.LANGUAGE_CAPABILITIES));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Is.EqualTo(Encoding.ASCII.GetBytes("endefr")));
            Assert.That(response.Value, Is.EqualTo(LANGUAGES));
        }

        // LANGUAGE_CAPABILITIES is GET only
        response = generated.ProcessRequestMessage_Internal(createRequest(ERDM_Parameter.LANGUAGE_CAPABILITIES, ERDM_Command.SET_COMMAND, Encoding.ASCII.GetBytes("es")));
        Assert.That(response, Is.Not.Null);
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(module.Languages, Is.EqualTo(LANGUAGES));
    }

    [Test, DependsOnTest(nameof(TestDisplayName))]
    public void TestGetSetLANGUAGE()
    {
        Assert.That(generated, Is.Not.Null);
        var module = generated.Modules.OfType<LanguageModule>().Single();

        #region Test Basic: default language
        Assert.That(module.CurrentLanguage, Is.EqualTo(DEFAULT_LANGUAGE));
        assertGetResponse("en");
        #endregion

        #region Test SET supported languages
        assertSetResponse(send(Encoding.ASCII.GetBytes("de")), null);
        Assert.That(module.CurrentLanguage, Is.EqualTo("de"));
        assertGetResponse("de");

        // ISO 639-1 codes are lower case, the device answers with its own code
        assertSetResponse(send(Encoding.ASCII.GetBytes("FR")), null);
        Assert.That(module.CurrentLanguage, Is.EqualTo("fr"));
        assertGetResponse("fr");
        #endregion

        #region Test 0x0000 resets to the default language
        assertSetResponse(send(new byte[] { 0x00, 0x00 }), null);
        Assert.That(module.CurrentLanguage, Is.EqualTo(DEFAULT_LANGUAGE));
        assertGetResponse("en");
        #endregion

        #region Test language the device does not support
        assertSetResponse(send(Encoding.ASCII.GetBytes("de")), null);
        foreach (string language in new[] { "es", "xx" })
        {
            assertSetResponse(send(Encoding.ASCII.GetBytes(language)), ERDM_NackReason.DATA_OUT_OF_RANGE);
            Assert.That(module.CurrentLanguage, Is.EqualTo("de"), "rejected request must not change the language");
            assertGetResponse("de");
        }
        #endregion

        #region Test invalid PDL
        foreach (byte[] data in new[] { Array.Empty<byte>(), Encoding.ASCII.GetBytes("d"), Encoding.ASCII.GetBytes("deu") })
        {
            assertSetResponse(send(data), ERDM_NackReason.FORMAT_ERROR);
            Assert.That(module.CurrentLanguage, Is.EqualTo("de"), "rejected request must not change the language");
        }
        #endregion

        #region Test set locally
        module.CurrentLanguage = "fr";
        assertGetResponse("fr");
        Assert.Throws<ArgumentOutOfRangeException>(() => module.CurrentLanguage = "es");
        Assert.That(module.CurrentLanguage, Is.EqualTo("fr"));

        // after a SET by RDM the device sets the language locally again, which was its value before the SET
        assertSetResponse(send(Encoding.ASCII.GetBytes("de")), null);
        module.CurrentLanguage = "fr";
        Assert.That(module.CurrentLanguage, Is.EqualTo("fr"), "setting the language locally after a SET must not be ignored");
        assertGetResponse("fr");
        #endregion
    }

    [Test, DependsOnTest(nameof(TestGetSetLANGUAGE))]
    public async Task TestRemoteDevice()
    {
        Assert.That(generated, Is.Not.Null);
        var generatedModule = generated.Modules.OfType<LanguageModule>().Single();

        using MockDevice mockDevice = new MockDevice(DEVCIE_UID);
        while (!mockDevice.IsInitialized)
            await Task.Delay(100);

        var module = mockDevice.Modules.OfType<LanguageModule>().Single();
        await Wait.UntilAsync(() =>
            Wait.Holds(module.CurrentLanguage, Is.EqualTo(DEFAULT_LANGUAGE))
            && Wait.Holds(module.Languages, Is.EqualTo(LANGUAGES)));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(module.CurrentLanguage, Is.EqualTo(DEFAULT_LANGUAGE));
            Assert.That(module.Languages, Is.EqualTo(LANGUAGES));
        }

        await Assert.ThatAsync(async () => await module.SetLanguage("es"), Throws.TypeOf<ArgumentOutOfRangeException>());

        Assert.That(await module.SetLanguage("de"), Is.True);
        await Wait.UntilAsync(() =>
            Wait.Holds(generatedModule.CurrentLanguage, Is.EqualTo("de"))
            && Wait.Holds(module.CurrentLanguage, Is.EqualTo("de")));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(generatedModule.CurrentLanguage, Is.EqualTo("de"));
            Assert.That(module.CurrentLanguage, Is.EqualTo("de"));
        }
    }

    private RDMMessage? send(byte[] data)
    {
        return generated!.ProcessRequestMessage_Internal(createRequest(ERDM_Parameter.LANGUAGE, ERDM_Command.SET_COMMAND, data));
    }
    private static RDMMessage createRequest(ERDM_Parameter parameter, ERDM_Command command, byte[]? data = null)
    {
        var request = new RDMMessage()
        {
            Command = command,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = parameter,
            SubDevice = SubDevice.Root,
        };
        if (data is not null)
            request.ParameterData = data;
        return request;
    }
    private void assertGetResponse(string expected)
    {
        RDMMessage? response = generated!.ProcessRequestMessage_Internal(createRequest(ERDM_Parameter.LANGUAGE, ERDM_Command.GET_COMMAND));
        Assert.That(response, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.LANGUAGE));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Is.EqualTo(Encoding.ASCII.GetBytes(expected)));
            Assert.That(response.Value, Is.EqualTo(expected));
        }
    }
    private static void assertSetResponse(RDMMessage? response, ERDM_NackReason? nackReason)
    {
        Assert.That(response, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.LANGUAGE));
            if (nackReason is null)
            {
                Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
                Assert.That(response.ParameterData, Has.Length.EqualTo(0));
            }
            else
            {
                Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
                Assert.That(response.NackReason, Is.EqualTo(nackReason));
            }
        }
    }

    class LanguageMockDevice : MockGeneratedDevice1
    {
        public LanguageMockDevice(UID uid) : base(uid, new IModule[] { new LanguageModule(DEFAULT_LANGUAGE, LANGUAGES) })
        {
        }
    }
}
