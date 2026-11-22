#!/usr/bin/env python3
"""Fetch what the solution needs to compile on Linux, into src/packages.

Does the job of `nuget restore` (which needs Windows or Mono) for this repo:
  * every package named in a packages.config, unpacked as <Id>.<Version>/
  * the .NET Framework 4.8 reference assemblies, so the compiler knows System.Web & co.
Linux file names are case-sensitive and Windows' are not, so any HintPath in a
project file that only differs by case from a real file gets a link under its
spelling. Run from anywhere; safe to run again.
"""
import io, os, re, sys, urllib.request, zipfile, glob
import xml.etree.ElementTree as ET

ROOT = os.path.normpath(os.path.join(os.path.dirname(os.path.abspath(__file__)), '..'))
PACKAGES = os.path.join(ROOT, 'src', 'packages')
REFASM = ('Microsoft.NETFramework.ReferenceAssemblies.net48', '1.0.3')

def normalized(version):
    """nuget.org's spelling of a version: 1.4.0.0 -> 1.4.0, 2.3 -> 2.3.0"""
    core, sep, rest = version.partition('-')
    parts = core.split('.')
    while len(parts) < 3: parts.append('0')
    if len(parts) == 4 and parts[3] == '0': parts = parts[:3]
    return ('.'.join(str(int(p)) for p in parts) + sep + rest).lower()

def fetch(pid, version):
    target = os.path.join(PACKAGES, f'{pid}.{version}')
    if os.path.isdir(target) and os.listdir(target): return False
    lid, lver = pid.lower(), normalized(version)
    url = f'https://api.nuget.org/v3-flatcontainer/{lid}/{lver}/{lid}.{lver}.nupkg'
    with urllib.request.urlopen(url) as r: data = r.read()
    os.makedirs(target, exist_ok=True)
    with zipfile.ZipFile(io.BytesIO(data)) as z:
        for info in z.infolist():
            name = urllib.request.url2pathname(info.filename) if '%' in info.filename else info.filename
            if name.endswith('/'): continue
            dest = os.path.join(target, name)
            os.makedirs(os.path.dirname(dest), exist_ok=True)
            with open(dest, 'wb') as f: f.write(z.read(info))
    return True

def find_ignoring_case(path):
    """The real path that matches `path` when case is ignored, or None."""
    current = os.sep
    for part in [p for p in path.split(os.sep) if p]:
        try: names = os.listdir(current)
        except OSError: return None
        if part in names: current = os.path.join(current, part); continue
        match = [n for n in names if n.lower() == part.lower()]
        if not match: return None
        current = os.path.join(current, match[0])
    return current

def main():
    os.makedirs(PACKAGES, exist_ok=True)
    wanted = {REFASM}
    for config in glob.glob(os.path.join(ROOT, 'src', '*', 'packages.config')):
        for p in ET.parse(config).getroot().iter('package'):
            wanted.add((p.get('id'), p.get('version')))
    fetched = 0
    for pid, version in sorted(wanted):
        try: fetched += fetch(pid, version)
        except Exception as e: print(f'  could not fetch {pid} {version}: {e}'); 
    print(f'packages: {len(wanted)} wanted, {fetched} downloaded')

    missing = []
    for proj in glob.glob(os.path.join(ROOT, 'src', '*', '*.csproj')):
        text = open(proj, encoding='utf-8-sig').read()
        for hint in re.findall(r'<HintPath>(.*?)</HintPath>', text):
            path = os.path.normpath(os.path.join(os.path.dirname(proj), hint.replace('\\', os.sep)))
            if os.path.exists(path): continue
            real = find_ignoring_case(path)
            if real is None: missing.append((os.path.basename(proj), hint)); continue
            os.makedirs(os.path.dirname(path), exist_ok=True)
            os.symlink(real, path)
    for proj, hint in missing: print(f'  {proj}: nothing at {hint}')
    print(f'references: {len(missing)} not found')
    return 1 if missing else 0

if __name__ == '__main__':
    sys.exit(main())
