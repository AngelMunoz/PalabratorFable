import PouchDB from 'pouchdb-browser';
const profilesdb = new PouchDB<Profile>('palabrator-profiles');
const adminsdb = new PouchDB<Admin>('palabrator-admins');

type Profile = { _id: string; _rev: string; name: string; picture: string; }
type Admin = Profile & { pin: string }

type AllDocsResult = {
  doc?: PouchDB.Core.ExistingDocument<(Profile | Admin) & PouchDB.Core.AllDocsMeta>;
  id: string;
  key: string;
  value: {
    rev: string;
    deleted?: boolean;
  };
};

function mapResult(row: AllDocsResult, isAdmin = false): Profile | Admin {
  const pictureAttachment = row.doc?._attachments?.picture as PouchDB.Core.FullAttachment;
  const picture = pictureAttachment?.data as string ?? "";
  const _id = row.doc?._id;
  const _rev = row.doc?._rev;
  const name = row.doc?.name;
  if (isAdmin) {
    const pin = (row.doc as Admin)?.pin;
    return { _id, _rev, name, pin, picture };
  }
  return { _id, _rev, name, picture };
}

export function GetProfiles(): Promise<Profile[]> {
  return profilesdb.allDocs({ attachments: true, binary: false, include_docs: true })
    .then(result => result.rows.map(row => mapResult(row, true) as Profile));
}


export function GetAdmins(): Promise<Admin[]> {
  return adminsdb.allDocs({ attachments: true, binary: false, include_docs: true })
    .then(result => result.rows.map(row => mapResult(row, true) as Admin));
}

export function CheckPin(adminid: string, pin: string): Promise<boolean> {
  return adminsdb.get(adminid).then(result => result.pin === pin);
}


