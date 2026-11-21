using RDMSharp.Metadata;
using RDMSharp.RDM.Device.Module;
using RDMSharpTests.Devices.Mock;

namespace RDMSharpTests.RDM.Devices.Modules;

/// <summary>
/// The corners of the <see cref="LanguageModule"/>: wrong arguments, a module that is not part of a device, invalid languages and a faulty request.
/// </summary>
public class TestLanguageModuleEdgeCases
{
    private static readonly UID CONTROLLER_UID = new UID(0x1fff + (ushort)ERDM_Parameter.LANGUAGE, 334);
    private static readonly UID DEVICE_UID = new UID(0x0a5b, 0x00000F10);

    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        await MetadataFactory.AwaitInitialize();
    }

    #region Constructor
    [Test]
    public void TestLanguagesAreRequired()
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.Throws<ArgumentException>(() => new LanguageModule("en", (string[])null!));
            Assert.Throws<ArgumentException>(() => new LanguageModule("en", Array.Empty<string>()));
        }
    }

    [Test]
    public void TestTooManyLanguages()
    {
        // LANGUAGE_CAPABILITIES has a PDL of at most 230 bytes: 115 languages
        var languages = Enumerable.Range(0, 116).Select(i => $"{(char)('a' + i / 26)}{(char)('a' + i % 26)}").ToArray();
        Assert.That(languages.Distinct().Count(), Is.EqualTo(116), "precondition");

        Assert.Throws<ArgumentOutOfRangeException>(() => new LanguageModule("aa", languages));
        Assert.DoesNotThrow(() => new LanguageModule("aa", languages.Take(115).ToArray()));
    }

    [Test]
    public void TestDefaultLanguageHasToBeOneOfTheLanguages()
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new LanguageModule("fr", "en", "de"));
            Assert.Throws<ArgumentOutOfRangeException>(() => new LanguageModule(null!, "en", "de"));
        }
    }
    #endregion

    #region A module that is not part of a device
    [Test]
    public async Task TestModuleWithoutADevice()
    {
        var module = new LanguageModule("en", "en", "de");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(module.Languages, Is.EqualTo(new[] { "en", "de" }));
            Assert.That(module.DefaultLanguage, Is.EqualTo("en"));
            Assert.That(module.CurrentLanguage, Is.Null, "nothing was set yet");
        }

        module.CurrentLanguage = null!; // nothing happens
        Assert.That(module.CurrentLanguage, Is.Null);

        module.CurrentLanguage = "DE";
        module.CurrentLanguage = "de"; // the same language again
        Assert.That(module.CurrentLanguage, Is.EqualTo("de"), "the language of the list");

        Assert.Throws<ArgumentOutOfRangeException>(() => module.CurrentLanguage = "fr");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await module.SetLanguage("en"), Is.False, "there is no device to set it on");
            Assert.That(await module.ResetToDefaultLanguage(), Is.False, "there is no device to reset");
        }
    }
    #endregion

    #region Generated device
    [Test]
    public async Task TestSetLanguageOfAGeneratedDevice()
    {
        using var generated = new MockGeneratedDevice1(DEVICE_UID, new IModule[] { new LanguageModule("en", "en", "de") });
        var module = generated.Modules.OfType<LanguageModule>().Single();
        Assert.That(module.CurrentLanguage, Is.EqualTo("en"));

        Assert.That(await module.SetLanguage("de"), Is.True);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(module.CurrentLanguage, Is.EqualTo("de"));
            Assert.That(generated.GetAllParameterValues()[ERDM_Parameter.LANGUAGE], Is.EqualTo("de"));
        }
    }

    [Test]
    public void TestSetWithAWrongPdlIsAFormatError()
    {
        using var generated = new MockGeneratedDevice1(DEVICE_UID, new IModule[] { new LanguageModule("en", "en", "de") });
        var module = generated.Modules.OfType<LanguageModule>().Single();

        // a language code has 2 bytes; the module checks it itself, no matter who sent the request
        var response = module.HandleRequest(new RDMMessage()
        {
            Command = ERDM_Command.SET_COMMAND,
            Parameter = ERDM_Parameter.LANGUAGE,
            DestUID = DEVICE_UID,
            SourceUID = CONTROLLER_UID,
            SubDevice = SubDevice.Root,
            ParameterData = new byte[] { 0x64, 0x65, 0x00 }
        });

        Assert.That(response, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response!.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
            Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.FORMAT_ERROR));
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND_RESPONSE));
            Assert.That(module.CurrentLanguage, Is.EqualTo("en"), "the language did not change");
        }
    }
    #endregion

    #region Remote device
    [Test, MaxTime(60000)]
    public async Task TestRemoteDeviceWithoutLanguageValues()
    {
        // the device has no LANGUAGE at all, the module of the remote device knows nothing
        // (an own device model: devices of one model share what the model delivered, the other tests have languages)
        using var generated = new NoLanguageMockDevice(DEVICE_UID);
        using MockDevice mockDevice = new MockDevice(DEVICE_UID);
        while (!mockDevice.IsInitialized)
            await Task.Delay(100);

        var module = new LanguageModule(mockDevice);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(module.Languages, Is.Empty);
            Assert.That(module.CurrentLanguage, Is.Null);
            Assert.That(module.DefaultLanguage, Is.Null, "a remote device does not tell its default language");
        }
    }

    [Test, MaxTime(60000)]
    public async Task TestRemoteDeviceSetsTheLanguage()
    {
        using var generated = new MockGeneratedDevice1(DEVICE_UID, new IModule[] { new LanguageModule("en", "en", "de") });
        var generatedModule = generated.Modules.OfType<LanguageModule>().Single();

        using MockDevice mockDevice = new MockDevice(DEVICE_UID);
        while (!mockDevice.IsInitialized)
            await Task.Delay(100);
        while (!mockDevice.AllDataPulled)
            await Task.Delay(100);
        var module = mockDevice.Modules.OfType<LanguageModule>().Single();
        await Wait.UntilAsync(() => Wait.Holds(module.Languages, Has.Count.EqualTo(2)));

        // a language that is not a code, and one the device does not have, are not even sent
        using (Assert.EnterMultipleScope())
        {
            Assert.That(async () => await module.SetLanguage("deutsch"), Throws.TypeOf<ArgumentException>());
            Assert.That(async () => await module.SetLanguage("fr"), Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        // the property sets it on the device, the same way SetLanguage does
        module.CurrentLanguage = "de";
        await Wait.UntilAsync(() => Wait.Holds(generatedModule.CurrentLanguage, Is.EqualTo("de")));
        Assert.That(generatedModule.CurrentLanguage, Is.EqualTo("de"));
    }
    #endregion

    private sealed class NoLanguageMockDevice : MockGeneratedDevice1
    {
        public override ushort DeviceModelID => 0x4D4D;

        public NoLanguageMockDevice(UID uid) : base(uid)
        {
        }
    }
}
