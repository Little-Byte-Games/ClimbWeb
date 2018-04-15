import * as React from "react";
import { RouteComponentProps } from "react-router";
import { Link } from "react-router-dom";
import { ClimbClient } from "../../gen/climbClient";
import AccountClient = ClimbClient.AccountClient;

interface IRegisterState {
    user: ClimbClient.ApplicationUser | null;
}

export class Register extends React.Component<RouteComponentProps<{}>, IRegisterState> {
    constructor(props: RouteComponentProps<{}>) {
        super(props);

        this.onRegisterClick = this.onRegisterClick.bind(this);
        this.onRegisterSuccess = this.onRegisterSuccess.bind(this);
        this.onRegisterFail = this.onRegisterFail.bind(this);

        this.state = { user: null };
    }

    public render() {
        if (this.state.user != null) {
            return <h1>Hello </h1>;
        }

        return <div>
                   <h2 id="subtitle">Register</h2>
                   <form onSubmit={this.onRegisterClick}>
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

    private onRegisterClick(event: React.FormEvent<HTMLFormElement>) {
        event.preventDefault();

        const email = (document.getElementById("emailInput") as HTMLInputElement).value;
        const password = (document.getElementById("passwordInput") as HTMLInputElement).value;
        const confirm = (document.getElementById("confirmInput") as HTMLInputElement).value;

        const accountClient = new AccountClient();
        accountClient.register(email, password, confirm)
            .then(this.onRegisterSuccess)
            .catch(this.onRegisterFail);
    }

    private onRegisterSuccess(user: ClimbClient.ApplicationUser | null) {
        this.setState({ user: user });
    }

    private onRegisterFail(reason: any) {
        alert("There was an error registering!");
    }
}