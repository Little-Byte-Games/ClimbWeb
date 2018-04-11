import './css/test.less';
import 'bootstrap';
import * as React from 'react';
import * as ReactDOM from 'react-dom';
import { AppContainer } from 'react-hot-loader';
import { BrowserRouter } from 'react-router-dom';
import * as RoutesModule from './routes';
import { ClimbClient } from "./gen/climbClient";
import SampleDataClient = ClimbClient.SampleDataClient;
let routes = RoutesModule.routes;

function renderApp() {
    // This code starts up the React app when it runs in a browser. It sets up the routing
    // configuration and injects the app into a DOM element.
    const baseUrl = document.getElementsByTagName('base')[0].getAttribute('href')!;

    ReactDOM.render(
        <AppContainer>
            <div>
                <span id="header">Hello world</span>
                <button onClick={loadWeather}>Load Weather</button>
            </div>
        </AppContainer>,
        document.getElementById('react-app')
    );
}

function loadWeather() {
    const sampleData = new SampleDataClient();
    sampleData.weatherForecasts()
        .then(value => {
            if (value == null) {
                console.log("No weather!");
                return;
            }
            for (let i = 0; i < value.length; i++) {
                console.log(value[i].toJSON());
            }
        });
}

renderApp();

// Allow Hot Module Replacement
if (module.hot) {
    module.hot.accept('./routes', () => {
        routes = require<typeof RoutesModule>('./routes').routes;
        renderApp();
    });
}
