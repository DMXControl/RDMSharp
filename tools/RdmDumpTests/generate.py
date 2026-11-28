"""Generates the parser tests from Wireshark captures:
- RDMSharpTests/Metadata/Parser/PayloadToParsedObjectTestSubject.Dumps.cs (valid payloads, expected values of the dissector)
- RDMSharpTests/Metadata/Parser/TestFaultyDevicePayloads.Dumps.cs (faulty payloads, RDMMessage.Value has to be null)

Usage:
    python generate.py <folder with .pcapng files> [--max-per-key 8] [--max-faulty-per-key 4] [--cache <folder>]

1. rdm_extract.py: every capture is read once with tshark (RDM dissector), the RDM messages are cached as JSON lines.
   Captures that are already in the cache are skipped, delete the cache to read them again.
2. dumps_gen.py: selects the valid payloads and writes the test subjects, the expected values are the dissector values.
   Payloads that violate the specification are not used, they are listed in the report.
3. ExportDefineLayouts (explicit test): writes the layout of all defines (PDL, fields) as JSON.
4. faulty_gen.py: checks every payload against the define (PDL, booleans 0x00/0x01) and writes the tests for the faulty ones.
"""
import argparse, os, subprocess, sys

here = os.path.dirname(os.path.abspath(__file__))
repo = os.path.abspath(os.path.join(here, '..', '..'))
parser_tests = os.path.join(repo, 'RDMSharpTests', 'Metadata', 'Parser')
enum_cs = os.path.join(repo, 'RDMSharp', 'RDM', 'Enum', 'ERDM_Parameter.cs')

parser = argparse.ArgumentParser()
parser.add_argument('captures', help='folder with the Wireshark captures (searched recursively)')
parser.add_argument('--max-per-key', type=int, default=8, help='maximum number of valid payloads per parameter and command')
parser.add_argument('--max-faulty-per-key', type=int, default=4, help='maximum number of faulty payloads per parameter, command and kind of fault')
parser.add_argument('--cache', default=os.path.join(here, '.cache'), help='folder for the extracted RDM messages and the reports')
args = parser.parse_args()

os.makedirs(args.cache, exist_ok=True)
subprocess.run([sys.executable, os.path.join(here, 'rdm_extract.py'), args.captures, args.cache], check=True)
subprocess.run([sys.executable, os.path.join(here, 'dumps_gen.py'), args.cache, enum_cs,
                os.path.join(parser_tests, 'PayloadToParsedObjectTestSubject.Dumps.cs'),
                os.path.join(args.cache, 'report.txt'), str(args.max_per_key)], check=True)

faulty_cs = os.path.join(parser_tests, 'TestFaultyDevicePayloads.Dumps.cs')
if not os.path.exists(faulty_cs):
    # the test project has to build for the layout export, faulty_gen.py replaces this file
    with open(faulty_cs, 'w', encoding='utf-8', newline='\r\n') as f:
        f.write('namespace RDMSharpTests.Metadata.Parser;\n\npublic partial class TestFaultyDevicePayloads\n{\n'
                '    private static IEnumerable<FaultyPayload> getDumpPayloads()\n    {\n        yield break;\n    }\n}\n')

layouts = os.path.join(args.cache, 'define_layouts.json')
env = dict(os.environ, RDMSHARP_DEFINE_LAYOUTS=layouts)
subprocess.run(['dotnet', 'test', os.path.join(repo, 'RDMSharpTests'), '-f', 'net8.0',
                '--filter', 'FullyQualifiedName~RDMSharpTests.Metadata.Parser.ExportDefineLayouts'], check=True, env=env)
subprocess.run([sys.executable, os.path.join(here, 'faulty_gen.py'), args.cache, layouts, enum_cs,
                faulty_cs, os.path.join(args.cache, 'faulty_report.txt'), str(args.max_faulty_per_key)], check=True)
print('reports:', os.path.join(args.cache, 'report.txt'), os.path.join(args.cache, 'faulty_report.txt'))
