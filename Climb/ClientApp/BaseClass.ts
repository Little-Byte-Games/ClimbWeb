class BaseClass {
    protected transformOptions(options: any) {
        options.headers.Authorization = localStorage.getItem("jwt");
        return Promise.resolve(options);
    }
}