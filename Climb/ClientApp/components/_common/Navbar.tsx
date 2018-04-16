import * as React from "react";
import "../../css/site.less";
import { ClimbClient } from "../../gen/climbClient";
import ApplicationUser = ClimbClient.ApplicationUser;

interface INavbarProps {
    user: ApplicationUser | null;
}

export class Navbar extends React.Component<INavbarProps> {
    public render() {
        return (
            <div id="navbar">
                {this.props.user != null
                    ? (
                        <div>
                            <span>{this.props.user.userName}</span>
                            <span> | </span>
                            <a onClick={this.onLogOutClick}>Log Out</a>
                        </div>
                    )
                    : (
                        <div>
                            <a href="/account/register">Register</a>
                            <span> | </span>
                            <a href="/account">Log In</a>
                        </div>
                    )}
            </div>
        );
    }

    onLogOutClick() {
        console.log("log out");
    }
}