# RDM dump tests

Generates parser tests (`RDMSharpTests/Metadata/Parser/PayloadToParsedObjectTestSubject.Dumps.cs`) from Wireshark captures of real devices.

The expected values are the values decoded by the Wireshark RDM dissector, not by RDMSharp,
so the parser is checked against an independent implementation.

## Requirements

- Python 3.10 or newer
- Wireshark with `tshark`. The default path is `C:\Program Files\Wireshark\tshark.exe`, set the environment variable `TSHARK` for another one.

## Usage

```powershell
python tools/RdmDumpTests/generate.py "C:\path\to\captures"
```

The extracted RDM messages are cached in `tools/RdmDumpTests/.cache` (not part of the repository),
a capture is read only once. Delete the cache to read all captures again, e.g. after a Wireshark update.

## How it works

1. **`rdm_extract.py`** reads every capture with `tshark -Y rdm -T json --no-duplicate-keys -x` and stores per RDM message:
   parameter, command class, response type, parameter data, the dissector fields of the parameter data and the dissector warnings (expert info, PDL mismatch, checksum).
2. **`dumps_gen.py`**
   - reassembles `ACK_OVERFLOW` responses,
   - leaves out payloads that are not valid by the specification (device bugs), they are listed in the report:
     dissector warnings, characters after a NUL terminator and the rules in `DEVIATIONS` (e.g. Endpoint ID 0, texts longer than 32 characters),
   - selects up to `--max-per-key` distinct payloads per parameter and command, if possible from different manufacturers,
   - writes one test subject per payload. Texts with a NUL terminator or NUL padding get a `normalizedPayload` for the round trip tests,
     padded texts have to report the issue "More then one Null-Delimiter" (E1.20 10.1: the PDL has to match the text).
3. **`rdm_map.py`, `rdm_map_ext.py`** map the dissector fields to the properties of the parsed objects.
   A parameter that is not mapped is listed in the report ("not mapped").

Manufacturer specific parameters and the File Transfer parameters (FTC) are not generated.

## Faulty payloads

Payloads of devices that do not fit the define must not be interpreted, `RDMMessage.Value` has to be `null`
(`RDMSharpTests/Metadata/Parser/TestFaultyDevicePayloads.cs`, generated cases in `TestFaultyDevicePayloads.Dumps.cs`):

- the PDL does not fit the define (too short, too long, incomplete list item),
- a boolean field is not `0x00` or `0x01`.

4. **`ExportDefineLayouts`** (explicit test in `RDMSharpTests/Metadata/Parser`) writes the layout of all defines (PDL, fields, sizes)
   to the file given by the environment variable `RDMSHARP_DEFINE_LAYOUTS`. `generate.py` runs it with `dotnet test`.
5. **`faulty_gen.py`** checks every payload of the captures against its define and writes up to `--max-faulty-per-key` cases
   per parameter, command and kind of fault. The report is `.cache/faulty_report.txt`.

ACK_OVERFLOW responses that never ended are not checked, they are no complete payload.
