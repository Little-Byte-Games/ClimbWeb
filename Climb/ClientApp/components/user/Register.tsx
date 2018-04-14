import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import { Link } from 'react-router-dom';

export class Register extends React.Component<RouteComponentProps<{}>, {}> {
    public render() {
        return <div>
                   <h2 id="subtitle">Register</h2>
                   <Link to={ '/user' }>Login</Link>
               </div>;
    }
}