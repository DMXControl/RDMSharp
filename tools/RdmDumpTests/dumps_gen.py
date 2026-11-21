"""Generates parser test subjects from all extracted Wireshark dumps.
Expected values come from the Wireshark RDM dissector (independent of RDMSharp's parser).
Samples the dissector warns about, or with bytes after a NUL terminator, are not used as tests, but reported."""
import json, sys, os, re, glob, collections
sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from rdm_map import MAP, num, PD
import rdm_map_ext

dumps_dir, enum_cs, out_cs, report = sys.argv[1:5]
MAX_PER_KEY = int(sys.argv[5]) if len(sys.argv) > 5 else 8
# "NAME|CC|pd" -> reason, for samples that turned out to be spec violations of the device
EXCLUDE = json.load(open(sys.argv[6], encoding='utf-8')) if len(sys.argv) > 6 and os.path.exists(sys.argv[6]) else {}
EXCLUDE_REASONS = EXCLUDE

names = {}
for m in re.finditer(r'^\s*([A-Z0-9_]+)\s*=\s*(0x[0-9A-Fa-f]+|\d+)', open(enum_cs, encoding='utf-8-sig').read(), re.M):
    names.setdefault(int(m.group(2), 0), m.group(1))
CC = {0x20: 'GET_COMMAND', 0x21: 'GET_COMMAND_RESPONSE', 0x30: 'SET_COMMAND', 0x31: 'SET_COMMAND_RESPONSE'}

# offset of the trailing String inside the Parameter Data of a response
STRING_PREFIX = {
    'DEVICE_LABEL': 0, 'MANUFACTURER_LABEL': 0, 'DEVICE_MODEL_DESCRIPTION': 0, 'SOFTWARE_VERSION_LABEL': 0,
    'BOOT_SOFTWARE_VERSION_LABEL': 0, 'DMX_PERSONALITY_DESCRIPTION': 3, 'SENSOR_DEFINITION': 13,
    'LOCK_STATE_DESCRIPTION': 1, 'ENDPOINT_TIMING_DESCRIPTION': 1, 'ENDPOINT_LABEL': 2, 'PARAMETER_DESCRIPTION': 20,
    'SLOT_DESCRIPTION': 2, 'CURVE_DESCRIPTION': 1, 'MODULATION_FREQUENCY_DESCRIPTION': 5, 'OUTPUT_RESPONSE_TIME_DESCRIPTION': 1,
    'SELF_TEST_DESCRIPTION': 1, 'STATUS_ID_DESCRIPTION': 0, 'SERIAL_NUMBER': 0, 'MANUFACTURER_URL': 0, 'PRODUCT_URL': 0,
    'FIRMWARE_URL': 0, 'INTERFACE_LABEL': 4, 'SENSOR_TYPE_CUSTOM': 1, 'SENSOR_UNIT_CUSTOM': 1, 'LANGUAGE': 0,
}

# payloads that are not valid by the specification (device bugs), they are reported instead of tested
def _text(data, prefix):
    return data[prefix:].split(b'\x00')[0]
DEVIATIONS = [
    ('FACTORY_DEFAULTS', 'GET_COMMAND_RESPONSE', lambda d, s: d[0] > 1, 'Boolean other than 0x00/0x01 (E1.20 10.4.1)'),
    ('DMX_BLOCK_ADDRESS', 'GET_COMMAND_RESPONSE', lambda d, s: int.from_bytes(d[2:4], 'big') == 0, 'Base DMX512 Address 0, valid is 1-512 or 0xFFFF (E1.20 10.6.4)'),
    ('LANGUAGE_CAPABILITIES', 'GET_COMMAND_RESPONSE', lambda d, s: any(not d[i:i + 2].isalpha() for i in range(0, len(d), 2)), 'no ISO 639-1 code (E1.20 10.5.7)'),
] + [
    (p, 'GET_COMMAND_RESPONSE', lambda d, s: int.from_bytes(d[0:2], 'big') == 0, 'Endpoint ID 0, valid is 1-63999 (E1.37-7)')
    for p in ('ENDPOINT_TO_UNIVERSE', 'ENDPOINT_MODE', 'ENDPOINT_LABEL', 'IDENTIFY_ENDPOINT', 'RDM_TRAFFIC_ENABLE', 'DISCOVERY_STATE',
              'BACKGROUND_DISCOVERY', 'ENDPOINT_TIMING', 'ENDPOINT_RESPONDERS', 'ENDPOINT_RESPONDER_LIST_CHANGE')
] + [
    (p, 'GET_COMMAND_RESPONSE', (lambda pre: lambda d, s: len(_text(d, pre)) > 32)(pre), 'Text longer than 32 characters (E1.20 10.1)')
    for p, pre in (('DEVICE_LABEL', 0), ('MANUFACTURER_LABEL', 0), ('DEVICE_MODEL_DESCRIPTION', 0), ('SOFTWARE_VERSION_LABEL', 0),
                   ('BOOT_SOFTWARE_VERSION_LABEL', 0), ('DMX_PERSONALITY_DESCRIPTION', 3), ('SENSOR_DEFINITION', 13),
                   ('PARAMETER_DESCRIPTION', 20), ('SLOT_DESCRIPTION', 2), ('CURVE_DESCRIPTION', 1),
                   ('MODULATION_FREQUENCY_DESCRIPTION', 5), ('OUTPUT_RESPONSE_TIME_DESCRIPTION', 1), ('SELF_TEST_DESCRIPTION', 1),
                   ('STATUS_ID_DESCRIPTION', 0), ('LOCK_STATE_DESCRIPTION', 1), ('ENDPOINT_TIMING_DESCRIPTION', 1), ('ENDPOINT_LABEL', 2))
]

def manf(uid):
    return uid.replace(':', '')[:4].upper()

# ---- load and reassemble ACK_OVERFLOW per file
msgs_by_file = collections.defaultdict(list)
for f in glob.glob(os.path.join(dumps_dir, '*.jsonl')):
    for line in open(f, encoding='utf-8'):
        m = json.loads(line)
        msgs_by_file[m['file']].append(m)

samples = []
for file, msgs in msgs_by_file.items():
    msgs.sort(key=lambda m: m['frame'])
    pending = {}
    for m in msgs:
        if m['cc'] not in CC:
            continue
        if m['cc'] in (0x21, 0x31):
            if m['rt'] not in (0, 3):
                continue
            key = (m['src'], m['dst'], m['pid'], m['cc'], m['sd'])
            if key in pending:
                p = pending[key]
                p['pd'] += m['pd']; p['frames'].append(m['frame']); p['fields'] += m['fields']; p['warn'] += m['warn']
            else:
                pending[key] = dict(m, frames=[m['frame']], fields=list(m['fields']), warn=list(m['warn']))
            if m['rt'] == 0:
                samples.append(pending.pop(key))
        else:
            samples.append(dict(m, frames=[m['frame']]))
    for p in pending.values():  # overflow never finished
        p['warn'].append('ACK_OVERFLOW not finished')
        samples.append(p)

# ---- classify
findings = collections.defaultdict(list)
cands = collections.defaultdict(dict)  # (name, cc) -> pd -> sample
unmapped = collections.Counter()
for s in samples:
    name = names.get(s['pid'])
    cc = CC[s['cc']]
    if name is None:
        continue  # manufacturer specific
    s['name'], s['ccn'] = name, cc
    s['device'] = manf(s['src'] if cc.endswith('RESPONSE') else s['dst'])
    key = (name, cc)
    data = bytes.fromhex(s['pd'])
    s['normalized'] = None
    s['padded'] = False
    reason = None
    if s['warn']:
        reason = 'dissector: ' + ', '.join(sorted(set(s['warn'])))
    elif cc.endswith('RESPONSE') and name in STRING_PREFIX and len(data) > STRING_PREFIX[name]:
        prefix = STRING_PREFIX[name]
        text = data[prefix:]
        if b'\x00' in text:
            head, _, tail = text.partition(b'\x00')
            if tail.strip(b'\x00'):
                reason = f'characters after the NUL terminator ({tail.hex()})'
            else:
                s['normalized'] = data[:prefix] + head
                s['padded'] = len(tail) > 0
    if f'{name}|{cc}|{s["pd"]}' in EXCLUDE:
        reason = reason or EXCLUDE_REASONS.get(f'{name}|{cc}|{s["pd"]}', 'excluded')
    if not reason:
        for d_name, d_cc, test, why in DEVIATIONS:
            if name == d_name and cc == d_cc:
                try:
                    if test(data, s):
                        reason = why
                        break
                except Exception:
                    pass
    if reason:
        findings[(name, cc)].append(dict(file=s['file'], frames=s['frames'], device=s['device'], pd=s['pd'], reason=reason))
        continue
    if key not in MAP:
        if s['pd']:
            unmapped[key] += 1
        continue
    cands[key].setdefault(s['pd'], s)

# ---- select: round robin over the device manufacturers, distinct payloads
selected = {}
for key, by_pd in cands.items():
    by_dev = collections.OrderedDict()
    for s in sorted(by_pd.values(), key=lambda s: (s['file'], s['frames'][0])):
        by_dev.setdefault(s['device'], []).append(s)
    pick = []
    while len(pick) < MAX_PER_KEY and any(by_dev.values()):
        for dev in list(by_dev):
            if by_dev[dev] and len(pick) < MAX_PER_KEY:
                pick.append(by_dev[dev].pop(0))
    selected[key] = pick

# ---- generate
def values(fl, key):
    out = []
    for k, v in fl:
        if k == PD + key:
            out.extend(v if isinstance(v, list) else [v])
    return out

def first(fl, key):
    v = values(fl, key)
    if not v:
        raise KeyError(key)
    return v[0]

import rdm_map
rdm_map.values = values
rdm_map.first = first
rdm_map_ext.values = values
rdm_map_ext.first = first

ind = '                    '
def asserts_for(spec, fl):
    lines = []
    if spec == 'EMPTY':
        lines.append(f'{ind}Assert.That(obj, Is.Empty);')
    elif isinstance(spec, str) and spec.startswith('LIST:'):
        _, key, enum = spec.split(':')
        items = [f'({enum})0x{num(v):04X}' for v in values(fl, key)]
        lines.append(f'{ind}Assert.That(obj, Is.EqualTo(new {enum}[] {{ {", ".join(items)} }}));')
    elif isinstance(spec, str) and spec.startswith('UIDS:'):
        _, key, prop = spec.split(':')
        items = [rdm_map.uid(v) for v in values(fl, key)]
        lines.append(f'{ind}Assert.That(obj.{prop}, Is.EqualTo(new UID[] {{ {", ".join(items)} }}));')
    elif spec == 'ENDPOINTS':
        lines.append(f'{ind}Assert.That(obj.ListChangedNumber, Is.EqualTo({num(first(fl, "endpoint_list.change_number"))}u));')
        ids = [num(v) for v in values(fl, 'endpoint_list.endpoint_id')]
        types = [num(v) for v in values(fl, 'endpoint_list.endpoint_type')]
        lines.append(f'{ind}Assert.That(obj.Endpoints.Select(e => e.EndpointId), Is.EqualTo(new ushort[] {{ {", ".join(map(str, ids))} }}));')
        lines.append(f'{ind}Assert.That(obj.Endpoints.Select(e => e.EndpointType), Is.EqualTo(new ERDM_EndpointType[] {{ {", ".join(f"(ERDM_EndpointType){t}" for t in types)} }}));')
    elif spec == 'RESPONDERS':
        lines.append(f'{ind}Assert.That(obj.EndpointId, Is.EqualTo((ushort){num(first(fl, "endpoint_responders.endpoint_id"))}));')
        lines.append(f'{ind}Assert.That(obj.ListChangedNumber, Is.EqualTo({num(first(fl, "endpoint_responders.change_number"))}u));')
        items = [rdm_map.uid(v) for v in values(fl, 'endpoint_responders.uid')]
        lines.append(f'{ind}Assert.That(obj.UIDs, Is.EqualTo(new UID[] {{ {", ".join(items)} }}));')
    elif callable(spec):
        lines.extend(ind + l for l in spec(fl))
    else:
        for key, prop, conv, cmp in spec:
            raw = first(fl, key)
            target = 'obj' if prop is None else f'obj.{prop}'
            if conv is rdm_map.S and raw == '':
                # an empty text field is parsed as null
                lines.append(f'{ind}Assert.That({target}, Is.Null.Or.Empty);')
                continue
            expected = conv(raw)
            within = '.Within(0.001)' if cmp == 'D' else ''
            lines.append(f'{ind}Assert.That({target}, Is.EqualTo({expected}){within});')
    return lines

def arr(b):
    return ', '.join(f'0x{x:02x}' for x in b)

out, count, gen_errors = [], 0, []
for key in sorted(selected):
    name, cc = key
    typ, spec = MAP[key]
    block = []
    for s in selected[key]:
        try:
            body = asserts_for(spec, s['fields'])
        except Exception as e:
            gen_errors.append(f'{name} {cc} {s["file"]} {s["frames"]}: {e!r}')
            continue
        data = bytes.fromhex(s['pd'])
        comment = f'        // {s["file"]}, frame {", ".join(map(str, s["frames"]))}' + (' (ACK_OVERFLOW)' if len(s['frames']) > 1 else '')
        if s['padded']:
            comment += ', String padded with NUL'
        elif s['normalized'] is not None:
            comment += ', String with NUL terminator'
        block.append(comment)
        block.append('        yield return new PayloadToParseBagData(')
        block.append(f'            ERDM_Command.{cc},')
        block.append(f'            ERDM_Parameter.{name},')
        block.append(f'            new byte[] {{ {arr(data)} }},')
        block.append('            new Action<DataTreeBranch>((dataTreeBranch) =>')
        block.append('            {')
        block.append('                Assert.That(dataTreeBranch.IsUnset, Is.False);')
        if s['padded']:
            block.append('                // E1.20 10.1: the PDL has to match the String, but a NUL terminates it, the padding is reported')
            block.append('                Assert.That(allIssues(dataTreeBranch).Select(i => i.ToString()), Has.Some.Contains("Null-Delimiter"));')
        else:
            block.append('                Assert.That(allIssues(dataTreeBranch), Is.Empty);')
        block.append(f'                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof({typ})));')
        block.append(f'                var obj = ({typ})dataTreeBranch.ParsedObject!;')
        block.append('                using (Assert.EnterMultipleScope())')
        block.append('                {')
        block.extend(body)
        block.append('                }')
        block.append('            })' + (f', normalizedPayload: new byte[] {{ {arr(s["normalized"])} }}' if s['normalized'] is not None else '') + ');')
        count += 1
    if block:
        out.append(f'        #region {name} {cc}')
        out.extend(block)
        out.append('        #endregion')
        out.append('')

header = '''using RDMSharp.Metadata;
using RDMSharp.PayloadObject;

namespace RDMSharpTests.Metadata.Parser;

/// <summary>
/// Payloads of real devices, captured with Wireshark (Art-Net RDM and others).
/// The expected values are the values decoded by the Wireshark RDM dissector, not by RDMSharp,
/// so the parser is checked against an independent implementation. ACK_OVERFLOW responses are reassembled.
/// Payloads the dissector warns about, or that are not valid by the specification, are not part of these tests.
/// Generated, see the comment of every Payload for the capture and the frame.
/// </summary>
public partial class PayloadToParsedObjectTestSubject
{
    private static IEnumerable<DataTreeIssue> allIssues(DataTreeBranch branch)
        => (branch.Children ?? Array.Empty<DataTree>()).SelectMany(allIssues);
    private static IEnumerable<DataTreeIssue> allIssues(DataTree tree)
        => (tree.Issues ?? Array.Empty<DataTreeIssue>()).Concat((tree.Children ?? Array.Empty<DataTree>()).SelectMany(allIssues));

    private static IEnumerable<PayloadToParseBagData> getDumpPayloadList()
    {
'''
footer = '''    }
}
'''
open(out_cs, 'w', encoding='utf-8', newline='\r\n').write(header + '\n'.join(out).rstrip() + '\n' + footer)

with open(report, 'w', encoding='utf-8') as r:
    r.write(f'samples total: {len(samples)}, generated: {count}\n\n')
    r.write('== not mapped (parameter, command): distinct samples\n')
    for k, v in unmapped.most_common():
        r.write(f'  {k[0]} {k[1]}: {v}\n')
    r.write('\n== generator errors\n')
    for e in gen_errors:
        r.write('  ' + e + '\n')
    r.write('\n== findings (not used as tests)\n')
    for k in sorted(findings):
        seen = set()
        for f in findings[k]:
            sig = (f['device'], f['reason'], f['pd'])
            if sig in seen: continue
            seen.add(sig)
            r.write(f'  {k[0]} {k[1]} | {f["file"]} frame {f["frames"][0]} | device manf 0x{f["device"]} | {f["reason"]} | pd={f["pd"][:80]}\n')
print('generated', count, 'unmapped keys', len(unmapped), 'finding keys', len(findings), 'gen errors', len(gen_errors))
