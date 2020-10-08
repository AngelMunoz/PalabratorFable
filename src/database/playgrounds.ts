import PouchDB from 'pouchdb-browser';
const playgroundsdb = new PouchDB('palabrator-playgrounds');

type Playground = {
  _id: string;
  _rev: string;
  name: string;
  picture: 'circle' | 'triangle' | 'square' | 'star' | string;
}

function GetOrCreateDefaultPlayground(): Promise<Playground> {
  return playgroundsdb.put<Partial<Playground>>({
    _id: 'default1',
    name: 'default',
    picture: 'circle'
  })
    .then(result => ({ _id: result.id, _rev: result.rev, name: 'default', picture: 'circle' }))
}

export function GetPlayground(_id: string): Promise<Playground | Omit<Playground, 'picture'>> {
  return playgroundsdb.get<Playground>(_id, { attachments: true, binary: false })
    .then((result) => {
      const picture = ((result._attachments?.picture as PouchDB.Core.FullAttachment)?.data as string) ?? result.picture;
      return { _id: result._id, _rev: result._rev, name: result.name, picture };
    })
    .catch(error => {
      if (error.status === 404) return GetOrCreateDefaultPlayground()
      return Promise.reject(error.message);
    });
}