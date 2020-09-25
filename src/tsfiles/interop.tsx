import React, { useState } from 'react';
import PouchDB from 'pouchdb-browser';
const db = new PouchDB('counterdb');


export function TsCounter({ counter }: { counter: number }) {
    const [state, setState] = useState({ counter, errorMsg: '' });

    const setInDb = async (counter: number) => {
        try {
            const state = await db.get('counterstate');
            await db.put({ ...state, counter });
        } catch (error) {
            if (error.status === 404) {
                await db.put({ _id: 'counterstate', counter });
            } else {
                setState({ ...state, errorMsg: error.message });
            }
        }
    };
    const getFromDb = async () => {
        try {
            const result = await db.get<{ counter: number }>('counterstate');
            setState({ ...state, counter: result.counter });
            if (state.errorMsg) { setState({ ...state, errorMsg: null }); }
        } catch (error) {
            setState({ ...state, errorMsg: error.message });
        }
    }

    return (
        <div>
            <h1>Hello Fable and Feliz from Typescript</h1>
            <h4>{state.counter}</h4>
            <button onClick={e => setState({...state, counter: state.counter + 1})}>Increment</button>
            <button onClick={e => setState({...state, counter: state.counter - 1})}>Decrement</button>
            <button onClick={e => setState({ ...state, counter: 0 })}>Reset</button>
            <button onClick={e => setInDb(state.counter)}>Save to local database</button>
            <button onClick={e => getFromDb()}>Restore from local database</button>
            {state.errorMsg ? <p>Failed to get from database: [{state.errorMsg}]</p> : null}
        </div>
    )
}