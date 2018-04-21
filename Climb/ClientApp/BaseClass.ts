class BaseClass {
    getAuthorizationToken() {
        return localStorage.getItem("jwt");
    }
}