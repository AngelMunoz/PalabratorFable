import React, { useState } from 'react';


export function TsCounter({ counter }: { counter: number }) {
    const [count, setCount] = useState(counter);
    return (
        <div>
            <h1>Hello Fable and Feliz from Typescript</h1>
            <h4>{count}</h4>
            <button onClick={e => setCount(count + 1)}>Increment</button>
            <button onClick={e => setCount(count - 1)}>Decrement</button>
            <button onClick={e => setCount(0)}>Reset</button>
        </div>
    )
}