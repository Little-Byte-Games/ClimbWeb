import * as React from "react";
import { RouteComponentProps } from "react-router";
import { Link } from "react-router-dom";
import { ClimbClient } from "../../gen/climbClient"; 
import AccountClient = ClimbClient.AccountClient; 

export class Register extends React.Component<RouteComponentProps<{}>, {}> {
    public render() {
        return <div>
                   <h2 id="subtitle">Register</h2>
                   <form onSubmit={this.onRegister}>
                       <div>
                           <label>Email</label>
                           <input id="emailInput" type="email"/>
                       </div>
                       <div>
                           <label>Password</label>
                           <input id="passwordInput" type="password"/>
                       </div>
                       <div>
                           <label>Confirm Password</label>
                           <input id="confirmInput" type="password"/>
                       </div>
                       <button>Register</button>
                   </form>

                   <Link to={ '/account' }>Login</Link>
               </div>;
    }

    private onRegister(event: React.FormEvent<HTMLFormElement>) {
        event.preventDefault();

        const email = (document.getElementById("emailInput") as HTMLInputElement).value;
        const password = (document.getElementById("passwordInput") as HTMLInputElement).value;
        const confirm = (document.getElementById("confirmInput") as HTMLInputElement).value;

        const accountClient = new AccountClient();
        accountClient.register(email, password, confirm);
    }
}