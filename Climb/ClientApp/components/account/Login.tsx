import * as React from "react";
import { RouteComponentProps } from "react-router";
import { Link } from "react-router-dom";
import { ClimbClient } from "../../gen/climbClient";
import AccountClient = ClimbClient.AccountClient;

interface ILoginState {
    isLoggingIn: boolean;
}

export class Login extends React.Component<RouteComponentProps<{}>, ILoginState> {
    constructor(props: RouteComponentProps<{}>) {
        super(props);

        this.onLogInClick = this.onLogInClick.bind(this);
        this.onLogInSuccess = this.onLogInSuccess.bind(this);
        this.onLogInFail = this.onLogInFail.bind(this);

        this.state = { isLoggingIn: false };
    }

    render() {
        if (this.state.isLoggingIn) {
            return <h1>Logging in...</h1>;
        }

        return <div>
                   <h2 id="subtitle">Log In</h2>
                   <form onSubmit={this.onLogInClick}>
                       <div>
                           <label>Email</label>
                           <input id="emailInput" type="email"/>
                       </div>
                       <div>
                           <label>Password</label>
                           <input id="passwordInput" type="password"/>
                       </div>
                       <button>Log In</button>
                   </form>

                   <form onSubmit={(e: any) => {
                       e.preventDefault();

                       const accountClient = new AccountClient("http://192.168.196.1:45455");
                       accountClient.test(accountClient.getAuthorizationToken(), "123456789")
                           .then(value => console.log(value))
                           .catch(reason => alert(reason));

                   }}>
                       <button>Test Cookie</button>
                   </form>

                   <Link to={ "/account/register" }>Register</Link>
               </div>;
    }

    private onLogInClick(event: React.FormEvent<HTMLFormElement>) {
        event.preventDefault();

        const email = (document.getElementById("emailInput") as HTMLInputElement).value;
        const password = (document.getElementById("passwordInput") as HTMLInputElement).value;

        const accountClient = new AccountClient("http://192.168.196.1:45455");
        accountClient.logIn(email, password)
            .then(this.onLogInSuccess)
            .catch(this.onLogInFail);
    }

    private onLogInSuccess(response: ClimbClient.LoginResponse) {
        if (response.token != null) {
            localStorage.setItem("jwt", response.token);
        }
        //window.location.assign("/user");
    }

    private onLogInFail(reason: any) {
        alert("There was an error logging in!");
    }
}