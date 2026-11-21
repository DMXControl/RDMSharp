"""Extracts all RDM messages of all pcapng files of a folder with tshark.
One JSON-lines file per capture: frame, src, dst, cc, pid, rt, sd, pd (hex), dissector fields of the parameter data, warnings."""
import json, os, subprocess, sys, glob, hashlib

TSHARK = os.environ.get('TSHARK') or (r'C:\Program Files\Wireshark\tshark.exe' if os.name == 'nt' else 'tshark')
src_dir, out_dir = sys.argv[1], sys.argv[2]
os.makedirs(out_dir, exist_ok=True)

def walk(x, out, warn):
    if isinstance(x, dict):
        for k, v in x.items():
            if k.endswith('_raw'):
                continue
            if k.startswith('_ws.') or k in ('rdm.pdl.mismatch',):
                warn.append(k if not isinstance(v, (str, list)) else f'{k}={v}')
            if k == 'rdm.checksum.status' and str(v) != '1':
                warn.append(f'checksum.status={v}')
            if k.startswith('rdm.pd') and k != 'rdm.pd' and not k.endswith('_tree'):
                if isinstance(v, list) and all(isinstance(i, str) for i in v):
                    out.append((k, v))
                elif isinstance(v, str):
                    out.append((k, v))
            if isinstance(v, (dict, list)):
                walk(v, out, warn)
    elif isinstance(x, list):
        for i in x:
            walk(i, out, warn)

files = sorted(glob.glob(os.path.join(src_dir, '**', '*.pcap*'), recursive=True))
for f in files:
    name = os.path.relpath(f, src_dir)
    out_file = os.path.join(out_dir, hashlib.md5(name.encode()).hexdigest() + '.jsonl')
    if os.path.exists(out_file) or os.path.getsize(f) == 0:
        continue
    p = subprocess.run([TSHARK, '-r', f, '-Y', 'rdm', '-T', 'json', '--no-duplicate-keys', '-x'], capture_output=True)
    count = 0
    with open(out_file + '.tmp', 'w', encoding='utf-8') as o:
        try:
            packets = json.loads(p.stdout.decode('utf-8', 'replace') or '[]')
        except Exception as e:
            print('JSON error', name, e, flush=True)
            packets = []
        for pk in packets:
            l = pk['_source']['layers']
            frame = int(l['frame']['frame.number'])
            rdms = l.get('rdm')
            raws = l.get('rdm_raw')
            if rdms is None or raws is None:
                continue
            if isinstance(rdms, dict):
                rdms, raws = [rdms], [raws]
            elif raws and isinstance(raws[0], str):
                raws = [raws]
            for r, raw in zip(rdms, raws):
                try:
                    mdb = r.get('rdm.mdb', {})
                    if 'rdm.pid' not in mdb:
                        continue
                    rb = bytes.fromhex(raw[0])
                    pdl = int(mdb['rdm.pdl'])
                    # rdm_raw starts at the Sub-START Code (Art-Net) or at the START Code
                    off = 23 if rb[0] == 0x01 else 24
                    pd = rb[off:off + pdl]
                    fields, warn = [], []
                    walk(r, fields, warn)
                    walk({k: v for k, v in l.items() if k.startswith('_ws')}, [], warn)
                    if len(pd) != pdl:
                        warn.append('pd shorter than pdl')
                    o.write(json.dumps(dict(file=name, frame=frame,
                        src=r.get('rdm.src', ''), dst=r.get('rdm.dst', ''),
                        cc=int(mdb['rdm.cc'], 0), pid=int(mdb['rdm.pid'], 0),
                        rt=int(r.get('rdm.rt', '0x00'), 0) if 'rdm.rt' in r else None,
                        sd=int(r.get('rdm.sd', '0'), 0), pd=pd.hex(), fields=fields, warn=warn)) + '\n')
                    count += 1
                except Exception as e:
                    print('error', name, frame, e, flush=True)
    os.replace(out_file + '.tmp', out_file)
    print(f'{count:6d} {name}', flush=True)
print('DONE', flush=True)
