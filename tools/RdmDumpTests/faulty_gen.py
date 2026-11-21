"""Generates tests for faulty payloads of real devices: RDMMessage.Value has to be null.

A payload is faulty if
- its length does not fit the PDL of the define (too short, too long, incomplete list item) or
- a boolean field is not 0x00 or 0x01.

Usage: faulty_gen.py <cache folder> <define_layouts.json> <ERDM_Parameter.cs> <out .cs> <report> [max per parameter, command and reason]
define_layouts.json is written by the test probe described in README.md.
"""
import json, sys, os, re, glob, collections

cache_dir, layouts_json, enum_cs, out_cs, report = sys.argv[1:6]
MAX_PER_KEY = int(sys.argv[6]) if len(sys.argv) > 6 else 4

names = {}
for m in re.finditer(r'^\s*([A-Z0-9_]+)\s*=\s*(0x[0-9A-Fa-f]+|\d+)', open(enum_cs, encoding='utf-8-sig').read(), re.M):
    names.setdefault(int(m.group(2), 0), m.group(1))
CC = {0x20: ('GET_COMMAND', 'GetRequest'), 0x21: ('GET_COMMAND_RESPONSE', 'GetResponse'),
      0x30: ('SET_COMMAND', 'SetRequest'), 0x31: ('SET_COMMAND_RESPONSE', 'SetResponse')}
# answers of other parameters (QUEUED_MESSAGE) and the File Transfer parameters are not checked
SKIP = {'QUEUED_MESSAGE'}

layouts = {(l['parameter'], l['command']): l for l in json.load(open(layouts_json, encoding='utf-8')) if 'error' not in l}

class Unsupported(Exception):
    pass

def size_of(f):
    if f.get('size') is not None:
        return f['size']
    if f['kind'] == 'string' and f.get('minBytes') is not None and f.get('minBytes') == f.get('maxBytes'):
        return f['minBytes']
    return None

def check_fields(fields, data, pos, bools, path):
    """Consumes the fields, returns the new position. Records boolean values. Raises ValueError if the data does not fit."""
    for i, f in enumerate(fields):
        last = i == len(fields) - 1
        size = size_of(f)
        name = f"{path}{f.get('name') or ''}"
        if f['kind'] == 'bool':
            if pos + 1 > len(data):
                raise ValueError(f'{name}: missing')
            bools.append((name, pos, data[pos]))
            pos += 1
        elif f['kind'] == 'compound':
            pos = check_fields(f['fields'], data, pos, bools, name + '.')
        elif f['kind'] == 'list':
            if not last:
                raise Unsupported('list not at the end')
            item = f['item']
            count = 0
            while pos < len(data):
                if item['kind'] == 'compound':
                    pos = check_fields(item['fields'], data, pos, bools, f'{name}[{count}].')
                else:
                    pos = check_fields([item], data, pos, bools, f'{name}[{count}].')
                count += 1
            if f.get('maxItems') is not None and count > f['maxItems']:
                raise ValueError(f'{name}: {count} items, maximum is {f["maxItems"]}')
            if f.get('minItems') is not None and count < f['minItems']:
                raise ValueError(f'{name}: {count} items, minimum is {f["minItems"]}')
        elif size is not None:
            if pos + size > len(data):
                raise ValueError(f'{name}: {len(data) - pos} of {size} bytes')
            pos += size
        elif f['kind'] == 'string':
            if not last:
                raise Unsupported('variable string not at the end')
            pos = len(data)
        else:
            raise Unsupported(f'{f["kind"]} without fixed size')
    return pos

def check(layout, data):
    """Returns the reason why the payload is faulty, or None."""
    if layout['empty']:
        return f'PDL {len(data)}, the define has no Parameter Data' if data else None
    if layout.get('size') is not None and len(data) != layout['size']:
        return f'PDL {len(data)}, the define needs {layout["size"]}'
    if layout.get('min') is not None and len(data) < layout['min']:
        return f'PDL {len(data)}, the define needs at least {layout["min"]}'
    if layout.get('max') is not None and len(data) > layout['max']:
        return f'PDL {len(data)}, the define allows at most {layout["max"]}'
    bools = []
    try:
        pos = check_fields(layout['fields'], data, 0, bools, '')
        if pos != len(data):
            return f'PDL {len(data)}, {len(data) - pos} bytes more than the define'
    except ValueError as e:
        return f'PDL {len(data)} does not fit the define ({e})'
    for name, pos, value in bools:
        if value > 1:
            return f'Boolean {name} is 0x{value:02X}, only 0x00 and 0x01 are valid'
    return None

# ---- load and reassemble ACK_OVERFLOW per capture
msgs_by_file = collections.defaultdict(list)
for f in glob.glob(os.path.join(cache_dir, '*.jsonl')):
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
                pending[key]['pd'] += m['pd']; pending[key]['frames'].append(m['frame'])
            else:
                pending[key] = dict(m, frames=[m['frame']])
            if m['rt'] == 0:
                samples.append(pending.pop(key))
        else:
            samples.append(dict(m, frames=[m['frame']]))
    # an ACK_OVERFLOW that never ended is not a complete payload, it is not checked

faulty = collections.defaultdict(dict)  # (name, cc, reason kind) -> pd -> sample
unsupported = collections.Counter()
checked = 0
for s in samples:
    name = names.get(s['pid'])
    if name is None or name in SKIP or name.startswith('FTC_'):
        continue
    cc, cd = CC[s['cc']]
    layout = layouts.get((name, cd))
    if layout is None:
        continue
    data = bytes.fromhex(s['pd'])
    try:
        reason = check(layout, data)
    except Unsupported as e:
        unsupported[(name, cc, str(e))] += 1
        continue
    checked += 1
    if reason:
        s['reason'] = reason
        s['name'], s['ccn'] = name, cc
        s['device'] = (s['src'] if cc.endswith('RESPONSE') else s['dst']).replace(':', '')[:4].upper()
        kind = 'Boolean' if reason.startswith('Boolean') else 'PDL'
        faulty[(name, cc, kind)].setdefault(s['pd'], s)

# ---- select: different manufacturers first
selected = []
for key in sorted(faulty):
    by_dev = collections.OrderedDict()
    for s in sorted(faulty[key].values(), key=lambda s: (s['file'], s['frames'][0])):
        by_dev.setdefault(s['device'], []).append(s)
    pick = []
    while len(pick) < MAX_PER_KEY and any(by_dev.values()):
        for dev in list(by_dev):
            if by_dev[dev] and len(pick) < MAX_PER_KEY:
                pick.append(by_dev[dev].pop(0))
    selected.extend(pick)

def cs_str(v):
    return '"' + v.replace('\\', '\\\\').replace('"', '\\"') + '"'

out = []
for s in selected:
    data = bytes.fromhex(s['pd'])
    arr = ', '.join(f'0x{b:02x}' for b in data)
    uid = s['src'] if s['ccn'].endswith('RESPONSE') else s['dst']
    manf = int(uid.replace(':', '')[:4], 16)
    out.append(f'        // {s["file"]}, frame {", ".join(map(str, s["frames"]))}')
    out.append(f'        yield return new FaultyPayload(ERDM_Command.{s["ccn"]}, ERDM_Parameter.{s["name"]}, 0x{manf:04X},')
    out.append(f'            new byte[] {{ {arr} }},')
    out.append(f'            {cs_str(s["reason"])});')

header = '''namespace RDMSharpTests.Metadata.Parser;

/// <summary>
/// Faulty Payloads of real devices, captured with Wireshark. Generated by tools/RdmDumpTests/faulty_gen.py.
/// </summary>
public partial class TestFaultyDevicePayloads
{
    private static IEnumerable<FaultyPayload> getDumpPayloads()
    {
'''
footer = '''    }
}
'''
open(out_cs, 'w', encoding='utf-8', newline='\r\n').write(header + '\n'.join(out) + '\n' + footer)

with open(report, 'w', encoding='utf-8') as r:
    r.write(f'payloads checked: {checked}, faulty (distinct): {sum(len(v) for v in faulty.values())}, tests: {len(selected)}\n\n')
    r.write('== faulty payloads per parameter, command and device manufacturer\n')
    per = collections.defaultdict(lambda: [0, set(), set()])
    for (name, cc, kind), by_pd in faulty.items():
        for s in by_pd.values():
            k = (name, cc, s['device'])
            per[k][0] += 1; per[k][1].add(s['file']); per[k][2].add(s['reason'][:90])
    for (name, cc, dev), (n, files, reasons) in sorted(per.items()):
        r.write(f'  {name} {cc} | manf 0x{dev} | {n} | {"; ".join(sorted(reasons))[:200]} | {"; ".join(sorted(files))[:160]}\n')
    r.write('\n== not checked (layout not supported)\n')
    for k, v in unsupported.most_common():
        r.write(f'  {k[0]} {k[1]}: {k[2]} ({v})\n')
print('checked', checked, 'faulty', sum(len(v) for v in faulty.values()), 'tests', len(selected), 'unsupported keys', len(unsupported))
