import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import { Link } from 'react-router-dom';

export class Login extends React.Component<RouteComponentProps<{}>, {}> {
    public render() {
        return <div>
                   <h2 id="subtitle">Login</h2>
                   <Link to={ '/user/register' }>Register</Link>
               </div>;
    }
}