using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RDMSharp.RDM.Device.Module;

/// <summary>
/// LANGUAGE_CAPABILITIES (E1.20 10.5.7) and LANGUAGE (E1.20 10.5.8).
/// <list type="bullet">
/// <item>The languages are 2 character ISO 639-1 codes, LANGUAGE_CAPABILITIES reports them as a packed list.</item>
/// <item>LANGUAGE can only be set to one of these languages, other languages are NACKed with DATA_OUT_OF_RANGE.</item>
/// <item>The Language Code 0x0000 resets the device to its default language.</item>
/// </list>
/// </summary>
public sealed class LanguageModule : AbstractModule
{
    private const string _moduleName = "Language";
    private const string _moduleDisplayName = "Language";
    private static readonly ERDM_Parameter[] _moduleParameters = new ERDM_Parameter[]
    {
        ERDM_Parameter.LANGUAGE_CAPABILITIES,
        ERDM_Parameter.LANGUAGE
    };
    private const int LANGUAGE_CODE_LENGTH = 2;
    // the PDL of LANGUAGE_CAPABILITIES is at most 0xE6 (230 bytes)
    private const int MAX_LANGUAGES = 0xE6 / LANGUAGE_CODE_LENGTH;

    public override string DisplayName => _moduleDisplayName;

    private readonly string[] _generatedLanguages = null;
    private readonly string _defaultLanguage = null;

    /// <summary>
    /// The default language of a generated device, it is unknown for a remote device.
    /// </summary>
    public string DefaultLanguage => _defaultLanguage;

    public IReadOnlyCollection<string> Languages
    {
        get
        {
            if (_generatedLanguages is not null)
                return _generatedLanguages;
            if (ParentDevice is not null && ParentDevice.GetAllParameterValues().TryGetValue(ERDM_Parameter.LANGUAGE_CAPABILITIES, out object res) && res is string[] languages)
                return languages;
            return Array.Empty<string>();
        }
    }

    private bool initialized = false;
    private string currentLanguage;
    public string CurrentLanguage
    {
        get
        {
            if (ParentDevice is null)
                return currentLanguage;
            if (ParentDevice.GetAllParameterValues().TryGetValue(ERDM_Parameter.LANGUAGE, out object res) && res is string language)
                return language;
            return currentLanguage;
        }
        set
        {
            if (value is null)
                return;

            string language = value;
            if (_generatedLanguages is not null)
                language = findGeneratedLanguage(value) ?? throw new ArgumentOutOfRangeException(nameof(value), $"The Language {value} is not supported, supported are: {string.Join(", ", _generatedLanguages)}");

            // compare with the stored value of the device, the field is outdated after a SET by RDM
            if (initialized)
                if (string.Equals(CurrentLanguage, language))
                    return;

            currentLanguage = language;
            initialized = true;
            ParentGeneratedDevice?.setParameterValue(ERDM_Parameter.LANGUAGE, language);

            if (ParentRemoteDevice is not null)
                _ = SetLanguage(language);
        }
    }

    /// <param name="defaultLanguage">The language after startup and after a reset (Language Code 0x0000)</param>
    /// <param name="languages">The supported languages as ISO 639-1 codes</param>
    public LanguageModule(string defaultLanguage, params string[] languages) : base(
        _moduleName,
        _moduleParameters)
    {
        if (languages is null || languages.Length == 0)
            throw new ArgumentException("At least one Language is needed", nameof(languages));
        if (languages.Length > MAX_LANGUAGES)
            throw new ArgumentOutOfRangeException(nameof(languages), $"There are to many Languages! Maximum is {MAX_LANGUAGES}");

        var normalized = new List<string>();
        foreach (string language in languages)
        {
            if (!isLanguageCode(language))
                throw new ArgumentException($"{language} is not a 2 character ISO 639-1 code", nameof(languages));
            string code = language.ToLowerInvariant();
            if (normalized.Contains(code))
                throw new ArgumentException($"{language} is used more then once", nameof(languages));
            normalized.Add(code);
        }
        _generatedLanguages = normalized.ToArray();

        _defaultLanguage = findGeneratedLanguage(defaultLanguage) ?? throw new ArgumentOutOfRangeException(nameof(defaultLanguage), $"The default Language {defaultLanguage} is not one of the Languages");
    }
    public LanguageModule(AbstractRemoteRDMDevice remoteDevice) : base(
        remoteDevice,
        _moduleName,
        _moduleParameters)
    {
    }

    private static bool isLanguageCode(string language)
    {
        return language is not null
            && language.Length == LANGUAGE_CODE_LENGTH
            && language.All(c => (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z'));
    }
    private string findGeneratedLanguage(string language)
    {
        if (language is null)
            return null;
        return _generatedLanguages?.FirstOrDefault(l => string.Equals(l, language, StringComparison.OrdinalIgnoreCase));
    }

    protected override void OnParentGeneratedDeviceChanged(AbstractGeneratedRDMDevice device)
    {
        device.setParameterValue(ERDM_Parameter.LANGUAGE_CAPABILITIES, _generatedLanguages);
        this.CurrentLanguage = _defaultLanguage;
    }
    protected override void ParameterChanged(ERDM_Parameter parameter, object newValue, object index)
    {
        switch (parameter)
        {
            case ERDM_Parameter.LANGUAGE_CAPABILITIES:
                OnPropertyChanged(nameof(Languages));
                break;
            case ERDM_Parameter.LANGUAGE:
                OnPropertyChanged(nameof(CurrentLanguage));
                break;
        }
    }

    public override bool IsHandlingParameter(ERDM_Parameter parameter, ERDM_Command command)
    {
        if (ParentGeneratedDevice is not null && parameter == ERDM_Parameter.LANGUAGE)
            return command == ERDM_Command.SET_COMMAND;
        return base.IsHandlingParameter(parameter, command);
    }
    protected override RDMMessage handleRequest(RDMMessage message)
    {
        if (message.Parameter == ERDM_Parameter.LANGUAGE && message.Command == ERDM_Command.SET_COMMAND)
        {
            if (message.PDL != LANGUAGE_CODE_LENGTH || message.Value is not string requestedLanguage)
                return createResponse(message, ERDM_NackReason.FORMAT_ERROR);

            // Language Code 0x0000 resets the device to the default language, it is parsed as an empty String
            if (requestedLanguage.Length == 0)
            {
                CurrentLanguage = _defaultLanguage;
                return createResponse(message);
            }

            string language = findGeneratedLanguage(requestedLanguage);
            if (language is null)
                return createResponse(message, ERDM_NackReason.DATA_OUT_OF_RANGE);

            CurrentLanguage = language;
            return createResponse(message);
        }
        return base.handleRequest(message);
    }

    private static RDMMessage createResponse(RDMMessage request, ERDM_NackReason? nackReason = null)
    {
        var response = nackReason.HasValue ? new RDMMessage(nackReason.Value) : new RDMMessage();
        response.SourceUID = request.DestUID;
        response.DestUID = request.SourceUID;
        response.SubDevice = request.SubDevice;
        response.Command = request.Command | ERDM_Command.RESPONSE;
        response.Parameter = request.Parameter;
        return response;
    }

    public async Task<bool> SetLanguage(string language)
    {
        if (ParentGeneratedDevice is not null)
        {
            CurrentLanguage = language;
            return true;
        }
        if (ParentRemoteDevice is not null)
        {
            if (!isLanguageCode(language))
                throw new ArgumentException($"{language} is not a 2 character ISO 639-1 code", nameof(language));
            var languages = Languages;
            if (languages.Count != 0 && !languages.Any(l => string.Equals(l, language, StringComparison.OrdinalIgnoreCase)))
                throw new ArgumentOutOfRangeException(nameof(language), $"The Language {language} is not supported, supported are: {string.Join(", ", languages)}");
            return await ParentRemoteDevice.SetParameter(ERDM_Parameter.LANGUAGE, language);
        }
        return false;
    }
}
