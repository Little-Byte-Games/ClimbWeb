import * as React from "react";
import { Route } from "react-router-dom";
import { Login } from "./Login";
import { Register } from "./Register";

export class Layout extends React.Component {
    public render() {
        return (
            <div id="container">
                <h1 id="title">Climb</h1>
                <Route exact path="/account" component={ Login }/>
                <Route exact path="/account/register" component={ Register }/>
            </div>
        );
    }
}