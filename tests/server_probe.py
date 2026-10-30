# Read-only endpoint test. Does not complete quests or change profiles.
import json,ssl,urllib.request,zlib
from pathlib import Path
def entry(i):return (1624453*(0x65F6D+i)+1023920427)%0x8ED7A18D
def pack(data):
 b=bytearray(len(data).to_bytes(4,'little')+data);start=len(b)%0xAAB
 for i in range(len(b)-1,0,-1):j=entry(i+start)%i;b[i],b[j]=b[j],b[i]
 return b
def unpack(data):
 b=bytearray(data);start=len(b)%0xAAB
 for i in range(1,len(b)):j=entry(i+start)%i;b[i],b[j]=b[j],b[i]
 n=int.from_bytes(b[:4],'little');assert n<=len(b)-4
 return b[4:4+n]

import argparse
parser=argparse.ArgumentParser()
parser.add_argument('--runtime',required=True)
parser.add_argument('--profile',required=True)
args=parser.parse_args()
assert (Path(args.runtime)/'user/profiles'/(args.profile+'.json')).is_file()
def request(path, body=None):
 req=urllib.request.Request('https://127.0.0.1:6969'+path,data=None if body is None else pack(zlib.compress(json.dumps(body).encode())),headers={'Cookie':'PHPSESSID='+args.profile,'Content-Type':'application/json'})
 with urllib.request.urlopen(req,context=ssl._create_unverified_context(),timeout=30) as response:data=unpack(response.read())
 try:data=zlib.decompress(data)
 except zlib.error:pass
 return json.loads(data)
quests=request('/all-quests-checkmarks/quests')
assert isinstance(quests,list) and len(quests)>100
for row in quests:
 quest=row['Quest'];assert len(quest['_id'])==24
 assert isinstance(quest['Conditions']['AvailableForFinish'],list)
active=request('/all-quests-checkmarks/active-quests',[args.profile])
assert isinstance(active[args.profile],list)
print(f'PASS: {len(quests)} future/active quests and {len(active[args.profile])} active item quests; valid JSON IDs/conditions')
